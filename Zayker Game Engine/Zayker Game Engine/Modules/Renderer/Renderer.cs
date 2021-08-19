using System;
using System.Collections.Generic;
using System.Text;
using Silk.NET.Input;
using Silk.NET.Windowing;

namespace Zayker_Game_Engine.Modules.Renderer
{
    class Renderer : Zayker_Game_Engine.Core.EngineModule
    {
        /// <summary>
        /// List of all windows. The window at index[0] is considered the main window and handles updates.
        /// </summary>
        public List<Window> windows;

        public Renderer()
        {
            this.id = "engine_renderer";
        }

        public override void OnEnable()
        {
            base.OnEnable();
        }

        public Window CreateWindow()
        {
            return new Window();
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
            options.Title = "LearnOpenGL with Silk.NET";

            window = Silk.NET.Windowing.Window.Create(options);

            //Assign events.
            window.Load += OnLoad;
            window.Update += OnUpdate;
            window.Render += OnRender;

            //Run the window.
            window.Run();
        }

        private void OnLoad()
        {
            //Set-up input context.
            IInputContext input = window.CreateInput();
            for (int i = 0; i < input.Keyboards.Count; i++)
            {
                input.Keyboards[i].KeyDown += KeyDown;
            }
        }

        private void OnRender(double obj)
        {
            //Here all rendering should be done.
        }

        private void OnUpdate(double obj)
        {
            //Here all updates to the program should be done.
        }

        private void KeyDown(IKeyboard arg1, Key arg2, int arg3)
        {
            //Check to close the window on escape.
            if (arg2 == Key.Escape)
            {
                window.Close();
            }
        }
    }
}
