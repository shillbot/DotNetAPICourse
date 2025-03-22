namespace DotnetAPI.Models;

public partial class Post
{
	public int PostId { get; set; }
	public int UserId { get; set; }
	public string PostTitle { get; set; }
	public string PostContent { get; set; }
	public DateTime PostDate { get; set; }
	public DateTime PostTime { get; set; }

	public Post()
	{
		PostTitle ??= string.Empty;
		PostContent ??= string.Empty;
	}
}
