using System.Collections.Generic;
using SoulRunProject.Common;
using UniRx;
using UnityEngine;

namespace SoulRunProject.InGame
{
    public class EntityPoolManager : AbstractSingletonMonoBehaviour<EntityPoolManager>
    {
        [SerializeField] int _preloadCount = 5;
        [SerializeField] int _threshold = 5;
        readonly Dictionary<DamageableEntity, EntityPool> _entityPoolDictionary = new();
        protected override bool UseDontDestroyOnLoad => false;

        public DamageableEntity RequestInstance(DamageableEntity original, Transform parent)
        {
            var entityPool = RequestPool(original);
            var entity = entityPool.Rent();
            var entityTransform = entity.transform;
            var prevParent = entityTransform.parent;
            
            entityTransform.SetParent(parent);
            entity.Initialize();
            entity.OnFinishedAsync.Take(1).Subscribe(_=>
            {
                entity.transform.SetParent(prevParent);
                entityPool.Return(entity);
            });
            return entity;
        }
        public DamageableEntity RequestInstance(DamageableEntity original, Vector3 position, Quaternion rotation)
        {
            var entityPool = RequestPool(original);
            var entity = entityPool.Rent();
            var entityTransform = entity.transform;
            
            entityTransform.position = position;
            entityTransform.rotation = rotation;
            entity.Initialize();
            entity.OnFinishedAsync.Take(1).Subscribe(_=> entityPool.Return(entity));
            return entity;
        }
        public DamageableEntity RequestInstance(DamageableEntity original, Vector3 position, Quaternion rotation, Transform parent)
        {
            var entityPool = RequestPool(original);
            var entity = entityPool.Rent();
            var entityTransform = entity.transform;
            var prevParent = entityTransform.parent;
            
            entityTransform.position = position;
            entityTransform.rotation = rotation;
            entityTransform.SetParent(parent);
            entity.Initialize();
            entity.OnFinishedAsync.Take(1).Subscribe(_=>
            {
                entity.transform.SetParent(prevParent);
                entityPool.Return(entity);
            });
            return entity;
        }
        public EntityPool RequestPool(DamageableEntity original)
        {
            //  既に指定されたIDのpoolが存在していればそのpoolを返す
            if (_entityPoolDictionary.TryGetValue(original, out var value))
            {
                return value;
            }
            //  無ければ新しく生成
            var newParent = new GameObject().transform;
            newParent.name = original.name;
            newParent.SetParent(transform);
            _entityPoolDictionary.Add(original, new EntityPool(newParent, original));
            _entityPoolDictionary[original].PreloadAsync(_preloadCount, _threshold).Subscribe();

            return _entityPoolDictionary[original];
        }
        public override void OnDestroyed()
        {
            foreach (var pool in _entityPoolDictionary)
            {
                pool.Value.Dispose();
            }
        }
    }
}
