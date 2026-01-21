using Microsoft.AspNetCore.Http;
using NeoVoting.Domain.ErrorHandling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoVoting.Application.ServicesContracts
{
    public interface IFileService
    {
        Task<Result<string>> SaveFileAsync(IFormFile file, string[] allowedExtensions);
        void DeleteFile(string filePath);
    }
}
