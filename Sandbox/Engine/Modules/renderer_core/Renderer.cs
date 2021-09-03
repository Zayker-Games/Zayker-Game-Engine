using System.Collections.Generic;
using Silk.NET.Windowing;
using System;
using System.Numerics;
using Silk.NET.Input;
using System.Linq;

namespace ZEngine.Rendering
{
    class RendererCore : Core.Module
    {
        /// <summary>
        /// List of all windows. 
        /// </summary>
        public static List<Window> windows = new List<Window>();

        public RendererCore()
        {
            this.id = "renderer_core";
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
                if (!window.window.IsClosing)
                {
                    window.window.DoUpdate();
                    window.window.DoEvents();
                    window.window.DoRender();
                } else
                {
                    Console.WriteLine("Closing window " + window.window.Title);
                    window.Gl.Dispose();
                    window.window.Dispose();

                    //window.window.DoUpdate();
                    //window.window.DoEvents();
                    //window.window.DoRender();
                    window._markedForDestruction = true;
                }
            }

            windows = windows.Where(w => !w._markedForDestruction).ToList();
        }

        public static Window CreateWindow()
        {
            Window window = new Window();
            windows.Add(window);
            return window;
        }
    }
}