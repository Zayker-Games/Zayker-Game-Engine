using System;
using System.Collections.Generic;
using System.Text;

namespace Zayker_Game_Engine.Example_Module
{
    class ExampleModule : Core.Module
    {
        public ExampleModule ()
        {
            this.id = "engine_example";
        }

        public override void OnEnable()
        {
            base.OnEnable();
        }

        public override void OnDisable()
        {
            base.OnDisable();
        }

        public override void Update(float deltaTime)
        {
            base.Update(deltaTime);
        }
    }
}
