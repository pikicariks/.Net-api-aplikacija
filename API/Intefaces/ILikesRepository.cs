using System;
using API.DTOs;
using API.Entities;
using API.Helpers;

namespace API.Intefaces;

public interface ILikesRepository
{
    Task<UserLike?> GetUserLike(int sourceUserId,int targetUserId);
     Task<PageList<MemberDTO>> GetUserLikes(LikesParams likesParams);

     Task<IEnumerable<int>> GetCurrentUserLikeIds(int current);

     void DeleteLike(UserLike like);
     void AddLike(UserLike like);

     
}
