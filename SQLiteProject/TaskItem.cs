using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLiteProject
{
    public class TaskItem
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime Deadline { get; set; }
        public string Type { get; set; }
        public bool IsCompleted { get; set; }

        public string SubjectName { get; set; }
        public string FilePath { get; set; }

        // Для отображения только даты 
        public string DeadlineDate => Deadline.ToString("dd.MM.yyyy");
        public string DeadlineDay => Deadline.ToString("dddd", new System.Globalization.CultureInfo("ru-RU"));
    }
}