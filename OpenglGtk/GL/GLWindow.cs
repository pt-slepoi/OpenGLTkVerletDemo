using System;

using OpenTK.Graphics.OpenGL;
using OpenTK;
using MathTools;
using System.Collections.Generic;
using OpenTK.Input;

namespace OpenGLTkVerletDemo
{

	public partial class GLWindow : OpenTK.GameWindow
	{
		int program;
		MouseTrackBall trackball;
		int mvpLocation = -1;
		int distanceLocation = -1;
		int useColorLocation = -1;
		int colorLocation = -1;
		int modeViewLocation = -1;

		bool useColor;
	
		IMatrixStack model;
		IMatrix view;
		IMatrix proj;
		bool wireframeOn = true;
		VerletMesh cloth;
		List<Sphere> spheres;
		List<Capsule> capsules = new List<Capsule> ();

		bool appliedForce = false;
		bool pause = false;
		bool useLines = true;
	

		protected override void OnResize (EventArgs e)
		{
			base.OnResize (e);
			GL.Viewport(0, 0, Width, Height);
		}

		protected override void OnLoad (EventArgs args)
		{
			base.OnLoad (args);
			GL.Viewport(0, 0, Width, Height);

			GL.ClearColor (0.0f, 0.0f, 0.0f,0.0f);
			GL.Enable (EnableCap.DepthTest);
			GL.Enable (EnableCap.Blend);
			GL.BlendFunc (BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
			GL.PointSize(2.5f);
			VSync = OpenTK.VSyncMode.Off;
			InitProgram ();
		
			model = new IMatrixStack ();
			view = new IMatrix ();
			proj = new IMatrix ();
			Reset ();
			ResetTrackBall ();
			useColor = false;

			Keyboard.KeyDown += (s, e) => {
				switch(e.Key){
					case Key.W:
					{
						wireframeOn = !wireframeOn;
						break;
					}
				case Key.S:
					{
						if (appliedForce)
							break;
						cloth.ApplyForce (clothForce);
						appliedForce = true;
						break;
					}
				case Key.T:
				{
					useLines = !useLines;
					break;
				}
				case Key.R:
					{
						Reset ();
						break;
					}
				case Key.X:
						{
							ResetTrackBall ();
							break;
						}
				case Key.P:
					{
						pause = !pause;
						break;
					}
				case Key.A:
					{
						animateSphere=!animateSphere;
						break;
					}
				case Key.C:
				{
					useColor = !useColor;
					Console.WriteLine("using color");
					break;
				}
				case Key.Number1:
					{
					Reset();
						Type1();
						break;
					}
				case Key.Number2:
					{
					Reset();
						Type2();
						break;
					}
				case Key.Number3:
					{
					Reset();
					Type3();

						break;
					}
				case Key.Number4:
				{
					Reset();
					Type4();

					break;
				}
				case Key.Number5:
				{
					Reset();
					Type5();

					break;
				}
				case Key.Number6:
				{
					Reset();
					Type6();

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
			animateSphere = false;
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

			proj = new IMatrix();

			float aspectRatio = (float) ((1.0f * this.Width) / this.Height);

			//fixing aspect ratio
			if (aspectRatio < 1.0)
				proj = IMatrix.Scaling(1.0f, aspectRatio, 0.5f);
			else
				proj = IMatrix.Scaling(1/aspectRatio,1, 0.5f);

			//perspective
			proj = proj.Dot(IMatrix.Perspective (1.0f, 0.2f, 20.0f));


			//view
			view = trackball.InvertibleTransform ();

	


			model.SetIdentity ();
			//from now on... model!
			foreach (var sphere in spheres) {

				model.Push ();
				model.MultMatrix (sphere.Model);
			
				SendUniforms (sphere.Color);
				DrawSphere (sphere.Radius, sphere.Rings, sphere.Segments);
				model.Pop ();
			}

			foreach (var capsule in capsules) {
				DrawCapsule1 (capsule);
			}


			model.Push ();
			model.MultMatrix (cloth.Model);
			SendUniforms (new Vec4(0.5f,1,0.3f,1.0f));
			DrawMesh (cloth);
			model.Pop ();

			SwapBuffers ();
			var N = 10;
			for (var i = 0; i<N; i++) {
				doPhysics ((float)e.Time/N);
			}

		}

		void DrawCapsule1(Capsule capsule){
			model.Push ();
			model.MultMatrix (capsule.Model);
			SendUniforms (capsule.Color);
			DrawLine (capsule.PointA, capsule.PointB);
			model.Push ();
			model.MultMatrix (IMatrix.Translation (capsule.PointA.X, capsule.PointA.Y,capsule.PointA.Z));
			SendUniforms ();
			DrawSphere (capsule.Radius, 32, 16);
			model.Pop ();
			model.MultMatrix (IMatrix.Translation (capsule.PointB.X, capsule.PointB.Y,capsule.PointB.Z));
			SendUniforms ();
			DrawSphere (capsule.Radius, 32, 16);
			model.Pop ();
			model.Pop ();
		}

		void DrawCapsule2(Capsule capsule){
			model.Push ();
			model.MultMatrix (capsule.Model);
			SendUniforms (capsule.Color);
			var a = capsule.PointA;
			var b = capsule.PointB;
			var r = capsule.Radius;
			DrawLine (new Vec3(a.X,a.Y,a.Z), new Vec3(b.X,b.Y,b.Z));

			model.Pop ();
		}

		float time = 0;
		bool animateSphere;

		protected override void OnUpdateFrame (FrameEventArgs e)
		{
			//base.OnUpdateFrame (e);

	
		


		}

		void doPhysics(float dt){

			if(!pause)
				time += dt;

			if (animateSphere)
				AnimateSpheres ();
		


				cloth.PhysicStep (dt);
				foreach (var sphere in spheres)
					cloth.FixCollisionWithSphere (new Vec3 (sphere.Model.Direct.Dot (new Vec4 (0, 0, 0, 1))), sphere.Radius);
				foreach (var capsule in capsules)
					cloth.FixCollisionWithCapsule (capsule.PointA, capsule.PointB, capsule.Radius);
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
			var mv = view.Dot (model.Top ());
			var mvp = proj.Dot (mv);
			GL.UniformMatrix4 (mvpLocation, 1, false, mvp.Direct.ToFloatArray ());
			GL.UniformMatrix4 (modeViewLocation, 1, false, mv.Direct.ToFloatArray ());
			GL.Uniform1 (distanceLocation,trackball.distance);
			GL.Uniform1 (useColorLocation, useColor ? 1:0);
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
			distanceLocation = GL.GetUniformLocation( program, "distance");
			useColorLocation = GL.GetUniformLocation( program, "useColor");
			modeViewLocation = GL.GetUniformLocation( program, "mv");

		}

		Vec3 clothForce;
		float clothWidth;
		float clothHeight;
		List<VertexConstraint> clothConstraints = null;
		void InitCloth(){
			clothForce = new Vec3(0,0,+1).Mult(5.0f);
			cloth = new VerletMesh (Mesh.CreatePlane (clothWidth=50,clothHeight=50, 20, 20));
			if (clothConstraints == null) {
				cloth.GenerateVertexConstraintsFromFaces ();
				clothConstraints = cloth.VertexConstraints;
			} else
				cloth.VertexConstraints = clothConstraints;
			cloth.Mass= (0.05f);
			cloth.Model =  IMatrix.Translation (0, 10.0f, 0.0f).Dot (IMatrix.RotationX (-90)).Dot (IMatrix.Translation (-clothWidth/2, -clothHeight/2, 0.0f));


		}

		void InitSphere(){
			spheres = new List<Sphere> ();
			Type3 ();
		}

		void Type1(){
			capsules.Clear ();
			spheres.Clear ();
			spheres.Add(new Sphere () { Model = IMatrix.Translation(10,0,0)  });
			spheres.Add(new Sphere () { Model = IMatrix.Translation(-10,0,0) });
			spheres.Add(new Sphere () { Model = IMatrix.Translation(0,0,10)  });
			spheres.Add(new Sphere () { Model = IMatrix.Translation(0,0,-10) });
			spheres.Add(new Sphere () { Model = IMatrix.Translation(0,5.0f,0), Animated = false});
		}

		void Type2(){
			capsules.Clear ();
			spheres.Clear ();
			spheres.Add(new Sphere () { Model = IMatrix.Translation(10,0,0) ,Radius = 3.5f });
			spheres.Add(new Sphere () { Model = IMatrix.Translation(-10,0,0) ,Radius = 3.5f });
			spheres.Add(new Sphere () { Model = IMatrix.Translation(0,0,10) ,Radius = 3.5f });
			spheres.Add(new Sphere () { Model = IMatrix.Translation(0,0,-10) ,Radius = 3.5f});
			spheres.Add(new Sphere () { Model = IMatrix.Translation(10,0,10) ,Radius = 3.5f });
			spheres.Add(new Sphere () { Model = IMatrix.Translation(-10,0,-10) ,Radius = 3.5f });
			spheres.Add(new Sphere () { Model = IMatrix.Translation(10,0,-10) ,Radius = 3.5f });
			spheres.Add(new Sphere () { Model = IMatrix.Translation(-10,0,10) ,Radius = 3.5f});
	
		}

		void Type3(){
			spheres.Clear ();
			capsules.Clear ();
			spheres.Add(new Sphere () { Model = IMatrix.Translation(0,-5.0f,0) , Radius = 10.0f, Animated = false});
		}

		
		void Type4(){
			spheres.Clear ();
			capsules.Clear ();
			spheres.Add(new Sphere () { Model = IMatrix.Translation(10,0,10) ,Radius = 3.5f });
			spheres.Add(new Sphere () { Model = IMatrix.Translation(-10,0,-10) ,Radius = 3.5f });
			spheres.Add(new Sphere () { Model = IMatrix.Translation(10,0,-10) ,Radius = 3.5f });
			spheres.Add(new Sphere () { Model = IMatrix.Translation(-10,0,10) ,Radius = 3.5f});

			spheres.Add(new Sphere () { Model = IMatrix.Translation(15,-10,15) ,Radius = 5.0f });
			spheres.Add(new Sphere () { Model = IMatrix.Translation(-15,-10,-15) ,Radius = 5.0f });
			spheres.Add(new Sphere () { Model = IMatrix.Translation(15,-10,-15) ,Radius = 5.0f });
			spheres.Add(new Sphere () { Model = IMatrix.Translation(-15,-10,15) ,Radius = 5.0f});
		}

		void Type5(){
			spheres.Clear ();
			capsules.Clear ();
			var w = 8;
			var h = 15;

			capsules.Add(new Capsule(){PointA = new Vec3(w,0,w), PointB = new Vec3(w,0,-w)});
			capsules.Add(new Capsule(){PointA = new Vec3(-w,0,w), PointB = new Vec3(-w,0,-w)});

			capsules.Add(new Capsule(){PointA = new Vec3(-w,0,w), PointB = new Vec3(w,0,w)});
			capsules.Add(new Capsule(){PointA = new Vec3(-w,0,-w), PointB = new Vec3(w,0,-w)});

			//legs
			capsules.Add(new Capsule(){PointA = new Vec3(w,0,w), PointB = new Vec3(w,-h,w)});
			capsules.Add(new Capsule(){PointA = new Vec3(-w,0,-w), PointB = new Vec3(-w,-h,-w)});
			capsules.Add(new Capsule(){PointA = new Vec3(w,0,-w), PointB = new Vec3(w,-h,-w)});
			capsules.Add(new Capsule(){PointA = new Vec3(-w,0,w), PointB = new Vec3(-w,-h,w)});



		}

		void Type6(){
			spheres.Clear ();
			capsules.Clear ();
			//capsules.Add(new Capsule(){PointA = new Vec3(10,0,5), PointB = new Vec3(10,0,-10)});
			capsules.Add(new Capsule(){PointA = new Vec3(-8,0,8), PointB = new Vec3(-8,-10,8)});


		}


		void AnimateSpheres(){
			foreach (var s in spheres) {
				if (!s.Animated)
					continue;
				s.Model = IMatrix.RotationY(2.0f).Dot(s.Model);
			}
		}
		void DrawLine(Vec3 pointA, Vec3 pointB){
				GL.Begin (BeginMode.Lines);
				GL.Vertex3 (pointA.X, pointA.Y, pointA.Z);
				GL.Vertex3 (pointB.X, pointB.Y, pointB.Z);
				GL.End ();
		}
	
	}

	class Sphere {
		public IMatrix Model { get; set; }
		public float Radius { get; set; }
		public int Rings { get; set; }
		public int Segments { get; set; }
		public Vec4 Color { get; set; }
		public bool Animated { get; set; }
		public Sphere(){
			Rings = 32;
			Radius = 4.0f;
			Segments = 16;
			Color = new Vec4 (1.0f, 0, 0, 0.5f);
			Animated = true;
		}

	}

	class Capsule {
		public IMatrix Model {get;set;}
		public float Radius { get; set; }
		public Vec3 PointA { get; set; }
		public Vec3 PointB { get; set; }
		public Vec4 Color { get; set; }
		public Capsule(){
			Radius = 2.0f;
			Color = new Vec4 (0.8f, 0.3f, 0.5f, 1.0f);
			Model = IMatrix.Identity ();
		}
	}




}

