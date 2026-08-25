using NeoVoting.Domain.Entities;
using NeoVoting.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoVoting.Domain.RepositoryContracts
{
    public interface ICandidateRepository
    {
        

        Task<bool> IsCandidateExistByVerificationHashAsync(string verificationHash);
        Task<Candidate?> GetByVerificationHashAsync(string verificationHash);


        void Add(Candidate candidate);
    }
}
