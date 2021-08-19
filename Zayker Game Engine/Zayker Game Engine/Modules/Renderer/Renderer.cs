using System;
using System.Collections.Generic;
using System.Text;
using Silk.NET.Input;
using Silk.NET.Windowing;

namespace Zayker_Game_Engine.Modules.Renderer
{
    class Renderer : Core.EngineModules.EngineModule
    {
        /// <summary>
        /// List of all windows. 
        /// </summary>
        public List<Window> windows = new List<Window>();

        public Renderer()
        {
            this.id = "engine_renderer";
        }

        public override void OnEnable()
        {
            base.OnEnable();
        }

        public override void Update(float deltaTime)
        {
            base.Update(deltaTime);

            foreach (Window window in windows)
            {
                window.Update();
                window.Render();
            }
        }

        public Window CreateWindow()
        {
            Window window = new Window();
            windows.Add(window);
            return window;
        }
    }

    public class Window
    {
        private static IWindow window;

        public Window()
        {
            //Create a window.
            var options = WindowOptions.Default;
            options.Size = new Silk.NET.Maths.Vector2D<int>(800, 600);
            options.Title = "Window";

            window = Silk.NET.Windowing.Window.Create(options);

            //Run the window.
            window.Initialize();
        }

        public void Update()
        {
            window.DoUpdate();
            window.DoEvents();
        }

        public void Render()
        {
            window.DoRender();
        }
    }
}
