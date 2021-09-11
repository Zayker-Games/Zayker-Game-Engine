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

        public static event KeyUpEvent OnKeyUp;
        public static event KeyDownEvent OnKeyDown;

        private static Dictionary<Silk.NET.Input.Key, bool> isKeyDown = new Dictionary<Key, bool>();

        public InputModule()
        {
            this.id = "input";
        }

        public override void OnEnable()
        {
            base.OnEnable();
            OnKeyUp += UpdateKeyState_up;
            OnKeyDown += UpdateKeyState_down;
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
    }
}
