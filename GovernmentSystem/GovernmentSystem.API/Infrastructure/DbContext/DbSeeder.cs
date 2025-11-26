using Bogus;
using GovernmentSystem.API.Application.RequestDTOs;
using GovernmentSystem.API.Application.ServicesContracts;
using GovernmentSystem.API.Domain.Shared;

public class DbSeeder
{
    private readonly IVoterServices _voterServices;
    private readonly ICandidateServices _candidateServices;

    public DbSeeder(IVoterServices voterServices, ICandidateServices candidateServices)
    {
        _voterServices = voterServices;
        _candidateServices = candidateServices;
    }

    public async Task SeedAsync(int voters_count = 50,int candidates_count = 50)
    {
        Console.WriteLine($"--- Starting Seeding Process ({voters_count} voter records) ---");
        Console.WriteLine($"--- Starting Seeding Process ({candidates_count} candidate records) ---");

        await SeedVotersAsync(voters_count);
        await SeedCandidatesAsync(candidates_count);

        Console.WriteLine("--- Seeding Process Completed ---");
    }

    private async Task SeedVotersAsync(int count)
    {
        Console.WriteLine(">> Seeding Voters...");

        // 1. Configure Bogus to generate data that passes your Validators
        var voterFaker = new Faker<CreateVoterRequestDTO>()
            // Ensure Gender is M or F (char)
            .RuleFor(v => v.Gender, f => f.PickRandom('M', 'F'))

            // Generate Name based on the gender selected above
            .RuleFor(v => v.FirstName, (f, v) => f.Name.FirstName(v.Gender == 'M' ? Bogus.DataSets.Name.Gender.Male : Bogus.DataSets.Name.Gender.Female))
            .RuleFor(v => v.LastName, f => f.Name.LastName())

            // Pick a random Enum value for Governorate
            .RuleFor(v => v.GovernorateId, f => (GovernorateId)f.PickRandom(1, 2, 3, 4, 5))

            // DateOfBirth must be >= 18 years ago (Validator Logic)
            .RuleFor(v => v.DateOfBirth, f => DateOnly.FromDateTime(f.Date.Past(60, DateTime.Now.AddYears(-18))))

            .RuleFor(v => v.EligibleForElection, f => f.Random.Bool());

        // 2. Generate the DTOs
        var fakeVoters = voterFaker.Generate(count);
        int successCount = 0;

        // 3. Loop and call your Service
        foreach (var dto in fakeVoters)
        {
            try
            {
                var result = await _voterServices.AddVoterAsync(dto);

                if (result.IsSuccess)
                {
                    successCount++;
                    // Optional: Console.Write("."); // Progress bar effect
                }
                else
                {
                    // Log specific domain errors returned by your Result object
                    Console.WriteLine($"   [Error] Failed to add Voter {dto.FirstName}: {result.Error.Code} - {result.Error.Description}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"   [Exception] Critical failure adding voter: {ex.Message}");
            }
        }

        Console.WriteLine($"\n   Successfully added {successCount}/{count} Voters.");
    }

    private async Task SeedCandidatesAsync(int count)
    {
        Console.WriteLine(">> Seeding Candidates...");

        var candidateFaker = new Faker<CreateCandidateRequestDTO>()
            .RuleFor(c => c.Gender, f => f.PickRandom('M', 'F'))
            .RuleFor(c => c.FirstName, (f, c) => f.Name.FirstName(c.Gender == 'M' ? Bogus.DataSets.Name.Gender.Male : Bogus.DataSets.Name.Gender.Female))
            .RuleFor(c => c.LastName, f => f.Name.LastName())
            .RuleFor(c => c.GovernorateId, f => (GovernorateId)f.PickRandom(1, 2, 3, 4, 5))
            // Candidates usually older? Let's say 25+ just for variety, but 18+ is the requirement
            .RuleFor(c => c.DateOfBirth, f => DateOnly.FromDateTime(f.Date.Past(50, DateTime.Now.AddYears(-25))))
            /*.RuleFor(c => c.EligibleForElection, f => true);*/ // Candidates usually must be eligible
            .RuleFor(c => c.EligibleForElection, f => f.Random.Bool());

        var fakeCandidates = candidateFaker.Generate(count);
        int successCount = 0;

        foreach (var dto in fakeCandidates)
        {
            try
            {
                var result = await _candidateServices.AddCandidateAsync(dto);

                if (result.IsSuccess)
                {
                    successCount++;
                }
                else
                {
                    Console.WriteLine($"   [Error] Failed to add Candidate {dto.FirstName}: {result.Error.Code}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"   [Exception] Critical failure adding candidate: {ex.Message}");
            }
        }

        Console.WriteLine($"   Successfully added {successCount}/{count} Candidates.");
    }
}