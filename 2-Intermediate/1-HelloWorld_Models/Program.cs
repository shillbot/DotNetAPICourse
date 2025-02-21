using System.Text.Json;
using AutoMapper;
using HelloWorld.Data;
using HelloWorld.Models;
using Microsoft.Extensions.Configuration;

namespace HelloWorld
{
	internal class Program
	{
		static void Main(string[] args)
		{
			IConfiguration config = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
			DataContextDapper dapper = new DataContextDapper(config);

			string computersJson = File.ReadAllText("ComputersSnake.json");

			Mapper mapper = new Mapper(new MapperConfiguration(cfg =>
			{
				cfg.CreateMap<ComputerSnake, Computer>()
				   .ForMember(dest => dest.ComputerId,  options => options.MapFrom(src => src.computer_id))
				   .ForMember(dest => dest.Motherboard, options => options.MapFrom(src => src.motherboard))
				   .ForMember(dest => dest.CPUCores,    options => options.MapFrom(src => src.cpu_cores))
				   .ForMember(dest => dest.HasWifi,     options => options.MapFrom(src => src.has_wifi))
				   .ForMember(dest => dest.HasLTE,      options => options.MapFrom(src => src.has_lte))
				   .ForMember(dest => dest.ReleaseDate, options => options.MapFrom(src => src.release_date))
				   .ForMember(dest => dest.Price,       options => options.MapFrom(src => src.price))
				   .ForMember(dest => dest.VideoCard,   options => options.MapFrom(src => src.video_card));
			}));

			// JsonSerializerOptions options = new JsonSerializerOptions() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
			IEnumerable<ComputerSnake>? computerSystem = JsonSerializer.Deserialize<IEnumerable<ComputerSnake>>(computersJson);
			//IEnumerable<Computer>? computers = JsonConvert.DeserializeObject<IEnumerable<Computer>>(computersJson, new IsoDateTimeConverter { DateTimeFormat = "yyyy-MM-dd" });

			if (computerSystem != null)
			{
				IEnumerable<Computer> computerResult = mapper.Map<IEnumerable<Computer>>(computerSystem);

				foreach (var computer in computerResult)
				{
					Console.WriteLine("Motherboard: " + computer.Motherboard);
				}
			}

			//if (computers != null)
			//{
			//	foreach (Computer myComputer in computers)
			//	{
			//		string sql = @"
			//		INSERT INTO TutorialAppSchema.Computer (
			//			Motherboard,
			//			HasWifi,
			//			HasLTE,
			//			ReleaseDate,
			//			Price,
			//			VideoCard
			//		) VALUES ('" +
			//			escSingleQuote(myComputer.Motherboard) + "', '" +
			//			myComputer.HasWifi + "', '" +
			//			myComputer.HasLTE + "', '" +
			//			myComputer.ReleaseDate?.ToString("yyyy-MM-dd") + "', '" +
			//			myComputer.Price + "', '" +
			//			escSingleQuote(myComputer.VideoCard)
			//		+ "')";

			//		dapper.ExecuteSql(sql);
			//	}
			//}

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