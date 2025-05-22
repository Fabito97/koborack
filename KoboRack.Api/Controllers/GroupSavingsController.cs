using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using KoboRack.Core.DTO;
using KoboRack.Core.IServices;

namespace KoboRack.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GroupSavingsController : ControllerBase
    {
        private readonly IGroupSavings _groupSavings;
        private readonly IMapper _mapper;
        private readonly IGroupSavingsMembersServices _groupSavingsMembersServices;

        public GroupSavingsController(IGroupSavings groupSavings, IMapper mapper, IGroupSavingsMembersServices groupSavingsMembersServices)
        {
            _groupSavings = groupSavings;
            _mapper = mapper;
            _groupSavingsMembersServices = groupSavingsMembersServices;
        }

        [HttpGet("ListOfExploreGroups")]
        public IActionResult GetAllExploreGroupSavingGroups()
        {
            var response = _groupSavings.GetExploreGroupSavingGroups();

            return response.StatusCode switch
            {
                200 => Ok(response),
                404 => NotFound(response),
                500 => StatusCode(500, response),
                _ => BadRequest(response)
            };
        }
        [HttpGet("ListOfActiveGroups")]
        public IActionResult GetAllActiveGroupSavingGroups()
        {
            var response = _groupSavings.GetListOfActiveSavingsGroups();

            return response.StatusCode switch
            {
                200 => Ok(response),
                404 => NotFound(response),
                500 => StatusCode(500, response),
                _ => BadRequest(response)
            };
        }

        [HttpPost("JoinGroup")]
        public async Task<IActionResult> JoinExistingGroup(string UserId, string GroupId)
        {
            var response = await _groupSavingsMembersServices.JoinGroupSavingsAsync(UserId, GroupId);
            if (response.StatusCode == 200)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        [HttpGet("ExploreGroupDetails")]
        public IActionResult GetExploreGroupSavingDetails(string groupId)
        {
            var response = _groupSavings.GetExploreGroupSavingDetails(groupId);

            return response.StatusCode switch
            {
                200 => Ok(response),
                404 => NotFound(response),
                500 => StatusCode(500, response),
                _ => BadRequest(response)

            };


        }

        [HttpGet("ActiveGroupDetails")]
        public IActionResult GetActiveGroupSavingAccountDetails(string groupId)
        {
            var response = _groupSavings.GetGroupSavingAccountDetails(groupId);

            return response.StatusCode switch
            {
                200 => Ok(response),
                404 => NotFound(response),
                500 => StatusCode(500, response),
                _ => BadRequest(response)

            };
        }


        [HttpPost("CreateGroupSavings")]
        public async Task<IActionResult> CreateNewGroupSavingAsync([FromForm] GroupDTO2 groupDTO)
        {
            var response = await _groupSavings.CreateSavingsGroup(groupDTO);
            return Ok(response);
        }
        [HttpGet("GetMembers")]
        public async Task<IActionResult> GetGroupMembers(string GroupId)
        {
            var response = await _groupSavingsMembersServices.GetGroupMembersAsync(GroupId);
            if (response.StatusCode == 200)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        [HttpGet("TotalSavingsGroupCount")]
        public IActionResult GetTotalSavingsGroupCountAsync()
        {
            try
            {
                var response =  _groupSavings.GetTotalSavingsGroupCountAsync();
                return StatusCode(response.StatusCode, response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ResponseDto<int>
                {
                    DisplayMessage = ex.Message,
                    StatusCode = StatusCodes.Status500InternalServerError
                });
            }
        }

        [HttpGet("GetAllGroups")]
        public IActionResult GetAllGroups()
        {
            var response = _groupSavings.GetAllGroups();

            return response.StatusCode switch
            {
                200 => Ok(response),
                404 => NotFound(response),
                500 => StatusCode(500, response),
                _ => BadRequest(response)
            };
        }

        [HttpGet("GetNewGroupCount")]
        public IActionResult GetNewGroupCount()
        {
            var response = _groupSavings.GetNewGroupCount();
            return Ok(response);
        }
        [HttpGet("GetGroupsCreatedToday")]
        public IActionResult GetGroupsCreatedToday()
        {
            var response = _groupSavings.GetGroupsCreatedToday();
            return Ok(response);
        }
        [HttpGet("GetAnyGroupDetails")]
        public IActionResult GetGroupDetails(string groupId)
        {
            var response = _groupSavings.GetGroupDetails(groupId);
            return Ok(response);
        }
    }    
    
}    
        

