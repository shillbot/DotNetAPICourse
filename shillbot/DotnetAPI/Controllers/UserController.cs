using System.Data;
using DotnetAPI.Data;
using DotnetAPI.Models;
using Microsoft.AspNetCore.Http;
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
		string sql = @"
			SELECT UserId,
				   FirstName,
				   LastName,
				   Email,
				   Gender,
				   Active
			FROM TutorialAppSchema.Users
            WHERE UserId = " + userId.ToString(); // Not parameterized -- This is horrible.
		User user = _dapper.LoadDataSingle<User>(sql);
		return user;
	}

	[HttpPut("EditUser")]
	public IActionResult EditUser(User user)
	{
		string sql = @"
            UPDATE TutorialAppSchema.Users
            SET FirstName = '" + user.FirstName + "',  LastName = '" + user.LastName + "', Email = '" + user.Email + "', Gender = '" + user.Gender + "', Active = '" + user.Active +
			"' WHERE UserId = " + user.UserId;

        if (_dapper.ExecuteSql(sql))
            return Ok();

        throw new DataException("Update Failed.");
    }

	[HttpPost("AddUser")]
	public IActionResult AddUser(User user)
    {
        string sql = @"
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
            '" + user.FirstName + "', '" + user.LastName + "', '" + user.Email + "', '" + user.Gender + "', '" + user.Active +
			"'" +
        ")";

        Console.WriteLine(sql);
        if (_dapper.ExecuteSql(sql))
            return Ok();

        throw new DataException("Insert Failed.");
	}
}

