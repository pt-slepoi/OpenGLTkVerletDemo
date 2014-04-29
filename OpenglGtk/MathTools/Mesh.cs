using System;
using System.Collections.Generic;

namespace MathTools
{
	public class Face{

		public const int N = 3; //tri-mesh

		//connectivity
		public int[] posIndex=new int[N];
		public int[] normIndex = new int[N];
		public int[] uvIndex = new int[N];
		public Face(){
			posIndex = new int[N];
			normIndex = new int[N];
			uvIndex = new int[N];
		}
		public Face(int a, int b, int c){

			posIndex[0] = a; posIndex[1] = b; posIndex[2] = c;

			for(int i=0; i<N; i++)
				normIndex[i]=uvIndex[i]=-1;			
		}
		public Face(
			int v0, int vt0, int vn0,
			int v1, int vt1, int vn1,
			int v2, int vt2, int vn2
		):base(){
			posIndex[0] = v0; uvIndex[0]=vt0; normIndex[0]=vn0;
			posIndex[1] = v1; uvIndex[1]=vt1; normIndex[1]=vn1;
			posIndex[2] = v2; uvIndex[2]=vt2; normIndex[2]=vn2;
		}

	}


	public class Mesh
	{


		//geometria
		public List<Vec3> VertexList{ get; protected set; } //per gestire la mesh
	
		//attributi
		public List<Vec3> Normals{ get; protected set; }

		//private List<Vec3> uv; 	//(Vec2) posizioni uv di tutti ivertici di nella tessitura 

		//connettivita'
		public List<Face> Faces{ get; protected set; }
		AABB aabb;
		public Mesh(){		
			init();
		}

		private void init(){
			VertexList = new List<Vec3>();
			Normals =  new List<Vec3>();
			//uv =  new List<Vec3>();
			Faces =  new List<Face>();
		
			aabb = new AABB();
		}



		private void UpdateAABB() {
			aabb.Clear();
			for(int i=0; i<VertexList.Count; i++){
				Vec3 p = new Vec3(VertexList[i].X,VertexList[i].Y,VertexList[i].Z);
				aabb.Add(p);
			}
		}


		/**
	 * @return AABB della mesh
	 */
		public AABB getAABB(){
			return aabb;
		}
		public static Mesh CreatePlane(float width, float height, int rows,int columns){
			if (rows == columns && rows == 0)
				return null;
			var plane = new Mesh ();
			var dw = width / rows;
			var dh = height / columns;


			for (var r = 0; r<rows+1; r++)
				for (var c = 0; c<columns+1; c++)
					plane.VertexList.Add (new Vec3 (c*dw, r*dh , 0.0f));

			for (var r = 0; r<rows; r++)
				for (var c = 0; c<columns; c++) {
				var v0 = r * (columns + 1) + c;
				var v1 = v0 + columns + 1;
				var v2 = v1 + 1;
				var v3 = v0 + 1;
				plane.VertexList.Add (
					plane.VertexList [v0] * 0.25f + plane.VertexList [v1] * 0.25f +
					plane.VertexList [v2] * 0.25f + plane.VertexList [v3] * 0.25f
				);
				var vc = plane.VertexList.Count - 1;


				plane.Faces.Add (new Face (v0, v1, vc));
				plane.Faces.Add (new Face (v1, v2, vc));
				plane.Faces.Add (new Face (v2, v3, vc));
				plane.Faces.Add (new Face (v3, v0, vc));
			}


			return plane;
		}
		public static Mesh CreatePlane2(float width, float height, int rows,int columns){
			if (rows == columns && rows == 0)
				return null;
			var plane = new Mesh ();
			var dw = width / rows;
			var dh = height / columns;


			for (var r = 0; r<rows+1; r++)
				for (var c = 0; c<columns+1; c++)
					plane.VertexList.Add (new Vec3 (c*dw, r*dh , 0.0f));

			for (var r = 0; r<rows; r++)
				for (var c = 0; c<columns; c++) {
					var v0 = r * (columns + 1) + c;
					var v1 = v0 + columns + 1;
					var v2 = v1 + 1;
					var v3 = v0 + 1;

					plane.Faces.Add (new Face (v0, v1, v2));
					plane.Faces.Add (new Face (v0, v2, v3));
				}


			return plane;
		}

		public static Mesh CreateSphere(float radius, float rings, float segments){

			double s, t;

			var list = new List<Vec3> ();

			for(var i = 0; i < rings; i++){
				for(var j = 0; j < segments; j++){
					s = (float)i * 2 * Math.PI / rings;
					t = (float)j * 2 * Math.PI / segments;
					list.Add(new Vec3(
						(float)(radius * Math.Sin(s) * Math.Cos(t)),
						(float)(radius * Math.Sin(s) * Math.Sin(t)), 
						(float)(radius * Math.Cos(s))
					));

					s = (double)(i + 1) * 2 * Math.PI / rings;
					list.Add( new Vec3(
						(float)(radius * Math.Sin(s) * Math.Cos(t)),
					        (float)(radius * Math.Sin(s) * Math.Sin(t)), 
					        (float)(radius * Math.Cos(s))
					));

				}
			}

			var mesh = new Mesh ();
			mesh.VertexList = list;
			for(var i = 0; i<list.Count-2; i++)
			{
				mesh.Faces.Add (new Face (i, i + 1, i + 2));
			}

			return mesh;


		}
	}
}

