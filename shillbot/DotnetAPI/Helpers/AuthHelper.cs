using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.IdentityModel.Tokens;

namespace DotnetAPI.Helpers;

public class AuthHelper(IConfiguration config)
{
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
}