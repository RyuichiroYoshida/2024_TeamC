﻿using UnityEngine;

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
        /// <summary>
        /// 継承先でDontDestroyOnLoadを使用するかどうかを制御します。
        /// </summary>
        protected virtual bool UseDontDestroyOnLoad { get; } = false;

        private static T _instance;

        public static T Instance
        {
            get
            {
                if (_instance != null) return _instance;

                // シーン内で既存のインスタンスを検索
                _instance = FindObjectOfType<T>();

                // インスタンスが見つからなかった場合に新しいオブジェクトを生成
                if (_instance == null)
                {
                    GameObject singletonObject = new GameObject();
                    _instance = singletonObject.AddComponent<T>();
                    singletonObject.name = typeof(T).Name + " (Singleton)";

                    if ((_instance as AbstractSingletonMonoBehaviour<T>).UseDontDestroyOnLoad)
                    {
                        DontDestroyOnLoad(singletonObject);
                    }
                }

                return _instance;
            }
        }

        private void Awake()
        {
            if (_instance == null)
            {
                _instance = this as T;
                if (UseDontDestroyOnLoad)
                {
                    DontDestroyOnLoad(gameObject);
                }

                OnAwake();
            }
            else if (_instance != this)
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