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
}