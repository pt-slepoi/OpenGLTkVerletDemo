using System;
using System.Collections.Generic;
using System.Text;

namespace MathTools
{
    public class Vec3 : Vec4
    {

        public Vec3(float x, float y, float z)
            : base(x, y, z, 1)
        {
        }

        public Vec3(Vec3 v)
            : base(v)
        {
        }

		public Vec3(Vec4 v)

		{
			X = v.X;
			Y = v.Y;
			Z = v.Z;
			W = 1;

		}



        /** Cross Product
         * @param v 
         * @return vettore ortogonale a v e this
         */
        public Vec3 Cross(Vec3 v)
        {
            return new Vec3(
                    Y * v.Z - Z * v.Y,  //x
                    Z * v.X - X * v.Z,  //y
                    X * v.Y - Y * v.X   //z
             );
        }

		public static Vec3 operator - (Vec3 v0, Vec3 v1){
			return v0.Sub (v1);
		}

		public static Vec3 operator - (Vec3 v0){
			return v0*(-1);
		}


		public static Vec3 operator + (Vec3 v0, Vec3 v1){
			return v0.Add (v1);
		}

		public static Vec3 operator * (Vec3 v0, float s){
			return v0.Mult(s);
		}

        public Vec3 Add(Vec3 v2)
        {
            Vec4 v = base.Add(v2);
            return new Vec3(v.X, v.Y, v.Z);
        }

        public Vec3 Sub(Vec3 v2)
        {
            Vec4 v = base.Sub(v2);
            return new Vec3(v.X, v.Y, v.Z);
        }

        public new Vec3 Mult(float f)
        {
            Vec4 v = base.Mult(f);
            return new Vec3(v.X, v.Y, v.Z);
        }

		public new Vec3 Normalized(){
			Vec4 v = base.Normalized ();
			return new Vec3 (v.X, v.Y, v.Z);
		}


    }


}
