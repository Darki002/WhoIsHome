using WhoIsHome.Entities;
using WhoIsHome.Shared.Exceptions;
using WhoIsHome.Shared.Helper;
using WhoIsHome.Shared.Types;

// ReSharper disable LocalVariableHidesMember

namespace WhoIsHome.Test.Application.Aggregates;

[TestFixture]
public class OneTimeEventTest
{
    private const string Title = "Title";
    private const int UserId = 0;
    private const PresenceType PresenceType = WhoIsHome.Shared.Types.PresenceType.Unknown;
    private readonly DateOnly date = new DateOnly(2024, 10, 10);
    private readonly TimeOnly startTime = new TimeOnly(13, 30, 0);
    private readonly TimeOnly endTime = new TimeOnly(14, 0, 0);
    private readonly TimeOnly? time = new TimeOnly(18, 0, 0);

    [TestFixture]
    private class Create : OneTimeEventTest
    {
        [Test]
        public void ReturnsNewOneTimeEvent()
        {
            // Act
            var result = OneTimeEvent.Create(Title, date, startTime, endTime, PresenceType, null, UserId);

            // Assert
            result.Id.Should().BeNull();
            result.Title.Should().Be(Title);
            result.Date.Should().Be(date);
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
            var result = OneTimeEvent.Create(Title, date, startTime, null, PresenceType, null, UserId);

            // Assert
            result.Id.Should().BeNull();
            result.Title.Should().Be(Title);
            result.Date.Should().Be(date);
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
            var act = () => OneTimeEvent.Create(title, date, startTime, endTime, PresenceType, null, UserId);

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
            var act = () => OneTimeEvent.Create(Title, date, startTime, endTime, PresenceType, null, UserId);

            // Assert
            act.Should().Throw<InvalidModelException>();
        }

        [Test]
        public void ThrowsInvalidModelException_WhenEndTimeIsBeforeDinnerTime()
        {
            // Arrange
            var time = endTime.AddHours(-1);

            // Act
            var act = () => OneTimeEvent.Create(Title, date, startTime, endTime, PresenceType.Late, time, UserId);

            // Assert
            act.Should().Throw<InvalidModelException>();
        }
    }

    [TestFixture]
    private class Update : OneTimeEventTest
    {
        [SetUp]
        public void SetUp()
        {
            var dinnerTime = new DinnerTime(PresenceType, time);
            oneTimeEvent = new OneTimeEvent(null, Title, date, startTime, endTime, dinnerTime, UserId);
        }

        private OneTimeEvent oneTimeEvent = null!;

        [Test]
        public void ReturnsNewOneTimeEvent()
        {
            // Arrange

            // Act
            oneTimeEvent.Update(Title, date, startTime, endTime, PresenceType, null);

            // Assert
            oneTimeEvent.Id.Should().BeNull();
            oneTimeEvent.Title.Should().Be(Title);
            oneTimeEvent.Date.Should().Be(date);
            oneTimeEvent.StartTime.Should().Be(startTime);
            oneTimeEvent.EndTime.Should().Be(endTime);
            oneTimeEvent.DinnerTime.PresenceType.Should().Be(PresenceType);
            oneTimeEvent.DinnerTime.Time.Should().BeNull();
            oneTimeEvent.UserId.Should().Be(UserId);
        }

        [Test]
        public void ThrowsInvalidModelException_WhenTitleIsTooLong()
        {
            // Arrange
            var title = string.Join("", Enumerable.Repeat("a", 51));

            // Act
            var act = () => oneTimeEvent.Update(title, date, startTime, endTime, PresenceType, null);

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
            var act = () => oneTimeEvent.Update(Title, date, startTime, endTime, PresenceType, null);

            // Assert
            act.Should().Throw<InvalidModelException>();
        }

        [Test]
        public void ThrowsInvalidModelException_WhenEndTimeIsBeforeDinnerTime()
        {
            // Arrange
            var time = endTime.AddHours(-1);

            // Act
            var act = () => oneTimeEvent.Update(Title, date, startTime, endTime, PresenceType.Late, time);

            // Assert
            act.Should().Throw<InvalidModelException>();
        }
    }

    [TestFixture]
    private class IsToday : OneTimeEventTest
    {
        [Test]
        public void ReturnsTrue_WhenEventIsToday()
        {
            // Arrange
            var date = new DateOnly(2024, 11, 26);
            var dinnerTime = new DinnerTime(PresenceType, time);
            var oneTimeEvent = new OneTimeEvent(null, Title, date, startTime, endTime, dinnerTime, UserId);

            // Act
            var result = oneTimeEvent.IsEventAt(date);

            // Assert
            result.Should().BeTrue();
        }

        [Test]
        public void ReturnsFalse_WhenEventIsYesterday()
        {
            // Arrange
            var date = new DateOnly(2024, 11, 26);
            var dinnerTime = new DinnerTime(PresenceType, time);
            var oneTimeEvent = new OneTimeEvent(null, Title, date, startTime, endTime, dinnerTime, UserId);

            // Act
            var result = oneTimeEvent.IsEventAt(new DateOnly(2024, 11, 25));

            // Assert
            result.Should().BeFalse();
        }

        [Test]
        public void ReturnsFalse_WhenEventIsTomorrow()
        {
            // Arrange
            var date = new DateOnly(2024, 11, 26);
            var dinnerTime = new DinnerTime(PresenceType, time);
            var oneTimeEvent = new OneTimeEvent(null, Title, date, startTime, endTime, dinnerTime, UserId);

            // Act
            var result = oneTimeEvent.IsEventAt(new DateOnly(2024, 11, 27));

            // Assert
            result.Should().BeFalse();
        }
    }

    [TestFixture]
    private class GetNextOccurrence : OneTimeEventTest
    {
        [Test]
        public void ReturnsDateOnly_ThatRepresentsTheOccurenceDate()
        {
            // Arrange
            var date = new DateOnly(2024, 11, 26);
            var dinnerTime = new DinnerTime(PresenceType, time);
            var oneTimeEvent = new OneTimeEvent(null, Title, date, startTime, endTime, dinnerTime, UserId);

            // Act
            var result = oneTimeEvent.GetNextOccurrence(date);

            // Assert
            result.Should().Be(date);
        }

        [Test]
        public void IsAtHomeIsTrue_AndNextOccurenceReturnsExpectedDate()
        {
            // Arrange
            var date = new DateOnly(2024, 11, 26);
            var dinnerTime = new DinnerTime(PresenceType, time);
            var oneTimeEvent = new OneTimeEvent(null, Title, date, startTime, endTime, dinnerTime, UserId);

            // Act
            var result = oneTimeEvent.GetNextOccurrence(date);

            // Assert
            oneTimeEvent.IsEventAt(date).Should().BeTrue();
            result.Should().Be(date);
        }
    }
}