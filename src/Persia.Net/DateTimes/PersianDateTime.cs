﻿using Persia.Net.Words;
using static Persia.Net.Constants.CalendarConstants;
using static Persia.Net.Constants.PersianCalendarConstants;

namespace Persia.Net.DateTimes;

public class PersianDateTime : IEquatable<PersianDateTime>
{
    /// <summary>
    /// Gets the year component of the date.
    /// </summary>
    public int Year { get; private set; }

    /// <summary>
    /// Gets the month component of the date.
    /// </summary>
    public int Month { get; private set; }

    /// <summary>
    /// Gets the day component of the date.
    /// </summary>
    public int Day { get; private set; }

    /// <summary>
    /// Gets the hour component of the time.
    /// </summary>
    public int Hour { get; private set; }

    /// <summary>
    /// Gets the minute component of the time.
    /// </summary>
    public int Minute { get; private set; }

    /// <summary>
    /// Gets the second component of the time.
    /// </summary>
    public int Second { get; private set; }

    /// <summary>
    /// Gets the millisecond component of the time.
    /// </summary>
    public int Millisecond { get; private set; }

    /// <summary>
    /// Gets the number of ticks that represent the time.
    /// </summary>
    public long Ticks { get; private set; }

    /// <summary>
    /// Gets the day number of the week. This number is zero based.
    /// </summary>
    public int DayOfWeek { get; private set; }

    /// <summary>
    /// Gets the days of current month.
    /// </summary>
    public int DaysInMonth { get; private set; }

    /// <summary>
    /// Gets the name of current month.
    /// </summary>
    public string MonthName { get; private set; } = string.Empty;

    /// <summary>
    /// Gets the day name of the week in Persian Calendar.
    /// </summary>
    public string DayOfWeekName { get; private set; } = string.Empty;

    /// <summary>
    /// Gets a value indicating whether the current Persian year is a leap year.
    /// </summary>
    public bool IsLeapYear { get; private set; }

    /// <summary>
    /// Gets the number of days that have passed since the beginning of the current year in the Persian calendar.
    /// </summary>
    public int DayOfYear { get; private set; }

    /// <summary>
    /// Gets the number of days remaining until the end of the current year in the Persian calendar.
    /// </summary>
    public int DaysRemainingInYear { get; private set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="PersianDateTime"/> class.
    /// </summary>
    /// <param name="year">The year component of the Persian date.</param>
    /// <param name="month">The month component of the Persian date.</param>
    /// <param name="day">The day component of the Persian date.</param>
    /// <param name="time">The optional parameter for the time of the given day.</param>
    public PersianDateTime(int year, int month, int day, TimeOnly time = default)
    {
        var dt = ToDateTime(year, month, day);
        var dtPersian = dt.ToPersianDateTime();
        Year = dtPersian.Year;
        Month = dtPersian.Month;
        Day = dtPersian.Day;
        DayOfWeek = dtPersian.DayOfWeek;
        DaysInMonth = dtPersian.DaysInMonth;
        MonthName = dtPersian.MonthName;
        DayOfWeekName = dtPersian.DayOfWeekName;
        IsLeapYear = dtPersian.IsLeapYear;
        DayOfYear = dtPersian.DayOfYear;
        DaysRemainingInYear = dtPersian.DaysRemainingInYear;
        Hour = time.Hour;
        Minute = time.Minute;
        Second = time.Second;
        Millisecond = time.Millisecond;
    }

    internal PersianDateTime(int year, int month, int day)
    {
        Year = year;
        Month = month;
        Day = day;
    }

    public bool Equals(PersianDateTime other)
    {
        if (other is null)
            return false;

        return Year == other.Year && Month == other.Month && Day == other.Day;
    }

    public override bool Equals(object obj)
    {
        if (ReferenceEquals(null, obj))
            return false;

        if (ReferenceEquals(this, obj))
            return true;

        if (obj.GetType() != GetType())
            return false;

        return Equals((PersianDateTime)obj);
    }

    public override int GetHashCode()
    {
        unchecked
        {
            var hashCode = Year;
            hashCode = (hashCode * 397) ^ Month;
            hashCode = (hashCode * 397) ^ Day;
            return hashCode;
        }
    }

    public static bool operator ==(PersianDateTime left, PersianDateTime right)
    {
        if (ReferenceEquals(left, right))
            return true;

        if (ReferenceEquals(left, null) || ReferenceEquals(right, null))
            return false;

        return left.Equals(right);
    }

    public static bool operator !=(PersianDateTime left, PersianDateTime right)
    {
        return !(left == right);
    }

    internal PersianDateTime SetDay(int day)
    {
        Day = day;
        return this;
    }

    internal PersianDateTime SetTime(TimeOnly time)
    {
        Hour = time.Hour;
        Minute = time.Minute;
        Second = time.Second;
        Millisecond = time.Millisecond;
        return this;
    }

    internal PersianDateTime SetTicks(long ticks)
    {
        Ticks = ticks;
        return this;
    }

    internal PersianDateTime SetDayOfWeek(int dayOfWeek)
    {
        DayOfWeek = (dayOfWeek + 1) % 7;
        DayOfWeekName = Weekdays[DayOfWeek];
        return this;
    }

    internal PersianDateTime SetDaysInMonth(int daysInMonth)
    {
        DaysInMonth = daysInMonth;
        MonthName = Months[Month - 1];
        return this;
    }

    internal PersianDateTime GetLeapYearStatus()
    {
        // Group years into cycles of 2820 years each
        IsLeapYear = CheckLeapYear(Year);
        return this;
    }

    internal PersianDateTime SetYearProgress(int dayOfYear, int daysRemainingInYear)
    {
        DayOfYear = dayOfYear;
        DaysRemainingInYear = daysRemainingInYear;
        return this;
    }

    /// <summary>
    /// Determines whether the specified year is a leap year in the Persian calendar.
    /// </summary>
    /// <param name="year">The year to check.</param>
    /// <returns>true if the specified year is a leap year; otherwise, false.</returns>
    public static bool CheckLeapYear(int year)
    {
        // Group years into cycles of 2820 years each
        var yearInCycle = year % PersianCalendarGrandCycle;

        // Calculate the ordinal number of the year within the cycle
        var ordinalNumber = yearInCycle % YearsInSmallCycle;

        // Check if the year is evenly divisible by 4
        var isLeapYear = ordinalNumber % 4 == 0;

        return isLeapYear;
    }

    internal PersianDateTime GetYearProgress(DateTime date)
    {
        var firstDay = ToDateTime(Year, 1, 1);
        DayOfYear = (date - firstDay).Days;
        var lastDay = ToDateTime(Year + 1, 1, 1).AddDays(-1);
        DaysRemainingInYear = (lastDay - date).Days;
        return this;
    }

    /// <summary>
    /// Converts the date to a string in the format "yyyy/MM/dd".
    /// </summary>
    /// <returns>برای مثال, 1402/12/20.</returns>
    public new string ToString()
    {
        return $"{Year}/{Month:D2}/{Day:D2}";
    }

    /// <summary>
    /// Converts the date to a string in the format "yyyy/MM/dd" with Persian numbers.
    /// </summary>
    /// <returns>برای مثال, ۱۴۰۲/۱۲/۲۰.</returns>
    public string ToShortPersianString()
    {
        return ($"{Year}/{Month:D2}/{Day:D2}").ToPersianString();
    }

    //ToOrdinalWords
    //ToClockNotation
    /// <summary>
    /// Converts the date to a string in the Persian format with Year and Day in digit and Month in word.
    /// </summary>
    /// <returns>برای مثال, ۲۰ اسفند ۱۴۰۲</returns>
    public string ToPersianString()
    {
        return ($"{Day} {Months[Month - 1]} {Year}").ToPersianString(true);
    }

    /// <summary>
    /// Converts the date to a string in the Persian format with Year and Day in digit and Month in word as well as weekday name and time.
    /// </summary>
    /// <returns>برای مثال, یکشنبه ۲۰ اسفند ۱۴۰۲</returns>
    public string ToLongPersianString(bool timeVisible = false)
    {
        return timeVisible
            ? ($"{Hour:D2}:{Minute:D2} {DayOfWeekName} {Day} {Months[Month - 1]} {Year}، ساعت ").ToPersianString()
            : ($"{DayOfWeekName} {Day} {Months[Month - 1]} {Year}").ToPersianString();
    }

    /// <summary>
    /// Converts the date to a string in the Persian format with Year in digit and Day and Month in words.
    /// </summary>
    /// <returns>برای مثال, بیستم اسفند ۱۴۰۲</returns>
    public string ToPersianDayMonthString()
    {
        return ($"{Year} {Days[Day - 1]} {Months[Month - 1]}").ToPersianString();
    }

    /// <summary>
    /// Converts the date to a string in the Persian as well as week day name.
    /// </summary>
    /// <returns>برای مثال, یکشنبه ۱۴۰۲/۱۲/۲۰</returns>
    public string ToPersianWeekdayString()
    {
        return ($"{Year}/{Month:D2}/{Day:D2} " + DayOfWeekName).ToPersianString();
    }

    /// <summary>
    /// Converts the date to a string in the Persian format with Year in digit and Day and Month in words as well as weekday name.
    /// </summary>
    /// <returns>برای مثال, یکشنبه بیستم اسفند ۱۴۰۲</returns>
    public string ToLongPersianOrdinalWords()
    {
        return $"{DayOfWeekName} {Days[Day - 1]} {Months[Month - 1]} {Year.ToPersianString()}";
    }

    /// <summary>
    /// Converts a Persian (Solar Hijri) date and time to the equivalent Gregorian (Western) DateTime.
    /// </summary>
    /// <param name="year">The Persian year.</param>
    /// <param name="month">The Persian month (1 to 12).</param>
    /// <param name="day">The Persian day of the month.</param>
    /// <param name="hour">The hour (0 to 23).</param>
    /// <param name="minute">The minute (0 to 59).</param>
    /// <param name="second">The second (0 to 59).</param>
    /// <param name="millisecond">The optional millisecond (0 to 999).</param>
    /// <returns>A DateTime object representing the converted date and time in the Gregorian calendar.</returns>
    public static DateTime ToDateTime(int year, int month, int day, int hour, int minute, int second, int millisecond = 0)
    {
        return Converter.ConvertToGregorian(year, month, day, hour, minute, second, millisecond);
    }

    /// <summary>
    /// Converts a Persian (Solar Hijri) date (year, month, and day) to the equivalent Gregorian (Western) DateTime.
    /// The current time (hour, minute, second, and millisecond) is obtained from the system clock.
    /// </summary>
    /// <param name="year">The Persian year.</param>
    /// <param name="month">The Persian month (1 to 12).</param>
    /// <param name="day">The Persian day of the month.</param>
    /// <returns>A DateTime object representing the converted date and time in the Gregorian calendar.</returns>
    public static DateTime ToDateTime(int year, int month, int day)
    {
        var timeOnly = TimeOnly.FromTimeSpan(DateTime.Now.TimeOfDay);
        return Converter.ConvertToGregorian(year, month, day, timeOnly.Hour, timeOnly.Minute, timeOnly.Second, timeOnly.Millisecond);
    }

    /// <summary>
    /// Converts a Persian (Solar Hijri) date to the equivalent DateOnly (date without time) in the Gregorian calendar.
    /// </summary>
    /// <param name="year">The Persian year.</param>
    /// <param name="month">The Persian month (1 to 12).</param>
    /// <param name="day">The Persian day of the month.</param>
    /// <returns>A DateOnly object representing the converted date.</returns>
    public static DateOnly ToDateOnly(int year, int month, int day)
    {
        return DateOnly.FromDateTime(Converter.ConvertToGregorian(year, month, day, 0, 0, 0));
    }

}