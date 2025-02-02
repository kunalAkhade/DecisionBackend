using DecisionBackend.Data;
using DecisionBackend.Services.Implementation;
using DecisionBackend.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
//    .AddJsonOptions(options =>
//{
//    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;
//});
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    // Add Bearer token as security definition
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        Description = "Enter your Bearer token in the format 'Bearer {your_token}'"
    });

    // Add a security requirement to the Swagger UI
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] { }
        }
    });
});


builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DbServer"));
});

var jwtSecret = builder.Configuration["ApplicationSettings:JWT_Secret"];
var jwtExpirationMinutes = builder.Configuration.GetValue<int>("ApplicationSettings:JWT_ExpirationMinutes");

builder.Services.AddScoped<IJwtService, JwtService>();

 //Configure JWT Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false;  // Don't require HTTPS for development
        options.SaveToken = true;

        // Token validation parameters
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret)),
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddAuthorization();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        // Set the Swagger UI endpoint and enable the Bearer token authorization
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "API v1");

        // This part does not need to define security again. Just enable it
        // Swagger UI will pick up the security definition from SwaggerGen
        c.OAuthClientId("swagger-ui");
        c.OAuthAppName("Swagger UI");
    });
}



app.UseAuthentication();  // Enable authentication

app.UseAuthorization();

app.MapControllers();

app.Run();
