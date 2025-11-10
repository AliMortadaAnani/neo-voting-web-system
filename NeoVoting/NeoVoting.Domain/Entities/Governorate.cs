namespace NeoVoting.Domain.Entities
{
    public class Governorate
    {
        // The primary key of the table.
        
        public int Id { get; set; }

        // The name of the governorate.
        public required string Name { get; set; }

        // --- Constructor ---

        /// <summary>
        /// A private constructor to force all object creation to go through
        /// the controlled, static factory methods.
        /// EF Core uses this for materializing objects from the database.
        /// </summary>
        private Governorate() { }


        // --- Factory Methods ---

        /// <summary>
        /// Creates a new instance representing the Beirut governorate.
        /// </summary>
        public static Governorate CreateBeirut()
        {
            return new Governorate { Id = 1, Name = "Beirut" };
        }

        /// <summary>
        /// Creates a new instance representing the Mount Lebanon governorate.
        /// </summary>
        public static Governorate CreateMountLebanon()
        {
            return new Governorate { Id = 2, Name = "Mount Lebanon" };
        }

        /// <summary>
        /// Creates a new instance representing the South governorate.
        /// </summary>
        public static Governorate CreateSouth()
        {
            return new Governorate { Id = 3, Name = "South" };
        }

        /// <summary>
        /// Creates a new instance representing the East governorate.
        /// </summary>
        public static Governorate CreateEast()
        {
            return new Governorate { Id = 4, Name = "East" };
        }

        /// <summary>
        /// Creates a new instance representing the North governorate.
        /// </summary>
        public static Governorate CreateNorth()
        {
            return new Governorate { Id = 5, Name = "North" };
        }
    }
}