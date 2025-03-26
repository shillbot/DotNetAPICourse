using System.Data;
using DotnetAPI.Data;
using DotnetAPI.Dtos;
using DotnetAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace DotnetAPI.Controllers;

[Route("[controller]")]
[ApiController]
public class UserCompleteController(IConfiguration config) : ControllerBase
{
	private readonly DataContextDapper _dapper = new(config);

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
		var sql = $@"EXEC TutorialAppSchema.spUser_Upsert
						@FirstName = {user.FirstName},
						@LastName = {user.LastName},
						@Email = {user.Email},
						@Gender = {user.Gender},
						@JobTitle = {user.JobTitle},
						@Department = {user.Department},
						@Salary = {user.Salary},
						@UserId = {user.UserId},
						@Active = {user.Active}";
		if (!_dapper.ExecuteSql(sql))
			throw new DataException("Update Failed.");
		return Ok();
	}

	// [HttpPost("AddUser")]
	// public IActionResult AddUser(UserAddDto userAdd)
	// {
	// 	string sql = $@"
 //        INSERT INTO TutorialAppSchema.Users
 //        (
 //        	FirstName, LastName, Email, Gender, Active
 //        )
 //        VALUES
 //        (
 //        	'{userAdd.FirstName}', '{userAdd.LastName}', '{userAdd.Email}', '{userAdd.Gender}', '{userAdd.Active}'
 //        )";
	// 	if (_dapper.ExecuteSql(sql))
	// 		return Ok();
	// 	throw new DataException("Insert Failed.");
	// }

	[HttpDelete("DeleteUser/{userId:int}")]
	public IActionResult DeleteUser(int userId)
	{
		var sql = $"EXEC TutorialAppSchema.spUser_Delete @UserId = {userId}";
		if (!_dapper.ExecuteSql(sql))
			throw new DataException("Delete Failed.");
		return Ok();
	}

	// [HttpPost("UserSalary")]
	// public IActionResult PostUserSalary(UserSalary userSalaryForInsert)
	// {
	// 	string sql = $@"
 //            INSERT INTO TutorialAppSchema.UserSalary (
 //                UserId,
 //                Salary
 //            ) VALUES ({userSalaryForInsert.UserId}, {userSalaryForInsert.Salary})";
	// 	if (_dapper.ExecuteSqlWithRowCount(sql) > 0)
	// 		return Ok(userSalaryForInsert);
	// 	throw new Exception("Adding User Salary failed on save");
	// }

	// [HttpPut("UserSalary")]
	// public IActionResult PutUserSalary(UserSalary userSalaryForUpdate)
	// {
	// 	string sql =
	// 		$@"UPDATE TutorialAppSchema.UserSalary 
	// 			SET Salary={userSalaryForUpdate.Salary} 
	// 			WHERE UserId={userSalaryForUpdate.UserId}";
	// 	if (_dapper.ExecuteSql(sql))
	// 		return Ok(userSalaryForUpdate);
	// 	throw new Exception("Updating User Salary failed on save");
	// }
}