using System;
using System.Collections.Generic;
using System.Text;

namespace ZEngine.ECS.Components
{
    class SceneCamera : ZEngine.ECS.Component
    {
        private Rendering.Camera _camera;

        public void SetAsActiveCamera(Rendering.Window window)
        {
            window.camera = _camera;
        }

        public override void DrawInspector()
        {

        }
    }
}
