using FluentValidation;
using NeoVoting.Application.ResponseDTOs;

namespace NeoVoting.Application.Validators
{
    public class CandidateProfile_ResponseDTO_Validator : AbstractValidator<CandidateProfile_ResponseDTO>
    {
        public CandidateProfile_ResponseDTO_Validator()
        {
            // Required fields (non-nullable)
            RuleFor(x => x.Id).NotEmpty();
            RuleFor(x => x.UserId).NotEmpty();
            RuleFor(x => x.ElectionId).NotEmpty();

            // Optional fields (nullable) - no validation rules needed
            // Goals, NominationReasons, ProfilePhotoFilename, FirstName, LastName, DateOfBirth, Gender, GovernorateId
        }
    }
}
