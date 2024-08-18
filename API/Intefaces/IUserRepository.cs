using System;
using API.DTOs;
using API.Entities;

namespace API.Intefaces;

public interface IUserRepository
{
    void Update(AppUser user);
    Task<bool> SaveAllAsync();
    Task<IEnumerable<AppUser>> GetUsersAsync();

     Task<AppUser?> GetUserByIdAsync(int id);

     Task<AppUser?> GetUserByUsernameAsync(String username);

     Task<IEnumerable<MemberDTO>> GetMembers();

     Task<MemberDTO?> GetMemberAsync(string username);
     
}
