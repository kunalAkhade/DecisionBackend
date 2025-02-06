using DecisionBackend.DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DecisionBackend.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;

namespace DecisionBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ApplicationDbContext dbContext;
        public CategoryController(ApplicationDbContext dbContext) {
            this.dbContext = dbContext;
        }

        [Authorize]
        [HttpPost("createCategory")]
        public async Task<IActionResult> createCategory(CategoryDTO categoryDTO)
        {
            if(categoryDTO is null)
            {
               return BadRequest(new { message = "No category Information provided" });
            }
            if(categoryDTO?.CategoryName is null )
            {
                return BadRequest(new { message = "No category Information provided" });

            }

            try
            {

                var username=User?.Identity?.Name;
                var existingCategory = await dbContext.Categories.Where(c => c.CategoryName == categoryDTO.CategoryName && c.User.Username == username).ToListAsync();
                if (existingCategory.Count !=0)
                {
                    return BadRequest(new { message = "Catagory already exists" });
                }
                var user = await dbContext.Users.FindAsync(username);
                if(user is null)
                {
                    return Unauthorized(new { message = "Invalid user" });
                }
                var category = new Models.Domain.Category
                {
                    User = user,
                    CategoryName = categoryDTO.CategoryName,

                };

                await dbContext.Categories.AddAsync(category);
                await dbContext.SaveChangesAsync();

                return Ok(new { data = new { categoryName = category.CategoryName, user=user.Username}, message = "category added" });
            }catch(Exception e)
            {
                return BadRequest(new { message = e.ToString() });
            }

        }

        [Authorize]
        [HttpGet("getCategory")]
        public async Task<IActionResult> getCategory()
        {
            try
            {
                var user = User?.Identity?.Name;
                var categories = await dbContext.Categories.Where(c => c.User != null && c.User.Username == user).ToListAsync();
                List<CategoryResDTO> result = new List<CategoryResDTO>();
                foreach(var data in categories)
                {
                    CategoryResDTO c = new CategoryResDTO
                    {
                        categoryName = data.CategoryName,
                        Id = data.Id
                    };
                    result.Add(c);  

                }
                return Ok(new { data = result, message = "Successfully fetched all categories" });

            }catch(Exception e)
            {
                return BadRequest(new { message = e.ToString() });
            }

        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> deleteCategory(string id)
        {
            if (id.IsNullOrEmpty())
            {
                return BadRequest(new { message = "Invalid id" });
            }



            using var transaction = dbContext.Database.BeginTransaction();

            try
            {
                // Retrieve the category with its related tasks
                var category = dbContext.Categories
                    .Include(c => c.Tasks)  // Include related tasks
                    .FirstOrDefault(c => c.Id == Guid.Parse(id)); // Assuming CategoryId = 1

                if (category != null)
                {
                    // Delete related tasks first
                    dbContext.Tasks.RemoveRange(category.Tasks);  // Remove all tasks related to this category

                    // Delete the category itself
                    dbContext.Categories.Remove(category);  // Remove the category

                    // Save changes to the database
                    dbContext.SaveChanges();
                    transaction.Commit();
                    return Ok(new { message = "Deleted" });
                }

                return BadRequest(new { message = "category cannot be found" });
               

            }
            catch (Exception e)
            {
                // Rollback the transaction if anything goes wrong
                transaction.Rollback();
                return BadRequest(new { message = e.ToString() });

            }



           
        }
    }
}
