using System.Data;
using Dapper;
using DotnetAPI.Data;
using DotnetAPI.Models;

namespace DotnetAPI.Helpers;

public class ReusableSql(IConfiguration config)
{
	private readonly DataContextDapper _dapper = new (config);

	public bool UpsertUser(UserComplete userComplete)
	{
		var sql = @"EXEC TutorialAppSchema.spUser_Upsert
                @FirstName = @FirstNameParameter, 
                @LastName = @LastNameParameter, 
                @Email = @EmailParameter, 
                @Gender = @GenderParameter, 
                @Active = @ActiveParameter, 
                @JobTitle = @JobTitleParameter, 
                @Department = @DepartmentParameter, 
                @Salary = @SalaryParameter, 
                @UserId = @UserIdParameter";

		var sqlParameters = new DynamicParameters();

		sqlParameters.Add("@FirstNameParameter", userComplete.FirstName, DbType.String);
		sqlParameters.Add("@LastNameParameter", userComplete.LastName, DbType.String);
		sqlParameters.Add("@EmailParameter", userComplete.Email, DbType.String);
		sqlParameters.Add("@GenderParameter", userComplete.Gender, DbType.String);
		sqlParameters.Add("@ActiveParameter", userComplete.Active, DbType.Boolean);
		sqlParameters.Add("@JobTitleParameter", userComplete.JobTitle, DbType.String);
		sqlParameters.Add("@DepartmentParameter", userComplete.Department, DbType.String);
		sqlParameters.Add("@SalaryParameter", userComplete.Salary, DbType.Decimal);
		sqlParameters.Add("@UserIdParameter", userComplete.UserId, DbType.Int32);

		return _dapper.ExecuteSqlWithParams(sql, sqlParameters);
	}
}