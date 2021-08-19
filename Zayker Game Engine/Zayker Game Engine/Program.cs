using System;
using System.Drawing;

namespace Zayker_Game_Engine
{
    class Program
    {
        public delegate void Update(float deltaTime);
        public static event Update OnUpdate;
        private static void Main(string[] args)
        {
            // Callbacks
            OnUpdate += Core.EngineModules.EngineModuleSystem.Update;

            // Renderer
            Core.EngineModules.EngineModuleSystem.Initialize();
            Core.EngineModules.EngineModuleSystem.EnableModule("engine_renderer");
            Modules.Renderer.Renderer renderer = (Modules.Renderer.Renderer)(Core.EngineModules.EngineModuleSystem.GetModuleById("engine_renderer"));
            renderer.CreateWindow();

            // Project
            //Core.Project_System.ProjectSystem.LoadProject(@"D:\C# Projects\Zayker-Game-Engine\Zayker Game Engine\Demo Game\");

            while(true)
            {
                OnUpdate.Invoke(0.1f); // TODO: Actuall dt
            }
        }
    }
}
