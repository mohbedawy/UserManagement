using System;
using System.Collections.Generic;
using System.Linq;
using UserManagement.Domain.Entities;
using UserManagement.Infrastructure.Database;
using UserManagement.Infrastructure.IRepository;

namespace UserManagement.Infrastructure.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly UserManagementDbContext _context;

        public UserRepository(UserManagementDbContext context)
        {
            _context = context;
        }

        public IEnumerable<User> GetAll() => _context.Users.ToList();

        public User GetById(Guid id) => _context.Users.FirstOrDefault(u => u.Id == id);

        public void Add(User user)
        {
            user.Id = Guid.NewGuid();
            _context.Users.Add(user);
            _context.SaveChanges();
        }

        public void Update(User user)
        {
            var existing = _context.Users.FirstOrDefault(u => u.Id == user.Id);
            if (existing != null)
            {
                existing.UserName = user.UserName;
                existing.Email = user.Email;
                existing.FirstName = user.FirstName;
                existing.LastName = user.LastName;
                _context.SaveChanges();
            }
        }

        public void Delete(Guid id)
        {
            var user = _context.Users.FirstOrDefault(u => u.Id == id);
            if (user != null)
            {
                _context.Users.Remove(user);
                _context.SaveChanges();
            }
        }
    }
}