using NeoVoting.Domain.EF_DTOs;

namespace NeoVoting.Application.ResponseDTOs.GeneralDTOs
{
    public class CompletedElectionResults_ResponseDTO
    {
        // global or per governorate results
        public List<CandidateResultResponseDTO> winners = new List<CandidateResultResponseDTO>();

        public List<CandidateResultResponseDTO> totalCandidatesResult = new List<CandidateResultResponseDTO>();
    }
}