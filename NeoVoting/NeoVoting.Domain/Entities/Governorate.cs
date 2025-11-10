using NeoVoting.Domain.Enums;

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


        public static Governorate CreateBeirut()
        {
            return new Governorate { Id = (int)GovernoratesEnum.Beirut, Name = GovernoratesEnum.Beirut.GetDescription() };
        }

        public static Governorate CreateMountLebanon()
        {
            return new Governorate { Id = (int)GovernoratesEnum.MountLebanon, Name = GovernoratesEnum.MountLebanon.GetDescription() };
        }

        public static Governorate CreateSouth()
        {
            return new Governorate { Id = (int)GovernoratesEnum.South, Name = GovernoratesEnum.South.GetDescription() };
        }

        public static Governorate CreateEast()
        {
            return new Governorate { Id = (int)GovernoratesEnum.East, Name = GovernoratesEnum.East.GetDescription() };
        }

        public static Governorate CreateNorth()
        {
            return new Governorate { Id = (int)GovernoratesEnum.North, Name = GovernoratesEnum.North.GetDescription() };
        }
    }
}