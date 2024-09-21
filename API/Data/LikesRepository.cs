using System;
using API.DTOs;
using API.Entities;
using API.Helpers;
using API.Intefaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace API.Data;

public class LikesRepository(DataContext context,IMapper mapper) : ILikesRepository
{
    public void AddLike(UserLike like)
    {
        context.Likes.Add(like);
    }

    public void DeleteLike(UserLike like)
    {
        context.Likes.Remove(like);
    }

    public async Task<IEnumerable<int>> GetCurrentUserLikeIds(int current)
    {
        return await context.Likes.Where(x=>x.SourceUserId==current)
        .Select(x=>x.TargetUserId).ToListAsync();
    }

    public async Task<UserLike?> GetUserLike(int sourceUserId, int targetUserId)
    {
        return await context.Likes.FindAsync(sourceUserId,targetUserId);
    }

    public async Task<PageList<MemberDTO>> GetUserLikes(LikesParams likesParams)
    {
        var likes = context.Likes.AsQueryable();
        IQueryable<MemberDTO> query;

        switch (likesParams.Predicate)
        {
            case "liked":
            query = likes.Where(x=>x.SourceUserId==likesParams.UserId)
            .Select(x=>x.TargetUser)
            .ProjectTo<MemberDTO>(mapper.ConfigurationProvider);
            break;

            case "likedBy":
            query= likes.Where(x=>x.TargetUserId==likesParams.UserId)
            .Select(x=>x.SourceUser)
            .ProjectTo<MemberDTO>(mapper.ConfigurationProvider);
            break;
            default:
                var likedIds = await GetCurrentUserLikeIds(likesParams.UserId);
                query= likes.Where(x=>x.TargetUserId==likesParams.UserId && likedIds.Contains(x.SourceUserId))
                .Select(x=>x.SourceUser)
                .ProjectTo<MemberDTO>(mapper.ConfigurationProvider);
            break;
        }

        return await PageList<MemberDTO>.CreateAsync(query,likesParams.PageNumber,likesParams.PageSize);
    }

    
}
