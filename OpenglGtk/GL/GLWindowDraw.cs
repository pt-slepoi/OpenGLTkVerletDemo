using System;
using MathTools;
using OpenTK.Graphics.OpenGL;

namespace OpenGLTkVerletDemo
{
	public partial class GLWindow
	{
		void DrawCube()
		{

			GL.Begin(BeginMode.LineLoop );


			GL.Vertex3(-1.0, 1.0, -1.0);
			GL.Vertex3(1.0, 1.0, -1.0);
			GL.Vertex3(1.0, -1.0, -1.0);

			GL.Vertex3(-1.0, -1.0, -1.0);
			GL.Vertex3(-1.0, -1.0, 1.0);
			GL.Vertex3(-1.0, 1.0, 1.0);
			GL.Vertex3(-1.0, 1.0, -1.0);

			GL.End();



		}

		void DrawSphere(float radius, int rings, int segments){
			int i, j;
			double s, t;
		
			GL.Begin(BeginMode.TriangleStrip);

			for(i = 0; i < rings; i++){
				for(j = 0; j < segments; j++){
					s = (double)i * 2 * Math.PI / rings;
					t = (double)j * 2 * Math.PI / segments;
					GL.Vertex3(
						radius * Math.Sin(s) * Math.Cos(t),
						radius * Math.Sin(s) * Math.Sin(t), 
						radius * Math.Cos(s)
						);

					s = (double)(i + 1) * 2 * Math.PI / rings;
					GL.Vertex3(
						radius * Math.Sin(s) * Math.Cos(t),
						radius * Math.Sin(s) * Math.Sin(t), 
						radius * Math.Cos(s)
						);

				}
			}

			GL.End();
		}

		void DrawPlane(float width,float height, int rows, int columns){
			if (rows == columns && rows == 0)
				return;

			var dw = width / rows;
			var dh = height / columns;
			int ff;
			GL.GetInteger (GetPName.FrontFace, out ff);

			GL.FrontFace (FrontFaceDirection.Cw);

			for (var r=0; r < rows; r++) {
				GL.Begin (BeginMode.TriangleStrip);
				for (var c = 0; c<columns; c++) {
					GL.Vertex3 (c * dw, r * dh, 0.0f);
					GL.Vertex3 (c * dw, (r * dh)+dh, 0.0f);
				}

				GL.End ();

			}

			var ffd = ((int)FrontFaceDirection.Cw) == ff ? FrontFaceDirection.Cw : FrontFaceDirection.Ccw; 
			GL.FrontFace(ffd);
		}


		void DrawMesh(Mesh m){
//			var c = 0;
			foreach (var f in m.Faces) {
				GL.Begin (BeginMode.Triangles);
				//				Console.Write (string.Format ("Face {0} [ ", c++)); 
				for (var i =0; i<Face.N; i++) {
					var v = m.VertexList [f.posIndex [i]];
					GL.Vertex3 (v.X, v.Y, v.Z);
					//					Console.Write (string.Format (" ({0},{1},{2}) ",v.X,v.Y,v.Z));
				}
				//				Console.WriteLine(" ] ");

				GL.End ();
			}
		}

		void DrawAxes(){
			GL.Begin (BeginMode.Lines);
			GL.Vertex3 (0.0f, 0.0f, 0.0f);
			GL.Vertex3 (2.0f, 0.0f, 0.0f);

			GL.Vertex3 (0.0f, 0.0f, 0.0f);
			GL.Vertex3 (0.0f, 2.0f, 0.0f);

			GL.Vertex3 (0.0f, 0.0f, 0.0f);
			GL.Vertex3 (0.0f, 0.0f, 2.0f);
			GL.End ();

		}
	}
}

