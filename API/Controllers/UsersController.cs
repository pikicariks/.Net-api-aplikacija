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
public class UsersController(IUserRepository userRepo) : BaseApiController
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
}
