using System;
using System.Drawing;

namespace Zayker_Game_Engine
{
    class Program
    {
        private static void Main(string[] args)
        {
            Core.EngineModuleSystem.Initialize();
            Core.EngineModuleSystem.EnableModule("engine_renderer");
            Modules.Renderer.Renderer renderer = (Modules.Renderer.Renderer)Core.EngineModuleSystem.GetModuleById("engine_renderer");
            renderer.CreateWindow();
        }
    }
}
