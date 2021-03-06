using System;
using System.Collections.Generic;
using System.Text;

namespace ZEngine.Examples
{
    class ExampleModule : Core.Module
    {
        public ExampleModule ()
        {
            // The id must match the modules folder name!
            this.id = "example";
            this.dependencies = new List<string>() {  };
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
