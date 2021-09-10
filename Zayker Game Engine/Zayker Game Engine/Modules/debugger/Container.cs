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
        public int id = 0;

        public string name = "unnamed";

        public bool opened = false;
        public bool temporary = false;

        protected GuiInstance debugger;

        /// <summary>
        /// Initialize this container with a random id.
        /// </summary>
        protected void Init(GuiInstance debugger)
        {
            Init(debugger, new Random().Next());
        }

        /// <summary>
        /// Initialize this container with the given id. This is usefull when wanting to save the layout between sessions. 
        /// </summary>
        /// <param name="debugger"></param>
        /// <param name="id"></param>
        protected void Init(GuiInstance debugger, int id)
        {
            this.id = id;
            this.debugger = debugger;
        }

        public abstract void Update(float dt);
    }
}
