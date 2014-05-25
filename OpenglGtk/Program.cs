using System;
using Gtk;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;


namespace OpenGLTkVerletDemo
{
	class MainClass
	{
		[STAThread]
		public static void Main (string[] args)
		{
			using (var game = new GLWindow()) {

				game.Run (100.0,200.0);
			
			}

		}


	}
}
