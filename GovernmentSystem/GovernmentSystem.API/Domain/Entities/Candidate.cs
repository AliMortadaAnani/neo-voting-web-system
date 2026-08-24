namespace GovernmentSystem.API.Domain.Entities
{
    public class Candidate
    {
        public int Id { get; private set; }

        public string NominationToken { get; private set; } = string.Empty;

        public string HashedData { get; private set; } = string.Empty;

        public int CitizenId { get; private set; }
        public Citizen Citizen { get; private set; } = null!;

        private Candidate()
        { }

        public static Candidate Create(
            string nominationToken,
            string hashedData,
            int citizenId
            )
        {
            ValidateNominationToken(nominationToken);
            ValidateHashedData(hashedData);
            ValidateCitizenId(citizenId);
            return new Candidate
            {
                NominationToken = nominationToken,
                HashedData = hashedData,
                CitizenId = citizenId
            };
        }

        private static void ValidateNominationToken(string nominationToken)
        {
            if (string.IsNullOrWhiteSpace(nominationToken))
                throw new ArgumentException("Nomination token cannot be null or empty.");
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
            string nominationToken,
            string hashedData
           )
        {
            ValidateNominationToken(nominationToken);
            ValidateHashedData(hashedData);
            NominationToken = nominationToken;
            HashedData = hashedData;
        }
    }
}