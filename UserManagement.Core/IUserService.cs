using System;
using System.Collections.Generic;
using UserManagement.Domain.Entities;
using UserManagement.Infrastructure.Database;

namespace UserManagement.Core
{
    public interface IUserService
    {
        IEnumerable<User> GetAll();
        User GetById(Guid id);
        void Create(User user);
        void Update(User user);
        void Delete(Guid id);
    }
}
