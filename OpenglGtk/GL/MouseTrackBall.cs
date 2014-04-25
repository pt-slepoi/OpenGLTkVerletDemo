using System;
using MathTools;
using OpenTK.Input;
using OpenTK;
namespace OpenGLTkVerletDemo
{
	public class MouseTrackBall
	{
		public float distance { get; set; }
		public float rx { get; set; }
		public float ry {get;set;}
		float zSpeed = 1.0f;
		float xySpeed = 0.5f;
		int lastX = 0;
		int lastY = 0;
		bool isDown=false;
		public Mat4 transform(){
			return Mat4.Identity() * (Mat4.Translation (0, 0, -distance)  * Mat4.RotatationX (ry) * Mat4.RotatationY (rx));
		}

		public IMatrix InvertibleTransform(){
			return IMatrix.Identity() * (IMatrix.Translation (0, 0, -distance) * IMatrix.RotationX (ry) * IMatrix.RotationY (rx));
		}
		public MouseTrackBall (GameWindow gm)
		{
			gm.Mouse.WheelChanged += (s,e) =>{

				distance+=e.Delta*zSpeed;
			};

			gm.Mouse.ButtonDown += (s, e) => {
				if(e.Button != OpenTK.Input.MouseButton.Middle && e.Button != OpenTK.Input.MouseButton.Right)
					return;
				isDown = true;
				lastX = e.X;
				lastY = e.Y;
			//	Console.WriteLine(string.Format("Mouse Left Click on ({0},{1}):",lastX,lastY));
			};

			gm.Mouse.ButtonUp += (s, e) => {
				if(e.Button != OpenTK.Input.MouseButton.Middle && e.Button != OpenTK.Input.MouseButton.Right)
				return;
				isDown = false;
			};

			gm.Mouse.Move += (s,e) => {
				if(!isDown) return;
				move(e);
			//	gm.SwapBuffers();
			};
		}

	 void move(MouseMoveEventArgs e){
			var dx = e.X - lastX;
			var dy = e.Y - lastY;
			lastX = e.X;
			lastY = e.Y;
			rx -= dx*xySpeed;
			ry -= dy*xySpeed;
			if(ry>=89)
				ry=89;
			if(ry<=-89)
				ry=-89;
	}
	}
}

