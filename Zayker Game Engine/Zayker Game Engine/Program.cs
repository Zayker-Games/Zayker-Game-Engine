using System;
using System.Drawing;

namespace ZEngine
{
    class Program
    {
        /// <summary>
        /// Directory to the "Modules"-Folder. This is inside the engine folder for the editor and inside the project folder for the game. 
        /// </summary>
        public static string modulesDirectory;
        public static string coreDirectory;

        public delegate void Update(float deltaTime);
        public static event Update OnUpdate;

        private static void Main(string[] args)
        {
            // Get modulesDirectory
            if (System.IO.Directory.GetCurrentDirectory().Contains("netcoreapp3.1"))
            {
                // If we are building the engine, use the working directory
                modulesDirectory = System.IO.Directory.GetCurrentDirectory().Substring(0, System.IO.Directory.GetCurrentDirectory().LastIndexOf(@"bin\")) + @"Modules\";
                coreDirectory = System.IO.Directory.GetCurrentDirectory().Substring(0, System.IO.Directory.GetCurrentDirectory().LastIndexOf(@"bin\")) + @"Core\";
            }
            else
            {
                // If we are not, use the current directory (Not tested!)
                modulesDirectory = System.IO.Directory.GetCurrentDirectory() + @"\Modules\";
                coreDirectory = System.IO.Directory.GetCurrentDirectory() + @"\Core\";
            }

            Console.WriteLine("Engine starting...");

            // Add event for update loop
            OnUpdate += Core.ModuleSystem.Update;

            // Enable modules
            Core.ModuleSystem.Initialize();
            Core.ModuleSystem.EnableModule("input");
            Core.ModuleSystem.EnableModule("renderer_core");
            Core.ModuleSystem.EnableModule("ecs");

            // Get reference to renderer module
            Rendering.RendererCore renderer = (Rendering.RendererCore)(Core.ModuleSystem.GetModuleById("renderer_core"));

            // Create testing window
            renderer.CreateWindow();

            // Test saving/loading system
            //Core.Project_System.ProjectSystem.SaveProject(@"D:\C# Projects\Zayker-Game-Engine\Zayker Game Engine\Demo Game\");
            Core.Project_System.ProjectSystem.LoadProject(@"D:\C# Projects\Zayker-Game-Engine\Sandbox\");

            Core.Project_System.ProjectSystem.ImportModuleSystemToProject();
            Core.Project_System.ProjectSystem.ReimportAllModulesToProject();

            // Test input module
            Input.Input.OnKeyDown += delegate (Silk.NET.Input.IKeyboard arg1, Silk.NET.Input.Key arg2, int arg3) { Console.WriteLine("↓" + arg2); };
            Input.Input.OnKeyUp += delegate (Silk.NET.Input.IKeyboard arg1, Silk.NET.Input.Key arg2, int arg3) { Console.WriteLine("↑" + arg2); };

            // Test build module
            //Core.Build_System.BuildSystem.BuildFolder(@"D:\C# Projects\Zayker-Game-Engine\Zayker Game Engine\Demo Game");

            Console.WriteLine("Engine initialized. Entering main loop...");
            while (true)
            {
                OnUpdate.Invoke(0.1f); // TODO: Actuall dt
            }
        }
    }
}
