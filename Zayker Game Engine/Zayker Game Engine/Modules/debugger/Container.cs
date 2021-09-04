using System;
using System.Collections.Generic;
using System.Text;
using ImGuiNET;

namespace ZEngine.Debugging
{
    /// <summary>
    /// Base class for a debugger container. Derive from this and override the update method to draw your own (ImGui) debugging container.
    /// To draw this, add it to the list of an DebuggerGuiInstance.
    /// </summary>
    public abstract class Container
    {
        /// <summary>
        /// Each container has a custom id. This is needed to have multiple instances of the same container work at the same time. 
        /// </summary>
        protected int id = 0;

        public abstract void Update(float dt);
    }

    public class FpsViewer : Container
    {
        private float averageDeltaTime = 0.1f;
        public FpsViewer()
        {
            id = new Random().Next();
        }

        public override void Update(float dt)
        {
            averageDeltaTime = Core.Math.Lerp(averageDeltaTime, dt, 0.1f);
            ImGui.Begin("FPS##" + id);
            ImGui.Text(MathF.Round(1f / averageDeltaTime) + " FPS");
            ImGui.End();
        }
    }
}
