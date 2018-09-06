using System;
using System.Collections.Generic;
using Vakapay.Models.Entities;

namespace Vakapay.Models.Repositories
{
    public interface IApiKeyRepository
    {
        ApiKey FindApiKeyById(string Id);
        List<ApiKey> FindApiKeyByUser(string UserId);
        bool CreateNewApiKey(ApiKey apiKey);
        bool DeleteApiKey(string Id);
        bool UpdateApiKey(string Id);

    }
}
