using System;
using API.DTOs;
using API.Entities;
using API.Helpers;

namespace API.Intefaces;

public interface IUserRepository
{
    void Update(AppUser user);
    Task<bool> SaveAllAsync();
    Task<IEnumerable<AppUser>> GetUsersAsync();

     Task<AppUser?> GetUserByIdAsync(int id);

     Task<AppUser?> GetUserByUsernameAsync(String username);

     Task<PageList<MemberDTO>> GetMembers(UserParams userParams);

     Task<MemberDTO?> GetMemberAsync(string username);
     
}
