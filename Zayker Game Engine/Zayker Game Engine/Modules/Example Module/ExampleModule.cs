﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Zayker_Game_Engine.Modules.Example_Module
{
    class ExampleModule : Core.EngineModules.EngineModule
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