using SoulRunProject.InGame;
using UniRx.Toolkit;
using UnityEngine;

namespace SoulRunProject.Common
{
    public class EntityPool : ObjectPool<DamageableEntity>
    {
        private readonly DamageableEntity _entity;
        private Transform _parent;
        
        public EntityPool(Transform parent, DamageableEntity entity)
        {
            _parent = parent;
            _entity = entity;
        }
        
        protected override DamageableEntity CreateInstance()
        {
            var entity = Object.Instantiate(_entity, _parent);
            entity.IsPooled = true;
            return entity;
        }
    }
}
