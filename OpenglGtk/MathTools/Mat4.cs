using System;
using System.Collections.Generic;
using System.Text;

namespace MathTools
{
    public class Mat4
    {
        /** struttura
	 * 00	04	08	12
	 * 01 	05 	09 	13
	 * 02 	06 	10 	14
	 * 03 	07	11	15*/
        private float[] m;
        private static float PI = (float)Math.PI;

        public Mat4()
        {
            m = new float[16];
        }

        /** Inizializza la matrice a partire da una esistente
         * @param f matrice
         */
        public Mat4(float[] f)
        {
            this.m = f;
        }

        /**
         * Inizializza la matrice a partire da un'altra
         * @param m
         */
        public Mat4(Mat4 m)
        {
            this.m = m.ToFloatArray();
        }

        /**  
         * @param i indice riga
         * @return restituisce la i-esima riga 
         */
        public Vec4 Row(int i)
        {
            return new Vec4(
                        m[i + 0],
                        m[i + 4],
                        m[i + 8],
                        m[i + 12]
                    );
        }

        /**  
         * @param i indice colonna
         * @return restituisce la i-esima colonna 
         */
        public Vec4 Column(int i)
        {
            i *= 4;
            return new Vec4(
                        m[i + 0],
                        m[i + 1],
                        m[i + 2],
                        m[i + 3]
                    );
        }



        /**
         * Restituisce l'elemento in pos (i,j)
         * @param i 
         * @param j
         * @return mat[i][j]
         */
        public float Element(int i, int j)
        {
            return m[i + j * 4];
        }



        /**
         * Setta l'elemento in posizione (i,j)
         * @param i
         * @param j
         * @param f nuovo valore
         */
        public void SetElement(int i, int j, float f)
        {
            m[i + j * 4] = f;
        }

        /**
         * @return array con gli elementi della matrice
         */
        public float[] ToFloatArray()
        {
             return m;//(float[])m.Clone();
        }

        /**
         * Prodotto matrice-vettore
         * @param v vettore
         * @return vettore risultante this*v
         */
        public Vec4 Dot(Vec4 v)
        {
            return new Vec4(
                    this.Row(0).Dot(v),
                    this.Row(1).Dot(v),
                    this.Row(2).Dot(v),
                    this.Row(3).Dot(v)
                    );
        }


        /**
         * Prodotto tra matrici
         * @param m2 seconda matrice
         * @return matrice risultante this*m2
         */
        public Mat4 Dot(Mat4 m2)
        {
            Mat4 n = new Mat4();

            for (int i = 0; i < 4; i++)
                for (int j = 0; j < 4; j++)
                    n.SetElement(i, j,
                            this.Row(i).Dot(m2.Column(j))
                    );

            return n;
        }

        public  static  Mat4 operator * (Mat4 m1, Mat4 m2){
            return m1.Dot(m2);
        }

        /** MATRICI DI TRASFORMAZIONE */

        /** @return Matrice d'identità */
        public static Mat4 Identity()
        {

            Mat4 n = new Mat4();
            for (int i = 0; i < 16; i++)
                n.m[i] = 0;

            n.m[0] = n.m[5] = n.m[10] = n.m[15] = 1;
            return n;
        }

        /** @return Matrice di traslazione */
        public static Mat4 Translation(float dx, float dy, float dz)
        {
            Mat4 n = Identity();

            n.SetElement(0, 3, dx);
            n.SetElement(1, 3, dy);
            n.SetElement(2, 3, dz);

            return n;

        }

        /** @return Matrice di rotazione rispetto all'asse X */
        public static Mat4 RotatationX(float angleDegrees)
        {

            Mat4 n = Identity();
            float s = (float)Math.Sin(angleDegrees * (PI / 180f));
            float c = (float)Math.Cos(angleDegrees * (PI / 180f));

            n.SetElement(1, 1, c);
            n.SetElement(2, 2, c);

            n.SetElement(1, 2, +s);
            n.SetElement(2, 1, -s);

            return n;

        }

        /** @return Matrice di rotazione rispetto all'asse Y */
        public static Mat4 RotatationY(float angleDegrees)
        {

            Mat4 n = Identity();

            float s = (float)Math.Sin(angleDegrees * (PI / 180f));
            float c = (float)Math.Cos(angleDegrees * (PI / 180f));

            n.SetElement(0, 0, c);
            n.SetElement(0, 2, s);
            n.SetElement(2, 0, -s);
            n.SetElement(2, 2, c);
            return n;
        }

        /** @return Matrice di rotazione rispetto all'asse Z */
        public static Mat4 RotatationZ(float angleDegrees)
        {

            Mat4 n = Identity();
            float s = (float)Math.Sin(angleDegrees * (PI / 180f));
            float c = (float)Math.Cos(angleDegrees * (PI / 180f));

            n.SetElement(0, 0, c);
            n.SetElement(0, 1, +s);
            n.SetElement(1, 0, -s);
            n.SetElement(1, 1, c);

            return n;
        }

        /** @return Matrice di scalatura anisotropica */
        public static Mat4 Scaling(float sx, float sy, float sz)
        {
            Mat4 n = Identity();

            n.SetElement(0, 0, sx);
            n.SetElement(1, 1, sy);
            n.SetElement(2, 2, sz);

            return n;
        }

        /** @return Matrice di scalatura isotropica */
        public static Mat4 UniformScaling(float s)
        {
            return Scaling(s, s, s);
        }

        /** @return Matrice di proiezione prospettica che rimappa z da [near .. far] a [-1 .. +1] */
        public static Mat4 Perspective(float focal, float near, float far)
        {
            Mat4 res = Identity();

            float A = (far + near) / (focal * (near - far));
            float B = (2 * far * near) / (focal * (near - far));

            res.SetElement(3, 3, 0);
            res.SetElement(3, 2, -1 / focal);
            res.SetElement(2, 2, A);
            res.SetElement(2, 3, B);

            return res;
        }

        ///** @return Matrice per centrare in (0,0,0) un'AABB */
        //public static Mat4 CenterAABB(AABB aabb)
        //{
        //    Mat4 m = Mat4.Identity();

        //    m = m.Dot(Mat4.Translation(-aabb.center().x(), -aabb.center().y(), -aabb.center().z())
        //    );
        //    return m;
        //}

        /** @return Inversa della matrice di proiezione prospettica */
        public static Mat4 InversePerspective(float focal, float near, float far)
        {
            Mat4 res = Identity();

            float A = (far + near) / (focal * (near - far));
            float B = (2 * far * near) / (focal * (near - far));

            res.SetElement(2, 2, 0);
            res.SetElement(3, 2, 1 / B);
            res.SetElement(2, 3, -focal);
            res.SetElement(3, 3, focal * A / B);

            return res;
        }

        ///**@return Inversa CenterAABB*/
        //public static Mat4 inverseCenterAABB(AABB aabb)
        //{
        //    return Translation(
        //            aabb.center().x(),
        //            aabb.center().y(),
        //            aabb.center().z()
        //    );
        //}

        /* (non-Javadoc)
         * @see java.lang.Object#toString()
         */
		public override String ToString()
        {
            String s = "";
            for (int i = 0; i < 16; i++)
                s += m[i] + " ";
            return s;
        }
    }
}
