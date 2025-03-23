using System.Data;
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

	[HttpPost("Post")]
	public IActionResult AddPost(PostAddDto post)
	{
		var userId = User.FindFirst("userId")?.Value;
		var sql = $@"
			INSERT INTO TutorialAppSchema.Posts (UserID, PostTitle, PostContent, PostCreated, PostUpdated)
			VALUES ({userId}, '{post.PostTitle}', '{post.PostContent}', GETDATE(), GETDATE())";
		if (!_dapper.ExecuteSql(sql))
			throw new DataException("Failed to add post");
		return Ok();
	}

	[HttpPut("Post")]
	public IActionResult EditPost(PostEditDto post)
	{
		var userId = User.FindFirst("userId")?.Value;
		var sql = $@"
			UPDATE TutorialAppSchema.Posts
			SET PostContent = '{post.PostContent}', PostUpdated = GETDATE(), PostTitle = '{post.PostTitle}'
			WHERE PostId = {post.PostId} AND UserId = {userId}";
		if (!_dapper.ExecuteSql(sql))
			throw new DataException("Failed to update post");
		return Ok();
	}

	[HttpDelete("Post/{postId}")]
	public IActionResult DeletePost(int postId)
	{
		var userId = User.FindFirst("userId")?.Value;
		var sql = $@"
			DELETE FROM TutorialAppSchema.Posts
			WHERE PostId = {postId} AND UserId = {userId}";
		if(!_dapper.ExecuteSql(sql))
			throw new DataException("Failed to delete post");
		return Ok();
	}

	[HttpGet("Search/{searchText}")]
	public IEnumerable<Post> SearchPosts(string searchText)
	{
		var sql = @$"
			SELECT PostId, UserId, PostTitle, PostContent, PostCreated, PostUpdated
			FROM TutorialAppSchema.Posts
			WHERE PostTitle LIKE '%{searchText}%' OR PostContent LIKE '%{searchText}%'";
		return _dapper.LoadData<Post>(sql);
	}
}