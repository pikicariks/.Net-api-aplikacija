using System;
using System.Security.Cryptography;
using System.Text;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Intefaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

public class AccountController(DataContext ctx,ITokenService tokenService):BaseApiController
{
    [HttpPost("register")]
    public async Task<ActionResult<UserDTO>> Register(RegisterDTO registerDTO){
        
        if (await UserExists(registerDTO.Username))
        {
            return BadRequest("Username is taken");
        }

        return Ok();
       /*  using var hmac=new HMACSHA512();

        var user = new AppUser{
            UserName = registerDTO.Username.ToLower(),
            PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDTO.Password)),
            PasswordSalt = hmac.Key
        };

        ctx.Users.Add(user);
        await ctx.SaveChangesAsync();

        return new UserDTO{
            Username = user.UserName,
            Token = tokenService.CreateToken(user)
        }; */
    }

    [HttpPost("login")]

    public async Task<ActionResult<UserDTO>> Login(LoginDTO loginDTO){
        
    var user = await ctx.Users.FirstOrDefaultAsync(x=>x.UserName==loginDTO.Username.ToLower());
    
    if (user==null)
    {
        return Unauthorized("Invalid username");
    }
   
   using var hmac = new HMACSHA512(user.PasswordSalt);
   var computed = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDTO.Password));

   for (int i = 0; i < computed.Length; i++)
   {
        if(computed[i] !=user.PasswordHash[i]) return Unauthorized("Invalid pasword");
   }
   
   return new UserDTO{
    Username = user.UserName,
    Token = tokenService.CreateToken(user)
   };
   
    }

    private async Task<bool> UserExists(string username){
            return await ctx.Users.AnyAsync(x=>x.UserName.ToLower()==username.ToLower());
    }
}

