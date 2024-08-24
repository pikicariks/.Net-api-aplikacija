using System.Security.Claims;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Intefaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;


[Authorize]
public class UsersController(IUserRepository userRepo,IMapper map) : BaseApiController
{
   
    [HttpGet]
    public async Task<ActionResult<IEnumerable<MemberDTO>>> GetUsers(){
        var users = await userRepo.GetMembers();


        return Ok(users);
    }
    
    [HttpGet("{username}")]
    public async Task<ActionResult<MemberDTO>> GetUsers(string username){
        var user =await userRepo.GetMemberAsync(username);

        if (user==null) return NotFound();
        
         return user;
    }

    [HttpPut]
    public async Task<ActionResult> UpdateUser(MemberUpdateDTO memberUpdateDTO){

        var username = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    
        if (username==null)
        {
            return BadRequest("No username found in token");
        }
        
        var user = await userRepo.GetUserByUsernameAsync(username);

        if(user==null) return BadRequest("Could not find user");

        map.Map(memberUpdateDTO,user);

        if(await userRepo.SaveAllAsync()) return NoContent();

        return BadRequest("Failed to update the user");
    }
}
