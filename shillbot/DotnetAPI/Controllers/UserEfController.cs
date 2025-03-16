using System.Data;
using AutoMapper;
using DotnetAPI.Data;
using DotnetAPI.Dtos;
using DotnetAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace DotnetAPI.Controllers;

[Route("[controller]")]
[ApiController]
public class UserEfController(IUserRepository userRepository) : ControllerBase
{
	private IUserRepository _userRepository = userRepository;
	private readonly IMapper _mapper = new Mapper(new MapperConfiguration(cfg =>
	{
		cfg.CreateMap<UserAddDto, User>();
		cfg.CreateMap<UserSalary, UserSalary>();
		cfg.CreateMap<UserJobInfo, UserJobInfo>();
	}));

	[HttpGet("GetUsers")]
	public IEnumerable<User> GetUsers()
	{
		return _userRepository.GetUsers();
	}

	[HttpGet("GetUser/{userId}")]
	public User GetUser(int userId)
	{
		return _userRepository.GetUser(userId);
	}

	[HttpPut("EditUser")]
	public IActionResult EditUser(User user)
	{
		User? userEf = _userRepository.GetUser(user.UserId);
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
	public IActionResult AddUser(UserAddDto userAdd)
	{
		User userEf = _mapper.Map<User>(userAdd);
		_userRepository.AddEntity<User>(userEf);
		if (_userRepository.SaveChanges())
			return Ok();
		throw new DataException("AddUser Failed.");
	}

	[HttpDelete("DeleteUser/{userId}")]
	public IActionResult DeleteUser(int userId)
	{
		User user = _userRepository.GetUser(userId);
		if (user != null)
			_userRepository.RemoveEntity(user);
		if (_userRepository.SaveChanges())
			return Ok();
		throw new DataException("DeleteUser Failed.");
	}

	[HttpGet("UserSalary/{userId}")]
	public UserSalary GetUserSalary(int userId)
	{
		return _userRepository.GetUserSalary(userId);
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
	public IActionResult PutUserSalary(UserSalary userForUpdate)
	{
		UserSalary userToUpdate = _userRepository.GetUserSalary(userForUpdate.UserId);

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
	public IActionResult DeleteUserSalary(int userId)
	{
		UserSalary userToDelete = _userRepository.GetUserSalary(userId);

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
	public UserJobInfo GetUserJobInfo(int userId)
	{
		return _userRepository.GetUserJobInfo(userId);
	}

	[HttpPost("UserJobInfo")]
	public IActionResult PostUserJobInfo(UserJobInfo userForInsert)
	{
		_userRepository.AddEntity<UserJobInfo>(userForInsert);
		if (_userRepository.SaveChanges())
		{
			return Ok();
		}
		throw new Exception("Adding UserJobInfo failed on save");
	}


	[HttpPut("UserJobInfo")]
	public IActionResult PutUserJobInfo(UserJobInfo userForUpdate)
	{
		UserJobInfo? userToUpdate = _userRepository.GetUserJobInfo(userForUpdate.UserId);

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
	public IActionResult DeleteUserJobInfo(int userId)
	{
		UserJobInfo userToDelete = _userRepository.GetUserJobInfo(userId);

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
