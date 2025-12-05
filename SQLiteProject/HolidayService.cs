using System;
using System.Collections.Generic;

namespace SQLiteProject
{
    public class HolidayService
    {
        private List<Holiday> holidays;

        public HolidayService()
        {
            holidays = LoadHolidays();
        }

        private List<Holiday> LoadHolidays()
        {
            return new List<Holiday>
            {
                new Holiday { Name = "Новый год", Date = "01.01", IsAnnual = true },
                new Holiday { Name = "Рождество Христово", Date = "07.01", IsAnnual = true },
                new Holiday { Name = "День защитника Отечества", Date = "23.02", IsAnnual = true },
                new Holiday { Name = "Международный женский день", Date = "08.03", IsAnnual = true },
                new Holiday { Name = "Праздник Весны и Труда", Date = "01.05", IsAnnual = true },
                new Holiday { Name = "День Победы", Date = "09.05", IsAnnual = true },
                new Holiday { Name = "День России", Date = "12.06", IsAnnual = true },
                new Holiday { Name = "День народного единства", Date = "04.11", IsAnnual = true }
            };
        }

        public bool IsHoliday(DateTime date)
        {
            return GetHolidayForDate(date) != null;
        }

        public Holiday GetHolidayForDate(DateTime date)
        {
            foreach (var holiday in holidays)
            {
                if (IsDateMatch(holiday, date))
                {
                    return holiday;
                }
            }
            return null;
        }

        public string GetHolidayName(DateTime date)
        {
            var holiday = GetHolidayForDate(date);
            return holiday?.Name ?? string.Empty;
        }

        private bool IsDateMatch(Holiday holiday, DateTime date)
        {
            if (holiday.IsAnnual)
            {
                try
                {
                    string[] parts = holiday.Date.Split('.');
                    if (parts.Length == 2)
                    {
                        int day = int.Parse(parts[0]);
                        int month = int.Parse(parts[1]);
                        return date.Day == day && date.Month == month;
                    }
                }
                catch
                {
                    return false;
                }
            }
            return false;
        }
    }
}