using FluentValidation;

namespace NeoVoting.Application.Validators.AdminDTOs
{
    public class ElectionCurrentActiveStatistics_ResponseDTO_Validator : AbstractValidator<ElectionCurrentActiveStatistics_ResponseDTO>
    {
        public ElectionCurrentActiveStatistics_ResponseDTO_Validator()
        {
            // All fields are nullable/optional in this DTO
            // This DTO is designed to allow partial population of statistics
        }
    }
}