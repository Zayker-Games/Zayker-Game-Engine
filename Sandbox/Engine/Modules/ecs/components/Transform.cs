using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace ZEngine.ECS.Components
{
    class Transform : Component
    {
        public Vector3 position = new Vector3(0f, 0f, 0f);
        public Vector3 rotation = new Vector3(0f, 0f, 0f);
        public Vector3 scale = new Vector3(1f, 1f, 1f);
    }
}
