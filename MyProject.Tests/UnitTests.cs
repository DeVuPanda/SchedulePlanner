using Xunit;
using ScheduleDomain.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using ScheduleInfrastructure;
using System.Threading.Tasks;

namespace MyProject.Tests
{
    public class DatabaseFixture : IDisposable
    {
        public UniversityPracticeContext Context { get; private set; }

        public DatabaseFixture()
        {
            var options = new DbContextOptionsBuilder<UniversityPracticeContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            Context = new UniversityPracticeContext(options);

            SeedData();
        }

        public void Dispose()
        {
            Context.Dispose();
        }

        private void SeedData()
        {
            var adminRole = new UserRole { RoleName = "Administrator" };
            var teacherRole = new UserRole { RoleName = "Teacher" };
            Context.UserRoles.AddRange(adminRole, teacherRole);

            var days = new[]
            {
                new DaysOfWeek { DayName = "Monday" },
                new DaysOfWeek { DayName = "Tuesday" },
                new DaysOfWeek { DayName = "Wednesday" },
                new DaysOfWeek { DayName = "Thursday" },
                new DaysOfWeek { DayName = "Friday" }
            };
            Context.DaysOfWeeks.AddRange(days);

            var pairs = new[]
            {
                new PairNumber { Description = "First pair" },
                new PairNumber { Description = "Second pair" },
                new PairNumber { Description = "Third pair" },
                new PairNumber { Description = "Forth pair" }
            };
            Context.PairNumbers.AddRange(pairs);

            var maxPairs = new[]
            {
                new MaxPairsPerDay { MaxPairs = 1 },
                new MaxPairsPerDay { MaxPairs = 2 },
                new MaxPairsPerDay { MaxPairs = 3 },
                new MaxPairsPerDay { MaxPairs = 4 }
            };
            Context.MaxPairsPerDays.AddRange(maxPairs);

            var classrooms = new[]
            {
                new Classroom { RoomNumber = "101" },
                new Classroom { RoomNumber = "102" },
                new Classroom { RoomNumber = "103" },
                new Classroom { RoomNumber = "104" }
            };
            Context.Classrooms.AddRange(classrooms);

            var teacher1 = new User
            {
                FullName = "John Smith",
                Email = "john.smith@university.edu",
                Password = "hashedPassword123",
                RoleId = 2  
            };

            var teacher2 = new User
            {
                FullName = "Jane Doe",
                Email = "jane.doe@university.edu",
                Password = "hashedPassword456",
                RoleId = 2  
            };

            var admin = new User
            {
                FullName = "Admin User",
                Email = "admin@university.edu",
                Password = "adminPassword789",
                RoleId = 1  
            };

            Context.Users.AddRange(teacher1, teacher2, admin);

            var group1 = new Group
            {
                GroupName = "TTP-31",
                Course = "3",
                Speciality = "Computer Science"
            };

            var group2 = new Group
            {
                GroupName = "TTP-32",
                Course = "3",
                Speciality = "Computer Science"
            };

            Context.Groups.AddRange(group1, group2);

            Context.SaveChanges();

            teacher1.RoleId = teacherRole.Id;
            teacher2.RoleId = teacherRole.Id;
            admin.RoleId = adminRole.Id;
            Context.SaveChanges();

            var subject1 = new Subject
            {
                Name = "SystemProgramming",
                TeacherId = teacher1.Id
            };

            var subject2 = new Subject
            {
                Name = "Algorithmics",
                TeacherId = teacher2.Id
            };

            Context.Subjects.AddRange(subject1, subject2);

            Context.SaveChanges();
        }
    }

    public class UserTests : IClassFixture<DatabaseFixture>
    {
        private readonly DatabaseFixture _fixture;

        public UserTests(DatabaseFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        [Trait("Category", "Basic")]
        public void CanAddUser()
        {
            var teacherRole = _fixture.Context.UserRoles.First(r => r.RoleName == "Teacher");
            var newUser = new User
            {
                FullName = "New Teacher",
                Email = "new.teacher@university.edu",
                Password = "securePassword123",
                RoleId = teacherRole.Id
            };

            _fixture.Context.Users.Add(newUser);
            _fixture.Context.SaveChanges();

            var retrievedUser = _fixture.Context.Users.FirstOrDefault(u => u.Email == "new.teacher@university.edu");
            Assert.NotNull(retrievedUser);
            Assert.Equal("New Teacher", retrievedUser.FullName);
            Assert.Equal("securePassword123", retrievedUser.Password);
            Assert.Equal(teacherRole.Id, retrievedUser.RoleId);
        }

        [Fact]
        [Trait("Category", "Basic")]
        public void CanUpdateUser()
        {
            var user = _fixture.Context.Users.First(u => u.FullName.Contains("John"));

            user.FullName = "John Smith Jr.";
            _fixture.Context.SaveChanges();

            var updatedUser = _fixture.Context.Users.First(u => u.Id == user.Id);
            Assert.Equal("John Smith Jr.", updatedUser.FullName);
        }

        [Fact]
        [Trait("Category", "Basic")]
        public void CanDeleteUser()
        {
            var user = new User
            {
                FullName = "Temporary User",
                Email = "temp@university.edu",
                Password = "tempPass",
                RoleId = _fixture.Context.UserRoles.First().Id
            };

            _fixture.Context.Users.Add(user);
            _fixture.Context.SaveChanges();

            _fixture.Context.Users.Remove(user);
            _fixture.Context.SaveChanges();

            var deletedUser = _fixture.Context.Users.FirstOrDefault(u => u.Email == "temp@university.edu");
            Assert.Null(deletedUser);
        }

        [Theory]
        [InlineData("Short", "short@test.com", "pass", "Teacher")]
        [InlineData("Very Long Name That Should Be Valid", "long@test.com", "password", "Teacher")]
        [InlineData("Valid User", "valid@email.com", "password", "Teacher")]
        [Trait("Category", "Parameterized")]
        public void ValidateUserProperties(string fullName, string email, string password, string roleName)
        {
            var roleId = _fixture.Context.UserRoles.First(r => r.RoleName == roleName).Id;
            var user = new User
            {
                FullName = fullName,
                Email = email,
                Password = password,
                RoleId = roleId
            };

            _fixture.Context.Users.Add(user);
            _fixture.Context.SaveChanges();

            var savedUser = _fixture.Context.Users.First(u => u.Email == email);
            Assert.Equal(fullName, savedUser.FullName);
            Assert.Equal(password, savedUser.Password);
        }
    }

    public class SubjectTests : IClassFixture<DatabaseFixture>
    {
        private readonly DatabaseFixture _fixture;

        public SubjectTests(DatabaseFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        [Trait("Category", "Basic")]
        public void CanAddSubject()
        {
            var teacher = _fixture.Context.Users.First(u => u.FullName.Contains("Jane"));
            var subject = new Subject
            {
                Name = "Computer Science",
                TeacherId = teacher.Id
            };

            _fixture.Context.Subjects.Add(subject);
            _fixture.Context.SaveChanges();

            var savedSubject = _fixture.Context.Subjects.FirstOrDefault(s => s.Name == "Computer Science");
            Assert.NotNull(savedSubject);
            Assert.Equal(teacher.Id, savedSubject.TeacherId);
        }


    }

    public class ClassroomTests : IClassFixture<DatabaseFixture>
    {
        private readonly DatabaseFixture _fixture;

        public ClassroomTests(DatabaseFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        [Trait("Category", "Basic")]
        public void CanAddClassroom()
        {
            var classroom = new Classroom { RoomNumber = "201" };

            _fixture.Context.Classrooms.Add(classroom);
            _fixture.Context.SaveChanges();

            var savedClassroom = _fixture.Context.Classrooms.FirstOrDefault(c => c.RoomNumber == "201");
            Assert.NotNull(savedClassroom);
        }

    }

    public class GroupTests : IClassFixture<DatabaseFixture>
    {
        private readonly DatabaseFixture _fixture;

        public GroupTests(DatabaseFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        [Trait("Category", "Basic")]
        public void CanAddGroup()
        {
            var group = new Group
            {
                GroupName = "TTP-33",
                Course = "3",
                Speciality = "Computer Science"
            };

            _fixture.Context.Groups.Add(group);
            _fixture.Context.SaveChanges();

            var savedGroup = _fixture.Context.Groups.FirstOrDefault(g => g.GroupName == "TTP-33");
            Assert.NotNull(savedGroup);
            Assert.Equal("TTP-33", savedGroup.GroupName);
            Assert.Equal("3", savedGroup.Course);
            Assert.Equal("Computer Science", savedGroup.Speciality);
        }

        [Theory]
        [InlineData("CS-11", "1", "Computer Science")]
        [InlineData("IS-22", "2", "Information Systems")]
        [InlineData("EN-33", "3", "Engineering")]
        [Trait("Category", "Parameterized")]
        public void CanAddMultipleGroups(string groupName, string course, string speciality)
        {
            var group = new Group
            {
                GroupName = groupName,
                Course = course,
                Speciality = speciality
            };

            _fixture.Context.Groups.Add(group);
            _fixture.Context.SaveChanges();

            var savedGroup = _fixture.Context.Groups.FirstOrDefault(g => g.GroupName == groupName);
            Assert.NotNull(savedGroup);
            Assert.Equal(groupName, savedGroup.GroupName);
            Assert.Equal(course, savedGroup.Course);
            Assert.Equal(speciality, savedGroup.Speciality);
        }
    }

    public class SchedulePreferenceTests : IClassFixture<DatabaseFixture>
    {
        private readonly DatabaseFixture _fixture;

        public SchedulePreferenceTests(DatabaseFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        [Trait("Category", "Basic")]
        public void CanTeacherMakeSchedulePreference()
        {
            var teacher = _fixture.Context.Users.First(u => u.FullName.Contains("John"));
            var subject = _fixture.Context.Subjects.First(s => s.TeacherId == teacher.Id);
            var dayOfWeek = _fixture.Context.DaysOfWeeks.First(d => d.DayName == "Monday");
            var pairNumber = _fixture.Context.PairNumbers.First();
            var maxPairsPerDay = _fixture.Context.MaxPairsPerDays.First(m => m.MaxPairs == 2);

            var preference = new SchedulePreference
            {
                TeacherId = teacher.Id,
                SubjectId = subject.Id,
                DayOfWeekId = dayOfWeek.Id,
                PairNumberId = pairNumber.Id,
                MaxPairsPerDayId = maxPairsPerDay.Id,
                Priority = 1
            };

            _fixture.Context.SchedulePreferences.Add(preference);
            _fixture.Context.SaveChanges();

            var savedPreference = _fixture.Context.SchedulePreferences
                .FirstOrDefault(p => p.TeacherId == teacher.Id && p.SubjectId == subject.Id);

            Assert.NotNull(savedPreference);
            Assert.Equal(dayOfWeek.Id, savedPreference.DayOfWeekId);
            Assert.Equal(pairNumber.Id, savedPreference.PairNumberId);
            Assert.Equal(1, savedPreference.Priority);
        }
    }

    public class FinalScheduleTests : IClassFixture<DatabaseFixture>
    {
        private readonly DatabaseFixture _fixture;

        public FinalScheduleTests(DatabaseFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        [Trait("Category", "Basic")]
        public void CanAddThePairToFinalSchedule()
        {
            var teacher = _fixture.Context.Users.First(u => u.FullName.Contains("Jane"));
            var subject = _fixture.Context.Subjects.First(s => s.TeacherId == teacher.Id);
            var classroom = _fixture.Context.Classrooms.First();
            var dayOfWeek = _fixture.Context.DaysOfWeeks.First();
            var pairNumber = _fixture.Context.PairNumbers.First();

            var schedule = new FinalSchedule
            {
                TeacherId = teacher.Id,
                SubjectId = subject.Id,
                ClassroomId = classroom.Id,
                DayOfWeekId = dayOfWeek.Id,
                PairNumberId = pairNumber.Id,
                IsClassroomAssigned = true
            };

            _fixture.Context.FinalSchedules.Add(schedule);
            _fixture.Context.SaveChanges();

            var savedSchedule = _fixture.Context.FinalSchedules
                .FirstOrDefault(s => s.TeacherId == teacher.Id && s.SubjectId == subject.Id);

            Assert.NotNull(savedSchedule);
            Assert.Equal(classroom.Id, savedSchedule.ClassroomId);
            Assert.Equal(dayOfWeek.Id, savedSchedule.DayOfWeekId);
            Assert.Equal(pairNumber.Id, savedSchedule.PairNumberId);
            Assert.True(savedSchedule.IsClassroomAssigned);
        }
    }

    public class CollectionTests : IClassFixture<DatabaseFixture>
    {
        private readonly DatabaseFixture _fixture;

        public CollectionTests(DatabaseFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        [Trait("Category", "Collection")]
        public void AllDaysOfWeekArePresent()
        {
            var expectedDays = new[] { "Monday", "Tuesday", "Wednesday", "Thursday", "Friday" };

            var actualDays = _fixture.Context.DaysOfWeeks.Select(d => d.DayName).ToList();

            Assert.Equal(expectedDays.Length, actualDays.Count);
            Assert.All(expectedDays, day => Assert.Contains(day, actualDays));
        }

        [Fact]
        [Trait("Category", "Collection")]
        public void AllPairsHaveDescriptions()
        {
            var pairs = _fixture.Context.PairNumbers.ToList();

            Assert.NotEmpty(pairs);
            Assert.All(pairs, pair => Assert.False(string.IsNullOrEmpty(pair.Description)));
        }

        [Fact]
        [Trait("Category", "Collection")]
        public void AllGroupsHaveRequiredProperties()
        {
            var groups = _fixture.Context.Groups.ToList();

            Assert.NotEmpty(groups);
            Assert.All(groups, group => {
                Assert.False(string.IsNullOrEmpty(group.GroupName));
                Assert.False(string.IsNullOrEmpty(group.Course));
                Assert.False(string.IsNullOrEmpty(group.Speciality));
            });
        }
    }

    public class StringTests : IClassFixture<DatabaseFixture>
    {
        private readonly DatabaseFixture _fixture;

        public StringTests(DatabaseFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        [Trait("Category", "String")]
        public void UserFullNameDoesNotContainNumbers()
        {
            var users = _fixture.Context.Users.ToList();

            Assert.All(users, user =>
                Assert.DoesNotMatch(@"\d", user.FullName));
        }

        [Fact]
        [Trait("Category", "String")]
        public void EmailsHaveValidFormat()
        {
            var users = _fixture.Context.Users.ToList();

            Assert.All(users, user =>
                Assert.Matches(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", user.Email));
        }

        [Fact]
        [Trait("Category", "String")]
        public void GroupNamesFollowNamingConvention()
        {
            var groups = _fixture.Context.Groups.ToList();

            Assert.All(groups, group =>
                Assert.Matches(@"^[A-Z]+-\d+$", group.GroupName));
        }
    }

    public class ExceptionTests : IClassFixture<DatabaseFixture>
    {
        private readonly DatabaseFixture _fixture;

        public ExceptionTests(DatabaseFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        [Trait("Category", "Exception")]
        public void ThrowsExceptionWhenAccessingNonExistentEntity()
        {
            Assert.Throws<InvalidOperationException>(() =>
            {
                _fixture.Context.Users.First(u => u.Email == "nonexistent@example.com");
            });
        }
    }
}