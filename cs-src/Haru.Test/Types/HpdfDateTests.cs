using System;
using FluentAssertions;
using Haru.Types;
using Xunit;

namespace Haru.Test.Types
{
    public class HpdfDateTests
    {
        [Fact]
        public void Constructor_SetsAllFields()
        {
            var date = new HpdfDate(2025, 10, 7, 14, 30, 45, '+', 5, 30);

            date.Year.Should().Be(2025);
            date.Month.Should().Be(10);
            date.Day.Should().Be(7);
            date.Hour.Should().Be(14);
            date.Minutes.Should().Be(30);
            date.Seconds.Should().Be(45);
            date.Ind.Should().Be('+');
            date.OffHour.Should().Be(5);
            date.OffMinutes.Should().Be(30);
        }

        [Fact]
        public void FromDateTime_ConvertsCorrectly()
        {
            var dateTime = new DateTime(2025, 10, 7, 14, 30, 45);
            var hpdfDate = HpdfDate.FromDateTime(dateTime);

            hpdfDate.Year.Should().Be(2025);
            hpdfDate.Month.Should().Be(10);
            hpdfDate.Day.Should().Be(7);
            hpdfDate.Hour.Should().Be(14);
            hpdfDate.Minutes.Should().Be(30);
            hpdfDate.Seconds.Should().Be(45);
        }

        [Fact]
        public void FromDateTimeOffset_ConvertsWithTimezone()
        {
            var offset = new TimeSpan(5, 30, 0);
            var dateTimeOffset = new DateTimeOffset(2025, 10, 7, 14, 30, 45, offset);
            var hpdfDate = HpdfDate.FromDateTimeOffset(dateTimeOffset);

            hpdfDate.Year.Should().Be(2025);
            hpdfDate.Ind.Should().Be('+');
            hpdfDate.OffHour.Should().Be(5);
            hpdfDate.OffMinutes.Should().Be(30);
        }

        [Fact]
        public void FromDateTimeOffset_HandlesNegativeOffset()
        {
            var offset = new TimeSpan(-5, -30, 0);
            var dateTimeOffset = new DateTimeOffset(2025, 10, 7, 14, 30, 45, offset);
            var hpdfDate = HpdfDate.FromDateTimeOffset(dateTimeOffset);

            hpdfDate.Ind.Should().Be('-');
            hpdfDate.OffHour.Should().Be(5);
            hpdfDate.OffMinutes.Should().Be(30);
        }

        [Fact]
        public void ToDateTime_ConvertsBack()
        {
            var hpdfDate = new HpdfDate(2025, 10, 7, 14, 30, 45);
            var dateTime = hpdfDate.ToDateTime();

            dateTime.Year.Should().Be(2025);
            dateTime.Month.Should().Be(10);
            dateTime.Day.Should().Be(7);
            dateTime.Hour.Should().Be(14);
            dateTime.Minute.Should().Be(30);
            dateTime.Second.Should().Be(45);
        }

        [Fact]
        public void ToString_FormatsWithTimezone()
        {
            var date = new HpdfDate(2025, 10, 7, 14, 30, 45, '+', 5, 30);

            date.ToString().Should().Be("2025-10-07T14:30:45+05:30");
        }

        [Fact]
        public void ToString_FormatsUtc()
        {
            var date = new HpdfDate(2025, 10, 7, 14, 30, 45, 'Z', 0, 0);

            date.ToString().Should().Be("2025-10-07T14:30:45Z");
        }
    }
}
