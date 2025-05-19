using System;
using System.Collections.Generic;
using UserManagement.Domain.Entities;
using UserManagement.Infrastructure.Database;
using UserManagement.Infrastructure.IRepository;

namespace UserManagement.Core
{

    public class UserService : IUserService
    {
        private readonly IUserRepository _repository;
        public UserService(IUserRepository repository)
        {
            _repository = repository;
        }
        public IEnumerable<User> GetAll() => _repository.GetAll();
        public User GetById(Guid id) => _repository.GetById(id);
        public void Create(User user) => _repository.Add(user);
        public void Update(User user) => _repository.Update(user);
        public void Delete(Guid id) => _repository.Delete(id);
       
    }
}
