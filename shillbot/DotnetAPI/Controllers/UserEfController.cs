using System.Data;
using AutoMapper;
using DotnetAPI.Data;
using DotnetAPI.Dtos;
using DotnetAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace DotnetAPI.Controllers;

[Route("[controller]")]
[ApiController]
public class UserEfController(IConfiguration config, IUserRepository userRepository) : ControllerBase
{
	private readonly DataContextEf _context = new(config);
	private IUserRepository _userRepository = userRepository;
	private readonly IMapper _mapper = new Mapper(new MapperConfiguration(cfg => { cfg.CreateMap<UserToAddDto, User>(); }));

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
			if (_userRepository.SaveChanges())
				return Ok();
		}
		throw new DataException("EditUser Failed.");
	}

	[HttpPost("AddUser")]
	public IActionResult AddUser(UserToAddDto userToAdd)
	{
		User userEf = _mapper.Map<User>(userToAdd);
		_userRepository.AddEntity<User>(userEf);
		if (_userRepository.SaveChanges())
			return Ok();
		throw new DataException("AddUser Failed.");
	}

	[HttpDelete("DeleteUser/{userId}")]
	public IActionResult DeleteUser(int userId)
	{
		User? user = _context.User.Find(userId);
		if (user != null)
			_userRepository.RemoveEntity(user);
		if (_userRepository.SaveChanges())
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
		_userRepository.AddEntity<UserSalary>(userForInsert);
		if (_userRepository.SaveChanges())
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
			if (_userRepository.SaveChanges())
			{
				return Ok();
			}
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
			_userRepository.RemoveEntity(userToDelete);
			if (_userRepository.SaveChanges())
			{
				return Ok();
			}
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
		_userRepository.AddEntity<UserJobInfo>(userForInsert);
		if (_userRepository.SaveChanges())
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
			if (_userRepository.SaveChanges())
			{
				return Ok();
			}
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
			_userRepository.RemoveEntity(userToDelete);
			if (_userRepository.SaveChanges())
			{
				return Ok();
			}
		}
		throw new Exception("Failed to find UserJobInfo to delete");
	}
}
