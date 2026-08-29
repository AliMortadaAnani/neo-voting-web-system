namespace NeoVoting.Application.RequestDTOs.AdminDTOs
{
    public class UserResetPasswordByUserName_RequestDTO
    {
        public string? UserName { get; set; }
        public string? NewPassword { get; set; }

        public string? ConfirmPassword { get; set; }
    }
}