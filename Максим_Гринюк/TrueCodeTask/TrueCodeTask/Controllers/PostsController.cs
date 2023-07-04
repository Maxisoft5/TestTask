using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using TrueCodeTask.Interfaces;

namespace TrueCodeTask.Controllers
{
    public class PostsController : Controller
    {
        private readonly IPostsServices _postsServices;
        public PostsController(IPostsServices postsServices) 
        { 
            _postsServices = postsServices;
        }

        /// <summary>
        /// Return postCount last posts concrete user with userId, posts will be ordered by asc publish date
        /// </summary>
        [SwaggerOperation(
            Summary = "Posts by one user",
            Description = "Returns defined posts count of concrete user with userId, posts will be ordered by ascending publish date",
            OperationId = "GetByUser")]
        [HttpGet("PostsByUser")]
        public async Task<IActionResult> GetByUser([FromQuery] int userId, [FromQuery] int postCount)
        {
            if (userId == 0)
            {
                return BadRequest("Empty user id");
            }
            return Ok(await _postsServices.GetAllPostsByUser(userId, postCount));
        }

        [SwaggerOperation(
            Summary = "Posts by users",
            Description = "Returns defined posts count of concrete users with userIds, posts will be ordered by ascending publish date",
            OperationId = "GetByUsers")]
        [HttpGet("PostsByUsers")]
        public async Task<IActionResult> GetByUsers(
            [FromBody] List<int> userIds, 
            [FromQuery] int postCount)
        {
            return Ok(await _postsServices.GetAllPostsByUsers(userIds, postCount));
        }
    }
}
