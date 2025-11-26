using Bogus;
using GovernmentSystem.API.Application.RequestDTOs;
using GovernmentSystem.API.Application.ServicesContracts;
using GovernmentSystem.API.Domain.Shared;

namespace GovernmentSystem.API.Infrastructure.DbContext
{
    public class DbSeeder
    {
        private readonly IVoterServices _voterServices;
        private readonly ICandidateServices _candidateServices;

        public DbSeeder(IVoterServices voterServices, ICandidateServices candidateServices)
        {
            _voterServices = voterServices;
            _candidateServices = candidateServices;
        }

        public async Task SeedAsync(int voters_count = 50, int candidates_count = 50)
        {
            Console.WriteLine($"--- Starting Seeding Process ({voters_count} voters, {candidates_count} candidates) ---");

            await SeedVotersAsync(voters_count);
            await SeedCandidatesAsync(candidates_count);

            Console.WriteLine("--- Seeding Process Completed ---");
        }

        private async Task SeedVotersAsync(int count)
        {
            Console.WriteLine(">> Seeding Voters with Lebanese Names...");

            var voterFaker = new Faker<CreateVoterRequestDTO>()
                // 1. Pick Gender first
                .RuleFor(v => v.Gender, f => f.PickRandom('M', 'F'))

                // 2. Pick Lebanese First Name based on Gender
                .RuleFor(v => v.FirstName, (f, v) =>
                    v.Gender == 'M' ? f.PickRandom(LebaneseMaleNames) : f.PickRandom(LebaneseFemaleNames))

                // 3. Pick Lebanese Last Name
                .RuleFor(v => v.LastName, f => f.PickRandom(LebaneseLastNames))

                // 4. Other Rules
                .RuleFor(v => v.GovernorateId, f => (GovernorateId)f.PickRandom(1, 2, 3, 4, 5))
                .RuleFor(v => v.DateOfBirth, f => DateOnly.FromDateTime(f.Date.Past(80, DateTime.Now.AddYears(-18))))
                //.RuleFor(c => c.EligibleForElection, f => f.Random.Bool());
                .RuleFor(c => c.EligibleForElection, f => /*f.Random.Bool()=*/true);

            var fakeVoters = voterFaker.Generate(count);
            int successCount = 0;

            foreach (var dto in fakeVoters)
            {
                try
                {
                    var result = await _voterServices.AddVoterAsync(dto);
                    if (result.IsSuccess) successCount++;
                    else Console.WriteLine($"   [Error] Voter {dto.FirstName} {dto.LastName}: {result.Error.Code}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"   [Exception] {ex.Message}");
                }
            }

            Console.WriteLine($"   Successfully added {successCount}/{count} Voters.");
        }

        private async Task SeedCandidatesAsync(int count)
        {
            Console.WriteLine(">> Seeding Candidates with Lebanese Names...");

            var candidateFaker = new Faker<CreateCandidateRequestDTO>()
                .RuleFor(c => c.Gender, f => f.PickRandom('M', 'F'))
                .RuleFor(c => c.FirstName, (f, c) =>
                    c.Gender == 'M' ? f.PickRandom(LebaneseMaleNames) : f.PickRandom(LebaneseFemaleNames))
                .RuleFor(c => c.LastName, f => f.PickRandom(LebaneseLastNames))
                .RuleFor(c => c.GovernorateId, f => (GovernorateId)f.PickRandom(1, 2, 3, 4, 5))
                .RuleFor(c => c.DateOfBirth, f => DateOnly.FromDateTime(f.Date.Past(80, DateTime.Now.AddYears(-18))))
                //.RuleFor(c => c.EligibleForElection, f => f.Random.Bool());
                .RuleFor(c => c.EligibleForElection, f => /*f.Random.Bool()=*/true);
            var fakeCandidates = candidateFaker.Generate(count);
            int successCount = 0;

            foreach (var dto in fakeCandidates)
            {
                try
                {
                    var result = await _candidateServices.AddCandidateAsync(dto);
                    if (result.IsSuccess) successCount++;
                    else Console.WriteLine($"   [Error] Candidate {dto.FirstName} {dto.LastName}: {result.Error.Code}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"   [Exception] {ex.Message}");
                }
            }

            Console.WriteLine($"   Successfully added {successCount}/{count} Candidates.");
        }

        // ==========================================
        // LEBANESE DATASETS
        // ==========================================

        private static readonly string[] LebaneseMaleNames = new[]
 {
    // Existing + expanded genuine Lebanese/Levantine male names
    "Ali", "Charbel", "Mohammad", "Jad", "Elie", "Omar", "Georges", "Hussein",
    "Joseph", "Fadi", "Hassan", "Rabih", "Tony", "Wissam", "Ziad", "Karim",
    "Tarek", "Bassam", "Elias", "Ahmad", "Ibrahim", "Walid", "Michel", "Roy",
    "Samer", "Ghassan", "Nabil", "Toufic", "Bilal", "Hadi", "Rami", "Alain",
    "Marwan", "Jean", "Pierre", "Youssef", "Mahmoud", "Khaled", "Assaad", "Danny",
    "Mazen", "Chadi", "Firas", "Hicham", "Kamil", "Mounir", "Nadim", "Riad",
    "Samir", "Zaher", "Abdallah", "Adnan", "Boutros", "Fouad", "Gaby", "Haitham",
    "Imad", "Jawad", "Kassem", "Louay", "Malek", "Naji", "Osama", "Paul", "Raed",
    "Said", "Talal", "Wael", "Yahya", "Zakaria", "Abbas", "Hamza", "Mustafa",
    // Additions
    "Fouad", "Issam", "Munir", "Mounir", "Amine", "Ramzi", "Farid", "Dani",
    "Mounzer", "Salim", "Fadi", "Khalil", "Hicham", "Elie", "Roger", "Salah",
    "Adel", "Anis", "Amin", "Antoine", "Bashir", "Bahaa", "Bahij", "Bassel",
    "Edmond", "Firas", "Fares", "Gamal", "Ghazi", "Habib", "Jihad", "Karim",
    "Khodor", "Labib", "Majed", "Mamdouh", "Naseem", "Nazih", "Rafic", "Riad",
    "Samy", "Selim", "Sleiman", "Walid", "Yamen", "Zaher", "Zaki", "Shadi",
    "Marcel", "Asaad", "Wadih", "Rashed", "Chafic", "Fayez", "Rifat", "Jamil",
    "Najib", "Habib", "Fahd", "Raouf", "Elia", "Samih", "Shaker", "Ramzi", "Rony"
};

        private static readonly string[] LebaneseFemaleNames = new[]
        {
    // Existing + expanded genuine Lebanese/Levantine female names
    "Nour", "Maya", "Zeina", "Fatima", "Layla", "Maria", "Rita", "Sara", "Yara",
    "Rima", "Zainab", "Lynn", "Jana", "Nayla", "Dalal", "Manal", "Hiba", "Rana",
    "Mona", "Amani", "Samar", "Roula", "Pascale", "Sabine", "Dima", "Hanan",
    "Amal", "Joumana", "Aline", "Elissa", "Carla", "Dania", "Elsa", "Farah",
    "Ghada", "Hala", "Joelle", "Karen", "Lama", "Maha", "Nada", "Ola", "Pamela",
    "Rawan", "Soha", "Tala", "Vanessa", "Wafa", "Yasmina", "Zahra", "Aya",
    "Bane", "Celine", "Dana", "Esraa", "Fida", "Grace", "Huda", "Ingrid", "Jessica",
    "Khouloud", "Lara", "Mirna", "Nancy", "Rola", "Sally", "Tamara", "Rim",
    // Additions
    "Siham", "Samira", "May", "Salwa", "Widad", "Hind", "Ghada", "Imane", "Diala",
    "Haifa", "Majida", "Raghd", "Suzanne", "Christelle", "Najwa", "Michelle", "Rosy",
    "Loraine", "Angie", "Ruwayda", "Claudine", "Rosine", "Micheline", "Nada", "Hala",
    "Melissa", "Ruba", "Jihane", "Racha", "Rabab", "Raghad", "Nadine", "Thuraya",
    "Feryal", "Mona", "Jumana", "Yasmine", "Sherine", "Maguy", "Reine", "Zeina",
    "Loulwa", "Samya", "Nehad", "Souha", "Chantal", "Jehan", "Fatmeh", "Abir", "Sabah",
    "Miriam", "Yousra", "Micheline", "Lea", "Jumana"
};

        private static readonly string[] LebaneseLastNames = new[]
        {
    // Existing + expanded (removing less common ones and adding frequent Lebanese surnames)
    "Khoury", "Haddad", "Ghanem", "Najjar", "Karam", "Saab", "Khalil", "Assaf",
    "Aoun", "Nasr", "Hamdan", "Fakih", "Awad", "Moussa", "Saleh", "Farhat",
    "Antoun", "Saliba", "Raad", "Jaber", "Haidar", "Zein", "Chehab", "Frangieh",
    "Jumblatt", "Hariri", "Berri", "Geagea", "Gemayel", "Arslan", "Karami",
    "Mikati", "Salam", "Solh", "Edde", "Chamoun", "Lahoud", "Helou",
    "Sfeir", "Rahi", "Qassem", "Mughniyeh", "Hobeika", "Barakat", "Chahine",
    "Daou", "Eid", "Fares", "Gebran", "Habib", "Issa", "Jabbour", "Kanaan",
    "Latif", "Maalouf", "Nader", "Obeid", "Rahme", "Sarkis", "Tawk", "Yazbek",
    "Zakhour", "Abdallah", "Baalbaki", "Chidiac", "Daghir", "Elian", "Fakhoury",
    "Ghosn", "Hage", "Itani", "Jammal", "Kaakour", "Labaki", "Matar", "Nahas",
    // Additions
    "AbiNader", "Abillama", "AbouChakra", "AbouJaoude", "AbouRjeily", "Accad", "Adjami", "Akl", "Alam",
    "Alameddine", "Arbid", "Assi", "Atallah", "Azar", "Bahri", "Baghdadi", "Bazzi", "Baz",
    "Berro", "BouAntoun", "BouChalhoub", "BouEid", "BouKhalil", "BouMansour", "BouRached",
    "BouYounes", "Boutros", "Chemali", "Chebli", "Daouk", "Debs", "Dib", "Doueihy", "Eid",
    "Fadel", "Fahed", "Fayad", "Fayez", "Fawaz", "Faraj", "Habib", "Haddadin", "Hadid",
    "Haikal", "Hakim", "Hamati", "Haouch", "Hashem", "Hindi", "Idriss", "Issa", "Kassis",
    "Kattar", "Kfoury", "Khater", "Khazen", "Kheir", "Khodr", "Khourani", "Kiwan", "Kortbawi",
    "Labbad", "Mansour", "Matta", "Mezher", "Moawad", "Murr", "Najm", "Nassar", "Nehme",
    "Obeid", "Rached", "Sadek", "Safieddine", "Sleiman", "Soubra", "Tabet", "Tannous", "Touma",
    "Trad", "Wehbe", "Yammine", "Zein", "Zgheib"
};
    }
}