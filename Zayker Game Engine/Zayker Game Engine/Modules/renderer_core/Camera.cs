using System;
using System.Collections.Generic;
using System.Text;

namespace ZEngine.Rendering
{
    public class Camera
    {
        public Math.Vector position = new Math.Vector(0.0f, 0.0f, 3.0f);
        public Math.Vector forwards = new Math.Vector(0.0f, 0.0f, -1.0f);
        public Math.Vector up = Math.Vector.Up;
        public Math.Vector direction = Math.Vector.Zero;
        public float fov = 45f;
        public float aspectRatio = 1f;
    }
}
