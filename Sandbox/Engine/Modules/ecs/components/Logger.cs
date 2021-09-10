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

            Debugging.Console.WriteToMain("The Logger on " + entity.name + " says hey!", "");
        }
        public override void DrawInspector()
        {
            ImGuiNET.ImGui.Text("Printing to console...");
        }

    }
}
