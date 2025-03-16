using System.Data;
using DotnetAPI.Data;
using DotnetAPI.Dtos;
using DotnetAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace DotnetAPI.Controllers;

[Route("[controller]")]
[ApiController]
public class UserController(IConfiguration config) : ControllerBase
{
	private readonly DataContextDapper _dapper = new(config);

	[HttpGet("GetUsers")]
	public IEnumerable<User> GetUsers()
	{
		string sql = @"
			SELECT UserId,
				   FirstName,
				   LastName,
				   Email,
				   Gender,
				   Active
			FROM TutorialAppSchema.Users";

		IEnumerable<User> users = _dapper.LoadData<User>(sql);
		return users;
	}

	[HttpGet("GetUser/{userId}")]
	public User GetUser(int userId)
	{
		string sql = $@"
			SELECT UserId,
				   FirstName,
				   LastName,
				   Email,
				   Gender,
				   Active
			FROM TutorialAppSchema.Users
            WHERE UserId = {userId}"; // Not parameterized -- This is horrible.
		User user = _dapper.LoadDataSingle<User>(sql);
		return user;
	}

	[HttpPut("EditUser")]
	public IActionResult EditUser(User user)
	{
		string sql = $@"
            UPDATE TutorialAppSchema.Users
            SET FirstName = '{user.FirstName}',  LastName = '{user.LastName}', 
                Email = '{user.Email}', Gender = '{user.Gender}', Active = '{user.Active}' 
            WHERE UserId = {user.UserId}";

		if (_dapper.ExecuteSql(sql))
			return Ok();
		throw new DataException("Update Failed.");
	}

	[HttpPost("AddUser")]
	public IActionResult AddUser(UserAddDto userAdd)
	{
		string sql = $@"
        INSERT INTO TutorialAppSchema.Users
        (
            FirstName,
            LastName,
            Email,
            Gender,
            Active
        )
        VALUES
        (
            '{userAdd.FirstName}', '{userAdd.LastName}', '{userAdd.Email}', '{userAdd.Gender}', '{userAdd.Active}')";

		if (_dapper.ExecuteSql(sql))
			return Ok();
		throw new DataException("Insert Failed.");
	}

	[HttpDelete("DeleteUser/{userId}")]
	public IActionResult DeleteUser(int userId)
	{
		string sql = $@"
        DELETE FROM TutorialAppSchema.Users
        WHERE UserId = '{userId}'";

		if (_dapper.ExecuteSql(sql))
			return Ok();
		throw new DataException("Delete Failed.");
	}

	[HttpGet("UserSalary/{userId}")]
	public IEnumerable<UserSalary> GetUserSalary(int userId)
	{
		return _dapper.LoadData<UserSalary>($@"
			SELECT UserSalary.UserId, UserSalary.Salary
			FROM  TutorialAppSchema.UserSalary
			WHERE UserId = {userId}");
	}

	[HttpPost("UserSalary")]
	public IActionResult PostUserSalary(UserSalary userSalaryForInsert)
	{
		string sql = $@"
            INSERT INTO TutorialAppSchema.UserSalary (
                UserId,
                Salary
            ) VALUES ({userSalaryForInsert.UserId}, {userSalaryForInsert.Salary})";
		if (_dapper.ExecuteSqlWithRowCount(sql) > 0)
			return Ok(userSalaryForInsert);
		throw new Exception("Adding User Salary failed on save");
	}

	[HttpPut("UserSalary")]
	public IActionResult PutUserSalary(UserSalary userSalaryForUpdate)
	{
		string sql =
			$@"UPDATE TutorialAppSchema.UserSalary 
				SET Salary={userSalaryForUpdate.Salary} 
				WHERE UserId={userSalaryForUpdate.UserId}";
		if (_dapper.ExecuteSql(sql))
			return Ok(userSalaryForUpdate);
		throw new Exception("Updating User Salary failed on save");
	}

	[HttpDelete("UserSalary/{userId}")]
	public IActionResult DeleteUserSalary(int userId)
	{
		string sql = $"DELETE FROM TutorialAppSchema.UserSalary WHERE UserId={userId}";
		if (_dapper.ExecuteSql(sql))
			return Ok();
		throw new Exception("Deleting User Salary failed on save");
	}

	[HttpGet("UserJobInfo/{userId}")]
	public IEnumerable<UserJobInfo> GetUserJobInfo(int userId)
	{
		return _dapper.LoadData<UserJobInfo>($@"
			SELECT  UserJobInfo.UserId, 
			        UserJobInfo.JobTitle,
			        UserJobInfo.Department
			FROM  TutorialAppSchema.UserJobInfo
			WHERE UserId = {userId}");
	}

	[HttpPost("UserJobInfo")]
	public IActionResult PostUserJobInfo(UserJobInfo userJobInfoForInsert)
	{
		string sql = $@"
            INSERT INTO TutorialAppSchema.UserJobInfo (
                UserId,
                Department,
                JobTitle
            ) VALUES ({userJobInfoForInsert.UserId}, '{userJobInfoForInsert.Department}', '{userJobInfoForInsert.JobTitle}')";
		if (_dapper.ExecuteSql(sql))
			return Ok(userJobInfoForInsert);
		throw new Exception("Adding User Job Info failed on save");
	}

	[HttpPut("UserJobInfo")]
	public IActionResult PutUserJobInfo(UserJobInfo userJobInfoForUpdate)
	{
		string sql =
			$@"UPDATE TutorialAppSchema.UserJobInfo 
				SET Department='{userJobInfoForUpdate.Department}', JobTitle='{userJobInfoForUpdate.JobTitle}' 
				WHERE UserId={userJobInfoForUpdate.UserId}";
		if (_dapper.ExecuteSql(sql))
			return Ok(userJobInfoForUpdate);
		throw new Exception("Updating User Job Info failed on save");
	}

	[HttpDelete("UserJobInfo/{userId}")]
	public IActionResult DeleteUserJobInfo(int userId)
	{
		string sql = $@"
			DELETE FROM TutorialAppSchema.UserJobInfo 
			WHERE UserId = '{userId}'";
		if (_dapper.ExecuteSql(sql)) 
			return Ok();
		throw new Exception("Failed to Delete User");
	}
}