using System.Data;
using DotnetAPI.Data;
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

	[HttpGet("Posts")]
	public IEnumerable<Post> GetPosts()
	{
		const string sql = @"
			SELECT PostId, UserId, PostTitle, PostContent, PostCreated, PostUpdated
			FROM TutorialAppSchema.Posts";
		return _dapper.LoadData<Post>(sql);
	}

	[HttpGet("Post/{postId}")]
	public Post GetPost(int postId)
	{
		var sql = @$"
			SELECT PostId, UserId, PostTitle, PostContent, PostCreated, PostUpdated
			FROM TutorialAppSchema.Posts
			WHERE PostId = {postId}";
		
		return _dapper.LoadDataSingle<Post>(sql);
	}

	[HttpGet("PostsByUser/{userId}")]
	public IEnumerable<Post> GetPostsByUser(int userId)
	{
		var sql = @$"
			SELECT PostId, UserId, PostTitle, PostContent, PostCreated, PostUpdated
			FROM TutorialAppSchema.Posts
			WHERE UserId = {userId}";
		
		return _dapper.LoadData<Post>(sql);
	}
	
	[HttpGet("MyPosts")]
	public IEnumerable<Post> GetMyPosts()
	{
		var userId = User.FindFirst("userId")?.Value;
		var sql = @$"
			SELECT PostId, UserId, PostTitle, PostContent, PostCreated, PostUpdated
			FROM TutorialAppSchema.Posts
			WHERE UserId = {userId}";
		
		return _dapper.LoadData<Post>(sql);
	}
}