using UnityEngine;

namespace SoulRunProject.Common
{
    /// <summary>
    /// 継承してSingleton使用します。
    /// 継承先でAwakeが必要な場合OnAwake()を呼んでください。
    /// DontDestroyOnLoadを使用する場合はUseDontDestroyOnLoadをオーバーライドしてください。
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <summary> シングルトンの基底クラス </summary>
    public abstract class AbstractSingletonMonoBehaviour<T> : MonoBehaviour where T : Component
    {
        private static T _instance;
        public static T Instance
        {
            get
            {
                if (_instance) return _instance;
                var foundInstance = FindObjectOfType<T>();
                if (foundInstance) return foundInstance;
                // インスタンスが見つからなかった場合に新しいオブジェクトを生成
                GameObject singletonObject = new GameObject();
                singletonObject.name = typeof(T).Name + " (Singleton)";
                var instance = singletonObject.AddComponent<T>();
                return instance;
            }
        }
        /// <summary>
        /// 継承先でDontDestroyOnLoadを使用するかどうかを制御します。
        /// </summary>
        protected virtual bool UseDontDestroyOnLoad { get; } = false;
        private void Awake()
        {
            if (_instance == null)
            {   //  シーン上に配置されていてstatic変数にインスタンスが無ければここで初期化される
                _instance = this as T;
                if (UseDontDestroyOnLoad)
                {
                    DontDestroyOnLoad(gameObject);
                }
                OnAwake();
            }
            else
            {
                Destroy(gameObject);
            }
        }

        /// <summary> Awake時に実行される処理 </summary>
        public virtual void OnAwake()
        {
        }

        private void OnDestroy()
        {
            if (_instance == this)
            {
                _instance = null;
                OnDestroyed();
            }
        }

        /// <summary> OnDestroy時に実行される処理 </summary>
        public virtual void OnDestroyed()
        {
        }
    }
}