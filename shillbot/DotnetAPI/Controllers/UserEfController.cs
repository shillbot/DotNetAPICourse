using System.Data;
using DotnetAPI.Data;
using DotnetAPI.Dtos;
using DotnetAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace DotnetAPI.Controllers;

[Route("[controller]")]
[ApiController]
public class UserEfController(IConfiguration config) : ControllerBase
{
	private readonly DataContextEf _context = new(config);

	[HttpGet("GetUsers")]
	public IEnumerable<User> GetUsers()
	{
		IEnumerable<User> users = _context.User.ToList();
		return users;
	}
	
	[HttpGet("GetUser/{userId}")]
	public User GetUser(int userId)
	{
		User? user = _context.User.Find(userId);
		if (user != null)
			return user;
		throw new DataException("GetUser Failed.");
	}

	[HttpPut("EditUser")]
	public IActionResult EditUser(User user)
	{
		User? userEf = _context.User.Find(user.UserId);
		if (userEf != null)
		{
			userEf.FirstName = user.FirstName;
			userEf.LastName = user.LastName;
			userEf.Email = user.Email;
			userEf.Gender = user.Gender;
			userEf.Active = user.Active;
			if (_context.SaveChanges() > 0)
				return Ok();
		}
		throw new DataException("EditUser Failed.");
	}

	[HttpPost("AddUser")]
	public IActionResult AddUser(UserToAddDto userToAdd)
	{
		User userEf = new()
					  {
						  FirstName = userToAdd.FirstName,
						  LastName = userToAdd.LastName,
						  Email = userToAdd.Email,
						  Gender = userToAdd.Gender,
						  Active = userToAdd.Active
					  };
		_context.User.Add(userEf);
		if(_context.SaveChanges() > 0)
			return Ok();
		throw new DataException("AddUser Failed.");
	}
	
	[HttpDelete("DeleteUser/{userId}")]
	public IActionResult DeleteUser(int userId)
	{
		User? user = _context.User.Find(userId);
		if (user != null)
			_context.User.Remove(user);
		if(_context.SaveChanges() > 0)
			return Ok();
		throw new DataException("DeleteUser Failed.");
	}
}