﻿using System.Collections.Generic;
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

        public override void Update(double deltaTime)
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

                    window._markedForDestruction = true;
                }
            }

            windows = windows.Where(w => !w._markedForDestruction).ToList();
        }

        public static Window CreateWindow(string title = "Game")
        {
            Window window = new Window();
            window.window.WindowState = WindowState.Maximized;
            window.SetTitle(title);
            windows.Add(window);
            return window;
        }
    }
}