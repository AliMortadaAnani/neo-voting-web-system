namespace NeoVoting.Application.RequestDTOs
{
    /// <summary>
    /// Represents the data required to update the text details of an existing candidate profile.
    /// </summary>
    public class CandidateProfileUpdateRequest
    {
        // While the ID will be in the URL route (e.g., PUT /api/profiles/{id}),
        // including it in the body can be useful for validation or logging.
        public Guid Id { get; set; }

        public string Goals { get; set; } = string.Empty;
        public string NominationReasons { get; set; } = string.Empty;

        // Note: The mapping for an update is different. We don't create a new entity.
        // Instead, the service layer will fetch the existing entity and then
        // call a method like 'UpdateDetails' on it, passing in the values from this DTO.
        // Therefore, a 'ToCandidateProfile' method is not appropriate here.
    }
}