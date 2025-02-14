using System;
using System.Text.RegularExpressions;
using HelloWorld.Models;

namespace HelloWorld
{
	internal class Program
	{
		static void Main(string[] args)
		{
			Computer myComputer = new Computer()
			{
				Motherboard = "Z690",
				HasWifi = true,
				HasLTE = false,
				ReleaseDate = DateTime.Now,
				Price = 943.87m,
				VideoCard = "RTX 2060"
			};
			myComputer.HasWifi = false;
			Console.WriteLine(myComputer.Motherboard);
			Console.WriteLine(myComputer.HasWifi);
			Console.WriteLine(myComputer.ReleaseDate);
			Console.WriteLine(myComputer.VideoCard);
		}

	}
}