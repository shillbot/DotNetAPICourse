using System.Data;
using AutoMapper;
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
	private IMapper _mapper = new Mapper(new MapperConfiguration(cfg => { cfg.CreateMap<UserToAddDto, User>(); }));

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
		User userEf = _mapper.Map<User>(userToAdd);
		_context.User.Add(userEf);
		if (_context.SaveChanges() > 0)
			return Ok();
		throw new DataException("AddUser Failed.");
	}

	[HttpDelete("DeleteUser/{userId}")]
	public IActionResult DeleteUser(int userId)
	{
		User? user = _context.User.Find(userId);
		if (user != null)
			_context.User.Remove(user);
		if (_context.SaveChanges() > 0)
			return Ok();
		throw new DataException("DeleteUser Failed.");
	}

	[HttpGet("UserSalary/{userId}")]
	public IEnumerable<UserSalary> GetUserSalaryEF(int userId)
	{
		return _context.UserSalary
			.Where(u => u.UserId == userId)
			.ToList();
	}

	[HttpPost("UserSalary")]
	public IActionResult PostUserSalaryEf(UserSalary userForInsert)
	{
		_context.UserSalary.Add(userForInsert);
		if (_context.SaveChanges() > 0)
		{
			return Ok();
		}

		throw new Exception("Adding UserSalary failed on save");
	}


	[HttpPut("UserSalary")]
	public IActionResult PutUserSalaryEf(UserSalary userForUpdate)
	{
		UserSalary? userToUpdate = _context.UserSalary
			.FirstOrDefault(u => u.UserId == userForUpdate.UserId);

		if (userToUpdate != null)
		{
			_mapper.Map(userForUpdate, userToUpdate);
			if (_context.SaveChanges() > 0)
			{
				return Ok();
			}

			throw new Exception("Updating UserSalary failed on save");
		}

		throw new Exception("Failed to find UserSalary to Update");
	}


	[HttpDelete("UserSalary/{userId}")]
	public IActionResult DeleteUserSalaryEf(int userId)
	{
		UserSalary? userToDelete = _context.UserSalary
			.FirstOrDefault(u => u.UserId == userId);

		if (userToDelete != null)
		{
			_context.UserSalary.Remove(userToDelete);
			if (_context.SaveChanges() > 0)
			{
				return Ok();
			}

			throw new Exception("Deleting UserSalary failed on save");
		}

		throw new Exception("Failed to find UserSalary to delete");
	}


	[HttpGet("UserJobInfo/{userId}")]
	public IEnumerable<UserJobInfo> GetUserJobInfoEF(int userId)
	{
		return _context.UserJobInfo
			.Where(u => u.UserId == userId)
			.ToList();
	}

	[HttpPost("UserJobInfo")]
	public IActionResult PostUserJobInfoEf(UserJobInfo userForInsert)
	{
		_context.UserJobInfo.Add(userForInsert);
		if (_context.SaveChanges() > 0)
		{
			return Ok();
		}
		throw new Exception("Adding UserJobInfo failed on save");
	}


	[HttpPut("UserJobInfo")]
	public IActionResult PutUserJobInfoEf(UserJobInfo userForUpdate)
	{
		UserJobInfo? userToUpdate = _context.UserJobInfo
			.FirstOrDefault(u => u.UserId == userForUpdate.UserId);

		if (userToUpdate != null)
		{
			_mapper.Map(userForUpdate, userToUpdate);
			if (_context.SaveChanges() > 0)
			{
				return Ok();
			}
			throw new Exception("Updating UserJobInfo failed on save");
		}
		throw new Exception("Failed to find UserJobInfo to Update");
	}


	[HttpDelete("UserJobInfo/{userId}")]
	public IActionResult DeleteUserJobInfoEf(int userId)
	{
		UserJobInfo? userToDelete = _context.UserJobInfo
			.FirstOrDefault(u => u.UserId == userId);

		if (userToDelete != null)
		{
			_context.UserJobInfo.Remove(userToDelete);
			if (_context.SaveChanges() > 0)
			{
				return Ok();
			}
			throw new Exception("Deleting UserJobInfo failed on save");
		}
		throw new Exception("Failed to find UserJobInfo to delete");
	}
}
