using System;
using System.Collections.Generic;
using System.Text;

namespace APIUsers.Library.Interfaces
{
    public interface IUser : IDisposable
    {
        List<Models.User> GetUsers();
        int InsertUser(string nick, string password);

        Models.User GetUser(string nick);

        bool UpdateUser(Models.User user);

        bool UpdateRefreshTokenNExpiryTime(Models.UserMin user);
        bool UpdateRefreshToken(Models.UserMin user);
    }
}
