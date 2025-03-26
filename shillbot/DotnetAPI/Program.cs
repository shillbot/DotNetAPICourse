using System.Text;
using DotnetAPI.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors((options) =>
{
    options.AddPolicy("DevCors", (corsBuilder) =>
    {
        corsBuilder.WithOrigins("http://localhost:4200", "http://localhost:3000", "http://localhost:8000")
                   .AllowAnyMethod()
                   .AllowAnyHeader()
                   .AllowCredentials();
    });
    options.AddPolicy("ProdCors", (corsBuilder) =>
    {
        corsBuilder.WithOrigins("https://myProductionSite.com")
                   .AllowAnyMethod()
                   .AllowAnyHeader()
                   .AllowCredentials();
    });
});

// builder.Services.AddScoped<IUserRepository, UserRepository>();

var tokenKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
    builder.Configuration.GetSection("AppSettings:TokenKey").Value ?? string.Empty));
var validationParams = new TokenValidationParameters()
                       {
                           IssuerSigningKey = tokenKey,
                           ValidateIssuer = false,
                           ValidateIssuerSigningKey = false,
                           ValidateAudience = false
                       };
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options => { options.TokenValidationParameters = validationParams; });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseCors("DevCors");
	app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseCors("ProdCors");
	app.UseHttpsRedirection();
}

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();

