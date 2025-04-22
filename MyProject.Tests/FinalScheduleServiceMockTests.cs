using Moq;
using ScheduleDomain.Model;
using ScheduleInfrastructure.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace MyProject.Tests
{
    public class FinalScheduleServiceMockTests
    {
        [Fact]
        public void AddSchedule_CalledOnceWithCorrectSchedule()
        {
            var mock = new Mock<IFinalScheduleService>();
            var schedule = new FinalSchedule { SubjectId = 1, ClassroomId = 2 };

            mock.Object.AddSchedule(schedule);

            mock.Verify(s => s.AddSchedule(It.Is<FinalSchedule>(fs => fs.SubjectId == 1 && fs.ClassroomId == 2)), Times.Once);
        }

        [Fact]
        public void DeleteSchedule_CalledTwiceInOrder()
        {
            var mock = new Mock<IFinalScheduleService>();
            var sequence = new MockSequence();

            mock.InSequence(sequence).Setup(s => s.DeleteSchedule(3));
            mock.InSequence(sequence).Setup(s => s.DeleteSchedule(5));

            mock.Object.DeleteSchedule(3);
            mock.Object.DeleteSchedule(5);

            mock.Verify(s => s.DeleteSchedule(3), Times.Once);
            mock.Verify(s => s.DeleteSchedule(5), Times.Once);
        }

        [Fact]
        public void GetSchedulesByTeacher_ThrowsException_WhenInvalidId()
        {
            var mock = new Mock<IFinalScheduleService>();
            mock.Setup(s => s.GetSchedulesByTeacher(-1)).Throws<ArgumentException>();

            Assert.Throws<ArgumentException>(() => mock.Object.GetSchedulesByTeacher(-1));
        }

        [Fact]
        public void IsClassroomAvailable_ReturnsTrue_IfNoConflict()
        {
            var mock = new Mock<IFinalScheduleService>();
            mock.Setup(s => s.IsClassroomAvailable(1, 2, 3)).Returns(true);

            var result = mock.Object.IsClassroomAvailable(1, 2, 3);

            Assert.True(result);
        }

        [Fact]
        public void IsClassroomAvailable_ReturnsFalse_OnSecondCall()
        {
            var mock = new Mock<IFinalScheduleService>();
            mock.SetupSequence(s => s.IsClassroomAvailable(1, 1, 1))
                .Returns(true)
                .Returns(false);

            Assert.True(mock.Object.IsClassroomAvailable(1, 1, 1));
            Assert.False(mock.Object.IsClassroomAvailable(1, 1, 1));
        }

        [Fact]
        public void GetSchedulesByTeacher_ReturnsCorrectCollection()
        {
            var mock = new Mock<IFinalScheduleService>();
            mock.Setup(s => s.GetSchedulesByTeacher(It.Is<int>(id => id > 0)))
                .Returns((int id) => new List<FinalSchedule> {
                    new FinalSchedule { Id = 1, TeacherId = id },
                    new FinalSchedule { Id = 2, TeacherId = id }
                });

            var schedules = mock.Object.GetSchedulesByTeacher(10).ToList();

            Assert.Equal(2, schedules.Count);
            Assert.All(schedules, s => Assert.Equal(10, s.TeacherId));
        }
    }
}
