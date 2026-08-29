namespace NeoVoting.Application.RequestDTOs.VoterDTOs
{
    public class Voter_Cast_In_Election_RequestDTO
    {
        public List<int>? SelectedCandidateProfileIds { get; set; } // 5 and from the voter governorate
    }
}