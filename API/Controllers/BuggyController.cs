using System;
using System.Reflection.Metadata.Ecma335;
using API.Data;
using API.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

public class BuggyController(DataContext ctx):BaseApiController
{

[Authorize]
[HttpGet("auth")]
public ActionResult<string> GetAuth(){
    return "neki tekst";
}

[HttpGet("not-found")]
public ActionResult<AppUser> notFound(){
    var thing = ctx.Users.Find(-1);

    if (thing==null)
    {
        return NotFound();
    }

    return thing;
}

[HttpGet("server-error")]
public ActionResult<AppUser> serverGet(){
    

    var thingi = ctx.Users.Find(-1)?? throw new Exception("nesto se desilo");

    return thingi;
    
}

[HttpGet("bad-request")]
public ActionResult<string> badGet(){

    return BadRequest("nije bilo dobro");
   
}
}
