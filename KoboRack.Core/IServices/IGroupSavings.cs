using Microsoft.AspNetCore.Http;
using KoboRack.Core.DTO;
using KoboRack.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KoboRack.Core.IServices
{
    public interface IGroupSavings
    {
        ResponseDto<List<GroupDTO>> GetExploreGroupSavingGroups();
        ResponseDto<GroupDTO> GetExploreGroupSavingDetails(string groupId);
        ResponseDto<GroupDTO> GetGroupSavingAccountDetails(string groupId);
        ResponseDto<List<GroupDTO>> GetListOfActiveSavingsGroups();
        Task<ResponseDto<string>> CreateSavingsGroup(GroupDTO2 dto);
        ResponseDto<int> GetTotalSavingsGroupCountAsync();
        ResponseDto<int> GetNewGroupCount();
        ResponseDto<List<GroupDTO>> GetAllGroups();
        ResponseDto<List<GroupDTO>> GetGroupsCreatedToday();
        ResponseDto<GroupDTO> GetGroupDetails(string groupId);
    }
}
