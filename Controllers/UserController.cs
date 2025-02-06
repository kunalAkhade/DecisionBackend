using BCrypt.Net;
using DecisionBackend.Data;
using DecisionBackend.DTO;
using DecisionBackend.Models.Domain;
using DecisionBackend.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace DecisionBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly ApplicationDbContext dbContext;
        private readonly IConfiguration _configuration;
        private readonly IJwtService jwtService;
        public UserController(ApplicationDbContext dbContext, IConfiguration configuration, IJwtService jwtService) { 
            
            this.dbContext = dbContext;
            _configuration = configuration;
            this.jwtService = jwtService;

        }


        // POST: api/auth/login
        [HttpPost("login")]
        public async Task<IActionResult> Login( UserDTO model)
        {
            if(model.Username == null || model.Password == null)
            {
                return BadRequest(new { message = "Invalid Credentials" });
            }
            try
            { 
                var user = await dbContext.Users.FindAsync(model.Username);
                if (user == null)
                {
                    return Ok(new { message = "User not found" });
                }
                if (!BCrypt.Net.BCrypt.Verify(model.Password, user.Password))
                {
                    return Ok(new { message = "Password is incorrect" });

                }   
                var token = jwtService.GenerateJwtToken(model.Username);
                return Ok(new {message="Successfully logged in", Token = token });

            }catch(Exception ex)
            {
                return BadRequest(new { message = ex.ToString() });
            }
            
        }

        
       

        [Authorize]
        [HttpGet("profile")]
        public IActionResult GetUserProfile()
        {
            var username = User?.Identity?.Name;

            if (username == null)
            {
                return Unauthorized(new { message = "Token does not contain a valid username.", isValid = false });
            }

            return Ok(new { isValid=true , Username = username, Message = "This is protected data." });
        }


        [HttpPost("register")]
        public async Task<IActionResult> CreateUser(UserDTO userDTO)
        {
               
            try
            {
                if(userDTO.Username == null || userDTO.Password == null)
                {
                    return BadRequest(new { message = "Invalid Credentials" });
                }
                var usr = await dbContext.Users.FindAsync(userDTO.Username);
                if (usr != null)
                {
                    return Ok(new {message="User already exists"});
                }
                var user = new User
                {
                    Username = userDTO.Username,
                    Password = BCrypt.Net.BCrypt.HashPassword(userDTO.Password, 12)
                };
                await dbContext.Users.AddAsync(user);
                await dbContext.SaveChangesAsync();

                return Ok(new { data=user, message = "User is created" });

            }
            catch (Exception ex)
            {

                return BadRequest(new { message = ex.ToString() });
            }
           

            

        }

      



    }
}
