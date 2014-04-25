using System;

using OpenTK.Graphics.OpenGL;
using OpenTK;
using MathTools;
using System.Collections.Generic;

namespace OpenGLTkVerletDemo
{

	public partial class GLWindow : OpenTK.GameWindow
	{
		int program;
		MouseTrackBall trackball;
		int mvpLocation = -1;
		int colorLocation = -1;
		IMatrixStack mvp;
		bool wireframeOn = true;
		VerletMesh cloth;
		List<Sphere> spheres;
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

			//GL.ClearColor (1.0f, 1.0f, 1.0f,0.0f);
			GL.ClearColor (0.0f, 0.0f, 0.0f,0.0f);

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
				case "p":
				{
					pause = !pause;
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
			foreach (var sphere in spheres) {
				mvp.Push ();
				mvp.MultMatrix (sphere.Model);
			
				SendUniforms (sphere.Color);
				DrawSphere (sphere.Radius, sphere.Rings, sphere.Segments);
				mvp.Pop ();
			}


			mvp.Push ();
			mvp.MultMatrix (cloth.Model);
			SendUniforms (new Vec4(0.5f,1,0.3f,1.0f));
			DrawMesh (cloth);
			mvp.Pop ();

			SwapBuffers ();


		}
		float time = 0;
	
		protected override void OnUpdateFrame (FrameEventArgs e)
		{
			base.OnUpdateFrame (e);

	
			if(!pause)
				time += (float)e.Time;

			AnimateSpheres();

			cloth.PhysicStep ((float)e.Time);
			foreach(var sphere in spheres)
				cloth.FixCollisionWithSphere (new Vec3 (sphere.Model.Direct.Dot (new Vec4 (0, 0, 0, 1))),sphere.Radius);
	
			cloth.ApplyConstraints ();

		}

		void DrawBigPoint(){
			GL.PointSize (15);
			GL.Begin (BeginMode.Points);
			GL.Vertex3 (0, 0, 0);
			GL.End ();
		}

		void SendUniforms (Vec4 color)
		{
			GL.UniformMatrix4 (mvpLocation, 1, false, mvp.Top().Direct.ToFloatArray ());
			GL.Uniform4 (colorLocation, 1, color.GetArray());
		}
		void SendUniforms ()
		{
			SendUniforms(new Vec4 (0.5f,0.5f,0.5f,1.0f));
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
			colorLocation = GL.GetUniformLocation( program, "color");


		}

		Vec3 clothForce;
		float clothWidth;
		float clothHeight;
		void InitCloth(){
			clothForce = new Vec3(0,0,+1).Mult(0.05f).Normalized();
			cloth = new VerletMesh (Mesh.CreatePlane (clothWidth=25,clothHeight=25, 30, 30));
			cloth.Mass= (0.05f);
			cloth.Model =  IMatrix.Translation (0, 10.0f, 0.0f).Dot (IMatrix.RotationX (-90)).Dot (IMatrix.Translation (-clothWidth/2, -clothHeight/2, 0.0f));
		}


		void InitSphere(){
			spheres = new List<Sphere> ();
			spheres.Add(new Sphere () { Model = IMatrix.Translation(10,0,0) });
			spheres.Add(new Sphere () { Model = IMatrix.Translation(-10,0,0) });
			spheres.Add(new Sphere () { Model = IMatrix.Translation(0,0,10) });
			spheres.Add(new Sphere () { Model = IMatrix.Translation(0,0,-10) });
			spheres.Add(new Sphere () { Model = IMatrix.Translation(0,2.5f,0) });
	
		}


		void AnimateSpheres(){
			foreach (var s in spheres) {
				s.Model = IMatrix.RotationY(2.0f).Dot(s.Model);
			}
		}

	
	}

	class Sphere {
		public IMatrix Model { get; set; }
		public float Radius { get; set; }
		public int Rings { get; set; }
		public int Segments { get; set; }
		public Vec4 Color { get; set; }
		public Sphere(){
			Rings = 32;
			Radius = 4.0f;
			Segments = 16;
			Color = new Vec4 (1.0f, 0, 0, 0.5f);
		}

	}
}

