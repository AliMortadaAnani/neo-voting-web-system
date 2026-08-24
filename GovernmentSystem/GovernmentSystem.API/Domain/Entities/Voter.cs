namespace GovernmentSystem.API.Domain.Entities
{
    public class Voter
    {
        public int Id { get; private set; }

        public string VotingToken { get; private set; } = string.Empty;

        public string HashedData { get; private set; } = string.Empty;

        public int CitizenId { get; private set; }
        public Citizen Citizen { get; private set; } = null!;

        private Voter()
        { }

        public static Voter Create(
            string votingToken,
            string hashedData,
            int citizenId
            )
        {
            ValidateVotingToken(votingToken);
            ValidateHashedData(hashedData);
            ValidateCitizenId(citizenId);
            return new Voter
            {
                VotingToken = votingToken,
                HashedData = hashedData,
                CitizenId = citizenId
            };
        }

        private static void ValidateVotingToken(string votingToken)
        {
            if (string.IsNullOrWhiteSpace(votingToken))
                throw new ArgumentException("Voting token cannot be null or empty.");
        }

        private static void ValidateHashedData(string hashedData)
        {
            if (string.IsNullOrWhiteSpace(hashedData))
                throw new ArgumentException("Hashed data cannot be null or empty.");
        }

        private static void ValidateCitizenId(int citizenId)
        {
            if (citizenId <= 0)
                throw new ArgumentException("Citizen ID must be a positive integer.");
        }

        public void Update(
            string votingToken,
            string hashedData
           )
        {
            ValidateVotingToken(votingToken);
            ValidateHashedData(hashedData);
            VotingToken = votingToken;
            HashedData = hashedData;
        }
    }
}