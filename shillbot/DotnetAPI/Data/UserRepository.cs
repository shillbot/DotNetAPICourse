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
}