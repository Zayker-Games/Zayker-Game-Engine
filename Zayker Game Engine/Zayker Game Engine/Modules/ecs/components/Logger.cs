using System;
using System.Collections.Generic;
using System.Text;

namespace ZEngine.ECS.Components
{
    class Logger : Component
    {
        public override void Update(double deltaTime)
        {
            base.Update(deltaTime);

            Console.WriteLine("The Logger on " + entity.name + " says hey!");
        }
    }
}
