using NeoVoting.Domain.Enums;

namespace NeoVoting.Domain.Entities
{
    public class SystemAuditLog
    {
        public long Id { get; private set; }
        public DateTime TimestampUTC { get; private set; }
        public SystemActionTypesEnum ActionType { get; private set; }
        public string? Details { get; private set; } // will contain a small description of the action, e.g., "Election created with ID: {electionId}" ....
        public int AdminId { get; private set; }
        public string Username { get; private set; } = string.Empty;

        private SystemAuditLog()
        { }

        public static SystemAuditLog Create(
            int adminId,
            string userName,
            SystemActionTypesEnum actionType,
            string? details)
        {
            Validate(userName, actionType);

            return new SystemAuditLog
            {
                AdminId = adminId,
                Username = userName,
                ActionType = actionType,
                Details = details,
                TimestampUTC = DateTime.UtcNow
            };
        }

        private static void Validate(string userName, SystemActionTypesEnum actionType)
        {
            if (string.IsNullOrWhiteSpace(userName))
                throw new ArgumentException("Username is required.", nameof(userName));

            if (!Enum.IsDefined(typeof(SystemActionTypesEnum), actionType))
                throw new ArgumentException("Valid ActionType is required.", nameof(actionType));
        }
    }
}