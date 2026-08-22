using GovernmentSystem.API.Application.RequestDTOs.CitizenDTOs;
using GovernmentSystem.API.Application.ResponseDTOs.CitizenDTOs;
using GovernmentSystem.API.Application.ServicesContracts;
using GovernmentSystem.API.Domain.RepositoryContracts;
using GovernmentSystem.API.Domain.ResultErrorDomain;

namespace GovernmentSystem.API.Application.Services
{
    public class CitizenServices : ICitizenServices
    {   
        private readonly ICitizenRepository _citizenRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CitizenServices(ICitizenRepository citizenRepository, IUnitOfWork unitOfWork)
        {
            _citizenRepository = citizenRepository;
            _unitOfWork = unitOfWork;
        }

        public Task<Result<CitizenResponseDTO>> AddCitizenAsync(CreateCitizenRequestDTO request)
        {
            throw new NotImplementedException();
        }

        public Task<Result<bool>> DeleteCitizenByNationalIdAsync(DeleteCitizenRequestDTO request)
        {
            throw new NotImplementedException();
        }

        public Task<Result<CitizenResponseDTO>> GetCitizenByNationalIdAsync(GetCitizenRequestDTO request)
        {
            throw new NotImplementedException();
        }

        public Task<Result<List<CitizenResponseDTO>>> GetCitizensPagedAsync(int pageNumber, int pageSize)
        {
            throw new NotImplementedException();
        }

        public Task<Result<int>> GetCitizensTotalCountAsync()
        {
            throw new NotImplementedException();
        }

        public Task<Result<CitizenResponseDTO>> UpdateCitizenByNationalIdAsync(UpdateCitizenRequestDTO request)
        {
            throw new NotImplementedException();
        }
    }
}
