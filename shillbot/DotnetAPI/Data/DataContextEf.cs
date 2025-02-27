using DotnetAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace DotnetAPI.Data;

public class DataContextEf(IConfiguration config) : DbContext
{
	public virtual DbSet<UserSalary> UserSalary { get; set; }
	public virtual DbSet<UserJobInfo> UserJobInfo { get; set; }
	public virtual DbSet<User> User { get; set; }

	protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
	{
		if (!optionsBuilder.IsConfigured)
		{
			optionsBuilder
				.UseSqlServer(config.GetConnectionString("DefaultConnection"), 
					ob => ob.EnableRetryOnFailure());
		}
	}
	
	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		modelBuilder.HasDefaultSchema("TutorialAppSchema");
		modelBuilder.Entity<User>()
			.ToTable("Users", "TutorialAppSchema")
			.HasKey(u => u.UserId);
		modelBuilder.Entity<UserJobInfo>()
			.HasKey(u => u.UserId);
		modelBuilder.Entity<UserSalary>()
			.HasKey(u => u.UserId);
	}
}