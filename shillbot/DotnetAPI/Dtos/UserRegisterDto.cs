namespace DotnetAPI.Dtos;

public partial class UserRegisterDto
{
	public string FirstName { get; set; }
	public string LastName { get; set; }
	public string Email { get; set; }
	public string Gender { get; set; }
	public string Password { get; set; }
	public string PasswordConfirm { get; set; }

	public UserRegisterDto()
	{
		FirstName ??= string.Empty;
		LastName ??= string.Empty;
		Email ??= string.Empty;
		Gender ??= string.Empty;
		Password ??= string.Empty;
		PasswordConfirm ??= string.Empty;
	}
}