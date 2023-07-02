using API.Data;
using API.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]

    public class UsersController:ControllerBase
    {
        private readonly DataContext ctx;

        public UsersController(DataContext ctx)
        {
            this.ctx = ctx;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<AppUser>>> GetUsers(){
            var users = await ctx.Users.ToListAsync();

            return users;
        }
        
        [HttpGet("{id}")]

        public async Task<ActionResult<AppUser>> GetUser(int id){
            var user = await ctx.Users.FindAsync(id);

            return user;
        }
    }
}