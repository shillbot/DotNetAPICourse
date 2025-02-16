using HelloWorld.Data;
using HelloWorld.Models;
using Microsoft.Extensions.Configuration;

namespace HelloWorld
{
	internal class Program
	{
		static void Main(string[] args)
		{
			IConfiguration config = new ConfigurationBuilder()
				.AddJsonFile("appsettings.json")
				.Build();

			DataContextDapper dapper = new DataContextDapper(config);
			DataContextEF entityFramework = new DataContextEF(config);

			Computer myComputer = new Computer()
			{
				Motherboard = "Z690",
				HasWifi = true,
				HasLTE = false,
				ReleaseDate = DateTime.Now,
				Price = 943.87m,
				VideoCard = "RTX 2060"
			};

			entityFramework.Add(myComputer);
			entityFramework.SaveChanges();
			Console.WriteLine("SAVED");

			// string sql = @"INSERT INTO TutorialAppSchema.Computer (
			// 	Motherboard,
			// 	HasWifi,
			// 	HasLTE,
			// 	ReleaseDate,
			// 	Price,
			// 	VideoCard
			// ) VALUES ('" +
			// 	myComputer.Motherboard + "', '" +
			// 	myComputer.HasWifi + "', '" +
			// 	myComputer.HasLTE + "', '" +
			// 	myComputer.ReleaseDate.ToString("yyyy-MM-dd HH:mm:ss.fff") + "', '" +
			// 	myComputer.Price + "', '" +
			// 	myComputer.VideoCard
			// + "')";
			// Console.WriteLine("SQL Statement: " + sql);

			// bool result = dapper.ExecuteSql(sql);
			// Console.WriteLine("Rows Affected: " + result);

			// string sqlSelect = @"
			// 	SELECT tasc.Motherboard,
			// 		tasc.HasWifi,
			// 		tasc.HasLTE,
			// 		tasc.ReleaseDate,
			// 		tasc.Price,
			// 		tasc.VideoCard
			// 	FROM TutorialAppSchema.Computer AS tasc"
			// ;
			// IEnumerable<Computer> computers = dapper.LoadData<Computer>(sqlSelect);
			IEnumerable<Computer>? computersEF = entityFramework.Computer?.ToList();
			if (computersEF != null)
			{
				foreach (Computer computer in computersEF)
				{
					Console.WriteLine("'" +
						computer.ComputerId + "', '" +
						computer.Motherboard + "', '" +
						computer.HasWifi + "', '" +
						computer.HasLTE + "', '" +
						computer.ReleaseDate + "', '" +
						computer.Price + "', '" +
						computer.VideoCard
					+ "'");
				}
			}

		}

	}
}