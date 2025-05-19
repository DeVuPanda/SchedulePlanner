using Moq;
using ScheduleDomain.Model;
using ScheduleInfrastructure.Services;
using System;
using System.Collections.Generic;
using Xunit;

namespace MyProject.Tests
{
    public class ScheduleManagerMockTests
    {
        [Fact]
        public void AddScheduleIfAvailable_ClassroomIsAvailable_AddsSchedule()
        {
            var mockService = new Mock<IFinalScheduleService>();
            var schedule = new FinalSchedule { ClassroomId = 1, DayOfWeekId = 2, PairNumberId = 3 };

            mockService.Setup(s => s.IsClassroomAvailable(1, 2, 3)).Returns(true);

            var manager = new ScheduleManager(mockService.Object);
            manager.AddScheduleIfAvailable(schedule);

            mockService.Verify(s => s.AddSchedule(schedule), Times.Once);
        }

        [Fact]
        public void AddScheduleIfAvailable_ClassroomNotAvailable_ThrowsException()
        {
            var mockService = new Mock<IFinalScheduleService>();
            var schedule = new FinalSchedule { ClassroomId = 1, DayOfWeekId = 2, PairNumberId = 3 };

            mockService.Setup(s => s.IsClassroomAvailable(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>())).Returns(false);

            var manager = new ScheduleManager(mockService.Object);

            Assert.Throws<InvalidOperationException>(() => manager.AddScheduleIfAvailable(schedule));
            mockService.Verify(s => s.AddSchedule(It.IsAny<FinalSchedule>()), Times.Never);
        }

        [Fact]
        public void RemoveSchedulesInOrder_CallsDeleteInCorrectSequence()
        {
            var mockService = new Mock<IFinalScheduleService>();
            var manager = new ScheduleManager(mockService.Object);

            var sequence = new MockSequence();
            mockService.InSequence(sequence).Setup(s => s.DeleteSchedule(1));
            mockService.InSequence(sequence).Setup(s => s.DeleteSchedule(2));

            manager.RemoveSchedulesInOrder(new List<int> { 1, 2 });

            mockService.Verify(s => s.DeleteSchedule(1), Times.Once);
            mockService.Verify(s => s.DeleteSchedule(2), Times.Once);
        }

        [Fact]
        public void TryAddSchedule_WhenAddThrowsException_ReturnsFalse()
        {
            var mockService = new Mock<IFinalScheduleService>();
            mockService.Setup(s => s.AddSchedule(It.IsAny<FinalSchedule>())).Throws<Exception>();

            var manager = new ScheduleManager(mockService.Object);
            var result = manager.TryAddSchedule(new FinalSchedule());

            Assert.False(result);
        }

        [Fact]
        public void TryAddSchedule_WhenAddSucceeds_ReturnsTrue()
        {
            var mockService = new Mock<IFinalScheduleService>();
            var manager = new ScheduleManager(mockService.Object);

            var result = manager.TryAddSchedule(new FinalSchedule());

            Assert.True(result);
            mockService.Verify(s => s.AddSchedule(It.IsAny<FinalSchedule>()), Times.Once);
        }

        [Fact]
        public void IsClassroomAvailable_ReturnsTrueThenFalse()
        {
            var mockService = new Mock<IFinalScheduleService>();
            var schedule = new FinalSchedule { ClassroomId = 1, DayOfWeekId = 2, PairNumberId = 3 };

            mockService.SetupSequence(s => s.IsClassroomAvailable(1, 2, 3))
                .Returns(true)
                .Returns(false);

            var manager = new ScheduleManager(mockService.Object);

            manager.AddScheduleIfAvailable(schedule);

            Assert.Throws<InvalidOperationException>(() => manager.AddScheduleIfAvailable(schedule));

            mockService.Verify(s => s.AddSchedule(It.IsAny<FinalSchedule>()), Times.Once); 
        }

    }
}
