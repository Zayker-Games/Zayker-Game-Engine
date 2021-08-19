using System;
using System.Collections.Generic;
using System.Text;

namespace Zayker_Game_Engine.Core.EngineModules
{
    /// <summary>
    /// Handles all enabled engine-modules. This includes setup, update and shutdown of those. 
    /// This is static, because we only ever want one ModuleSystem.
    /// </summary>
    static class EngineModuleSystem
    {
        /// <summary>
        /// List of all modules included in the engine, regardless of rather or not they are enabled. 
        /// </summary>
        public static List<EngineModule> modules;

        public static void Initialize()
        {
            modules = new List<EngineModule>();
            // Initiate all modules as disabled and store them in modules list
            modules.Add(new Modules.Renderer.Renderer());
        }

        public static void Update(float deltaTime)
        {
            foreach (EngineModule module in modules)
            {
                if (module.isEnabled)
                    module.Update(deltaTime);
            }
        }

        public static EngineModule GetModuleById(string moduleId)
        {
            foreach (EngineModule engineModule in modules)
            {
                if(engineModule.id == moduleId)
                {
                    return engineModule;
                }
            }
            return null;
        }

        public static void EnableModule(string moduleId)
        {
            EngineModule moduleToEnable = GetModuleById(moduleId);

            moduleToEnable.isEnabled = true;
            moduleToEnable.OnEnable();
        }

        public static void DisableModule(string moduleId)
        {
            EngineModule moduleToDisable = GetModuleById(moduleId);

            moduleToDisable.isEnabled = false;
            moduleToDisable.OnDisable();
        }
    }
}
