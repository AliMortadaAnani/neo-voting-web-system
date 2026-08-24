using Bogus;
using GovernmentSystem.API.Application.RequestDTOs.CandidateDTOs;
using GovernmentSystem.API.Application.RequestDTOs.CitizenDTOs;
using GovernmentSystem.API.Application.RequestDTOs.VoterDTOs;
using GovernmentSystem.API.Application.ServicesContracts;
using GovernmentSystem.API.Domain.Enums;

namespace GovernmentSystem.API.Application.CLI
{
    public class DataSeederCLI
    {
        private readonly IVoterServices _voterServices;

        private readonly ICandidateServices _candidateServices;

        private readonly ICitizenServices _citizenServices;

        public DataSeederCLI(IVoterServices voterServices, ICandidateServices candidateServices, ICitizenServices citizenServices)
        {
            _voterServices = voterServices;
            _candidateServices = candidateServices;
            _citizenServices = citizenServices;
        }

        public async Task SeedAsync()
        {
            Console.WriteLine($"--- Starting Seeding Process ---");

            
            await SeedCitizensAsync(200);
            
            //await SeedVotersAsync(new List<string>(), 100);
            
            //await SeedCandidatesAsync(new List<string>(), 50);

            Console.WriteLine("--- Seeding Process Completed ---");
        }

        private async Task SeedCitizensAsync(int count)
        {
            Console.WriteLine(">> Seeding Citizens with Lebanese Names...");

            var citizenFaker = new Faker<CreateCitizenRequestDTO>()
                // 1. Pick Gender first
                .RuleFor(v => v.Gender, f => f.PickRandom('M', 'F'))

                // 2. Pick Lebanese First Name based on Gender
                .RuleFor(v => v.FirstName, (f, v) =>
                    v.Gender == 'M' ? f.PickRandom(LebaneseMaleNames) : f.PickRandom(LebaneseFemaleNames))

                // 3. Pick Lebanese Last Name
                .RuleFor(v => v.LastName, f => f.PickRandom(LebaneseLastNames))

                // 4. Other Rules
                .RuleFor(v => v.GovernorateId, f => (GovernorateIdEnum)f.PickRandom(1, 2, 3, 4, 5))
                .RuleFor(v => v.DateOfBirth, f => DateOnly.FromDateTime(f.Date.Past(80, DateTime.Now.AddYears(-18))));
               

            var fakeCitizens = citizenFaker.Generate(count);
            int successCount = 0;

            Console.WriteLine(">> Seeding Citizens with Lebanese Names...");

        

            foreach (var dto in fakeCitizens)
            {
                try
                {
                    var result = await _citizenServices.AddCitizenAsync(dto);
                    if (result.IsSuccess) successCount++;
                    else Console.WriteLine($"   [Error] Citizen {dto.FirstName} {dto.LastName}: {result.Error.Code}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"   [Exception] {ex.Message}");
                }
            }

            Console.WriteLine($"   Successfully added {successCount}/{count} Citizens.");
        }

        private async Task SeedVotersAsync(List<string> citizenNationalIds, int count)
        {
            Console.WriteLine(">> Seeding Voters...");

            var voterIds = citizenNationalIds.Take(count).ToList();
            int successCount = 0;

            foreach (var nationalId in voterIds)
            {
                try
                {
                    CreateVoterRequestDTO voterDto = new CreateVoterRequestDTO
                    {
                        NationalId = nationalId,
                        // You can add other properties if needed
                    };
                    var result = await _voterServices.AddVoterAsync(voterDto);
                    if (result.IsSuccess) successCount++;
                    else Console.WriteLine($"   [Error] Voter {nationalId}: {result.Error.Code}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"   [Exception] {ex.Message}");
                }
            }

            Console.WriteLine($"   Successfully added {successCount}/{count} Voters.");
        }

        private async Task SeedCandidatesAsync(List<string> citizenNationalIds, int count)
        {
            Console.WriteLine(">> Seeding Candidates...");

            var candidateIds = citizenNationalIds.Skip(count).Take(count).ToList();
            int successCount = 0;

            foreach (var nationalId in candidateIds)
            {
                try
                {
                    CreateCandidateRequestDTO candidateDto = new CreateCandidateRequestDTO
                    {
                        NationalId = nationalId,
                        // You can add other properties if needed
                    };
                    var result = await _candidateServices.AddCandidateAsync(candidateDto);
                    if (result.IsSuccess) successCount++;
                    else Console.WriteLine($"   [Error] Candidate {nationalId}: {result.Error.Code}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"   [Exception] {ex.Message}");
                }
            }

            Console.WriteLine($"   Successfully added {successCount}/{count} Candidates.");
        }




        private static readonly string[] LebaneseMaleNames = new[]
{
    // ─── Existing + massively expanded genuine Lebanese male names ───
    "Abbas", "Abdallah", "Abdo", "Abed", "Adel", "Adib", "Adnan",
    "Afif", "Ahmad", "Akram", "Alaa", "Alain", "Ali", "Amer",
    "Amin", "Amine", "Amjad", "Anis", "Antoine", "Anwar", "Aref",
    "Asaad", "Assaad", "Ayman", "Azmi", "Badr", "Bahaa", "Bahij",
    "Bahjat", "Baraa", "Bashir", "Bassam", "Bassel", "Bechara",
    "Bilal", "Bishara", "Boulos", "Boutros", "Burhan", "Chadi",
    "Chafic", "Charbel", "Dani", "Danny", "Daoud", "Darwish",
    "Edmond", "Elia", "Elias", "Elie", "Fadel", "Fadi", "Fahd",
    "Faraj", "Fares", "Farid", "Farouk", "Fawaz", "Fawzi",
    "Fayez", "Faysal", "Firas", "Fouad", "Gaby", "Gamal",
    "Georges", "Ghaleb", "Ghanem", "Ghassan", "Ghattas", "Ghazi",
    "Habib", "Hadi", "Haitham", "Hakam", "Hakim", "Hamza",
    "Hani", "Harb", "Haroun", "Hashem", "Hassan", "Hatim",
    "Haydar", "Hicham", "Hilal", "Husam", "Hussein", "Ibrahim",
    "Ihab", "Imad", "Issam", "Iyad", "Jaber", "Jad", "Jalal",
    "Jamal", "Jamil", "Jawad", "Jean", "Jihad", "Joseph",
    "Kamal", "Kamil", "Karim", "Kassem", "Khaled", "Khalil",
    "Khodor", "Kifah", "Labib", "Louay", "Mahdi", "Maher",
    "Mahmoud", "Majd", "Majdi", "Majed", "Malek", "Malik",
    "Mamdouh", "Mansour", "Marcel", "Maroun", "Marwan", "Mazen",
    "Maan", "Mehdi", "Michel", "Mohammad", "Mounir", "Mounzer",
    "Moussa", "Muhannad", "Munir", "Murad", "Musa", "Mustafa",
    "Nabil", "Nader", "Nadim", "Naeem", "Najati", "Naji",
    "Najib", "Naseem", "Nasser", "Nazih", "Nuhad", "Nuri",
    "Omar", "Osama", "Paul", "Pierre", "Qasim", "Rabih",
    "Rachid", "Raed", "Rafic", "Ramez", "Rami", "Ramzi",
    "Raouf", "Rashad", "Rashed", "Rasheed", "Raymond", "Reda",
    "Refik", "Riad", "Rifat", "Riyad", "Roger", "Rony",
    "Roy", "Saad", "Saadallah", "Saeed", "Saeb", "Safi",
    "Said", "Salah", "Salih", "Salim", "Samer", "Samih",
    "Samir", "Sami", "Samy", "Selim", "Shadi", "Shafic",
    "Shaker", "Shakir", "Sharif", "Shawki", "Shibli", "Sleiman",
    "Suleiman", "Talal", "Tamer", "Tamim", "Tarek", "Tawfik",
    "Tony", "Toufic", "Wadih", "Wael", "Wahib", "Wajdi",
    "Walid", "Wissam", "Yahya", "Yamen", "Yassin", "Younes",
    "Youssef", "Yusuf", "Zaher", "Zakaria", "Zaki", "Zayn",
    "Ziad"
};

        private static readonly string[] LebaneseFemaleNames = new[]
        {
    // ─── Existing + massively expanded genuine Lebanese female names ───
    "Abir", "Aline", "Amal", "Amani", "Amira", "Angie", "Asma",
    "Aya", "Badia", "Bane", "Basma", "Bushra", "Carla", "Celine",
    "Chantal", "Christelle", "Christine", "Claudette", "Claudine",
    "Dalal", "Dalia", "Dana", "Dania", "Diala", "Dima", "Dina",
    "Dunia", "Elissa", "Elsa", "Eman", "Esraa", "Farah", "Fatima",
    "Fatmeh", "Fawzia", "Feryal", "Fida", "Gabrielle", "Ghada",
    "Ghenwa", "Gisele", "Grace", "Gulnar", "Haifa", "Hala", "Hana",
    "Hanan", "Hania", "Hayat", "Hiba", "Hind", "Hiyam", "Huda",
    "Ibtisam", "Iman", "Imane", "Inaya", "Ingrid", "Jala", "Jana",
    "Jamila", "Jehan", "Jessica", "Jihane", "Joelle", "Josiane",
    "Joumana", "Joyce", "Jumana", "Karen", "Karima", "Khouloud",
    "Lama", "Lamia", "Lara", "Latifa", "Layla", "Lea", "Liliane",
    "Lina", "Linda", "Loraine", "Loulwa", "Lourdes", "Lujain",
    "Lynn", "Maguy", "Maha", "Majida", "Manal", "Manar", "Maria",
    "May", "Maya", "Maysa", "Maysun", "Melissa", "Micheline",
    "Michelle", "Mira", "Miriam", "Mirna", "Mona", "Nada", "Nadia",
    "Nadine", "Nahed", "Naila", "Najwa", "Najat", "Nancy", "Nayla",
    "Nazira", "Nehad", "Nicole", "Nisrine", "Nour", "Nuhad", "Ola",
    "Odette", "Pamela", "Pascale", "Qamar", "Rabab", "Racha", "Raghad",
    "Raghd", "Rana", "Rania", "Rasha", "Rawan", "Raya", "Rayan",
    "Reem", "Reine", "Renée", "Rihab", "Rim", "Rima", "Rita",
    "Rola", "Rosine", "Rosy", "Rouba", "Roula", "Ruba", "Ruwayda",
    "Saba", "Sabah", "Sabine", "Sahar", "Salma", "Sally", "Salwa",
    "Samar", "Samira", "Samya", "Sana", "Sara", "Selma", "Shadia",
    "Shatha", "Sherine", "Siham", "Soha", "Souad", "Souha", "Suha",
    "Suhair", "Suzanne", "Taghrid", "Tala", "Tamara", "Therese",
    "Thuraya", "Vanessa", "Vivian", "Wafa", "Wafaa", "Walaa",
    "Warda", "Wiam", "Widad", "Yara", "Yasmina", "Yasmine",
    "Yousra", "Zahra", "Zaina", "Zainab", "Zakia", "Zeina"
};

        private static readonly string[] LebaneseLastNames = new[]
        {
    // ─── Existing + massively expanded genuine Lebanese surnames ───
    "Abdallah", "Abi-Aad", "Abi-Haydar", "Abi-Saab", "Abillama",
    "AbiNader", "Abou-Assi", "AbouChakra", "Abou-Hamad", "Abou-Harb",
    "Abou-Hassan", "AbouJaoude", "Abou-Nader", "Abou-Rahme",
    "AbouRjeily", "Abou-Said", "Abou-Shakra", "Accad", "Achkar",
    "Adjami", "Akl", "Alam", "Alameddine", "Antoun", "Aouad",
    "Aoun", "Arbid", "Arslan", "Assaf", "Assi", "Atallah",
    "Awad", "Ayash", "Azar", "Azrak", "Baalbaki", "Baghdadi",
    "Bahri", "Ballout", "Barakat", "Baroud", "Basha", "Baydoun",
    "Baz", "Bazzi", "Berri", "Berro", "Bitar", "BouAntoun",
    "BouChalhoub", "BouEid", "Bou-Farhat", "Bou-Habib", "Bou-Halim",
    "BouKhalil", "BouMansour", "Bou-Nasr", "BouRached", "BouYounes",
    "Boutros", "Chaaban", "Chahine", "Chahwan", "Chammas", "Chamoun",
    "Charaf", "Chehab", "Chemali", "Chebli", "Chidiac", "Daghir",
    "Daher", "Dandan", "Daou", "Daouk", "Darwish", "Debs", "Diab",
    "Dib", "Domit", "Douaihy", "Doueihy", "Edde", "Eid", "El-Hachem",
    "El-Hajj", "Elian", "Fadel", "Fahed", "Fakhoury", "Fakih",
    "Farah", "Faraj", "Fares", "Farhat", "Farran", "Fawaz", "Fayad",
    "Fayed", "Fayez", "Fayyad", "Fleihan", "Frangieh", "Gargour",
    "Geagea", "Gebran", "Gedeon", "Gemayel", "Ghanem", "Ghosn",
    "Habib", "Hachem", "Haddad", "Haddadin", "Hadid", "Hage",
    "Haidar", "Haikal", "Hajj", "Hakim", "Hamati", "Hamdan",
    "Haouch", "Harb", "Hariri", "Hashem", "Hasrouni", "Hawi",
    "Hayek", "Helou", "Hindi", "Hitti", "Hneiny", "Hobeika",
    "Ibrahim", "Idriss", "Idris", "Issa", "Itani", "Jabbour",
    "Jaber", "Jafet", "Jammal", "Jbeily", "Jumblatt", "Kaakour",
    "Kanaan", "Karami", "Karam", "Kassis", "Kattar", "Kfoury",
    "Khairallah", "Khalil", "Khater", "Khattar", "Khazen", "Kheir",
    "Kheireddine", "Khodr", "Khourani", "Khoury", "Kiwan", "Kmeid",
    "Kojok", "Kortbawi", "Labaki", "Labbad", "Lahoud", "Lakkis",
    "Latif", "Lichaa", "Maalouf", "Maatouk", "Makarem", "Makhlouf",
    "Makki", "Mansour", "Maroun", "Matar", "Matta", "Melki",
    "Metni", "Mezher", "Mikati", "Moawad", "Mokbel", "Mougharbel",
    "Moussa", "Mrad", "Mughniyeh", "Murr", "Nader", "Nahas",
    "Nahra", "Najjar", "Najm", "Nammour", "Nasr", "Nassar",
    "Nassif", "Nehme", "Nohra", "Obeid", "Qassem", "Raad",
    "Rached", "Rahbani", "Rahi", "Rahme", "Rizk", "Saab",
    "Saad", "Saadeh", "Saade", "Saba", "Sabbagh", "Sadek",
    "Safieddine", "Sakr", "Salam", "Salameh", "Saleh", "Saliba",
    "Salloum", "Salman", "Sarkis", "Sawaya", "Sayegh", "Sfeir",
    "Shadid", "Shamieh", "Shamoun", "Shehadeh", "Sidani", "Sleiman",
    "Slim", "Solh", "Soubra", "Tabet", "Tahan", "Tannous",
    "Tarabay", "Tawk", "Tayah", "Tohme", "Touma", "Traboulsi",
    "Trad", "Turk", "Usta", "Wakim", "Wehbe", "Yammine",
    "Yazbek", "Yazigi", "Younes", "Zaarour", "Zaatari", "Zain",
    "Zaitoun", "Zakhour", "Zein", "Zgheib"
};
    }
}