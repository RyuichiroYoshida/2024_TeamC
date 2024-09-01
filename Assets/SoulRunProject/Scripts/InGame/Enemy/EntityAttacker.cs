using UnityEngine;

namespace SoulRunProject.InGame
{
    /// <summary>
    /// Enemyの攻撃処理の抽象クラス
    /// </summary>
    public abstract class EntityAttacker
    {
        public Animator Animator;
        /// <summary>
        /// 起動時に一度のみ呼ばれる
        /// </summary>
        public virtual void OnStart(Transform myTransform){}
        /// <summary>
        /// Updateで呼ばれる攻撃処理
        /// </summary>
        public virtual void OnUpdateAttack(Transform myTransform , Transform playerTransform){}
        public virtual void Pause(){}
        public virtual void Resume(){}
    }
}
