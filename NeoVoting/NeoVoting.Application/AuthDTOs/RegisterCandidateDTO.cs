namespace NeoVoting.Application.AuthDTOs
{
    public class RegisterCandidateDTO
    {
        public string? UserName { get; set; }
        public string? NewPassword { get; set; }
        public string? ConfirmPassword { get; set; }

        public Guid? NationalId { get; set; }

        public Guid? NominationToken { get; set; }
    }
}