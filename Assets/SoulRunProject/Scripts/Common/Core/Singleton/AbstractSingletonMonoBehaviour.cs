using UnityEngine;

namespace SoulRunProject.Common
{
    /// <summary>
    /// 継承してシングルトンを使用します。
    /// シーン上に配置された場合でも、動的に生成された場合でもインスタンスが管理されます。
    /// DontDestroyOnLoad を使用する場合は UseDontDestroyOnLoad をオーバーライドしてください。
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class AbstractSingletonMonoBehaviour<T> : MonoBehaviour where T : Component
    {
        /// <summary>
        /// 継承先で DontDestroyOnLoad を使用するかどうかを制御します。
        /// </summary>
        protected virtual bool UseDontDestroyOnLoad { get; } = false;

        private static T _instance;
        private static bool _isInitialized = false;
        private static readonly object _lock = new object();

        public static T Instance
        {
            get
            {
                // ロックを使用してスレッドセーフにする
                lock (_lock)
                {
                    if (_instance != null)
                        return _instance;

                    // まず、シーン内で既存のインスタンスを検索
                    _instance = FindObjectOfType<T>();

                    if (_instance != null)
                    {
                        Debug.Log($"[Singleton] Using existing instance of {typeof(T)}");
                        return _instance;
                    }

                    // インスタンスが見つからなかった場合、新しいオブジェクトを生成
                    GameObject singletonObject = new GameObject();
                    _instance = singletonObject.AddComponent<T>();
                    singletonObject.name = typeof(T).Name + " (Singleton)";

                    // DontDestroyOnLoadが指定されている場合に適用
                    if ((_instance as AbstractSingletonMonoBehaviour<T>).UseDontDestroyOnLoad)
                    {
                        DontDestroyOnLoad(singletonObject);
                    }

                    return _instance;
                }
            }
        }

        private void Awake()
        {
            if (_instance == null)
            {
                // シーン上に配置されていた場合の初期化処理
                _instance = this as T;

                if (UseDontDestroyOnLoad)
                {
                    DontDestroyOnLoad(gameObject);
                }

                InitializeSingleton();
            }
            else if (_instance != this)
            {
                // すでに別のインスタンスが存在している場合、重複したオブジェクトを破棄
                Destroy(gameObject);
            }
        }

        private void InitializeSingleton()
        {
            if (_isInitialized) return;

            // 初期化処理がここに入る（1度だけ実行される）
            OnAwake();

            _isInitialized = true;
        }

        /// <summary> 継承先で Awake 時に実行される処理 </summary>
        public virtual void OnAwake() { }

        private void OnDestroy()
        {
            if (_instance == this)
            {
                _instance = null;
                _isInitialized = false;
                OnDestroyed();
            }
        }

        /// <summary> 継承先で OnDestroy 時に実行される処理 </summary>
        public virtual void OnDestroyed() { }
    }
}
