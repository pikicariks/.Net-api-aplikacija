﻿using System.Security.Claims;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Intefaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace API.Controllers;


[Authorize]
public class UsersController(IUserRepository userRepo,IMapper map,IPhotoService photoService) : BaseApiController
{
   
    [HttpGet]
    public async Task<ActionResult<IEnumerable<MemberDTO>>> GetUsers(){
        var users = await userRepo.GetMembers();


        return Ok(users);
    }
    
    [HttpGet("{username}")]
    public async Task<ActionResult<MemberDTO>> GetUser(string username){
        var user =await userRepo.GetMemberAsync(username);

        if (user==null) return NotFound();
        
         return user;
    }

    [HttpPut]
    public async Task<ActionResult> UpdateUser(MemberUpdateDTO memberUpdateDTO){

        
        
        var user = await userRepo.GetUserByUsernameAsync(User.GetUsername());

        if(user==null) return BadRequest("Could not find user");

        map.Map(memberUpdateDTO,user);

        if(await userRepo.SaveAllAsync()) return NoContent();

        return BadRequest("Failed to update the user");
    }

    [HttpPost("add-photo")]
    public async Task<ActionResult<PhotoDto>> AddPhoto(IFormFile file){

        var user = await  userRepo.GetUserByUsernameAsync(User.GetUsername());

        if (user==null)
        {
            return BadRequest("Cannot update user");
        }

        var result = await photoService.AddPhotoAsync(file);

        if (result.Error!=null)
        {
            return BadRequest(result.Error.Message);
        }

        var photo = new Photo{
            Url = result.SecureUrl.AbsoluteUri,
            PublicId = result.PublicId
        };

        if (user.Photos.Count == 0) photo.IsMain = true;

        user.Photos.Add(photo);

        if (await userRepo.SaveAllAsync())
        {
            return CreatedAtAction(nameof(GetUser),
            new {username = user.UserName},map.Map<PhotoDto>(photo));
        }

        return BadRequest("Problem adding photos");
    } 

    [HttpPut("set-main-photo/{photoId:int}")]
    public async Task<ActionResult> SetMainPhoto(int photoId){

        var user = await userRepo.GetUserByUsernameAsync(User.GetUsername());

        if (user==null)
        {
            return BadRequest("Could not find user");


        }

        var photo = user.Photos.FirstOrDefault(x=>x.Id==photoId);

        if(photo==null || photo.IsMain) return BadRequest("Cannot use this as main photo");

        var current = user.Photos.FirstOrDefault(x=>x.IsMain);
        if (current!=null) current.IsMain=false;
        photo.IsMain = true;

        if (await userRepo.SaveAllAsync())
        {
            return NoContent();
        }

        return BadRequest("Problem setting main photo");
    }

     [HttpDelete("delete-photo/{photoId:int}")]
    public async Task<ActionResult> DeletePhoto(int photoId){

        var user = await userRepo.GetUserByUsernameAsync(User.GetUsername());

        if (user==null)
        {
            return BadRequest("Could not find user");


        }

        var photo = user.Photos.FirstOrDefault(x=>x.Id==photoId);

        if(photo==null || photo.IsMain) return BadRequest("Cannot use this as main photo");

        if(photo.PublicId!=null){
            var result  = await photoService.DeletePhotoAsync(photo.PublicId);
            if(result.Error!=null) return BadRequest(result.Error.Message);
        }

        user.Photos.Remove(photo);
        

        if (await userRepo.SaveAllAsync())
        {
            return Ok();
        }

        return BadRequest("Problem deleting photo");
    }
}
