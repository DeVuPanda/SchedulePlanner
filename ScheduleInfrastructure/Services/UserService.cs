using ScheduleDomain.Model;
using System.Collections.Generic;
using System.Linq;

namespace ScheduleInfrastructure.Services
{
    public class UserService : IUserService
    {
        private readonly UniversityPracticeContext _context;

        public UserService(UniversityPracticeContext context)
        {
            _context = context;
        }

        public User? GetUserByEmail(string email)
        {
            return _context.Users.FirstOrDefault(u => u.Email == email);
        }

        public IEnumerable<User> GetAllTeachers()
        {
            return _context.Users.Where(u => u.Role.RoleName == "Teacher").ToList();
        }

        public void AddUser(User user)
        {
            _context.Users.Add(user);
            _context.SaveChanges();
        }

        public void DeleteUser(int id)
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
