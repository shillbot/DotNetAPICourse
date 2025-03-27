using System.Data;
using DotnetAPI.Data;
using DotnetAPI.Dtos;
using DotnetAPI.Helpers;
using DotnetAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DotnetAPI.Controllers;

[Authorize]
[Route("[controller]")]
[ApiController]
public class UserCompleteController(IConfiguration config) : ControllerBase
{
	private readonly DataContextDapper _dapper = new(config);
	private readonly ReusableSql _reusableSql = new(config);

	[HttpGet("GetUsers/{active:bool}")]
	public IEnumerable<UserComplete> GetUsers(bool active = true)
	{
		var sql = $"EXEC TutorialAppSchema.spUsers_Get";
		sql += active ? $" @Active = {active}" : "";
		return _dapper.LoadData<UserComplete>(sql);
	}

	[HttpGet("GetUser/{userId:int}")]
	public UserComplete GetUser(int userId)
	{
		var sql = $"EXEC TutorialAppSchema.spUsers_Get @UserId = {userId}";
		return _dapper.LoadDataSingle<UserComplete>(sql);
	}

	[HttpPut("UpsertUser")]
	public IActionResult UpsertUser(UserComplete user)
	{
		if (!_reusableSql.UpsertUser(user))
			throw new DataException("Update Failed.");
		return Ok();
	}

	[HttpDelete("DeleteUser/{userId:int}")]
	public IActionResult DeleteUser(int userId)
	{
		var sql = $"EXEC TutorialAppSchema.spUser_Delete @UserId = {userId}";
		if (!_dapper.ExecuteSql(sql))
			throw new DataException("Delete Failed.");
		return Ok();
	}
}