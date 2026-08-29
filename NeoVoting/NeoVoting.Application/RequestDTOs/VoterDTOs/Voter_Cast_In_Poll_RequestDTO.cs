namespace NeoVoting.Application.RequestDTOs.VoterDTOs
{
    public class Voter_Cast_In_Poll_RequestDTO
    {
        public int? SelectedPollAnswerId { get; set; }
        public string? SelectedPollAnswer { get; set; }

        //double check if the voter chose the correct choice he wanted to vote for
    }
}