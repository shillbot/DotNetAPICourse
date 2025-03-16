namespace DotnetAPI.Dtos;

public partial class UserLoginConfirmDto
{
	public byte[] PasswordHash { get; set; }
	public byte[] PasswordSalt { get; set; }

	public UserLoginConfirmDto()
	{
		PasswordHash ??= Array.Empty<byte>();
		PasswordSalt ??= Array.Empty<byte>();
	}
}