using System;
using System.Data;
using System.Text.RegularExpressions;
using Dapper;
using HelloWorld.Models;
using Microsoft.Data.SqlClient;

namespace HelloWorld
{
	internal class Program
	{
		static void Main(string[] args)
		{
			string connectionString = "Server=localhost; Database=DotNetCourseDatabase; TrustServerCertificate=true; Trusted_Connection=false; User Id=sa; Password=SQLConnect1!";
			IDbConnection dbConnection = new SqlConnection(connectionString);

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