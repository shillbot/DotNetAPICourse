using DotnetAPI.Data;
using DotnetAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DotnetAPI.Controllers;

[Route("[controller]")]
[ApiController]
public class UserController : ControllerBase
{
	DataContextDapper _dapper;

	public UserController(IConfiguration config)
	{
		_dapper = new DataContextDapper(config);
	}

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

	//[HttpGet("TestConnection")]
	//public DateTime TestConnection()
	//{
	//    return _dapper.LoadDataSingle<DateTime>("SELECT GETDATE()");
	//}

	//  [HttpGet("GetUsers/{testValue}")]
	//  public string[] GetUsers(string testValue)
	//  {
	//      string[] responseArray =
	//[
	//	"test1", 
	//	testValue
	//      ];
	//      return responseArray;
	//  }
}

