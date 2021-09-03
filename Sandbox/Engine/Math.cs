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
    }
}
