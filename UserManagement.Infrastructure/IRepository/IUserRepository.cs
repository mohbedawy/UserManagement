using System;
using System.Collections.Generic;
using UserManagement.Domain.Entities;

namespace UserManagement.Infrastructure.IRepository
{
    public interface IUserRepository
    {
        IEnumerable<User> GetAll();
        User GetById(Guid id);
        void Add(User user);
        void Update(User user);
        void Delete(Guid id);
    }
}
