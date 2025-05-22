using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using KoboRack.Data.Context;
using KoboRack.Data.Repositories.Interface;
using KoboRack.Data.Repository.DTO;
using KoboRack.Model.Entities;
using System.Linq.Expressions;

namespace KoboRack.Data.Repositories.Implementation
{
    public class GroupSavingsMembersRepository : IGroupSavingsMembersRepository
    {
        private readonly SaviDbContext _saviDbContext;
        private readonly UserManager<AppUser> _userManager;
        private readonly IMapper _mapper;

        public GroupSavingsMembersRepository(SaviDbContext saviDbContext, IMapper mapper, UserManager<AppUser> userManager)
        {
            _saviDbContext = saviDbContext;
            _mapper = mapper;
            _userManager = userManager;
        }

        public async Task<List<GroupMembersDto2>> GetListOfGroupMembersAsync(string UserId)
        {
            var listOfGroups = await _saviDbContext.GroupSavingsMembers.Where(x => x.UserId == UserId).ToListAsync();
            var member = new List<GroupMembersDto2>();
            if (listOfGroups.Count > 0)
            {
                foreach (var item in listOfGroups)
                {
                    var user = await _userManager.FindByIdAsync(item.UserId);
                    var mapUser = _mapper.Map<AppUserDto2>(user);
                    var mapGroup = _mapper.Map<GroupMembersDto2>(item);
                    mapGroup.User = mapUser;
                    member.Add(mapGroup);
                }
                return member;
            }
            return null;
        }
        public async Task<bool> CheckIfUserExist(string UserId, string GroupId)
        {
            var group = await GetListOfGroupMembersAsync(UserId);
            if (group != null)
            {
                var userExist = group.FirstOrDefault(x => x.UserId == UserId);
                if (userExist != null)
                {
                    return true;
                }
                return false;
            }
            return false;
        }
        public async Task<ResponseDto2<AppUserDto2>> GetUserByIdAsync(string userId)
        {
            var user = await _saviDbContext.Users.FindAsync(userId);
            if (user == null)
            {
                var notFoundResponse = new ResponseDto2<AppUserDto2>
                {
                    StatusCode = StatusCodes.Status404NotFound,
                    DisplayMessage = "User not found"
                };
                return notFoundResponse;
            }
            var result = _mapper.Map<AppUserDto2>(user);
            var success = new ResponseDto2<AppUserDto2>
            {
                StatusCode = StatusCodes.Status200OK,
                DisplayMessage = "User Found",
                Result = result
            };
            return success;
        }
        public async Task<int> GetGroupLastUserPosition(string GroupId)
        {
            var group = await _saviDbContext.GroupSavingsMembers.Where(x => x.GroupSavingsId == GroupId).ToListAsync();
            int lastPosition = group.OrderByDescending(user => user.Positions)
                              .Select(user => user.Positions)
                              .FirstOrDefault();
            return lastPosition;
        }
        public async Task<bool> CreateSavingsGroupMembersAsync(GroupSavingsMembers groupSavingsMembers)
        {
            await _saviDbContext.GroupSavingsMembers.AddAsync(groupSavingsMembers);
            var result = await _saviDbContext.SaveChangesAsync();
            if (result > 0)
            {
                return true;

            }
            return false;
        }
        public List<GroupSavingsMembers> FindAsync(Expression<Func<GroupSavingsMembers, bool>> expression)
        {
            return _saviDbContext.Set<GroupSavingsMembers>().Where(expression).ToList();
        }
        public async Task<GroupSavingsMembers> FindAsync2(Expression<Func<GroupSavingsMembers, bool>> expression)
        {
            return await _saviDbContext.Set<GroupSavingsMembers>().FirstOrDefaultAsync(expression);
        }
        public async Task<int> GetNextPositionToPay(string GroupId)
        {
            var nextPosition = await _saviDbContext.GroupSavingsMembers
           .Where(m => m.GroupSavingsId == GroupId && !m.IsPaid)
           .OrderBy(m => m.Positions)
           .Select(m => m.Positions)
           .FirstOrDefaultAsync();

            return nextPosition;
        }
        public async Task<int> GetTotalPositionsInGroup(string groupId)
        {
            return await _saviDbContext.GroupSavingsMembers
                .Where(x => x.GroupSavingsId == groupId)
                .Select(x => x.Positions)
                .MaxAsync(); 
        }
        public void UpdateGroupSavingsMember(GroupSavingsMembers groupSavingMembers)
        {
            _saviDbContext.GroupSavingsMembers.Update(groupSavingMembers);
        }
    }
}
