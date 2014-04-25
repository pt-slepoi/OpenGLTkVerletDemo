using System;

using OpenTK.Graphics.OpenGL;
using OpenTK;
using MathTools;

namespace OpenGLTkVerletDemo
{

	public partial class GLWindow : OpenTK.GameWindow
	{
		int program;
		MouseTrackBall trackball;
		int mvpLocation = -1;
//		int colorLocation = -1;
		IMatrixStack mvp;
		bool wireframeOn = true;
		VerletMesh cloth;

		bool appliedForce = false;
		bool pause = false;
	
	

		protected override void OnResize (EventArgs e)
		{
			base.OnResize (e);
			GL.Viewport(0, 0, Width, Height);
		}

		protected override void OnLoad (EventArgs args)
		{
			base.OnLoad (args);
			GL.Viewport(0, 0, Width, Height);
			//GL.ClearColor (0.0f, 0.66f, 0.93f, 1.0f);
			GL.ClearColor (1.0f, 1.0f, 1.0f, 1.0f);

			VSync = OpenTK.VSyncMode.On;
			InitProgram ();
			mvp = new IMatrixStack ();
			Reset ();
			ResetTrackBall ();
		

			KeyPress += (s, e) => {

				switch (e.KeyChar.ToString ().ToLower ()) {
				case "w":
					{
						wireframeOn = !wireframeOn;
						break;
					}
				case "s":
					{
						if (appliedForce)
							break;
						cloth.ApplyForce (clothForce);
						appliedForce = true;
						break;
					}
				case "r":
					{
						Reset ();
						break;}
				case "c":
					{
						ResetTrackBall ();
						break;
					}

				}
			};

		}
		void ResetTrackBall(){
			trackball = new MouseTrackBall(this){
				distance = 10.0f, 
				rx = 0.0f,
				ry = 0.0f
			};
		}

		void Reset(){
			Console.WriteLine ("Resetting");
			InitCloth ();
			InitSphere ();
			appliedForce = false;
			elapsedTime = 0;
			pause = false;
		}

		protected override void OnRenderFrame (OpenTK.FrameEventArgs e)
		{
			base.OnRenderFrame (e);
			GL.Clear (ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
			if (wireframeOn)
				GL.PolygonMode (MaterialFace.FrontAndBack, PolygonMode.Line);
			else
				GL.PolygonMode (MaterialFace.FrontAndBack, PolygonMode.Fill);

			mvp.SetIdentity ();

			float aspectRatio = (float) ((1.0f * this.Width) / this.Height);

			//fixing aspect ratio
			if (aspectRatio < 1.0)
				mvp.MultMatrix(IMatrix.Scaling(1.0f, aspectRatio, 0.5f));
			else
				mvp.MultMatrix(IMatrix.Scaling(1/aspectRatio,1, 0.5f));

			//perspective
			mvp.MultMatrix(IMatrix.Perspective (1.0f, 0.2f, 20.0f));

			//view
			mvp.MultMatrix(trackball.InvertibleTransform ());
			SendUniforms ();


			//from now on... model!
			//var sphereModel = IMatrix.Translation (sphereCenter.X, sphereCenter.Y, sphereCenter.Z);
			mvp.Push ();
			mvp.MultMatrix (sphereModel);
			SendUniforms ();
			DrawSphere (sphereRadius, 32, 16);

			//DrawSphere (sphereRadius, 5, 5);
			mvp.Pop ();

			mvp.Push ();



			mvp.MultMatrix (clothModel);

			SendUniforms ();

			var sc = new Vec3 (sphereModel.Direct.Dot (new Vec4 (0, 0, 0, 1)));
			cloth.UpdatePosition ((float)elapsedTime);

			cloth.FixCollisionWithSphere (sc, sphereRadius,clothModel);
			cloth.ApplyConstraints ();
			DrawMesh (cloth);
			mvp.Pop ();

			mvp.Push ();
			SendUniforms ();
			DrawBigPoint ();
			mvp.Pop ();

			mvp.Push ();
			mvp.MultMatrix (IMatrix.Translation (5, 5, 5));
			SendUniforms ();
			DrawBigPoint ();
			mvp.Pop ();

			mvp.Push ();
			mvp.MultMatrix (IMatrix.Translation (50, 50, 7));
			SendUniforms ();
			DrawBigPoint ();
			mvp.Pop ();
			SwapBuffers ();


		}
		double deltaTime = 0;
		double elapsedTime = 0;
		protected override void OnUpdateFrame (FrameEventArgs e)
		{
			base.OnUpdateFrame (e);
			deltaTime = e.Time - (deltaTime);
			if(!pause)
				elapsedTime += e.Time;


		}

		void DrawBigPoint(){
			GL.PointSize (15);
			GL.Begin (BeginMode.Points);
			GL.Vertex3 (0, 0, 0);
			GL.End ();
		}

		void SendUniforms ()
		{
			GL.UniformMatrix4 (mvpLocation, 1, false, mvp.Top().Direct.ToFloatArray ());
		}

		void InitProgram(){
			//creating program
			program = GL.CreateProgram ();

			//creating shaders
			var vertex = GL.CreateShader (ShaderType.VertexShader);
			var fragment = GL.CreateShader (ShaderType.FragmentShader);

			GL.ShaderSource (vertex, Shaders.Vertex0);
			GL.ShaderSource (fragment, Shaders.Fragment0);

			GL.CompileShader (vertex);
			Console.WriteLine(string.Format("Vertex shader info : {0}",GL.GetShaderInfoLog(vertex)));
		
			GL.CompileShader (fragment);
			Console.WriteLine(string.Format("Fragment shader info : {0}",GL.GetShaderInfoLog (fragment)));
			Console.WriteLine(string.Format("Program Info: {0}",GL.GetProgramInfoLog(program)));
			GL.AttachShader (program, vertex);
			GL.AttachShader (program, fragment);

			//binding program
			GL.LinkProgram (program);
			GL.UseProgram (program);

			//getting locations indexes
			mvpLocation = GL.GetUniformLocation (program, "mvp");
//			colorLocation = GL.GetUniformLocation( program, "color");


		}

		Vec3 clothForce;
		float clothWidth;
		float clothHeight;
		IMatrix clothModel;
		void InitCloth(){
			clothForce = new Vec3(0,0,+1).Mult(0.0005f).Normalized();
			cloth = new VerletMesh (Mesh.CreatePlane (clothWidth=100,clothHeight=100, 50, 50));
			cloth.Mass= (0.05f);
			clothModel =  IMatrix.Translation (0, 7.0f, 0.0f).Dot (IMatrix.RotationX (-90)).Dot (IMatrix.Translation (-clothWidth/2, -clothHeight/2, 0.0f));
		}

		IMatrix sphereModel;
		float sphereRadius;
	
		void InitSphere(){
			sphereModel = IMatrix.Identity ();
			sphereModel = sphereModel.Dot (IMatrix.Translation (0, 0, 0));
			sphereRadius = 5;
		}

	
	}
}

