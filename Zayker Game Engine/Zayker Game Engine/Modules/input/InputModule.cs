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

        public static event KeyUpEvent OnKeyUp;
        public static event KeyDownEvent OnKeyDown;
        public static event MouseMoveEvent OnMouseMove;

        private static Dictionary<Silk.NET.Input.Key, bool> isKeyDown = new Dictionary<Key, bool>();
        private static System.Numerics.Vector2 mousePosition = new System.Numerics.Vector2();

        public InputModule()
        {
            this.id = "input";
        }

        public override void OnEnable()
        {
            base.OnEnable();
            OnKeyUp += UpdateKeyState_up;
            OnKeyDown += UpdateKeyState_down;
            OnMouseMove += UpdateMousePosition;
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

        /// <summary>
        /// Returns rather or not the given key is being hold down. 
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

        public static System.Numerics.Vector2 GetMousePos()
        {
            return mousePosition;
        }
    }
}
