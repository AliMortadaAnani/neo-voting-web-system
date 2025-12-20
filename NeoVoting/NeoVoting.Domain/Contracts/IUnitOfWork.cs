namespace NeoVoting.Domain.Contracts
{
    // ==========================================================
    // IUnitOfWork: Unit of Work pattern interface
    //
    // Intent:
    // - Ensures all changes (across one or more repositories)
    //   are committed in a single atomic database transaction.
    // - Repositories should only stage (Add/Update/Delete) data.
    //   They do NOT call SaveChanges.
    // - The service or application layer calls SaveChangesAsync ONCE
    //   per business operation, controlling when all changes are persisted.
    //
    // Usage example:
    //   - Service handles a use case, calling repository methods.
    //   - Only after all changes are staged, service calls SaveChangesAsync.
    //   - This helps keep business logic and transaction scope clear/centralized.
    //
    // Disposal:
    // - Interface inherits IDisposable.
    // - When DI-registered as Scoped, disposal/cleanup is automatic after request.
    // ==========================================================
    public interface IUnitOfWork : IDisposable
    {
        // Commit all staged changes in this unit of work to the database.
        // Call this ONCE per use case/transaction—typically from service, not repository.
        //
        // Returns:
        // - The number of state entries written to the database.
        //   This number might reflect multiple objects affected by cascades etc.
        //   Do NOT write business logic that depends on this value equaling "1 per operation".
        // - For read-only operations: returns 0 (if no tracked changes).
        // - For success/failure: If no exception is thrown, the commit succeeded.
        //   If a DbUpdateException or DbUpdateConcurrencyException is thrown, the commit failed.
        // - The returned int is useful for diagnostics/logging, not workflow/business decisions.
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}