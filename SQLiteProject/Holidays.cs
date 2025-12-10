using System;

namespace SQLiteProject
{
    public class Holiday
    {
        public int HolidayID { get; set; }
        public string Name { get; set; }
        public string DateValue { get; set; }  // "01.01" или "2025.01.01"
        public bool IsAnnual { get; set; }
        public string Description { get; set; }

        public DateTime? GetDate(int year)
        {
            try
            {
                if (IsAnnual)
                    return DateTime.ParseExact($"{DateValue}.{year}", "dd.MM.yyyy", null);

                return DateTime.ParseExact(DateValue, "yyyy.MM.dd", null);
            }
            catch { return null; }
        }
    }
}
