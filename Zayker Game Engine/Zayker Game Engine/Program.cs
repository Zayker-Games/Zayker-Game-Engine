using System;
using System.Drawing;

namespace Zayker_Game_Engine
{
    class Program
    {
        /// <summary>
        /// Directory to the "Modules"-Folder. This is inside the engine folder for the editor and inside the project folder for the game. 
        /// </summary>
        public static string modulesDirectory;

        public delegate void Update(float deltaTime);
        public static event Update OnUpdate;
        private static void Main(string[] args)
        {
            if(System.IO.Directory.GetCurrentDirectory().Contains("netcoreapp3.1")) // If we are building the engine, use the working directory
                modulesDirectory = System.IO.Directory.GetCurrentDirectory().Substring(0, System.IO.Directory.GetCurrentDirectory().LastIndexOf(@"bin\")) + @"Modules\";
            else // If we are not, use the current directory (Not tested!)
                modulesDirectory = System.IO.Directory.GetCurrentDirectory() + @"\Modules\";

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
