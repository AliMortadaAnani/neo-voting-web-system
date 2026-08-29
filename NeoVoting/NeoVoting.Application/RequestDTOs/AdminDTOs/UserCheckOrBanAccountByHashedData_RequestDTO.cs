namespace NeoVoting.Application.RequestDTOs.AdminDTOs
{
    public class UserCheckOrBanAccountByHashedData_RequestDTO
    {
        public string? HashedData { get; set; } //this info is provided manually by Government System here,
        // but in Login, its returned as Success Response from Government System API
    }
}