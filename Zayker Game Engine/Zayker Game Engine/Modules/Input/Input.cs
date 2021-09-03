using Silk.NET.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace ZEngine.Input
{
    class Input : Core.Module
    {
        public delegate void KeyUpEvent(IKeyboard arg1, Key arg2, int arg3);
        public delegate void KeyDownEvent(IKeyboard arg1, Key arg2, int arg3);

        public static event KeyUpEvent OnKeyUp;
        public static event KeyDownEvent OnKeyDown;

        public Input()
        {
            this.id = "input";
        }

        public override void OnEnable()
        {
            base.OnEnable();
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
    }
}
