using System.Data;
using System.Security.Cryptography;
using System.Text;
using DotnetAPI.Data;
using DotnetAPI.Dtos;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace DotnetAPI.Controllers;

public class AuthController : Controller
{
	private readonly DataContextDapper _dapper;
	private readonly IConfiguration _config;
	
	public AuthController(IConfiguration config)
	{
		_dapper = new DataContextDapper(config);
		_config = config;
	}

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
		string passwordSaltPlusString = _config.GetSection("AppSettings:PasswordKey").Value 
			+ Convert.ToBase64String(passwordSalt);
		byte[] passwordHash = KeyDerivation.Pbkdf2(
			password: userRegister.Password, 
			salt: Encoding.ASCII.GetBytes(passwordSaltPlusString),
			prf: KeyDerivationPrf.HMACSHA256, 
			iterationCount: 100000, 
			numBytesRequested: 256 / 8);

		string sqlAuth =
			@$"INSERT INTO  TutorialAppSchema.Auth(Email, PasswordHash, PasswordSalt) 
				VALUES ('{userRegister.Email}', @PasswordHash, @PasswordSalt)";
		SqlParameter[] sqlParams = new SqlParameter[2];
		SqlParameter passSaltParam = new("@PasswordSalt", SqlDbType.VarBinary);
		SqlParameter passHashParam = new("@PasswordHash", SqlDbType.VarBinary);
		passSaltParam.Value = passwordSalt;
		passHashParam.Value = passwordHash;
		sqlParams[0] = passHashParam;
		sqlParams[1] = passSaltParam;
		if (!_dapper.ExecuteSqlWithParams(sqlAuth, sqlParams))
			throw new Exception("Could not register user");
		
		return Ok();
	}

	[HttpPost("Login")]
	public IActionResult Login(UserLoginDto userLogin)
	{
		return Ok();
	}
}