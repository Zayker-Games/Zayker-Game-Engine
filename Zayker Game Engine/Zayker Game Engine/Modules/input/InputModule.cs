using Silk.NET.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace ZEngine.Input
{
    class InputModule : Core.Module
    {
        public delegate void KeyUpEvent(IKeyboard arg1, Key arg2, int arg3);
        public delegate void KeyDownEvent(IKeyboard arg1, Key arg2, int arg3);
        public delegate void MouseMoveEvent(IMouse mouse, System.Numerics.Vector2 pos);
        public delegate void MouseDownEvent(IMouse mouse, Silk.NET.Input.MouseButton button);
        public delegate void MouseUpEvent(IMouse mouse, Silk.NET.Input.MouseButton button);

        public static event KeyUpEvent OnKeyUp;
        public static event KeyDownEvent OnKeyDown;
        public static event MouseMoveEvent OnMouseMove;
        public static event MouseDownEvent OnMouseDown;
        public static event MouseUpEvent OnMouseUp;

        private static Dictionary<Silk.NET.Input.Key, bool> isKeyDown = new Dictionary<Key, bool>();
        private static Dictionary<Silk.NET.Input.MouseButton, bool> isMouseDown = new Dictionary<MouseButton, bool>();
        private static System.Numerics.Vector2 mousePosition = new System.Numerics.Vector2();

        public InputModule()
        {
            this.id = "input";
            this.dependencies = new List<string>() { };
        }

        public override void OnEnable()
        {
            base.OnEnable();
            OnKeyUp += UpdateKeyState_up;
            OnKeyDown += UpdateKeyState_down;
            OnMouseMove += UpdateMousePosition;
            OnMouseUp += UpdateMouseState_up;
            OnMouseDown += UpdateMouseState_down;
        }

        private void UpdateMouseState_down(IMouse mouse, MouseButton button)
        {
            if (isMouseDown.ContainsKey(button))
                isMouseDown[button] = true;
            else
                isMouseDown.Add(button, true);
        }

        private void UpdateMouseState_up(IMouse mouse, MouseButton button)
        {
            if (isMouseDown.ContainsKey(button))
                isMouseDown[button] = false;
            else
                isMouseDown.Add(button, false);
        }

        public override void OnDisable()
        {
            base.OnDisable();
        }

        public override void Update(double deltaTime)
        {
            base.Update(deltaTime);
        }

        public static void InvokeKeyDownEvent(IKeyboard arg1, Key arg2, int arg3)
        {
            if(OnKeyDown != null)
                OnKeyDown.Invoke(arg1, arg2, arg3);
        }

        public static void InvokeKeyUpEvent(IKeyboard arg1, Key arg2, int arg3)
        {
            if(OnKeyUp != null)
                OnKeyUp.Invoke(arg1, arg2, arg3);
        }

        public static void InvokeMouseMoveEvent(IMouse mouse, System.Numerics.Vector2 pos)
        {
            if (OnMouseMove != null)
                OnMouseMove.Invoke(mouse, pos);
        }

        public static void InvokeMouseDownEvent(IMouse mouse, Silk.NET.Input.MouseButton button)
        {
            if (OnMouseDown != null)
                OnMouseDown.Invoke(mouse, button);
        }

        public static void InvokeMouseUpEvent(IMouse mouse, Silk.NET.Input.MouseButton button)
        {
            if (OnMouseUp != null)
                OnMouseUp.Invoke(mouse, button);
        }

        /// <summary>
        /// Returns rather or not the given key is being held down. 
        /// </summary>
        public static bool IsKeyDown(Key key)
        {
            if (isKeyDown.ContainsKey(key))
                return isKeyDown[key];
            else 
                return false;
        }

        private static void UpdateKeyState_up(IKeyboard arg1, Key arg2, int arg3)
        {
            if (isKeyDown.ContainsKey(arg2))
                isKeyDown[arg2] = false;
            else
                isKeyDown.Add(arg2, false);
        }

        private static void UpdateKeyState_down(IKeyboard arg1, Key arg2, int arg3)
        {
            if (isKeyDown.ContainsKey(arg2))
                isKeyDown[arg2] = true;
            else
                isKeyDown.Add(arg2, true);
        }

        // Wait... does this mean I can have 2 virtual cursors if there are 2 mice connected?
        private static void UpdateMousePosition(IMouse mouse, System.Numerics.Vector2 pos)
        {
            mousePosition = pos;
        }

        /// <summary>
        /// Get the position of the mouse in relation to the window rectangle.
        /// </summary>
        /// <returns></returns>
        public static System.Numerics.Vector2 GetMousePos()
        {
            return mousePosition;
        }

        /// <summary>
        /// Returns rather or not the given mousebutton is held down.
        /// </summary>
        public static bool IsMouseDown(Silk.NET.Input.MouseButton button)
        {
            if (isMouseDown.ContainsKey(button))
                return isMouseDown[button];
            else
                return false;
        }
    }
}
