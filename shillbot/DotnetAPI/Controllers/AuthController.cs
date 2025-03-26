using System.Data;
using System.Security.Cryptography;
using Dapper;
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

		var userSetPass = new UserLoginDto()
						  {
							  Email = userRegister.Email,
							  Password = userRegister.Password
						  };
		_authHelper.SetPassword(userSetPass);

		var sqlUser = $@"EXEC TutorialAppSchema.spUser_Upsert
						@FirstName = '{userRegister.FirstName}',
						@LastName = '{userRegister.LastName}',
						@Email = '{userRegister.Email}',
						@Gender = '{userRegister.Gender}',
						@JobTitle = '{userRegister.JobTitle}',
						@Department = '{userRegister.Department}',
						@Salary = {userRegister.Salary},
						@Active = {userRegister.Active}";
		if (!_dapper.ExecuteSql(sqlUser))
			throw new Exception("Could not add or update user");
		
		return Ok();
	}

	[HttpPut("ResetPassword")]
	public IActionResult ResetPassword(UserLoginDto userLogin)
	{
		_authHelper.SetPassword(userLogin);
		return Ok();
	}

	[AllowAnonymous]
	[HttpPost("Login")]
	public IActionResult Login(UserLoginDto userLogin)
	{
		var sqlHashSalt = """
						   EXEC TutorialAppSchema.spLoginConfirmation_Get 
						   @Email = @EmailParam
						  """;
		var sqlParams = new DynamicParameters();
		sqlParams.Add("@EmailParam", userLogin.Email, DbType.String);
		var userConfirm = _dapper.LoadDataSingleWithParameters<UserLoginConfirmDto>(sqlHashSalt, sqlParams);

		byte[] passwordHash = _authHelper.HashPassword(userLogin.Password, userConfirm.PasswordSalt);
		if (passwordHash.Where((t, i) => t != userConfirm.PasswordHash[i]).Any())
			return Unauthorized("Username or password doesn't match");
		
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