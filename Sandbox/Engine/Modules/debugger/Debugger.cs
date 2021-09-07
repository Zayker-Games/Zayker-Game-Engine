using System;
using System.Collections.Generic;
using System.Text;

namespace ZEngine.Debugging
{
    class Debugger : Core.Module
    {
        private static Dictionary<Rendering.Window, DebuggerGuiInstance> debuggerGuiInstances = new Dictionary<Rendering.Window, DebuggerGuiInstance>();

        /// <summary>
        /// We reuse the render request for every debug ui object.
        /// </summary>

        public Debugger ()
        {
            this.id = "debugger";
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

            foreach (DebuggerGuiInstance guiInstance in debuggerGuiInstances.Values)
            {
                guiInstance.Draw((float)deltaTime);
            }
        }

        /// <summary>
        /// Returns the ImGuiInstance for a given instance
        /// </summary>
        public static DebuggerGuiInstance GetDebuggerGuiInstance(Rendering.Window window)
        {
            if (!debuggerGuiInstances.ContainsKey(window))
                debuggerGuiInstances.Add(window, new DebuggerGuiInstance(window));

            return debuggerGuiInstances[window];
        }
    }
}
