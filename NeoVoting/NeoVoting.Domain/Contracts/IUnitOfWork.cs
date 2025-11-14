namespace NeoVoting.Domain.Contracts
{
    /// <summary>
    /// Represents the Unit of Work pattern. It is responsible for coordinating the work of multiple
    /// repositories into a single transaction.
    /// </summary>
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
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
        /// <returns>The number of state entries written to the database.</returns>
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);


        /*
         * By having IUnitOfWork implement IDisposable, we ensure that the consuming class can properly dispose of the DbContext when it's done (often handled automatically by the dependency injection container).
         */

    }
}
