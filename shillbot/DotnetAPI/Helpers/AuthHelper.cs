using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;
using DotnetAPI.Dtos;
using DotnetAPI.Data;

namespace DotnetAPI.Helpers;

public class AuthHelper(IConfiguration config)
{
	private readonly DataContextDapper _dapper = new(config);
	
	public byte[] HashPassword(string password, byte[] passwordSalt)
	{
		string passwordSaltPlusString = config.GetSection("AppSettings:PasswordKey").Value 
										+ Convert.ToBase64String(passwordSalt);
		byte[] passwordHash = KeyDerivation.Pbkdf2(
			password: password, 
			salt: Encoding.ASCII.GetBytes(passwordSaltPlusString),
			prf: KeyDerivationPrf.HMACSHA256, 
			iterationCount: 100000, 
			numBytesRequested: 256 / 8);
		
		return passwordHash;
	}

	public string CreateToken(int userId)
	{
		Claim[] claims =
		[
			new("userId", userId.ToString())
		];
		var tokenKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
			config.GetSection("AppSettings:TokenKey").Value ?? string.Empty));
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
	
	public bool SetPassword(UserLoginDto userSetPass)
	{
		byte[] passwordSalt = new byte[128 / 8];
		using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
		{
			rng.GetNonZeroBytes(passwordSalt);
		}
		var passwordHash = HashPassword(userSetPass.Password, passwordSalt);

		var sqlAuth = @"[TutorialAppSchema].spRegistration_Upsert
							@Email = @EmailParam,
							@PasswordSalt = @PasswordSaltParam,
							@PasswordHash = @PasswordHashParam";
		List<SqlParameter> sqlParams = new List<SqlParameter>();
		SqlParameter passSaltParam = new("@PasswordSaltParam", SqlDbType.VarBinary);
		SqlParameter passHashParam = new("@PasswordHashParam", SqlDbType.VarBinary);
		SqlParameter emailParam = new("@EmailParam", SqlDbType.VarChar, 50);
		passSaltParam.Value = passwordSalt;
		passHashParam.Value = passwordHash;
		emailParam.Value = userSetPass.Email;
		sqlParams.Add(passHashParam);
		sqlParams.Add(passSaltParam);
		sqlParams.Add(emailParam);
		if (!_dapper.ExecuteSqlWithParams(sqlAuth, sqlParams))
			throw new Exception("Could not authorize user");
		return true;
	}
}