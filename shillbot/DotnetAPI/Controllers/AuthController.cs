using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using DotnetAPI.Data;
using DotnetAPI.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;

namespace DotnetAPI.Controllers;

[Route("[controller]")]
[Authorize]
[ApiController]
public class AuthController : Controller
{
	private readonly DataContextDapper _dapper;
	private readonly IConfiguration _config;
	
	public AuthController(IConfiguration config)
	{
		_dapper = new DataContextDapper(config);
		_config = config;
	}

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
		var passwordHash = HashPassword(userRegister.Password, passwordSalt);

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

		byte[] passwordHash = HashPassword(userLogin.Password, userConfirm.PasswordSalt);
		for (int i = 0; i < passwordHash.Length; i++)
		{
			if (passwordHash[i] != userConfirm.PasswordHash[i])
				return Unauthorized("Username or password doesn't match");
		}
		
		string sqlUserId = $"SELECT userId FROM TutorialAppSchema.Users WHERE Email = '{userLogin.Email}'";
		int userId = _dapper.LoadDataSingle<int>(sqlUserId);
		
		return Ok(new Dictionary<string, string>
				  {
					  {"token", CreateToken(userId)}
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
					  {"token", CreateToken(userIdInt)}
				  });
	}
	
	private byte[] HashPassword(string password, byte[] passwordSalt)
    {
     	string passwordSaltPlusString = _config.GetSection("AppSettings:PasswordKey").Value 
     									+ Convert.ToBase64String(passwordSalt);
     	byte[] passwordHash = KeyDerivation.Pbkdf2(
     		password: password, 
     		salt: Encoding.ASCII.GetBytes(passwordSaltPlusString),
     		prf: KeyDerivationPrf.HMACSHA256, 
     		iterationCount: 100000, 
     		numBytesRequested: 256 / 8);
		
     	return passwordHash;
    }

	private string CreateToken(int userId)
	{
		Claim[] claims =
		[
			new("userId", userId.ToString())
		];
		var tokenKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
			_config.GetSection("AppSettings:TokenKey").Value ?? string.Empty));
		var credentials = new SigningCredentials(
			tokenKey, SecurityAlgorithms.HmacSha512Signature);
		var descriptor = new SecurityTokenDescriptor()
						 {
							 Subject = new ClaimsIdentity(claims),
							 SigningCredentials = credentials,
							 Expires = DateTime.UtcNow.AddDays(7)
						 };
		var tokenHandler = new JwtSecurityTokenHandler();
		SecurityToken token = tokenHandler.CreateToken(descriptor);
		
		return tokenHandler.WriteToken(token);
	}
}