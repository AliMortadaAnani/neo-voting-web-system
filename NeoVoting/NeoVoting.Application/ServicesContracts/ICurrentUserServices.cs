using NeoVoting.Domain.Enums;
using System;
using System.Collections.Generic;

namespace NeoVoting.Application.ServicesContracts
{


    public interface ICurrentUserServices
    {
        int? ApplicationUserId { get; }
        int? AccountId { get; }
        string? UserName { get; }
        GovernorateIdEnum? Governorate { get; }
        bool IsAuthenticated { get; }

        // Optional helper to fetch raw claims if needed later
        string? GetClaim(string claimType);
    }
}