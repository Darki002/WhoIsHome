using WhoIsHome.Aggregates;
using WhoIsHome.Shared.Exceptions;
using WhoIsHome.Shared.Types;

namespace WhoIsHome.Test.Application.Aggregates;

[TestFixture]
public class DinnerTimeTest
{
    private readonly TimeOnly timeOnly = new TimeOnly(13, 30, 0);

    [TestFixture]
    private class Create : DinnerTimeTest
    {
        [Test]
        public void ReturnsNewDinnerTime_FromUnknownType()
        {
            // Arrange
            const PresenceType presenceType = PresenceType.Unknown;

            // Act
            var result = DinnerTime.Create(presenceType, null);

            // Assert
            result.PresenceType.Should().Be(presenceType);
            result.Time.Should().BeNull();
        }

        [Test]
        public void ThrowsInvalidModelException_UnknownTimeWithTime()
        {
            // Arrange
            const PresenceType presenceType = PresenceType.Unknown;

            // Act
            var result = () => DinnerTime.Create(presenceType, timeOnly);

            // Assert
            result.Should().Throw<InvalidModelException>();
        }

        [Test]
        public void ReturnsNewDinnerTime_FromDefaultType()
        {
            // Arrange
            const PresenceType presenceType = PresenceType.Default;

            // Act
            var result = DinnerTime.Create(presenceType, timeOnly);

            // Assert
            result.PresenceType.Should().Be(presenceType);
            result.Time.Should().Be(timeOnly);
        }

        [Test]
        public void ThrowsInvalidModelException_DefaultTimeWithTime()
        {
            // Arrange
            const PresenceType presenceType = PresenceType.Default;

            // Act
            var result = () => DinnerTime.Create(presenceType, null);

            // Assert
            result.Should().Throw<InvalidModelException>();
        }

        [Test]
        public void ReturnsNewDinnerTime_FromLateType()
        {
            // Arrange
            const PresenceType presenceType = PresenceType.Late;

            // Act
            var result = DinnerTime.Create(presenceType, timeOnly);

            // Assert
            result.PresenceType.Should().Be(presenceType);
            result.Time.Should().Be(timeOnly);
        }

        [Test]
        public void ThrowsInvalidModelException_LateTimeWithTime()
        {
            // Arrange
            const PresenceType presenceType = PresenceType.Late;

            // Act
            var result = () => DinnerTime.Create(presenceType, null);

            // Assert
            result.Should().Throw<InvalidModelException>();
        }

        [Test]
        public void ReturnsNewDinnerTime_FromNotPresentType()
        {
            // Arrange
            const PresenceType presenceType = PresenceType.NotPresent;

            // Act
            var result = DinnerTime.Create(presenceType, null);

            // Assert
            result.PresenceType.Should().Be(presenceType);
            result.Time.Should().BeNull();
        }

        [Test]
        public void ThrowsInvalidModelException_NotPresentTimeWithTime()
        {
            // Arrange
            const PresenceType presenceType = PresenceType.NotPresent;

            // Act
            var result = () => DinnerTime.Create(presenceType, timeOnly);

            // Assert
            result.Should().Throw<InvalidModelException>();
        }
    }

    [TestFixture]
    private class Update : DinnerTimeTest
    {
        private readonly DinnerTime dinnerTime = new DinnerTime(PresenceType.Unknown, null);

        [Test]
        public void ReturnsNewDinnerTime_FromUnknownType()
        {
            // Arrange
            const PresenceType presenceType = PresenceType.Unknown;

            // Act
            var result = dinnerTime.Update(presenceType, null);

            // Assert
            result.PresenceType.Should().Be(presenceType);
            result.Time.Should().BeNull();
        }

        [Test]
        public void ThrowsInvalidModelException_UnknownTimeWithTime()
        {
            // Arrange
            const PresenceType presenceType = PresenceType.Unknown;

            // Act
            var result = () => dinnerTime.Update(presenceType, timeOnly);

            // Assert
            result.Should().Throw<InvalidModelException>();
        }

        [Test]
        public void ReturnsNewDinnerTime_FromDefaultType()
        {
            // Arrange
            const PresenceType presenceType = PresenceType.Default;

            // Act
            var result = dinnerTime.Update(presenceType, timeOnly);

            // Assert
            result.PresenceType.Should().Be(presenceType);
            result.Time.Should().Be(timeOnly);
        }

        [Test]
        public void ThrowsInvalidModelException_DefaultTimeWithTime()
        {
            // Arrange
            const PresenceType presenceType = PresenceType.Default;

            // Act
            var result = () => dinnerTime.Update(presenceType, null);

            // Assert
            result.Should().Throw<InvalidModelException>();
        }

        [Test]
        public void ReturnsNewDinnerTime_FromLateType()
        {
            // Arrange
            const PresenceType presenceType = PresenceType.Late;

            // Act
            var result = dinnerTime.Update(presenceType, timeOnly);

            // Assert
            result.PresenceType.Should().Be(presenceType);
            result.Time.Should().Be(timeOnly);
        }

        [Test]
        public void ThrowsInvalidModelException_LateTimeWithTime()
        {
            // Arrange
            const PresenceType presenceType = PresenceType.Late;

            // Act
            var result = () => dinnerTime.Update(presenceType, null);

            // Assert
            result.Should().Throw<InvalidModelException>();
        }

        [Test]
        public void ReturnsNewDinnerTime_FromNotPresentType()
        {
            // Arrange
            const PresenceType presenceType = PresenceType.NotPresent;

            // Act
            var result = dinnerTime.Update(presenceType, null);

            // Assert
            result.PresenceType.Should().Be(presenceType);
            result.Time.Should().BeNull();
        }

        [Test]
        public void ThrowsInvalidModelException_NotPresentTimeWithTime()
        {
            // Arrange
            const PresenceType presenceType = PresenceType.NotPresent;

            // Act
            var result = () => dinnerTime.Update(presenceType, timeOnly);

            // Assert
            result.Should().Throw<InvalidModelException>();
        }
    }

    [TestFixture]
    private class IsAtHome
    {
        [Test]
        [TestCase(PresenceType.NotPresent, false)]
        [TestCase(PresenceType.Unknown, true)]
        [TestCase(PresenceType.Default, true)]
        [TestCase(PresenceType.Late, true)]
        public void ReturnsExpected_FromType(PresenceType presenceType, bool expected)
        {
            // Act
            var time = presenceType is PresenceType.Default or PresenceType.Late
                ? new TimeOnly(19, 0, 0)
                : (TimeOnly?)null;
            var result = DinnerTime.Create(presenceType, time);

            // Assert
            result.IsAtHome.Should().Be(expected);
        }
    }
}