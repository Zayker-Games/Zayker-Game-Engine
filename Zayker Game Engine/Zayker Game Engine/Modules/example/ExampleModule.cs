using System;
using System.Collections.Generic;
using System.Text;

namespace ZEngine.ExampleModule
{
    class Example : Core.Module
    {
        public Example ()
        {
            // The id must match the modules folder name!
            this.id = "example";
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
