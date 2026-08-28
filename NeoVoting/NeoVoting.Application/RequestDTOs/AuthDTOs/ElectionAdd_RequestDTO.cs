using NeoVoting.Domain.Entities;

namespace NeoVoting.Application.RequestDTOs
{
    public class ElectionAdd_RequestDTO
    {
        public string? Name { get; set; }
        public DateTime? NominationStartDate { get; set; }
        public DateTime? NominationEndDate { get; set; }
        public DateTime? VotingStartDate { get; set; }
        public DateTime? VotingEndDate { get; set; }

        public Election ToElection()
        {
            return Election.Create(
                Name!,
                NominationStartDate!.Value,
                NominationEndDate!.Value,
                VotingStartDate!.Value,
                VotingEndDate!.Value
                );
        }
    }
}