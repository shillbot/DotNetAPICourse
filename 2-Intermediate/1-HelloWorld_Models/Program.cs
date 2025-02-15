using HelloWorld.Data;
using HelloWorld.Models;

namespace HelloWorld
{
	internal class Program
	{
		static void Main(string[] args)
		{
			DataContextDapper dapper = new DataContextDapper();

			Computer myComputer = new Computer()
			{
				Motherboard = "Z690",
				HasWifi = true,
				HasLTE = false,
				ReleaseDate = DateTime.Now,
				Price = 943.87m,
				VideoCard = "RTX 2060"
			};

			string sql = @"INSERT INTO TutorialAppSchema.Computer (
				Motherboard,
				HasWifi,
				HasLTE,
				ReleaseDate,
				Price,
				VideoCard
			) VALUES ('" +
				myComputer.Motherboard + "', '" +
				myComputer.HasWifi + "', '" +
				myComputer.HasLTE + "', '" +
				myComputer.ReleaseDate.ToString("yyyy-MM-dd HH:mm:ss.fff") + "', '" +
				myComputer.Price + "', '" +
				myComputer.VideoCard
			+ "')";
			Console.WriteLine("SQL Statement: " + sql);

			bool result = dapper.ExecuteSql(sql);
			// Console.WriteLine("Rows Affected: " + result);

			string sqlSelect = @"
				SELECT tasc.Motherboard,
					tasc.HasWifi,
					tasc.HasLTE,
					tasc.ReleaseDate,
					tasc.Price,
					tasc.VideoCard
				FROM TutorialAppSchema.Computer AS tasc"
			;

			IEnumerable<Computer> computers = dapper.LoadData<Computer>(sqlSelect);
			foreach (Computer computer in computers)
			{
				Console.WriteLine("'" +
					computer.Motherboard + "', '" +
					computer.HasWifi + "', '" +
					computer.HasLTE + "', '" +
					computer.ReleaseDate + "', '" +
					computer.Price + "', '" +
					computer.VideoCard
				+ "'");
			}

			// myComputer.HasWifi = false;
			// Console.WriteLine(myComputer.Motherboard);
			// Console.WriteLine(myComputer.HasWifi);
			// Console.WriteLine(myComputer.ReleaseDate);
			// Console.WriteLine(myComputer.VideoCard);
		}

	}
}