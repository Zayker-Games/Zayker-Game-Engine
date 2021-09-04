using System;
using System.Collections.Generic;
using System.Text;

namespace ZEngine.Debugging
{
    class Debugger : Core.Module
    {
        private static List<UIEntity> uiEntities = new List<UIEntity>();

        /// <summary>
        /// We reuse the render request for every debug ui object.
        /// </summary>

        public Debugger ()
        {
            this.id = "debugger";
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

            foreach (UIEntity uIEntity in uiEntities)
            {
                uIEntity.Draw();
            }
        }

        public static void AddDebuggingUiEntity(UIEntity uiEntity)
        {
            uiEntities.Add(uiEntity);
        }
    }
}
