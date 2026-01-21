using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoVoting.Application.ResponseDTOs
{
    public class ElectionCurrentActiveStatistics_ResponseDTO
    {
        // Voter statistics
        public int? RegisteredVotersCount { get; set; }
        
        

        // Gender breakdown
        public int? RegisteredMalesCount { get; set; }
        public int? RegisteredFemalesCount { get; set; }
        
        public double? RegisteredMalePercentage { get; set; }
        public double? RegisteredFemalePercentage { get; set; }



        // Age group breakdown (18-29, 30-45, 46-64, 65+)
        public int? RegisteredAge18To29Count { get; set; }
        public int? RegisteredAge30To45Count { get; set; }
        public int? RegisteredAge46To64Count { get; set; }
        public int? RegisteredAge65AndOverCount { get; set; }
        
        public double? RegisteredAge18To29Percentage { get; set; }
        public double? RegisteredAge30To45Percentage { get; set; }
        public double? RegisteredAge46To64Percentage { get; set; }
        public double? RegisteredAge65AndOverPercentage { get; set; }


        // Candidates 
        public int? CandidatesNominatedCount { get; set; }
        
        

        // Election dates
        public DateTime? NominationStartDate { get; set; }
        public DateTime? NominationEndDate { get; set; }
        public DateTime? VotingStartDate { get; set; }
        public DateTime? VotingEndDate { get; set; }

        // Parliamentary term dates
        public DateTime? ParliamentaryTermStartDate { get; set; }
        public DateTime? ParliamentaryTermEndDate { get; set; }
    }
}
