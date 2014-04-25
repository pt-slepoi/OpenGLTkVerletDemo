using System;
using System.IO;


namespace OpenGLTkVerletDemo
{
	public class Shaders
	{
		public static string Vertex0;

		public static string Fragment0;

		static Shaders(){
			var folder = "./Shaders/";

			Vertex0 = new StreamReader (folder+"VertexShader.glsl").ReadToEnd();
			Fragment0 = new StreamReader (folder+"FragmentShader.glsl").ReadToEnd();
		}
	}
}

