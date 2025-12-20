using NeoVoting.Domain.Enums;

namespace NeoVoting.Domain.Entities
{
    public class Governorate
    {
        public int Id { get; private set; }
        public string Name { get; init; }
        //we will use init to make it immutable after creation

        private Governorate()
        { }

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