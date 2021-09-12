using System;
using System.Collections.Generic;
using System.Text;

namespace ZEngine.Core
{
    public static class Math
    {
        public static float DegreesToRadians(float degrees)
        {
            return degrees * MathF.PI / 180f;
        }

        public static float RadiansToDegrees(float radians)
        {
            return radians * 180f/MathF.PI;
        }

        public static float Lerp(float a, float b, float t)
        {
            return a + (b - a) * t;
        }

        /// <summary>
        /// Wrap x in the range of min[included] and max[excluded].
        /// </summary>
        public static float Wrap(float x, float min, float max)
        {
            if (min > max)
                return Wrap(x, max, min);
            return (x >= 0 ? min : max) + (x % (max - min));
        }

        public class Vector
        {
            public float x;
            public float y;
            public float z;

            public Vector() { x = 0; y = 0; z = 0; }

            public Vector(float x, float y)
            {
                this.x = x;
                this.y = y;
            }

            public Vector(float x, float y, float z)
            {
                this.x = x;
                this.y = y;
                this.z = z;
            }
            
            // Vector-Vector operator overloads:
            public static Vector operator +(Vector a, Vector b)
            {
                return new Vector(a.x + b.x, a.y + b.y, a.z + b.z);
            }
            public static Vector operator -(Vector a, Vector b)
            {
                return new Vector(a.x - b.x, a.y - b.y, a.z - b.z);
            }

            // Boolean operator overloads
            public static bool operator ==(Vector a, Vector b)
            {
                return (a.x == b.x && a.y == b.y && a.z == b.z);
            }
            public static bool operator !=(Vector a, Vector b)
            {
                return (a.x != b.x || a.y != b.y || a.z != b.z);
            }

            // Vector-Float operator overloads
            public static Vector operator *(Vector a, float s)
            {
                return new Vector(a.x * s, a.y * s, a.z * s);
            }

            public override string ToString()
            {
                return "(" + x + ", " + y + ", " + z + ")";
            }
        }

        public class Quaternion
        {
            public float s;
            public Vector v;

            /// <summary>
            /// Creates an identiy quaternion.
            /// </summary>
            public Quaternion()
            {
                s = 1f;
                v = new Vector();
            }

            public Quaternion(float w, float i, float j, float k)
            {
                this.s = w;
                this.v = new Vector(i, j, k);
            }

            /// <summary>
            /// Create a quaternion from given eulerangles. 
            /// 
            /// Based on:
            /// http://www.euclideanspace.com/maths/geometry/rotations/conversions/eulerToQuaternion/index.htm
            /// </summary>
            public static Quaternion FromEulerAngles(Vector eulers)
            {
                // Convert the degrees to radians and wrap them in a range of -180° and 180°
                eulers.x = Math.DegreesToRadians(Wrap(eulers.x, -180f, 180f));
                eulers.y = Math.DegreesToRadians(Wrap(eulers.y, -180f, 180f));
                eulers.z = Math.DegreesToRadians(Wrap(eulers.z, -180f, 180f));

                float C1 = MathF.Cos(eulers.x);
                float C2 = MathF.Cos(eulers.y);
                float C3 = MathF.Cos(eulers.z);
                float S1 = MathF.Sin(eulers.x);
                float S2 = MathF.Sin(eulers.y);
                float S3 = MathF.Sin(eulers.z);

                float w = MathF.Sqrt(1.0f + C1 * C2 + C1 * C3 - S1 * S2 * S3 + C2 * C3) / 2f;
                float i = (C2 * S3 + C1 * S3 + S1 * S2 * C3) / (4.0f * w);
                float j = (S1 * C2 + S1 * C3 + C1 * S2 * S3) / (4.0f * w);
                float k = (-S1 * S3 + C1 * S2 * C3 + S2) / (4.0f * w);

                return new Quaternion(w, i, j, k);
            }

            /// <summary>
            /// Returns a vector representing this quaternion in euler angles. 
            /// </summary>
            public Vector GetEulerAngles()
            {
                float qw = s;
                float qx = v.x;
                float qy = v.y;
                float qz = v.z;

                float heading = MathF.Atan2(2f * qy * qw - 2f * qx * qz, 1f - 2f * MathF.Pow(qy,2f) - 2f * MathF.Pow(qz,2f));
                float attitude = MathF.Asin(2f * qx * qy + 2f * qz * qw);
                float bank = MathF.Atan2(2f * qx * qw - 2f * qy * qz, 1f - 2f * MathF.Pow(qx,2f) - 2f * MathF.Pow(qz,2f));

                if (qx * qy + qz * qw == 0.5f) { // (north pole)
                    heading = 2 * MathF.Atan2(qx, qw);
                    bank = 0;
                }
                if (qx * qy + qz * qw == -0.5f) { // (south pole)
                    heading = -2 * MathF.Atan2(qx, qw);
                    bank = 0;
                }

                return new Vector(RadiansToDegrees(heading), RadiansToDegrees(attitude), RadiansToDegrees(bank));
            }

            /// <summary>
            /// Get the direction vector, that represents (1, 0, 0) rotated by this quaternion.
            /// </summary>
            public Vector GetDirectionX()
            {
                throw new System.NotImplementedException();
                return null;
            }

            public override string ToString()
            {
                return "(s:" + s + ", x:" + v.x + ", y:" + v.y + ", z:" + v.z + ")";
            }
        }
    }
}
