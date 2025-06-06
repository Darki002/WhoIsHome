using WhoIsHome.Aggregates;
using WhoIsHome.Shared.Exceptions;
using WhoIsHome.Shared.Types;
// ReSharper disable LocalVariableHidesMember

namespace WhoIsHome.Test.Application.Aggregates;

[TestFixture]
public class RepeatedEventTest
{
    private const string Title = "Title";
    private const int UserId = 0;
    private const PresenceType PresenceType = WhoIsHome.Shared.Types.PresenceType.Unknown;
    private readonly DateOnly firstOccurrence = new DateOnly(2024, 10, 10);
    private readonly DateOnly lastOccurrence = new DateOnly(2024, 10, 31);
    private readonly TimeOnly startTime = new TimeOnly(13, 30, 0);
    private readonly TimeOnly endTime = new TimeOnly(14, 0, 0);
    private readonly TimeOnly? time = new TimeOnly(18, 0, 0);
    
    [TestFixture]
    private class Create : RepeatedEventTest
    {
        [Test]
        public void ReturnsNewOneTimeEvent()
        {
            // Act
            var result = RepeatedEvent.Create(Title, firstOccurrence, lastOccurrence, startTime, endTime, PresenceType, null, UserId);
            
            // Assert
            result.Id.Should().BeNull();
            result.Title.Should().Be(Title);
            result.FirstOccurrence.Should().Be(firstOccurrence);
            result.LastOccurrence.Should().Be(lastOccurrence);
            result.StartTime.Should().Be(startTime);
            result.EndTime.Should().Be(endTime);
            result.DinnerTime.PresenceType.Should().Be(PresenceType);
            result.DinnerTime.Time.Should().BeNull();
            result.UserId.Should().Be(UserId);
        }
        
        [Test]
        public void ReturnsNewOneTimeEvent_WithoutEndTime()
        {
            // Act
            var result = RepeatedEvent.Create(Title, firstOccurrence, null, startTime, null, PresenceType, null, UserId);
            
            // Assert
            result.Id.Should().BeNull();
            result.Title.Should().Be(Title);
            result.FirstOccurrence.Should().Be(firstOccurrence);
            result.LastOccurrence.Should().BeNull();
            result.StartTime.Should().Be(startTime);
            result.EndTime.Should().BeNull();
            result.DinnerTime.PresenceType.Should().Be(PresenceType);
            result.DinnerTime.Time.Should().BeNull();
            result.UserId.Should().Be(UserId);
        }
        
        [Test]
        public void ThrowsInvalidModelException_WhenTitleIsTooLong()
        {
            // Arrange
            var title = string.Join("", Enumerable.Repeat("a", 51));
            
            // Act
            var act = () => RepeatedEvent.Create(title, firstOccurrence, lastOccurrence, startTime, endTime, PresenceType, null, UserId);
            
            // Assert
            act.Should().Throw<InvalidModelException>();
        }
        
        [Test]
        public void ThrowsInvalidModelException_WhenStartTimeIsBeforeEndTime()
        {
            // Arrange
            var startTime = new TimeOnly(13, 0, 0);
            var endTime = new TimeOnly(12, 0, 0);
            
            // Act
            var act = () => RepeatedEvent.Create(Title, firstOccurrence, lastOccurrence, startTime, endTime, PresenceType, null, UserId);
            
            // Assert
            act.Should().Throw<InvalidModelException>();
        }
        
        [Test]
        public void ThrowsInvalidModelException_WhenEndTimeIsBeforeDinnerTime()
        {
            // Arrange
            var time = endTime.AddHours(-1);
            
            // Act
            var act = () => RepeatedEvent.Create(Title, firstOccurrence, lastOccurrence, startTime, endTime, PresenceType.Late, time, UserId);
            
            // Assert
            act.Should().Throw<InvalidModelException>();
        }
        
        [Test]
        public void ThrowsInvalidModelException_WhenFirstIsBeforeLastOccurence()
        {
            // Arrange
            var firstOccurrence = new DateOnly(2024, 10, 10);
            var lastOccurrence = new DateOnly(2024, 10, 9);
            
            // Act
            var act = () => RepeatedEvent.Create(Title, firstOccurrence, lastOccurrence, startTime, endTime, PresenceType.Late, time, UserId);
            
            // Assert
            act.Should().Throw<InvalidModelException>();
        }
    }
    
    [TestFixture]
    private class Update : RepeatedEventTest
    {
        private RepeatedEvent repeatedEvent = null!;

        [SetUp]
        public void SetUp()
        {
            var dinnerTime = new DinnerTime(PresenceType, time);
            repeatedEvent = new RepeatedEvent(null, Title, firstOccurrence, lastOccurrence, startTime, endTime, dinnerTime, UserId);
        }
        
        [Test]
        public void ReturnsNewOneTimeEvent()
        {
            // Arrange
            
            // Act
            repeatedEvent.Update(Title, firstOccurrence, lastOccurrence, startTime, endTime, PresenceType, null);
            
            // Assert
            repeatedEvent.Id.Should().BeNull();
            repeatedEvent.Title.Should().Be(Title);
            repeatedEvent.FirstOccurrence.Should().Be(firstOccurrence);
            repeatedEvent.LastOccurrence.Should().Be(lastOccurrence);
            repeatedEvent.StartTime.Should().Be(startTime);
            repeatedEvent.EndTime.Should().Be(endTime);
            repeatedEvent.DinnerTime.PresenceType.Should().Be(PresenceType);
            repeatedEvent.DinnerTime.Time.Should().BeNull();
            repeatedEvent.UserId.Should().Be(UserId);
        }
        
        [Test]
        public void ThrowsInvalidModelException_WhenTitleIsTooLong()
        {
            // Arrange
            var title = string.Join("", Enumerable.Repeat("a", 51));
            
            // Act
            var act = () => repeatedEvent.Update(title, firstOccurrence, lastOccurrence, startTime, endTime, PresenceType, null);
            
            // Assert
            act.Should().Throw<InvalidModelException>();
        }
        
        [Test]
        public void ThrowsInvalidModelException_WhenStartTimeIsBeforeEndTime()
        {
            // Arrange
            var startTime = new TimeOnly(13, 0, 0);
            var endTime = new TimeOnly(12, 0, 0);
            
            // Act
            var act = () => repeatedEvent.Update(Title, firstOccurrence, lastOccurrence, startTime, endTime, PresenceType, null);
            
            // Assert
            act.Should().Throw<InvalidModelException>();
        }
        
        [Test]
        public void ThrowsInvalidModelException_WhenEndTimeIsBeforeDinnerTime()
        {
            // Arrange
            var time = endTime.AddHours(-1);
            
            // Act
            var act = () => repeatedEvent.Update(Title, firstOccurrence, lastOccurrence, startTime, endTime, PresenceType.Late, time);
            
            // Assert
            act.Should().Throw<InvalidModelException>();
        }
        
        [Test]
        public void ThrowsInvalidModelException_WhenFirstIsBeforeLastOccurence()
        {
            // Arrange
            var firstOccurrence = new DateOnly(2024, 10, 10);
            var lastOccurrence = new DateOnly(2024, 10, 9);
            
            // Act
            var act = () => repeatedEvent.Update(Title, firstOccurrence, lastOccurrence, startTime, endTime, PresenceType.Late, time);
            
            // Assert
            act.Should().Throw<InvalidModelException>();
        }
    }
    
    [TestFixture]
    private class IsToday : RepeatedEventTest
    {
        [Test]
        public void ReturnsTrue_WhenEventIsToday()
        {
            // Arrange
            var firstOccurrence = new DateOnly(2024, 11, 1);
            var lastOccurrence = new DateOnly(2024, 11, 15);
            var dinnerTime = new DinnerTime(PresenceType, time);
            var oneTimeEvent = new RepeatedEvent(null, Title, firstOccurrence, lastOccurrence, startTime, endTime, dinnerTime, UserId);
            
            // Act
            var result = oneTimeEvent.IsEventAt(new DateOnly(2024, 11, 8));
            
            // Assert
            result.Should().BeTrue();
        }
        
        [Test]
        public void ReturnsTrue_WhenFirstOccurenceIsToday()
        {
            // Arrange
            var firstOccurrence = new DateOnly(2024, 11, 1);
            var lastOccurrence = new DateOnly(2024, 11, 14);
            var dinnerTime = new DinnerTime(PresenceType, time);
            var oneTimeEvent = new RepeatedEvent(null, Title, firstOccurrence, lastOccurrence, startTime, endTime, dinnerTime, UserId);
            
            // Act
            var result = oneTimeEvent.IsEventAt(firstOccurrence);
            
            // Assert
            result.Should().BeTrue();
        }
        
        [Test]
        public void ReturnsTrue_WhenLastOccurenceIsToday()
        {
            // Arrange
            var firstOccurrence = new DateOnly(2024, 11, 1);
            var lastOccurrence = new DateOnly(2024, 11, 15);
            var dinnerTime = new DinnerTime(PresenceType, time);
            var oneTimeEvent = new RepeatedEvent(null, Title, firstOccurrence, lastOccurrence, startTime, endTime, dinnerTime, UserId);
            
            // Act
            var result = oneTimeEvent.IsEventAt(lastOccurrence);
            
            // Assert
            result.Should().BeTrue();
        }
        
        [Test]
        public void ReturnsFalse_WhenEventIsBeforeFirstOccurence()
        {
            // Arrange
            var firstOccurrence = new DateOnly(2024, 11, 3);
            var lastOccurrence = new DateOnly(2024, 11, 10);
            var dinnerTime = new DinnerTime(PresenceType, time);
            var oneTimeEvent = new RepeatedEvent(null, Title, firstOccurrence, lastOccurrence, startTime, endTime, dinnerTime, UserId);
            
            // Act
            var result = oneTimeEvent.IsEventAt(new DateOnly(2024, 11, 1));
            
            // Assert
            result.Should().BeFalse();
        }
        
        [Test]
        public void ReturnsFalse_WhenEventIsAfterLastOccurence()
        {
            // Arrange
            var firstOccurrence = new DateOnly(2024, 11, 1);
            var lastOccurrence = new DateOnly(2024, 11, 7);
            var dinnerTime = new DinnerTime(PresenceType, time);
            var oneTimeEvent = new RepeatedEvent(null, Title, firstOccurrence, lastOccurrence, startTime, endTime, dinnerTime, UserId);
            
            // Act
            var result = oneTimeEvent.IsEventAt(new DateOnly(2024, 11, 14));
            
            // Assert
            result.Should().BeFalse();
        }
        
        [Test]
        public void ReturnsTrue_WhenEventIsInfinitelyLong()
        {
            // Arrange
            var firstOccurrence = new DateOnly(2024, 11, 1);
            var dinnerTime = new DinnerTime(PresenceType, time);
            var repeatedEvent = new RepeatedEvent(null, Title, firstOccurrence, null, startTime, endTime, dinnerTime, UserId);
            
            // Act
            var result = repeatedEvent.IsEventAt(firstOccurrence);
            
            // Assert
            result.Should().BeTrue();
        }
    }
    
    [TestFixture]
    private class GetNextOccurrence : RepeatedEventTest
    {
        [Test]
        public void ThrowsInvalidOperationException_WhenTodayIsAfterLastOccurence()
        {
            // Arrange
            var firstOccurrence = new DateOnly(2024, 11, 1);
            var lastOccurrence = new DateOnly(2024, 11, 14);
            var dinnerTime = new DinnerTime(PresenceType, time);
            var repeatedEvent = new RepeatedEvent(null, Title, firstOccurrence, lastOccurrence, startTime, endTime, dinnerTime, UserId);
             
            // Act
            var act = () => repeatedEvent.GetNextOccurrence(new DateOnly(2024, 11, 21));
             
            // Assert
            act.Should().Throw<InvalidOperationException>();
        }

        [Test]
        [TestCase(7, 21, 7)]
        [TestCase(-7, 0, 0)]
        [TestCase(-7, 7, 0)]
        [TestCase(-8, 7, 6)]
        [TestCase(-5, 14, 2)]
        public void ReturnsExpectedOccurence(int firstDayShift, int lastDayShift, int expectedDayShift)
        {
            // Arrange
            var date = new DateOnly(2024, 11, 1);
            
            var expected = date.AddDays(expectedDayShift);
            
            var firstOccurrence = date.AddDays(firstDayShift);
            var lastOccurrence = date.AddDays(lastDayShift);
            
            var dinnerTime = new DinnerTime(PresenceType, time);
            var repeatedEvent = new RepeatedEvent(null, Title, firstOccurrence, lastOccurrence, startTime, endTime, dinnerTime, UserId);
             
            // Act
            var result = repeatedEvent.GetNextOccurrence(date);
             
            // Assert
            result.Should().Be(expected);
        }
        
        [Test]
        [TestCase(-7, 0, 0)]
        [TestCase(-7, 7, 0)]
        [TestCase(0, 7, 0)]
        [TestCase(0, 14, 0)]
        public void IsAtHomeIsTrue_AndNextOccurenceReturnsExpectedDate(int firstDayShift, int lastDayShift, int expectedDayShift)
        {
            // Arrange
            var date = new DateOnly(2024, 11, 1);
            
            var expected = date.AddDays(expectedDayShift);
            
            var firstOccurrence = date.AddDays(firstDayShift);
            var lastOccurrence = date.AddDays(lastDayShift);
            
            var dinnerTime = new DinnerTime(PresenceType, time);
            var repeatedEvent = new RepeatedEvent(null, Title, firstOccurrence, lastOccurrence, startTime, endTime, dinnerTime, UserId);
             
            // Act
            var result = repeatedEvent.GetNextOccurrence(date);
             
            // Assert
            repeatedEvent.IsEventAt(date).Should().BeTrue();
            result.Should().Be(expected);
        }
    }
}