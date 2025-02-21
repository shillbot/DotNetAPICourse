using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelloWorld.Models
{
	public class ComputerSnake
	{
		[Key]
		public int computer_id { get;        set; }
		public string    motherboard  { get; set; }
		public int?      cpu_cores    { get; set; }
		public bool      has_wifi     { get; set; }
		public bool      has_lte      { get; set; }
		public DateTime? release_date { get; set; }
		public decimal   price        { get; set; }
		public string    video_card   { get; set; }

		public ComputerSnake()
		{
			video_card ??= "";
			motherboard ??= "";
			cpu_cores ??= 0;
		}
	}
}
