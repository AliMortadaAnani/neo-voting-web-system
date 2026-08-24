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

            //await SeedVotersAsync(nationalIds);

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
                .RuleFor(v => v.Governorate, f => (GovernorateIdEnum)f.PickRandom(1, 2, 3, 4, 5))
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

        private async Task SeedVotersAsync(List<string> citizenNationalIds)
        {
            Console.WriteLine(">> Seeding Voters...");

            var voterIds = citizenNationalIds;
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

            Console.WriteLine($"   Successfully added {successCount}/{citizenNationalIds.Count} Voters.");
        }

        private async Task SeedCandidatesAsync(List<string> citizenNationalIds)
        {
            Console.WriteLine(">> Seeding Candidates...");

            var candidateIds = citizenNationalIds;
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

            Console.WriteLine($"   Successfully added {successCount}/{citizenNationalIds.Count} Candidates.");
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


        List<string> nationalIdsC = new List<string>
{
    "NI-KT2M2166-J4HL",
    "NI-MY1F2189-PBCR",
    "NI-NB5F8173-0Z3P",
    "NI-CJ2F2230-E7YT",
    "NI-NB5F1456-CT9V",
    "NI-TB3M1170-QQMI",
    "NI-HN3M1292-QT12",
    "NI-HG4M2184-HC9B",
    "NI-GK3F3151-JWYQ",
    "NI-SF5F2840-AOTB",
    "NI-MG2F5199-5AKB",
    "NI-AH5F8177-W7Y2",
    "NI-DM1F9504-ACDR",
    "NI-SF4M3334-WP9V",
    "NI-TJ2M3191-N69Z",
    "NI-SM4F1104-O30W",
    "NI-RL5F1135-VDQ2",
    "NI-MA2M2553-7GMU",
    "NI-SA4F1550-GYFR",
    "NI-TM3M2958-9L0Z",
    "NI-LH4M2557-P8W6",
    "NI-MK1F1889-KCOC",
    "NI-DL2F2189-QHTK",
    "NI-RN5F3169-W9YH",
    "NI-TD2M2494-8EQQ",
    "NI-BK3M1147-KFYU",
    "NI-MS1M9372-528L",
    "NI-KA4M2741-9CY1",
    "NI-LZ3F1160-LIKI",
    "NI-WC4F1785-6IMA",
    "NI-KM2M2190-09SH",
    "NI-BA4M2143-V22W",
    "NI-RC3F7103-A7OP",
    "NI-RM3F3674-U7X6",
    "NI-NZ2F2157-ZUYJ",
    "NI-JK3F2751-21UR",
    "NI-RF5M1301-ZIYF",
    "NI-NF3F2668-YEP9",
    "NI-HM3M1701-S3AU",
    "NI-FH2M2791-W0K6",
    "NI-MA2M1591-AB7P",
    "NI-BG2M2269-OYZN",
    "NI-JE3M1770-OLHS",
    "NI-RH3F2394-GAVQ",
    "NI-CB5F5128-VMLD",
    "NI-AA2F1137-9SUN",
    "NI-DS3F4665-BZW7",
    "NI-RL4M6695-XMN4",
    "NI-SL3F1441-HZEN",
    "NI-FB1M1101-IKXH",
    "NI-NB4F2556-9OTW",
    "NI-IF5M1887-G541",
    "NI-IY5M1749-MG2M",
    "NI-EA2M3993-UJQD",
    "NI-ME3F2148-XKFX",
    "NI-TM4M7638-Y8GY",
    "NI-RA3F1647-G4FW",
    "NI-HA2M2147-872T",
    "NI-WH2F6462-1NEB",
    "NI-GS3F2842-LLN7",
    "NI-ZA2F2476-F7XK",
    "NI-CC5F1130-CHVP",
    "NI-NJ1F2490-W0TJ",
    "NI-JG2F2797-AYYD",
    "NI-AK3F2154-9W5H",
    "NI-JG3M1195-6V89",
    "NI-SH3F2394-4LIC",
    "NI-TT5F1155-5W8T",
    "NI-RS2F1187-4EVU",
    "NI-KC2F1161-RX92",
    "NI-WE2F1150-N7AS",
    "NI-CF3F1195-DZF7",
    "NI-SA2M1487-O15Z",
    "NI-SB2F5304-G348",
    "NI-MS5F1106-BMET",
    "NI-NH1F3430-21UY",
    "NI-SB5M8193-WPDL",
    "NI-IB3M1142-9CR8",
    "NI-JF1F1384-GT0W",
    "NI-KC5F2501-RKEP",
    "NI-JI4M8172-41A2",
    "NI-NS4M4183-MR3D",
    "NI-NE3M3149-2VHJ",
    "NI-SN3F3162-ZMNS",
    "NI-RZ4F1940-ZZT0",
    "NI-RT1F8752-34M7",
    "NI-TN3F9189-14AO",
    "NI-HS4M1157-LHM0",
    "NI-AM5M2704-FYSH",
    "NI-CB1M2461-3ETG",
    "NI-LK3F1666-895R",
    "NI-RZ5F1278-5WBB",
    "NI-SH1M1181-0C62",
    "NI-KJ5M2557-KIFB",
    "NI-HA4F2868-GK9K",
    "NI-ZS5F1473-DWIH",
    "NI-RS5F2129-S3W8",
    "NI-LN4F2458-8891",
    "NI-AA4F1175-GG8V",
    "NI-SA4M2371-WFZY"
};

        List<string> nationalIdsV = new List<string>
{
    "NI-RO2M9182-XK94",
    "NI-RA5F2140-YH9T",
    "NI-RS5M1771-QZQ2",
    "NI-MR2M2142-UHE9",
    "NI-SC4F1763-4LH1",
    "NI-AS5M2892-EE4O",
    "NI-FS5F1274-R5F3",
    "NI-MK2M8937-GRHS",
    "NI-LB3M3129-GBFN",
    "NI-FT4F2136-YN5V",
    "NI-MH5M2243-4RTZ",
    "NI-RA4F5186-61E6",
    "NI-SS5F3307-Q8X6",
    "NI-RK2M1293-UI33",
    "NI-NJ5F2364-I9W9",
    "NI-RY1F1794-TZV1",
    "NI-RM3M1970-A9L9",
    "NI-HS4F2963-YGTA",
    "NI-LI1F1197-1JNX",
    "NI-PD1F2153-D4PS",
    "NI-DA5F1846-6M23",
    "NI-SR5F4703-TNLL",
    "NI-QH2F2341-FDZP",
    "NI-FE3F2552-TTP4",
    "NI-TB5M2875-CBKS",
    "NI-SK5F2946-D8I9",
    "NI-RA4M2655-3TQA",
    "NI-ST2F1674-BBQ4",
    "NI-BB3F9755-XCW8",
    "NI-MS1F2277-WE81",
    "NI-AN5M9565-S6P0",
    "NI-ZM5M1196-Z8NK",
    "NI-RS5F2204-QVKR",
    "NI-EA2F1367-EO76",
    "NI-SS1F1445-67EF",
    "NI-EM3M1136-9BGE",
    "NI-HD3M2745-WWUH",
    "NI-RB1F7187-YWGA",
    "NI-NA2F2651-Q9YN",
    "NI-WH2M1255-SNWR",
    "NI-FR2M2194-AJLX",
    "NI-SA5F1479-R281",
    "NI-IS2F1176-YJQY",
    "NI-IA3F1241-URN8",
    "NI-DH4M1241-ZFUI",
    "NI-CH1F3107-VX18",
    "NI-RH3F7729-MF6U",
    "NI-CG2F2143-GOE3",
    "NI-SH2F2351-PX9P",
    "NI-RC3F8174-KAYJ",
    "NI-AD3M4175-YU4Y",
    "NI-ZS4M8892-GQGL",
    "NI-HG1M2541-ZRMI",
    "NI-CH3F2568-KOTF",
    "NI-IH2F2273-8AGQ",
    "NI-FF5M2171-UJB6",
    "NI-NN2F2129-7S7A",
    "NI-RA1F1183-XVBL",
    "NI-IH3F2150-NM4F",
    "NI-BS3M5168-RDET",
    "NI-YK4F1206-RX8Q",
    "NI-AF2M4164-FRW0",
    "NI-SK3M2159-RN8L",
    "NI-JS3F6954-UK5G",
    "NI-RA5F1167-ERY0",
    "NI-BD2F1696-U2F0",
    "NI-ML5M7774-EPQP",
    "NI-RA1M6164-E098",
    "NI-ML3F1668-VX8P",
    "NI-CM5F2929-AFR4",
    "NI-YF3M2364-Y21T",
    "NI-KK3M1133-T6E6",
    "NI-DS3M2198-LZ45",
    "NI-GH1M1653-3B7Q",
    "NI-AT1M7379-1HKQ",
    "NI-SR3M7651-VKQH",
    "NI-SS4M3144-9RM2",
    "NI-MH5F1797-ZTHB",
    "NI-WB3F4962-O0AL",
    "NI-RZ3M1737-V6JB",
    "NI-DK1F9103-EVSX",
    "NI-CL4F5192-30QD",
    "NI-TK1M1145-LRFN",
    "NI-SM4F2742-YSNT",
    "NI-AT5F2982-GKOD",
    "NI-TF5M1988-DBD1",
    "NI-TA1M1177-TJJY",
    "NI-MK4M2160-RM46",
    "NI-SK5F3101-EFI5",
    "NI-RT2F6508-DFOK",
    "NI-EZ2F4870-L05Y",
    "NI-CS3F2280-HCQF",
    "NI-SF4M6166-OL5K",
    "NI-DA3M2469-W7QV",
    "NI-NF1F5188-ARE8",
    "NI-MB4F1128-VPEE",
    "NI-HA4M2546-ZXF3",
    "NI-AJ2M1161-286Q",
    "NI-MB5M2962-M7XX",
    "NI-AA1M1403-8425"
};
    }
}