using System;
using System.Collections.Generic;
using System.Text;

namespace MathTools
{
    public class Vec4
    {

        float[] v;



        public Vec4()
        {
            v = new float[4];
			X = 0;
			Y = 0;
			Z = 0;
			W = 1;
        }


        public Vec4(float x, float y, float z, float w)
        {
            v = new float[4];
            v[0] = x;
            v[1] = y;
            v[2] = z;
            v[3] = w;
        }


        public Vec4(Vec4 u)
        {
            v = new float[4];
            v[0] = u.X;
            v[1] = u.Y;
            v[2] = u.Z;
            v[3] = u.W;
        }


        public float X { get { return v[0]; } set { v[0] = value; } }

        public float Y { get { return v[1]; } set { v[1] = value; } }

        public float Z { get { return v[2]; } set { v[2] = value; } }

        public float W { get { return v[3]; } set { v[3] = value; } }


        public float[] GetArray()
        {
            return new float[] { this.X, this.Y, this.Z, this.W };
        }


        public Vec4 Add(Vec4 v2)
        {
            return new Vec4(
                    this.X + v2.X,
                    this.Y + v2.Y,
                    this.Z + v2.Z,
                    this.W + v2.W
                    );
        }

        public float Dot(Vec4 v2)
        {
            return this.X * v2.X
                    + this.Y * v2.Y
                    + this.Z * v2.Z
                    + this.W * v2.W;
        }

        public Vec4 Mult(float f)
        {
            return new Vec4(
                this.X * f,
                this.Y * f,
                this.Z * f,
                this.W * f
                );
        }

		public static Vec4 operator - (Vec4 v0, Vec4 v1){
			return v0.Sub (v1);
		}

		public static Vec4 operator - (Vec4 v0){
			return v0*(-1);
		}

		
		public static Vec4 operator + (Vec4 v0, Vec4 v1){
			return v0.Add (v1);
		}

		public static Vec4 operator * (Vec4 v0, float s){
			return v0.Mult(s);
		}

        public Vec4 Sub(Vec4 v2)
        {
            return this.Add(v2.Mult(-1));
        }
        public Vec4 Div(float f)
        {
            return this.Mult(1 / f);
        }


        public float SquaredNorm()
        {
            return this.Dot(this);
        }

        public float Norm()
        {
            return (float)Math.Sqrt(
                    this.X * this.X +
                    this.Y * this.Y +
                    this.Z * this.Z +
                    this.W * this.W
            );
        }

        public Vec4 Normalized()
        {
            return this.Div(this.Norm());
        }

        public void Normalize()
        {
            Vec4 v2 = this.Normalized();
            this.X = (v2.X);
            this.Y = (v2.Y);
            this.Z = (v2.Z);
            this.W = (v2.W);
        }

        public override String ToString()
        {
            return "[ " + X + ", " + Y + ", " + Z + ", " + W + " ]";
        }

        public override bool Equals(Object o)
        {
            if (o is Vec4)
            {
                Vec4 v = (Vec4)o;
                if (v.X == X && v.Y == Y && v.Z == Z && v.W == W)
                    return true;
            }
            return false;
        }

		public override int GetHashCode ()
		{
			return base.GetHashCode ();
		}

    }
}
