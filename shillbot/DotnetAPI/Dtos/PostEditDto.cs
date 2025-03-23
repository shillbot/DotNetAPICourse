namespace DotnetAPI.Dtos;

public class PostEditDto
{
	public int PostId { get; set; }
	public string PostTitle { get; set; }
	public string PostContent { get; set; }

	public PostEditDto()
	{
		PostTitle ??= string.Empty;
		PostContent ??= string.Empty;
	}
}