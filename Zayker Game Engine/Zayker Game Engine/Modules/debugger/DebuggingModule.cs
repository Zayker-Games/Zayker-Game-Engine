using System;
using System.Collections.Generic;
using System.Text;

namespace ZEngine.Debugging
{
    class DebuggingModule : Core.Module
    {
        private static Dictionary<Rendering.Window, GuiInstance> debuggerGuiInstances = new Dictionary<Rendering.Window, GuiInstance>();

        /// <summary>
        /// We reuse the render request for every debug ui object.
        /// </summary>

        public DebuggingModule ()
        {
            this.id = "debugger";
            this.dependencies = new List<string>() { "renderer_core" };
        }

        public override void OnEnable()
        {
            base.OnEnable();
        }

        public override void OnDisable()
        {
            base.OnDisable();
        }

        public override void Update(double deltaTime)
        {
            base.Update(deltaTime);

            foreach (GuiInstance guiInstance in debuggerGuiInstances.Values)
            {
                guiInstance.Draw((float)deltaTime);
            }
        }

        /// <summary>
        /// Returns the ImGuiInstance for a given window. 
        /// Creates an instance if there is none for the given window.
        /// </summary>
        public static GuiInstance GetDebuggerGuiInstance(Rendering.Window window)
        {
            if (!debuggerGuiInstances.ContainsKey(window))
                debuggerGuiInstances.Add(window, new GuiInstance(window));

            return debuggerGuiInstances[window];
        }
    }
}
