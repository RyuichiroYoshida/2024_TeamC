using System;
using System.Collections.Generic;
using System.Linq;
using SoulRunProject.Common;
using UnityEngine;
using Random = UnityEngine.Random;

namespace SoulRunProject.InGame
{
    /// <summary>
    /// インゲーム中のステートの管理
    /// ステージデータの所持
    /// </summary>
    public class StageManager : MonoBehaviour, IPausable
    {
        [SerializeField, EnumDrawer(typeof(StageName))] private StageDatum[] _stageData;
        [SerializeField] private FieldMover _fieldMover;
        private bool _isPause;
        private int _stageDataIndex;
        private int _fieldPatternIndex;
        private int _fieldSegmentIndex;
        private float _fieldTimer;
        private StageState _currentStageState;

        public Action ToNextStage;
        public Action ToBossStage;

        private void Start()
        {
            //CurrentFieldCreatePattern = _stageData[_stageDataIndex].FieldPatterns;
            _currentStageState = StageState.PlayingRunGame;

            ToNextStage += NextStage;
            ToBossStage += BossStage;
        }

        /// <summary> PlayingRunGameStateでのUpdate </summary>
        public void PlayingRunGameUpdate()
        {
            if (_currentStageState != StageState.PlayingRunGame) return;
            
            var pattern = _stageData[_stageDataIndex].FieldPatterns[_fieldPatternIndex];
            for (int i = 0; i < _fieldMover.FreeMoveSegmentsCount; i++)
            {
                GenerateFieldToOrder(pattern, ref _fieldSegmentIndex);
                GenerateFieldToRandom(pattern);
            }

            if (_fieldTimer >= pattern.Seconds && _fieldSegmentIndex == 0)
            {
                if (_stageData[_stageDataIndex].FieldPatterns.Count > _fieldPatternIndex + 1)
                {
                    _fieldPatternIndex++;
                }
                else
                {
                    ToBossStage?.Invoke();
                }

                _fieldTimer = 0;
            }
            else
            {
                _fieldTimer += Time.deltaTime;
            }
        }

        public void PlayingBossStageUpdate()
        {
            if (_currentStageState != StageState.PlayingBossStage) return;
            
            for (int i = 0; i < _fieldMover.FreeMoveSegmentsCount; i++)
            {
                if (_fieldMover.MoveSegments.Count <= 0)
                {
                    var instantiatedSegment = InstantiateSegment(_stageData[_stageDataIndex].BossStageField, true);
                    _fieldMover.MoveSegments.Add(instantiatedSegment);
                }
                else
                {
                    //  一番後ろのタイルを取得
                    var prevSegment = _fieldMover.MoveSegments[^1]; 
                    //  一番後ろのタイルのEndPosの座標を保存する
                    var prevEndPos = prevSegment.transform.TransformPoint(prevSegment.EndPos);
                    var instantiatedSegment = InstantiateSegment(_stageData[_stageDataIndex].BossStageField, false, prevEndPos);
                    _fieldMover.MoveSegments.Add(instantiatedSegment);
                }
            }
        }

        /// <summary> ボスステージに移行 </summary>
        void BossStage()
        {
            _currentStageState = StageState.PlayingBossStage;
            // ボスの生成と通知設定
            Instantiate(_stageData[_stageDataIndex].BossPrefab).GetComponent<DamageableEntity>().OnDead += () => ToNextStage?.Invoke();
        }
        
        /// <summary> 次のステージに移行 </summary>
        private void NextStage()
        {
            _currentStageState = StageState.PlayingRunGame;
            _stageDataIndex++;
            _stageDataIndex %= _stageData.Length;
            _fieldTimer = 0;
            _fieldPatternIndex = 0;
        }

        /// <summary>
        /// 設定された順番通りにフィールド生成
        /// </summary>
        private void GenerateFieldToOrder(FieldCreatePattern pattern, ref int segmentIndex)
        {
            if (pattern.Mode == FieldMoverMode.Random) return;
            
            FieldSegment originalSegment;
            Vector3 prevEndPos = Vector3.zero;
            bool isFirstSegment = false;
            if (_fieldMover.MoveSegments.Count <= 0)
            {   //  生成したタイルが0個の時
                //  FieldSegmentsに登録されているタイルの一番最初を選ぶ
                originalSegment = pattern.List[segmentIndex];
                isFirstSegment = true;
            }
            else
            {   //  生成されたタイルが1つ以上あるとき
                //  一番後ろのタイルを取得
                var prevSegment = _fieldMover.MoveSegments[^1]; 
                //  一番後ろのタイルのEndPosの座標を保存する
                prevEndPos = prevSegment.transform.TransformPoint(prevSegment.EndPos);
                //  FieldSegmentsに設定されている次のタイルを持ってくる
                originalSegment = pattern.List[segmentIndex];
            }
            
            var instantiatedSegment = InstantiateSegment(originalSegment, isFirstSegment, prevEndPos);
            _fieldMover.MoveSegments.Add(instantiatedSegment);
            segmentIndex++;
            segmentIndex %= pattern.List.Count;
        }
        /// <summary>
        /// ランダムにフィールド生成
        /// </summary>
        private void GenerateFieldToRandom(FieldCreatePattern pattern)
        {
            if (pattern.Mode == FieldMoverMode.Order) return;
            
            FieldSegment originalSegment;
            Vector3 prevEndPos = Vector3.zero;
            bool isFirstSegment = false;
            if (_fieldMover.MoveSegments.Count <= 0)
            {
                //  AdjacentGraphの中のタイルからランダムで選ぶ
                var processor = pattern.AdjacentGraph.Processor;
                originalSegment = processor.FieldSegmentNodes
                    [Random.Range(0, processor.FieldSegmentNodes.Count)].FieldSegment;
                isFirstSegment = true;
            }
            else
            {
                //  一番後ろのタイルを取得
                var prevSegment = _fieldMover.MoveSegments[^1]; 
                //  一番後ろのタイルのEndPosの座標を保存する
                prevEndPos = prevSegment.transform.TransformPoint(prevSegment.EndPos);
                
                var processor = pattern.AdjacentGraph.Processor;
                //  AdjacentGraphに一番後ろのタイルが登録されているかを探す
                var node = processor.FieldSegmentNodes
                    .FirstOrDefault(node =>
                        node.FieldSegment.GetInstanceID() == prevSegment.OriginalInstanceID);
                if (node != null)
                {   //  AdjacentGraphに一番後ろのタイルが登録されていた場合
                    //  そのタイルの出口と隣接しているタイルからランダムにタイルを選ぶ
                    var outSegments = node.OutSegments.ToList();
                    originalSegment = outSegments[Random.Range(0, outSegments.Count)];
                }
                else
                {   //  登録されていなかった場合
                    //  AdjacentGraphに登録されているタイルからランダムに選ぶ
                    originalSegment = processor.FieldSegmentNodes
                        [Random.Range(0, processor.FieldSegmentNodes.Count)].FieldSegment;
                }
            }
            var instantiatedSegment = InstantiateSegment(originalSegment, isFirstSegment, prevEndPos);
            _fieldMover.MoveSegments.Add(instantiatedSegment);
        }

        /// <summary>
        /// フィールドを実体化させるメソッド
        /// </summary>
        /// <param name="original">実体化させるプレハブ</param>
        /// <param name="isFirstSegment">実体化しているフィールドが無い状態か</param>
        /// <param name="prevEndPos">実体化しているフィールドがある場合、一番後ろのフィールドのEndPosの座標</param>
        /// <returns>実体化したフィールド</returns>
        private FieldSegment InstantiateSegment(FieldSegment original, bool isFirstSegment, Vector3 prevEndPos = default)
        {
            //  タイル実体化(fieldMoverの子オブジェクトにする)
            var instantiatedSegment = Instantiate(original, _fieldMover.transform);
            //  タイルのプレハブのInstanceIDをメモする
            instantiatedSegment.ApplyOriginalInstanceID(original);
            if (!isFirstSegment)
            {   //  実体化しているタイルで一番最初のタイルじゃないのなら
                //  前のタイルのEndPosの座標に生成したタイルを移動させる
                instantiatedSegment.transform.position = prevEndPos;
            }
            //  タイルに設定された-StartPos分だけ座標をずらす
            instantiatedSegment.transform.position = instantiatedSegment.transform
                .TransformPoint(-instantiatedSegment.StartPos);

            return instantiatedSegment;
        }

        public void Pause(bool isPause) => _isPause = isPause;

        enum StageState
        {
            PlayingRunGame,
            PlayingBossStage
        }

        enum StageName
        {
            Stage1,
            Stage2
        }
    }

    /// <summary>
    /// 各ステージの情報
    /// </summary>
    [Serializable]
    public class StageDatum
    {
        [SerializeField, CustomLabel("フィールド情報")] private List<FieldCreatePattern> _fieldPatterns;
        [SerializeField, CustomLabel("ボスステージのタイル")] private FieldSegment _bossStageField;
        [SerializeField, CustomLabel("ボス")] private BossController _bossPrefab; // 仮
        
        /// <summary> 通常時(PlayingRunGame)のFieldPattern </summary>
        public List<FieldCreatePattern> FieldPatterns => _fieldPatterns;
        /// <summary> ボスステージのFieldPattern </summary>
        public FieldSegment BossStageField => _bossStageField;
        public BossController BossPrefab => _bossPrefab;
    }
}
