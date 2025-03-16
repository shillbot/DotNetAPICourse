namespace DotnetAPI.Dtos;

public partial class UserRegisterDto
{
	public string Email { get; set; }
	public string Password { get; set; }
	public string PasswordConfirm { get; set; }

	public UserRegisterDto()
	{
		Email ??= "";
		Password ??= "";
		PasswordConfirm ??= "";
	}
}