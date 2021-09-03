using System;
using System.Collections.Generic;
using System.Text;

/// <summary>
/// Base of every game made with the engine. Handles all engine and module stuff and calls user made scripts. 
/// </summary>
namespace ZEngine.Core
{
    public static class EngineRuntime
    {
        // The games main update loop event 
        public delegate void Update(double deltaTime);
        public static event Update OnUpdate;

        private static void Main(string[] args)
        {
            Start();
        }

        public static void Start()
        {
            OnUpdate += Core.ModuleSystem.Update;
            OnUpdate += Game.Update;

            // Initialize and enable all modules
            Core.ModuleSystem.Initialize();
            foreach (Module m in ModuleSystem.modules)
            {
                ModuleSystem.EnableModule(m.id);
            }

            Game.Start();

            System.Diagnostics.Stopwatch deltaTimeStopwatch = new System.Diagnostics.Stopwatch();
            deltaTimeStopwatch.Start();

            while (true)
            {
                double dt = deltaTimeStopwatch.Elapsed.TotalSeconds;
                deltaTimeStopwatch.Restart();

                OnUpdate.Invoke(dt);
            }
        }
    }
}
