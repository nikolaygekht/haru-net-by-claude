using System;

namespace Haru.Types
{
    /// <summary>
    /// Represents a date and time with timezone offset as used in PDF documents.
    /// </summary>
    public struct HpdfDate : IEquatable<HpdfDate>
    {
        /// <summary>
        /// Year (e.g., 2025).
        /// </summary>
        public int Year { get; set; }

        /// <summary>
        /// Month (1-12).
        /// </summary>
        public int Month { get; set; }

        /// <summary>
        /// Day of month (1-31).
        /// </summary>
        public int Day { get; set; }

        /// <summary>
        /// Hour (0-23).
        /// </summary>
        public int Hour { get; set; }

        /// <summary>
        /// Minutes (0-59).
        /// </summary>
        public int Minutes { get; set; }

        /// <summary>
        /// Seconds (0-59).
        /// </summary>
        public int Seconds { get; set; }

        /// <summary>
        /// Timezone indicator: '+' for positive offset, '-' for negative, 'Z' for UTC.
        /// </summary>
        public char Ind { get; set; }

        /// <summary>
        /// Timezone offset hours.
        /// </summary>
        public int OffHour { get; set; }

        /// <summary>
        /// Timezone offset minutes.
        /// </summary>
        public int OffMinutes { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="HpdfDate"/> struct.
        /// </summary>
        public HpdfDate(int year, int month, int day, int hour, int minutes, int seconds,
                       char ind = ' ', int offHour = 0, int offMinutes = 0)
        {
            Year = year;
            Month = month;
            Day = day;
            Hour = hour;
            Minutes = minutes;
            Seconds = seconds;
            Ind = ind;
            OffHour = offHour;
            OffMinutes = offMinutes;
        }

        /// <summary>
        /// Creates an HpdfDate from a DateTime value.
        /// </summary>
        public static HpdfDate FromDateTime(DateTime dateTime)
        {
            return new HpdfDate(
                dateTime.Year,
                dateTime.Month,
                dateTime.Day,
                dateTime.Hour,
                dateTime.Minute,
                dateTime.Second
            );
        }

        /// <summary>
        /// Creates an HpdfDate from a DateTimeOffset value with timezone information.
        /// </summary>
        public static HpdfDate FromDateTimeOffset(DateTimeOffset dateTimeOffset)
        {
            var offset = dateTimeOffset.Offset;
            char ind = offset.TotalMinutes >= 0 ? '+' : '-';

            return new HpdfDate(
                dateTimeOffset.Year,
                dateTimeOffset.Month,
                dateTimeOffset.Day,
                dateTimeOffset.Hour,
                dateTimeOffset.Minute,
                dateTimeOffset.Second,
                ind,
                Math.Abs(offset.Hours),
                Math.Abs(offset.Minutes)
            );
        }

        /// <summary>
        /// Converts this HpdfDate to a DateTime (timezone info is lost).
        /// </summary>
        public DateTime ToDateTime()
        {
            try
            {
                return new DateTime(Year, Month, Day, Hour, Minutes, Seconds);
            }
            catch
            {
                return DateTime.MinValue;
            }
        }

        /// <summary>
        /// Converts this HpdfDate to a DateTimeOffset (preserves timezone).
        /// </summary>
        public DateTimeOffset ToDateTimeOffset()
        {
            try
            {
                var dt = new DateTime(Year, Month, Day, Hour, Minutes, Seconds);
                int offsetMinutes = OffHour * 60 + OffMinutes;
                if (Ind == '-')
                    offsetMinutes = -offsetMinutes;

                var offset = TimeSpan.FromMinutes(offsetMinutes);
                return new DateTimeOffset(dt, offset);
            }
            catch
            {
                return DateTimeOffset.MinValue;
            }
        }

        public bool Equals(HpdfDate other)
        {
            return Year == other.Year &&
                   Month == other.Month &&
                   Day == other.Day &&
                   Hour == other.Hour &&
                   Minutes == other.Minutes &&
                   Seconds == other.Seconds &&
                   Ind == other.Ind &&
                   OffHour == other.OffHour &&
                   OffMinutes == other.OffMinutes;
        }

        public override bool Equals(object obj)
        {
            return obj is HpdfDate other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = Year;
                hash = (hash * 397) ^ Month;
                hash = (hash * 397) ^ Day;
                hash = (hash * 397) ^ Hour;
                hash = (hash * 397) ^ Minutes;
                hash = (hash * 397) ^ Seconds;
                hash = (hash * 397) ^ Ind.GetHashCode();
                hash = (hash * 397) ^ OffHour;
                hash = (hash * 397) ^ OffMinutes;
                return hash;
            }
        }

        public static bool operator ==(HpdfDate left, HpdfDate right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(HpdfDate left, HpdfDate right)
        {
            return !left.Equals(right);
        }

        public override string ToString()
        {
            if (Ind == 'Z')
                return $"{Year:D4}-{Month:D2}-{Day:D2}T{Hour:D2}:{Minutes:D2}:{Seconds:D2}Z";
            else if (Ind == '+' || Ind == '-')
                return $"{Year:D4}-{Month:D2}-{Day:D2}T{Hour:D2}:{Minutes:D2}:{Seconds:D2}{Ind}{OffHour:D2}:{OffMinutes:D2}";
            else
                return $"{Year:D4}-{Month:D2}-{Day:D2}T{Hour:D2}:{Minutes:D2}:{Seconds:D2}";
        }
    }
}
