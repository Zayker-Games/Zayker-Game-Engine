using System;
using System.Collections.Generic;
using System.Text;

namespace ZEngine.Core
{
    /// <summary>
    /// Handles all enabled engine-modules. This includes setup, update and shutdown of those. 
    /// This is static, because we only ever want one ModuleSystem.
    /// </summary>
    static class ModuleSystem
    {
        /// <summary>
        /// List of all modules included in the engine, regardless of rather or not they are enabled. 
        /// </summary>
        public static List<Module> modules;

        public static void Initialize()
        {
            modules = new List<Module>();
            // Initiate all modules as disabled and store them in modules list
            modules.Add(new Rendering.RendererCore());
            modules.Add(new Input.Input());
            modules.Add(new ECS.EntityComponentSystem());
        }

        public static void Update(float deltaTime)
        {
            foreach (Module module in modules)
            {
                if (module.isEnabled)
                    module.Update(deltaTime);
            }
        }

        public static Module GetModuleById(string moduleId)
        {
            foreach (Module engineModule in modules)
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
            Module moduleToEnable = GetModuleById(moduleId);

            if (moduleToEnable == null)
                throw new System.Exception("Module " + moduleId + " was not found!");

            moduleToEnable.isEnabled = true;
            moduleToEnable.OnEnable();
        }

        public static void DisableModule(string moduleId)
        {
            Module moduleToDisable = GetModuleById(moduleId);

            moduleToDisable.isEnabled = false;
            moduleToDisable.OnDisable();
        }
    }
}
