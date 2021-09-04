using System;
using System.Collections.Generic;
using System.Text;
using Silk.NET.Input;
using Silk.NET.OpenGL.Extensions.ImGui;

namespace ZEngine.Debugging
{
    /// <summary>
    /// Each window that has some debug information to be displayed, has its own DebuggerInstance
    /// </summary>
    public class DebuggerGuiInstance
    {
        Rendering.Window window;
        ImGuiController controller = null;
        IInputContext inputContext = null;

        private List<Container> containers = new List<Container>();

        public DebuggerGuiInstance(Rendering.Window window)
        {
            this.window = window;

            controller = new ImGuiController(
                    window.Gl, 
                    window.window,
                    inputContext = window.window.CreateInput()
                );
        }

        public void AddContainer(Container container)
        {
            containers.Add(container);
        }

        /// <summary>
        /// Add this container and all its containers to the render que.
        /// </summary>
        public void Draw(float dt)
        {
            controller.Update((float)dt);

            foreach (Container c in containers)
            {
                c.Update(dt);
            }

            window.AddToGuiRenderQue(controller);
        }
    }
}
