using System;
using System.Collections.Generic;
using System.Text;

namespace ZEngine.Core
{
    public static class Math
    {
        public static float DegreesToRadians(float degrees)
        {
            return MathF.PI / 180f * degrees;
        }

        public static float Lerp(float a, float b, float t)
        {
            return a + (b - a) * t;
        }
    }
}
