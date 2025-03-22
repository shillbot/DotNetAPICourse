using System.Data;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using DotnetAPI.Data;
using DotnetAPI.Dtos;
using DotnetAPI.Helpers;

namespace DotnetAPI.Controllers;

[Route("[controller]")]
[Authorize]
[ApiController]
public class AuthController(IConfiguration config) : Controller
{
	private readonly DataContextDapper _dapper = new(config);
	private readonly AuthHelper _authHelper = new(config);

	[AllowAnonymous]
	[HttpPost("Register")]
	public IActionResult Register(UserRegisterDto userRegister)
	{
		if (userRegister.Password != userRegister.PasswordConfirm)
			throw new Exception("Passwords don't match");
		
		string sql = $@"SELECT Email FROM TutorialAppSchema.Auth 
             			WHERE Email = '{userRegister.Email}'";
		IEnumerable<string> existingUser = _dapper.LoadData<string>(sql);
		if (existingUser.Any())
			throw new Exception("Email already exists");
		
		byte[] passwordSalt = new byte[128 / 8];
		using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
		{
			rng.GetNonZeroBytes(passwordSalt);
		}
		var passwordHash = _authHelper.HashPassword(userRegister.Password, passwordSalt);

		string sqlAuth =
			@$"INSERT INTO  TutorialAppSchema.Auth(Email, PasswordHash, PasswordSalt) 
				VALUES ('{userRegister.Email}', @PasswordHash, @PasswordSalt)";
		List<SqlParameter> sqlParams = new List<SqlParameter>();
		SqlParameter passSaltParam = new("@PasswordSalt", SqlDbType.VarBinary);
		SqlParameter passHashParam = new("@PasswordHash", SqlDbType.VarBinary);
		passSaltParam.Value = passwordSalt;
		passHashParam.Value = passwordHash;
		sqlParams.Add(passHashParam);
		sqlParams.Add(passSaltParam);
		if (!_dapper.ExecuteSqlWithParams(sqlAuth, sqlParams))
			throw new Exception("Could not register user");
		
		string sqlUser = $@"
	        INSERT INTO TutorialAppSchema.Users
	        (
        		FirstName, LastName, Email, Gender, Active
	        )
	        VALUES
	        (
        		'{userRegister.FirstName}', '{userRegister.LastName}', '{userRegister.Email}', '{userRegister.Gender}', 1
	        )";
		if (!_dapper.ExecuteSql(sqlUser))
			throw new Exception("Could not register user");
		
		return Ok();
	}

	[AllowAnonymous]
	[HttpPost("Login")]
	public IActionResult Login(UserLoginDto userLogin)
	{
		string sqlHashSalt = @$"
			SELECT PasswordHash, PasswordSalt
			FROM TutorialAppSchema.Auth 
			WHERE Email = '{userLogin.Email}'";
		UserLoginConfirmDto userConfirm = _dapper.LoadDataSingle<UserLoginConfirmDto>(sqlHashSalt);

		byte[] passwordHash = _authHelper.HashPassword(userLogin.Password, userConfirm.PasswordSalt);
		for (int i = 0; i < passwordHash.Length; i++)
		{
			if (passwordHash[i] != userConfirm.PasswordHash[i])
				return Unauthorized("Username or password doesn't match");
		}
		
		string sqlUserId = $"SELECT userId FROM TutorialAppSchema.Users WHERE Email = '{userLogin.Email}'";
		int userId = _dapper.LoadDataSingle<int>(sqlUserId);
		
		return Ok(new Dictionary<string, string>
				  {
					  {"token", _authHelper.CreateToken(userId)}
				  });
	}

	[HttpGet("RefreshToken")]
	public IActionResult RefreshToken()
	{
		string userId = User.FindFirst("userId")?.Value + "";
		
		string sqlUserId = $"SELECT UserId FROM TutorialAppSchema.Users WHERE UserId = '{userId}'";
		int userIdInt = _dapper.LoadDataSingle<int>(sqlUserId);
		
		return Ok(new Dictionary<string, string>
				  {
					  {"token", _authHelper.CreateToken(userIdInt)}
				  });
	}
}