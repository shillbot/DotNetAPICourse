using System.Runtime.Serialization;
using System.Text.Json;
using HelloWorld.Data;
using HelloWorld.Models;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace HelloWorld
{
	internal class Program
	{
		static void Main(string[] args)
		{
			IConfiguration config = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
			DataContextDapper dapper = new DataContextDapper(config);

			string computersJson = File.ReadAllText("Computers.json");
			// Console.WriteLine(computersJson);

			// JsonSerializerOptions options = new JsonSerializerOptions() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
			// IEnumerable<Computer>? computers = JsonSerializer.Deserialize<IEnumerable<Computer>>(computersJson, options);
			IEnumerable<Computer>? computers = JsonConvert.DeserializeObject<IEnumerable<Computer>>(computersJson, new IsoDateTimeConverter { DateTimeFormat = "yyyy-MM-dd" });

			if (computers != null)
			{
				foreach (Computer myComputer in computers)
				{
					string sql = @"
					INSERT INTO TutorialAppSchema.Computer (
						Motherboard,
						HasWifi,
						HasLTE,
						ReleaseDate,
						Price,
						VideoCard
					) VALUES ('" +
						escSingleQuote(myComputer.Motherboard) + "', '" +
						myComputer.HasWifi + "', '" +
						myComputer.HasLTE + "', '" +
						myComputer.ReleaseDate?.ToString("yyyy-MM-dd") + "', '" +
						myComputer.Price + "', '" +
						escSingleQuote(myComputer.VideoCard)
					+ "')";

					dapper.ExecuteSql(sql);
				}
			}

			// string computersCopy = System.Text.Json.JsonSerializer.Serialize(computers, options);
			// File.WriteAllText("log.txt", computersCopy);

			// JsonSerializerSettings settings = new JsonSerializerSettings() { ContractResolver = new CamelCasePropertyNamesContractResolver() };
			// string computersCopyNS = JsonConvert.SerializeObject(computers, settings);
			// File.WriteAllText("logNS.txt", computersCopyNS);

		}

		static string escSingleQuote(string sql)
		{
			string output = sql.Replace("'", "''");
			return output;
		}

	}
}