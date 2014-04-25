using System;
using System.Collections.Generic;

using System.Text;

namespace MathTools
{
   public  class IMatrix
    {
        public Mat4 Direct { get; set; }
        public Mat4 Inverse { get; set; }
        public IMatrix()
        {
            Direct = Mat4.Identity();
            Inverse = Mat4.Identity();
        }

        public IMatrix(Mat4 direct, Mat4 inverse)
        {
            Direct = direct;
            Inverse = inverse;
        }

        public IMatrix Inverted()
        {
            return new IMatrix(Inverse, Direct);
        }

        public IMatrix Dot(IMatrix im)
        {
            return new IMatrix(Direct.Dot(im.Direct), im.Inverse.Dot(Inverse));
        }

		public static IMatrix operator * (IMatrix m1, IMatrix m2){
			return m1.Dot(m2);
		}

        public static IMatrix Identity()
        {
            return new IMatrix(Mat4.Identity(), Mat4.Identity());
        }

        public static IMatrix Translation(float x, float y, float z)
        {
            return new IMatrix(Mat4.Translation(x, y, z), Mat4.Translation(-x, -y, -z));
        }

        public static IMatrix RotationX(float angleDegrees){
            return new IMatrix(Mat4.RotatationX(angleDegrees), Mat4.RotatationX(-angleDegrees));
        }

        public static IMatrix RotationY(float angleDegrees)
        {
            return new IMatrix(Mat4.RotatationY(angleDegrees), Mat4.RotatationY(-angleDegrees));
        }

        public static IMatrix RotationZ(float angleDegrees)
        {
            return new IMatrix(Mat4.RotatationZ(angleDegrees), Mat4.RotatationZ(-angleDegrees));
        }

        public static IMatrix UniformScaling(float s)
        {
            return new IMatrix(Mat4.UniformScaling(s), Mat4.UniformScaling(1 / s));
        }

        public static IMatrix Scaling(float x, float y, float z)
        {
            return new IMatrix(Mat4.Scaling(x, y, z), Mat4.Scaling(1 / x, 1 / y, 1 / z));
        }

		public static IMatrix Perspective(float focal, float near, float far)
        {
            return new IMatrix(Mat4.Perspective(focal, near, far), Mat4.InversePerspective(focal, near, far));
        }


    }
}
