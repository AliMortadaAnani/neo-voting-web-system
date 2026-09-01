using Microsoft.AspNetCore.Http;
using NeoVoting.Application.RequestDTOs.CandidateDTOs;
using NeoVoting.Domain.ResultErrorDomain;

namespace NeoVoting.Application.ServicesContracts
{
    public interface IFileServices
    {
        Task<Result<string>> SaveFileAsync(CandidateProfileUploadImage_RequestDTO dto);

        bool DeleteFile(string filePath);
    }
}