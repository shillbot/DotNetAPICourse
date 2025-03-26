using System.Data;
using Dapper;
using DotnetAPI.Data;
using DotnetAPI.Dtos;
using DotnetAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace DotnetAPI.Controllers;

[Authorize]
[ApiController]
[Route("[controller]")]
public class PostController(IConfiguration config) : Controller
{
	private readonly DataContextDapper _dapper = new(config);

	[HttpGet("Posts/{postId:int}/{userId:int}/{searchParam}")]
	public IEnumerable<Post> GetPosts(int postId = 0, int userId = 0, string searchParam = "None")
	{
		string sql = @"EXEC TutorialAppSchema.spPosts_Get";
		string stringParameters = "";

		DynamicParameters sqlParameters = new DynamicParameters();
		if (postId != 0)
		{
			stringParameters += ", @PostId=@PostIdParameter";
			sqlParameters.Add("@PostIdParameter", postId, DbType.Int32);
		}
		if (userId != 0)
		{
			stringParameters += ", @UserId=@UserIdParameter";
			sqlParameters.Add("@UserIdParameter", userId, DbType.Int32);
		}
		if (searchParam.ToLower() != "none")
		{
			stringParameters += ", @SearchValue=@SearchValueParameter";
			sqlParameters.Add("@SearchValueParameter", searchParam, DbType.String);
		}
		if (stringParameters.Length > 0)
			sql += stringParameters.Substring(1);
		return _dapper.LoadDataWithParameters<Post>(sql, sqlParameters);
	}
	
	[HttpGet("MyPosts")]
	public IEnumerable<Post> GetMyPosts()
	{
		var userId = User.FindFirst("userId")?.Value;
		var sql = @$"EXEC TutorialAppSchema.spPosts_Get @UserId = {userId}";
		return _dapper.LoadData<Post>(sql);
	}

	[HttpPut("UpsertPost")]
	public IActionResult UpsertPost(Post postToUpsert)
	{
		string sql = @"EXEC TutorialAppSchema.spPosts_Upsert
                @UserId=@UserIdParameter, 
                @PostTitle=@PostTitleParameter, 
                @PostContent=@PostContentParameter";
		var sqlParameters = new DynamicParameters();
		sqlParameters.Add("@UserIdParameter", User.FindFirst("userId")?.Value, DbType.Int32);
		sqlParameters.Add("@PostTitleParameter", postToUpsert.PostTitle, DbType.String);
		sqlParameters.Add("@PostContentParameter", postToUpsert.PostContent, DbType.String);

		if (postToUpsert.PostId > 0)
		{
			sql += ", @PostId=@PostIdParameter";
			sqlParameters.Add("@PostIdParameter", postToUpsert.PostId, DbType.Int32);
		}
		if (!_dapper.ExecuteSqlWithParams(sql, sqlParameters))
			throw new Exception("Failed to upsert post!");
		return Ok();
	}
	
	[HttpDelete("Post/{postId}")]
	public IActionResult DeletePost(int postId)
	{
		var sql = @"EXEC TutorialAppSchema.spPost_Delete 
	                @UserId=@UserIdParameter, 
	                @PostId=@PostIdParameter";
		DynamicParameters sqlParameters = new DynamicParameters();
		sqlParameters.Add("@UserIdParameter", User.FindFirst("userId")?.Value, DbType.Int32);
		sqlParameters.Add("@PostIdParameter", postId, DbType.Int32);

		if (!_dapper.ExecuteSqlWithParams(sql, sqlParameters))
			throw new Exception("Failed to delete post");
		return Ok();
	}

}