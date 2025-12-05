using System;

namespace SQLiteProject
{
    public class Holiday
    {
        public string Name { get; set; }
        public string Date { get; set; } // Формат: "01.01" для ежегодных или "2025.01.01" для конкретных дат
        public bool IsAnnual { get; set; }
        public string Description { get; set; }

        // Свойство для отображения
        public string DisplayDate
        {
            get
            {
                if (IsAnnual)
                    return $"Ежегодно, {Date}";
                else
                    return Date;
            }
        }

        // Метод для получения даты (опционально)
        public DateTime? GetDateForYear(int year)
        {
            try
            {
                if (IsAnnual)
                {
                    // Для ежегодных праздников: "01.01" + год
                    return DateTime.ParseExact($"{Date}.{year}", "dd.MM.yyyy", null);
                }
                else
                {
                    // Для конкретных дат: "2025.01.01"
                    return DateTime.ParseExact(Date, "yyyy.MM.dd", null);
                }
            }
            catch
            {
                return null;
            }
        }
    }
}
