namespace GovernmentSystem.API.Domain.Contracts
{
    public interface IUnitOfWork : IDisposable
    {
        // Here you can also expose your repository interfaces if you want the UoW to be
        // a single gateway for all data access. This is an optional but common pattern.
        // Example: IElectionRepository Elections { get; }
        // Example: ICandidateProfileRepository CandidateProfiles { get; }

        /// <summary>
        /// Saves all changes made in this unit of work to the underlying database.
        /// This is the method that commits the transaction.
        /// </summary>
        
        /// <returns>The number of state entries written to the database.</returns>
        Task<int> SaveChangesAsync();


        /*
         * By having IUnitOfWork implement IDisposable, we ensure that the consuming class can properly dispose of the DbContext when it's done (often handled automatically by the dependency injection container).
         */

    }
}
