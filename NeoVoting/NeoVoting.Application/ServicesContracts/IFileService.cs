using Microsoft.AspNetCore.Http;
using NeoVoting.Domain.ResultErrorDomain;

namespace NeoVoting.Application.ServicesContracts
{
    public interface IFileService
    {
        Task<Result<string>> SaveFileAsync(IFormFile file, string[] allowedExtensions);

        void DeleteFile(string filePath);
    }
}