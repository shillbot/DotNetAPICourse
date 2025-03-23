namespace DotnetAPI.Dtos;

public partial class PostAddDto
{
	public string PostTitle { get; set; }
	public string PostContent { get; set; }

	public PostAddDto()
	{
		PostTitle ??= string.Empty;
		PostContent ??= string.Empty;
	}
}