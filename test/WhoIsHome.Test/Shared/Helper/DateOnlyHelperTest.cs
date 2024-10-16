using WhoIsHome.Shared.Helper;

namespace WhoIsHome.Test.Shared.Helper;

[TestFixture]
public class DateOnlyHelperTest
{
    [Test]
    public void IsThisWeek_ReturnsTrue_WhenGivenDateOnlyIsInTheSameWeekAs()
    {
        // Arrange
        var date1 = new DateOnly(2024, 10, 10);
        var date2 = new DateTime(2024, 10, 12, 0, 0, 0);

        // Act
        var result = date1.IsSameWeek(date2);

        // Asser
        result.Should().BeTrue();
    }

    [Test]
    public void IsThisWeek_ReturnsFalse_WhenGivenDateOnlyIsInTheNextWeek()
    {
        // Arrange
        var date1 = new DateOnly(2024, 10, 10);
        var date2 = new DateTime(2024, 10, 15, 0, 0, 0);

        // Act
        var result = date1.IsSameWeek(date2);

        // Asser
        result.Should().BeFalse();
    }
    
    [Test]
    public void IsThisWeek_ReturnsFalse_WhenGivenDateOnlyIsInThePreviousWeek()
    {
        // Arrange
        var date1 = new DateOnly(2024, 10, 10);
        var date2 = new DateTime(2024, 10, 5, 0, 0, 0);

        // Act
        var result = date1.IsSameWeek(date2);

        // Asser
        result.Should().BeFalse();
    }  
    
    [Test]
    public void IsThisWeek_ReturnsDateTime_WhichIsTheLastMondayTimeZero()
    {
        // Arrange
        var expected = new DateTime(2024, 10, 7, 0, 0, 0);
        var date = new DateTime(2024, 10, 10, 13, 31, 7);

        // Act
        var result = date.StartOfWeek();

        // Asser
        result.Should().Be(expected);
    }

    [Test]
    [TestCase(10, DayOfWeek.Friday, 1)]
    [TestCase(10, DayOfWeek.Saturday, 2)]
    [TestCase(10, DayOfWeek.Sunday, 3)]
    [TestCase(10, DayOfWeek.Monday, 4)]
    public void DaysUntilNext_ReturnsDaysUntilGivenWeekDay(int day, int weekDay, int expected)
    {
        // Arrange
        var dateTime = new DateOnly(2024, 10, day);
        
        // Act
        var result = dateTime.DaysUntilNext((DayOfWeek)weekDay);
        
        // Assert
        result.Should().Be(expected);
    }
}