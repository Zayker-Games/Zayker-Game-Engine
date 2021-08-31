using System;
using System.Collections.Generic;
using System.Text;

/// <summary>
/// Base of every game made with the engine. Handles all engine and module stuff and calls user made scripts. 
/// </summary>
namespace ZEngine.Core
{
    public static class Game
    {
        // The games main update loop event 
        public delegate void Update(float deltaTime);
        public static event Update OnUpdate;

        private static void Main(string[] args)
        {
            Start();
        }

        public static void Start()
        {
            OnUpdate += Core.ModuleSystem.Update;

            Core.ModuleSystem.Initialize();

            while (true)
            {
                Console.WriteLine("Hey");
                OnUpdate.Invoke(0.1f); // TODO: Actuall dt
            }
        }
    }
}
