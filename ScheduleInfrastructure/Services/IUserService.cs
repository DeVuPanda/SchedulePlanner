using ScheduleDomain.Model;
using System.Collections.Generic;

namespace ScheduleInfrastructure.Services
{
    public interface IUserService
    {
        User? GetUserByEmail(string email);
        IEnumerable<User> GetAllTeachers();
        void AddUser(User user);
        void DeleteUser(int id);
    }
}
