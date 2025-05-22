using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using KoboRack.Core.DTO;
using KoboRack.Core.IServices;
using KoboRack.Data.Repositories.Interface;
using KoboRack.Model;
using KoboRack.Model.Entities;
using KoboRack.Utility.Pagination;

namespace KoboRack.Core.Services
{
    public class KycService : IKycService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<KycService> _logger;
        private readonly ICloudinaryServices<Kyc> _cloudinaryServices;
        private readonly UserManager<AppUser> _userManager;

        public KycService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<KycService> logger, IServices.ICloudinaryServices<Kyc> cloudinaryServices, UserManager<AppUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
            _cloudinaryServices = cloudinaryServices;
            _userManager = userManager;
        }

        public async Task<ApiResponse<KycResponseDto>> AddKycAsync(string userId, KycRequestDto kycRequestDto)
        {
            try
            {
                var existingUser = await _userManager.FindByIdAsync(userId);
                if (existingUser == null)
                {
                    return new ApiResponse<KycResponseDto>(false, "User does not exist", StatusCodes.Status404NotFound);
                }
                var existingKyc = await _unitOfWork.KycRepository.GetKycByIdAsync(userId);
                if (existingKyc != null)
                {
                    return new ApiResponse<KycResponseDto>(false, "KYC already exists for the user", StatusCodes.Status400BadRequest);
                }
                var newKyc = _mapper.Map<Kyc>(kycRequestDto);
                await _unitOfWork.KycRepository.AddKycAsync(newKyc);
                var entity = newKyc.Id;

                var identificationImageUrl = await _cloudinaryServices.UploadImage(entity, kycRequestDto.IdentificationDocumentUrl);
                var proofOfAddressImageUrl = await _cloudinaryServices.UploadImage(entity, kycRequestDto.ProofOfAddressUrl);

                if (identificationImageUrl == null || proofOfAddressImageUrl == null)
                {
                    _logger.LogError("Failed to upload one or more documents");
                    return new ApiResponse<KycResponseDto>(false, "Failed to upload one or more documents", StatusCodes.Status500InternalServerError);
                }

                newKyc.IdentificationDocumentUrl = identificationImageUrl;
                newKyc.ProofOfAddressUrl = proofOfAddressImageUrl;
                newKyc.AppUserId = userId;

                _unitOfWork.SaveChanges();

                var addedKycDto = _mapper.Map<KycResponseDto>(newKyc);
                return new ApiResponse<KycResponseDto>(true, "KYC added successfully", StatusCodes.Status201Created, addedKycDto);                
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while adding KYC");
                return new ApiResponse<KycResponseDto>(false, "Error occurred while processing your request", StatusCodes.Status500InternalServerError, new List<string> { ex.Message});
            }
        }

        public async Task<ApiResponse<bool>> DeleteKycByIdAsync(string kycId)
        {
            try
            {
                var existingKyc = await _unitOfWork.KycRepository.GetKycByIdAsync(kycId);
                if (existingKyc == null)
                {
                    return new ApiResponse<bool>(false, "KYC not found", StatusCodes.Status404NotFound);
                }
                await _unitOfWork.KycRepository.DeleteKycAsync(existingKyc);
                return new ApiResponse<bool>(true, "KYC deleted successfully", StatusCodes.Status200OK);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting KYC");
                return new ApiResponse<bool>(false, "Error occurred while processing your request", StatusCodes.Status500InternalServerError, new List<string> { ex.Message });
            }
        }

        public async Task<ApiResponse<KycRequestDto>> GetKycByIdAsync(string id)
        {
            try
            {
                var kyc = await _unitOfWork.KycRepository.GetKycByIdAsync(id);
                if (kyc == null)
                {
                    return new ApiResponse<KycRequestDto>(false, "KYC not found", StatusCodes.Status404NotFound);
                }
                var kycDto = _mapper.Map<KycResponseDto>(kyc);
                return new ApiResponse<KycRequestDto>(true, "KYC found successfully", StatusCodes.Status200OK);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting KYC by Id");
                return new ApiResponse<KycRequestDto>(false, "Error occurred while processing your request", StatusCodes.Status500InternalServerError, new List<string> { ex.Message });
            }
        }

        public async Task<ApiResponse<GetAllKycsDto>> GetKycsByPaginationAsync(int page, int perPage)
        {
            try
            {
                var kycs = _unitOfWork.KycRepository.GetAllKycs();
                var kycDtos = _mapper.Map<List<KycResponseDto>>(kycs);
                var pagedResult = await PagePagination<KycResponseDto>.GetPager(
                    kycDtos,
                    perPage,
                    page,
                    kyc => kyc.IdentificationDocumentUrl, 
                    kyc => kyc.IdentificationNumber
                );
                var response = new GetAllKycsDto
                {
                    Kycs = pagedResult.Data.ToList(),
                    TotalCount = pagedResult.TotalCount,
                    TotalPageCount = pagedResult.TotalPageCount,
                    PerPage = pagedResult.PerPage,
                    CurrentPage = pagedResult.CurrentPage
                };
                return new ApiResponse<GetAllKycsDto>(true, "KYCs retrieved successfully", StatusCodes.Status200OK, response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting all KYCs");
                return new ApiResponse<GetAllKycsDto>(false, "Error occurred while processing your request", StatusCodes.Status500InternalServerError, new List<string> { ex.Message });
            }
        }

        public async Task<ApiResponse<bool>> IsKycVerifiedAsync(string userId)
        {
            try
            {
                var existingKyc = await _unitOfWork.KycRepository.FindKyc(kyc => kyc.AppUserId == userId);
                if (existingKyc == false)
                {
                    return new ApiResponse<bool>(false, "KYC is not verified", StatusCodes.Status404NotFound, false);
                }
                return new ApiResponse<bool>(true, "KYC is verified", StatusCodes.Status200OK, true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while verifying KYC");
                return new ApiResponse<bool>(false, "Error occurred while processing your request", StatusCodes.Status500InternalServerError, new List<string> { ex.Message});
            }
        }
    }
}
