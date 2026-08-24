namespace GovernmentSystem.API.Application.RequestDTOs.AdminDTOs

{
    public class LoginDTO
    {
        // we are making fields optional to ensure only FluentValidation Rules work,not default ASP
        public string? Username { get; set; }

        public string? Password { get; set; }
    }
}