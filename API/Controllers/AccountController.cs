using System;
using System.Security.Cryptography;
using System.Text;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Intefaces;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

public class AccountController(DataContext ctx,ITokenService tokenService,IMapper mapper):BaseApiController
{
    [HttpPost("register")]
    public async Task<ActionResult<UserDTO>> Register(RegisterDTO registerDTO){
        
        if (await UserExists(registerDTO.Username))
        {
            return BadRequest("Username is taken");
        }

      
        using var hmac=new HMACSHA512();

        var user = mapper.Map<AppUser>(registerDTO);

        user.UserName = registerDTO.Username.ToLower();
        user.PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDTO.Password));
        user.PasswordSalt = hmac.Key;

       

        ctx.Users.Add(user);
        await ctx.SaveChangesAsync();

        return new UserDTO{
            Username = user.UserName,
            Token = tokenService.CreateToken(user),
            KnownAs = user.KnownAs
        }; 
    }

    [HttpPost("login")]

    public async Task<ActionResult<UserDTO>> Login(LoginDTO loginDTO){
        
    var user = await ctx.Users.Include(p=>p.Photos).FirstOrDefaultAsync(x=>x.UserName==loginDTO.Username.ToLower());
    
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
    KnownAs = user.KnownAs,
    Token = tokenService.CreateToken(user),
    PhotoUrl = user.Photos.FirstOrDefault(x=>x.IsMain)?.Url
   };
   
    }

    private async Task<bool> UserExists(string username){
            return await ctx.Users.AnyAsync(x=>x.UserName.ToLower()==username.ToLower());
    }
}

