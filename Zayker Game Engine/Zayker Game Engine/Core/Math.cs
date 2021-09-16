using System;
using System.Collections.Generic;
using System.Text;

namespace ZEngine
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
            if (x < min)
                x = max - (min - x) % (max - min);
            else
                x = min + (x - min) % (max - min);

            return x;
        }

        public static float Clamp(float x, float min, float max)
        {
            return MathF.Min(MathF.Max(x, min), max);
        }

        public class Vector
        {
            public float x;
            public float y;
            public float z;

            // Constructors
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
            
            // Methods
            public float magnitude
            {
                get
                {
                    return MathF.Sqrt(
                    ((x == 0) ? 0f : MathF.Pow(x, 2f)) +
                    ((y == 0) ? 0f : MathF.Pow(y, 2f)) +
                    ((z == 0) ? 0f : MathF.Pow(z, 2f))
                    );
                }
            }

            public Vector normalized
            {
                get
                {
                    float mag = this.magnitude;
                    return new Vector(x / mag, y / mag, z / mag);
                }
            }

            public static Vector Forwards { get { return new Vector(0f, 0f, 1f); } }
            public static Vector Right { get { return new Vector(1f, 0f, 0f); } }
            public static Vector Up { get { return new Vector(0f, 1f, 0f); } }
            public static Vector Zero { get { return new Vector(0f, 0f, 0f); } }

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

            // Conversion overrides
            public static explicit operator System.Numerics.Vector3(Math.Vector v) { return new System.Numerics.Vector3(v.x, v.y, v.z); }

            public static explicit operator Vector(System.Numerics.Vector3 v) { return new Vector(v.X, v.Y, v.Z); }
        }

        public class Quaternion
        {
            public float w;
            public float i;
            public float j;
            public float k;

            /// <summary>
            /// Creates an identiy quaternion.
            /// </summary>
            public Quaternion()
            {
                w = 1f;
                i = 0f;
                j = 0f;
                k = 0f;
            }

            public Quaternion(float w, float i, float j, float k)
            {
                this.w = w;
                this.i = i;
                this.j = j;
                this.k = k;
            }

            /// <summary>
            /// Create a quaternion from given eulerangles. 
            /// 
            /// Based on:
            /// http://www.euclideanspace.com/maths/geometry/rotations/conversions/eulerToQuaternion/index.htm
            /// </summary>
            public static Quaternion FromEulerAngles(float x, float y, float z)
            {
                // Convert the degrees to radians and wrap them in a range of -180° and 180°
                float heading = Math.DegreesToRadians(Wrap(y, -180f, 180f));
                float attitude = Math.DegreesToRadians(Wrap(x, -180f, 180f));
                float bank = Math.DegreesToRadians(Wrap(z, -180f, 180f));

                float C1 = MathF.Cos(heading);
                float C2 = MathF.Cos(attitude);
                float C3 = MathF.Cos(bank);
                float S1 = MathF.Sin(heading);
                float S2 = MathF.Sin(attitude);
                float S3 = MathF.Sin(bank);

                float w = MathF.Sqrt(1.0f + C1 * C2 + C1 * C3 - S1 * S2 * S3 + C2 * C3) / 2f;
                float i = (C2 * S3 + C1 * S3 + S1 * S2 * C3) / (4.0f * w);
                float j = (S1 * C2 + S1 * C3 + C1 * S2 * S3) / (4.0f * w);
                float k = (-S1 * S3 + C1 * S2 * C3 + S2) / (4.0f * w);

                return new Quaternion(w, i, j, k);
            }

            public static Quaternion FromEulerAngles(Vector eulers)
            {
                return FromEulerAngles(eulers.x, eulers.y, eulers.z);
            }

            /// <summary>
            /// Returns a vector representing this quaternion in euler angles. 
            /// </summary>
            public Vector GetEulerAngles()
            {
                float heading = MathF.Atan2(2f * j * w - 2f * i * k, 1f - 2f * MathF.Pow(j,2f) - 2f * MathF.Pow(k,2f));
                float attitude = MathF.Asin(2f * i * j + 2f * k * w);
                float bank = MathF.Atan2(2f * i * w - 2f * j * k, 1f - 2f * MathF.Pow(i,2f) - 2f * MathF.Pow(k,2f));

                if (i * j + k * w == 0.5f) { // (north pole)
                    heading = 2 * MathF.Atan2(i, w);
                    bank = 0;
                }
                if (i * j + k * w == -0.5f) { // (south pole)
                    heading = -2 * MathF.Atan2(i, w);
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

            public float magnitude
            {
                get
                {
                    return MathF.Sqrt(
                    ((w == 0) ? 0f : MathF.Pow(w, 2f)) +
                    ((i == 0) ? 0f : MathF.Pow(i, 2f)) +
                    ((j == 0) ? 0f : MathF.Pow(j, 2f)) +
                    ((k == 0) ? 0f : MathF.Pow(k, 2f))
                    );
                }
            }

            public override string ToString()
            {
                return "(w:" + w + ", i:" + i + ", j:" + j + ", k:" + k + ")";
            }

            public static Vector operator *(Quaternion rotation, Vector point)
            {
                Quaternion p = new Quaternion(0, point.x, point.y, point.z);

                Quaternion pOut = rotation * p * Conjugate(rotation);

                return new Vector(pOut.i, pOut.j, pOut.k);
            }

            public static Quaternion operator *(Quaternion a, Quaternion b)
            {
                float w = -a.i * b.i - a.j * b.j - a.k * b.k + a.w * b.w;
                float i = a.i * b.w + a.j * b.k - a.k * b.j + a.w * b.i;
                float j = -a.i * b.k + a.j * b.w + a.k * b.i + a.w * b.j;
                float k = a.i * b.j - a.j * b.i + a.k * b.w + a.w * b.k;

                return new Quaternion(w, i, j, k);
            }

            public static Quaternion Conjugate(Quaternion q)
            {
                return new Quaternion(q.w, -q.i, -q.j, -q.k);
            }
        }
    }
}
