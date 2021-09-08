using System;
using System.Collections.Generic;
using System.Text;

namespace ZEngine.ECS
{
    class EntityComponentSystem : Core.Module
    {
        private List<Entity> entities;

        public EntityComponentSystem()
        {
            this.id = "ecs";
            this.entities = new List<Entity>();
        }

        public Entity AddEntity()
        {
            Entity e = new Entity();
            entities.Add(e);
            return e;
        }

        public List<Entity> GetEntities()
        {
            return entities;
        }

        public override void Update(double deltaTime)
        {
            base.Update(deltaTime);

            foreach (Entity entity in entities)
            {
                entity.Update(deltaTime);
            }
        }
    }
}
