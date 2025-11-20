using WhoIsHome.Entities;
using WhoIsHome.Shared.Types;
// ReSharper disable LocalVariableHidesMember

namespace WhoIsHome.Test.Application.Entities;

[TestFixture]
public class EventGroupTest
{
    private const string Title = "Title";
    private const int UserId = 0;
    private const PresenceType PresenceType = WhoIsHome.Shared.Types.PresenceType.Unknown;
    private readonly DateOnly startDate = new DateOnly(2024, 10, 10);
    private readonly DateOnly endDate = new DateOnly(2024, 10, 31);
    private readonly TimeOnly startTime = new TimeOnly(13, 30, 0);
    private readonly TimeOnly endTime = new TimeOnly(14, 0, 0);
    private const WeekDay WeekDays = WeekDay.Monday | WeekDay.Wednesday;

    [TestFixture]
    private class HasRepetitions : EventGroupTest
    {
        [Test]
        public void ReturnsFalse_WhenStartAndEndDateIsEqual()
        {
            // Arrange
            var eventGroup = new EventGroup(Title, startDate, startDate, WeekDays, startTime, endTime, PresenceType, null, UserId);
            
            // Act
            var result = eventGroup.HasRepetitions;
            
            // Assert
            result.Should().BeFalse();
        }
        
        [Test]
        public void ReturnsFalse_WhenStartAndEndDateIsNotEqual()
        {
            // Arrange
            var eventGroup = new EventGroup(Title, startDate, endDate, WeekDays, startTime, endTime, PresenceType, null, UserId);
            
            // Act
            var result = eventGroup.HasRepetitions;
            
            // Assert
            result.Should().BeTrue();
        }
    }
    
    [TestFixture]
    private class Validate : EventGroupTest
    {
        [Test]
        public void ReturnsNoValidationErrors()
        {
            // Arrange
            var eventGroup = new EventGroup(Title, startDate, endDate, WeekDays, startTime, endTime, PresenceType, null, UserId);
            
            // Act
            var result = eventGroup.Validate();
            
            // Assert
            result.Should().HaveCount(0);
        }
        
        [Test]
        public void ReturnsValidationError_WhenTitleIsTooLong()
        {
            // Arrange
            var title = string.Join("", Enumerable.Repeat("a", 51));
            var eventGroup = new EventGroup(title, startDate, endDate, WeekDays, startTime, endTime, PresenceType, null, UserId);
            
            // Act
            var result = eventGroup.Validate();
            
            // Assert
            result.Should().HaveCount(1);
        }
        
        [Test]
        public void ReturnsValidationError_WhenStartTimeIsBeforeEndTime()
        {
            // Arrange
            var startTime = new TimeOnly(13, 0, 0);
            var endTime = new TimeOnly(12, 0, 0);
            var eventGroup = new EventGroup(Title, startDate, endDate, WeekDays, startTime, endTime, PresenceType, null, UserId);
            
            // Act
            var result = eventGroup.Validate();
            
            // Assert
            result.Should().HaveCount(1);
        }
        
        [Test]
        public void ReturnsValidationError_WhenEndTimeIsBeforeDinnerTime()
        {
            // Arrange
            var time = endTime.AddHours(-1);
            var eventGroup = new EventGroup(Title, startDate, endDate, WeekDays, startTime, endTime, PresenceType.Late, time, UserId);
            
            // Act
            var result = eventGroup.Validate();
            
            // Assert
            result.Should().HaveCount(1);
        }
        
        [Test]
        public void ReturnsValidationError_WhenFirstIsBeforeLastOccurence()
        {
            // Arrange
            var startDate = new DateOnly(2024, 10, 10);
            var endDate = new DateOnly(2024, 10, 9);
            var eventGroup = new EventGroup(Title, startDate, endDate, WeekDays, startTime, endTime, PresenceType, null, UserId);
            
            // Act
            var result = eventGroup.Validate();
            
            // Assert
            result.Should().HaveCount(1);
        }
    }
}