namespace NeoVoting.Application.RequestDTOs.AdminDTOs
{
    public class ElectionCreate_RequestDTO
    {
        public string? Name { get; set; }
        public DateTime? NominationStartDate { get; set; }
        public DateTime? NominationEndDate { get; set; }
        public DateTime? VotingStartDate { get; set; }
        public DateTime? VotingEndDate { get; set; }
    }
}