using System;
using System.Collections.Generic;
using System.Text;

/// <summary>
/// Base of every game made with the engine. Handles all engine and module stuff and calls user made scripts. 
/// </summary>
namespace Zayker_Game_Engine.Core
{
    public static class Game
    {
        // The games main update loop event 
        public delegate void Update(float deltaTime);
        public static event Update OnUpdate;

        public static void Main(string[] args)
        {

        }

        public static void Start()
        {
            while (true)
            {
                OnUpdate.Invoke(0.1f); // TODO: Actuall dt
            }
        }
    }
}
