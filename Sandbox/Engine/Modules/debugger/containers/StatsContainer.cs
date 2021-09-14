using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Text;

namespace ZEngine.Debugging
{
    class StatsContainer : Container
    {
        private float averageDeltaTime = 0.1f;
        public StatsContainer(GuiInstance debugger)
        {
            base.Init(debugger);
            name = "Debug Info";
        }

        public override void Update(float dt)
        {
            averageDeltaTime = Math.Lerp(averageDeltaTime, dt, 0.1f);

            if (opened)
            {
                ImGui.Begin("Stats##" + id, ref opened, ImGuiWindowFlags.AlwaysAutoResize);

                ImGui.Text("Version: " + "dev");
                ImGui.Text("FPS: " + MathF.Round(1f / averageDeltaTime));

                ImGui.End();
            }
        }
    }
}
