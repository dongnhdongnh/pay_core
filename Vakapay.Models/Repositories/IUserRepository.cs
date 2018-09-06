using System.Collections.Generic;
using Vakapay.Models.Entities;

namespace Vakapay.Models.Repositories
{
    public interface IUserRepository
    {
        User FindUserById(string Id);
        User FindUserByEmail(string Email);
        List<User> FindUserBySql(string SqlString);
        bool UpdateUser(User user);
        bool DeleteUser(string userId);
        bool CreateNewUser(User user);
    }
}