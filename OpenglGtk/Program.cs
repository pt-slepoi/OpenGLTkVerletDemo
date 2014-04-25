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
			//Application.Init ();

			//MainWindow win = new MainWindow ();
			using (var game = new GLWindow()) {
				game.Run (200.0);
			}

			//win.Show ();

		//	Application.Run ();
		}


	}
}
