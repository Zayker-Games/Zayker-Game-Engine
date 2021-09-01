using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace ZEngine.Rendering
{
    public class Camera
    {
        public Vector3 position = new Vector3(0.0f, 0.0f, 3.0f);
        public Vector3 forwards = new Vector3(0.0f, 0.0f, -1.0f);
        public Vector3 up = Vector3.UnitY;
        public Vector3 direction = Vector3.Zero;
        public float yaw = -90f;
        public float pitch = 0f;
        public float fov = 45f;
        public float aspectRatio = 1f;
    }
}
