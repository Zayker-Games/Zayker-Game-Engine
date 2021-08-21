using System;
//using Utility;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;

public class Program
{
    //public static IWindow window;
	public static void Main()
	{
		var options = WindowOptions.Default;
		options.Size = new Silk.NET.Maths.Vector2D<int>(500, 500);
		options.Title = "LearnOpenGL with Silk.NET";
		window = Silk.NET.Windowing.Window.Create(options);
		
		window.Run();
		
		System.Console.WriteLine("Hey");
		System.Console.ReadLine();
	}
}