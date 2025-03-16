using System.Data;
using DotnetAPI.Models;

namespace DotnetAPI.Data;

public class UserRepository(IConfiguration config) : IUserRepository
{
	private readonly DataContextEf _dataContext = new(config);

	public bool SaveChanges()
	{
		return _dataContext.SaveChanges() > 0;
	}

	public void AddEntity<T>(T entity)
	{
		if (entity != null)
			_dataContext.Add(entity);
	}

	public void RemoveEntity<T>(T entity)
	{
		if (entity != null)
			_dataContext.Remove(entity);
	}

	public IEnumerable<User> GetUsers()
	{
		IEnumerable<User> users = _dataContext.User.ToList();
		return users;
	}

	public User GetUser(int userId)
	{
		User? user = _dataContext.User.Find(userId);
		if (user != null)
			return user;
		throw new DataException("GetUser Failed.");
	}

	public UserSalary GetUserSalary(int userId)
	{
		UserSalary? userSalary = _dataContext.UserSalary.FirstOrDefault(u => u.UserId == userId);
		if (userSalary != null)
			return userSalary;
		throw new DataException("Get User Salary Failed");
	}

	public UserJobInfo GetUserJobInfo(int userId)
	{
		UserJobInfo? userJobInfo = _dataContext.UserJobInfo.FirstOrDefault(u => u.UserId == userId);
		if (userJobInfo != null)
			return userJobInfo;
		throw new DataException("Get User Job Info Failed.");
	}
}