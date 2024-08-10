using System;
using API.Entities;

namespace API.Intefaces;

public interface ITokenService
{
 string CreateToken(AppUser user);
}
