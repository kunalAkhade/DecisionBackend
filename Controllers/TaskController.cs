using DecisionBackend.Data;
using DecisionBackend.DTO;
using DecisionBackend.Models.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.Data.Common;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace DecisionBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TaskController : ControllerBase
    {
        private readonly ApplicationDbContext dbContext;
        public TaskController(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> createTask(TaskDTO taskDTO)
        {
            if (taskDTO == null)
            {
                return BadRequest(new { message = "Task is null" });
            }
            if ((taskDTO.Title == null || taskDTO.Title.Length == 0) || (taskDTO.Description == null || taskDTO.Description.Length == 0))
            {
                return BadRequest(new { message = "Task is Invalid" });
            }

            try
            {
                Category? category = null;
                if (taskDTO.Category != null)
                {
                    category = await dbContext.Categories.FindAsync(Guid.Parse(taskDTO.Category));
                }
                var username = User?.Identity?.Name;
                if (username == null)
                {
                    return BadRequest(new { message = "Invalid User" });
                }
                var user = await dbContext.Users.FindAsync(username);

                var task = new DecisionBackend.Models.Domain.Task
                {
                    Title = taskDTO.Title,
                    Description = taskDTO.Description,
                    User = user,
                    Category = category,
                    Status = Models.Domain.StatusType.Pending,
                    PriorityType = taskDTO.PriorityType

                };
                await dbContext.Tasks.AddAsync(task);
                await dbContext.SaveChangesAsync();
                var data = new TaskDTO
                {
                    Title = task.Title,
                    Description = task.Description,
                    Category = task.Category?.Id.ToString()
                };
                return Ok(new { data = data, message = "Task created successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.ToString() });
            }

        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> getTasks()
        {
            try
            {
                var username = User?.Identity?.Name;
                List<Models.Domain.Task> data = await dbContext.Tasks.Where(t => t.User != null && t.User.Username == username).ToListAsync();
                List<TaskResDTO> list = new List<TaskResDTO>();
                foreach (var i in data)
                {
                    TaskResDTO task = new TaskResDTO
                    {   
                        Id = i.Id,
                        Title = i.Title,
                        Description = i.Description,
                        Category = i.Category?.Id,
                        Status = i.Status,
                        PriorityType = i.PriorityType
                    };
                    list.Add(task);
                }

                return Ok(new { data = list, message = "Successfully fetched all tasks" });


            }
            catch (Exception ex)
            {

                return BadRequest(ex.ToString());
            }
        }

        [Authorize]
        [HttpGet("ByCategory")]
        public async Task<IActionResult> GetByCategory(string category)
        {
            if (!Guid.TryParse(category, out Guid categoryId))
            {
                return BadRequest(new { message = "Invalid category ID format" });
            }

            try
            {
                var username = User?.Identity?.Name;
                if (string.IsNullOrEmpty(username))
                {
                    return Unauthorized(new { message = "User is not authorized" });
                }

                // Projecting data directly to TaskResDTO for better performance
                var tasks = await dbContext.Tasks
                    .Where(t => t.User.Username == username && t.Category.Id == categoryId)
                    .Select(t => new TaskResDTO
                    {
                        Id = t.Id,
                        Title = t.Title,
                        Description = t.Description,
                        Category = t.Category.Id,
                        Status = t.Status,
                        PriorityType = t.PriorityType
                    })
                    .ToListAsync();

                return Ok(new { data = tasks, message = "Successfully fetched all tasks" });
            }
            catch (Exception ex)
            {
                
                return BadRequest(new { message = "An error occurred while fetching tasks", error = ex.ToString() });
            }
        }


        [Authorize]
        [HttpPut("UpdateTask/{taskId}")]
        public async Task<IActionResult> UpdateTask(string taskId, [FromBody] TaskUpdateDTO taskUpdateDto)
        {
            if (taskUpdateDto == null)
            {
                return BadRequest(new { message = "Invalid data." });
            }

            try
            {
                // Find the task by taskId
                var task = await dbContext.Tasks
                    .FirstOrDefaultAsync(t => t.Id == Guid.Parse(taskId));

                if (task == null)
                {
                    return NotFound(new { message = "Task not found" });
                }

                

                // Update task properties
                task.Title = taskUpdateDto.Title ?? task.Title;
                task.Description = taskUpdateDto.Description ?? task.Description;
                task.Status = taskUpdateDto.Status ?? task.Status;
                task.PriorityType = taskUpdateDto.PriorityType ?? task.PriorityType;

                if(taskUpdateDto.CategoryId != null)
                {
                    var category = await dbContext.Categories.FindAsync(Guid.Parse(taskUpdateDto.CategoryId));
                    task.Category = category;
                }
                else
                {
                    task.Category = task.Category;
                }

               

                // Save changes to the database
                dbContext.Tasks.Update(task);
                await dbContext.SaveChangesAsync();

                return Ok(new { message = "Task updated successfully." });
            }
            catch (Exception ex)
            {
                // Log the error (you can use a logging library for this)
                return BadRequest(new { message = "An error occurred while updating the task.", error = ex.Message });
            }
        }

        [Authorize]
        [HttpDelete("Delete/{taskId}")]
        public async Task<IActionResult> DeleteTask(string taskId)
        {
            try
            {
                // Find the task by its ID
                var task = await dbContext.Tasks.FindAsync(Guid.Parse(taskId));

                if (task == null)
                {
                    return NotFound(new { message = "Task not found" });
                }

                // Delete the task
                dbContext.Tasks.Remove(task);

                // Save changes to the database
                await dbContext.SaveChangesAsync();

                return Ok(new { message = "Task deleted successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "An error occurred while deleting the task.", error = ex.Message });
            }

        }



    }
}
