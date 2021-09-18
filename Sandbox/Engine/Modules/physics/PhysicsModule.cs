using System;
using System.Collections.Generic;
using System.Text;

namespace ZEngine.Examples
{
    class PhysicsModule : Core.Module
    {
        public PhysicsModule()
        {
            // The id must match the modules folder name!
            this.id = "physics";
            this.dependencies = new List<string>() { "ecs" };
        }

        public override void OnEnable()
        {
            base.OnEnable();
        }

        public override void OnDisable()
        {
            base.OnDisable();
        }

        public override void Update(double deltaTime)
        {
            base.Update(deltaTime);
        }
    }
}
