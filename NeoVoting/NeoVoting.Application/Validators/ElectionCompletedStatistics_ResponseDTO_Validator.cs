using FluentValidation;
using NeoVoting.Application.ResponseDTOs;

namespace NeoVoting.Application.Validators
{
    public class ElectionCompletedStatistics_ResponseDTO_Validator : AbstractValidator<ElectionCompletedStatistics_ResponseDTO>
    {
        public ElectionCompletedStatistics_ResponseDTO_Validator()
        {
            // All fields are nullable/optional in this DTO
            // This DTO is designed to allow partial population of statistics
        }
    }
}
