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
            // Get modulesDirectory
            if(System.IO.Directory.GetCurrentDirectory().Contains("netcoreapp3.1")) // If we are building the engine, use the working directory
                modulesDirectory = System.IO.Directory.GetCurrentDirectory().Substring(0, System.IO.Directory.GetCurrentDirectory().LastIndexOf(@"bin\")) + @"Modules\";
            else // If we are not, use the current directory (Not tested!)
                modulesDirectory = System.IO.Directory.GetCurrentDirectory() + @"\Modules\";

            // Add event for update loop
            OnUpdate += Core.EngineModules.EngineModuleSystem.Update;

            // Enable modules
            Core.EngineModules.EngineModuleSystem.Initialize();
            Core.EngineModules.EngineModuleSystem.EnableModule("engine_input");
            Core.EngineModules.EngineModuleSystem.EnableModule("engine_renderer");

            // Get reference to renderer module
            Renderer.Renderer renderer = (Renderer.Renderer)(Core.EngineModules.EngineModuleSystem.GetModuleById("engine_renderer"));

            // Create testing window
            renderer.CreateWindow();

            // Test saving/loading system
            //Core.Project_System.ProjectSystem.LoadProject(@"D:\C# Projects\Zayker-Game-Engine\Zayker Game Engine\Demo Game\");

            // Test input module
            Input.Input.OnKeyDown += delegate (Silk.NET.Input.IKeyboard arg1, Silk.NET.Input.Key arg2, int arg3) { Console.WriteLine("↓" + arg2); };
            Input.Input.OnKeyUp += delegate (Silk.NET.Input.IKeyboard arg1, Silk.NET.Input.Key arg2, int arg3) { Console.WriteLine("↑" + arg2); };

            while (true)
            {
                OnUpdate.Invoke(0.1f); // TODO: Actuall dt
            }
        }
    }
}
