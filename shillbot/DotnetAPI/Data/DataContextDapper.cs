using System.Data;
using Dapper;
using Microsoft.Data.SqlClient;

namespace DotnetAPI.Data;

public class DataContextDapper(IConfiguration config)
{
    public IEnumerable<T> LoadData<T>(string sql)
    {
        IDbConnection dbConnection = new SqlConnection(config.GetConnectionString("DefaultConnection"));
        return dbConnection.Query<T>(sql);
    }

    public T LoadDataSingle<T>(string sql)
    {
        IDbConnection dbConnection = new SqlConnection(config.GetConnectionString("DefaultConnection"));
        return dbConnection.QuerySingle<T>(sql);
    }

    public bool ExecuteSql(string sql)
    {
        IDbConnection dbConnection = new SqlConnection(config.GetConnectionString("DefaultConnection"));
        return dbConnection.Execute(sql) > 0;
	}

    public int ExecuteSqlWithRowCount(string sql)
    {
        IDbConnection dbConnection = new SqlConnection(config.GetConnectionString("DefaultConnection"));
        return dbConnection.Execute(sql);
    }
    
    public bool ExecuteSqlWithParams(string sql, DynamicParameters parameters)
    {
        IDbConnection dbConnection = new SqlConnection(config.GetConnectionString("DefaultConnection"));
        return dbConnection.Execute(sql, parameters) > 0;
    }
    
    public IEnumerable<T> LoadDataWithParameters<T>(string sql, DynamicParameters parameters)
    {
        IDbConnection dbConnection = new SqlConnection(config.GetConnectionString("DefaultConnection"));
        return dbConnection.Query<T>(sql, parameters);
    }

    public T LoadDataSingleWithParameters<T>(string sql, DynamicParameters parameters)
    {
        IDbConnection dbConnection = new SqlConnection(config.GetConnectionString("DefaultConnection"));
        return dbConnection.QuerySingle<T>(sql, parameters);
    }
}

