using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Silk.NET.Input;
using Silk.NET.OpenGL.Extensions.ImGui;

namespace ZEngine.Debugging
{
    /// <summary>
    /// Each window that has some debug information to be displayed, has its own GuiInstance
    /// </summary>
    public class GuiInstance
    {
        Rendering.Window window;
        ImGuiController controller = null;
        IInputContext inputContext = null;

        private List<Container> containers = new List<Container>();

        public GuiInstance(Rendering.Window window)
        {
            this.window = window;

            controller = new ImGuiController(
                    window.Gl, 
                    window.window,
                    inputContext = window.window.CreateInput()
                );

            //ImGuiNET.ImGui.GetIO().ConfigFlags = ImGuiNET.ImGuiConfigFlags.ViewportsEnable;
        }

        public void AddContainer(Container container)
        {
            containers.Add(container);
        }

        public List<Container> GetContainers()
        {
            return containers;
        }

        /// <summary>
        /// Add this container and all its containers to the render que.
        /// </summary>
        public void Draw(float dt)
        {
            controller.Update((float)dt);

            foreach (Container c in containers.ToList())
            {
                c.Update(dt);
            }

            containers = containers.Where(c => (!c.temporary || (c.temporary && c.opened))).ToList();

            window.AddImGuiRenderQue(controller);
        }
    }
}
