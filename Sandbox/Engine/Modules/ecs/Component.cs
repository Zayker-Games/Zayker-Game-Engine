using System;
using System.Collections.Generic;
using System.Text;

namespace ZEngine.ECS
{
    class Component
    {
        public Entity entity;

        public virtual void _init()
        {

        }

        public virtual void Update(double deltaTime)
        {

        }
    }
}
