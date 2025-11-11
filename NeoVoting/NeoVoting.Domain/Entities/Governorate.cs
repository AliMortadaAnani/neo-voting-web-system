using NeoVoting.Domain.Enums;

namespace NeoVoting.Domain.Entities
{
    public class Governorate
    {
        // TODO: Consider caching these instances if they are frequently used.
        // The primary key of the table.

        public int Id { get; private set; }

        // The name of the governorate.
        public required string Name { get; init; }
        //we will use init to make it immutable after creation
        /*private set; } */  /*= null!;*/

        // --- Constructor ---

        /// <summary>
        /// A private constructor to force all object creation to go through
        /// the controlled, static factory methods.
        /// EF Core uses this for materializing objects from the database.
        /// </summary>
        private Governorate() { }

        public override string ToString()
        {
            return $"{Name} (Id: {Id})";
        }

        public static Governorate CreateBeirutGovernorate()
        {
            return new Governorate { Id = (int)GovernoratesEnum.Beirut, Name = GovernoratesEnum.Beirut.GetDescription() };
        }

        public static Governorate CreateMountLebanonGovernorate()
        {
            return new Governorate { Id = (int)GovernoratesEnum.MountLebanon, Name = GovernoratesEnum.MountLebanon.GetDescription() };
        }

        public static Governorate CreateSouthGovernorate()
        {
            return new Governorate { Id = (int)GovernoratesEnum.South, Name = GovernoratesEnum.South.GetDescription() };
        }

        public static Governorate CreateEastGovernorate()
        {
            return new Governorate { Id = (int)GovernoratesEnum.East, Name = GovernoratesEnum.East.GetDescription() };
        }

        public static Governorate CreateNorthGovernorate()
        {
            return new Governorate { Id = (int)GovernoratesEnum.North, Name = GovernoratesEnum.North.GetDescription() };
        }
    }
}