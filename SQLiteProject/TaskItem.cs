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
        public string Type { get; set; } // "ДЗ" или "КР"
        public bool IsCompleted { get; set; }

        //отображение срока сдачи задания
        public string DeadlineDate => Deadline.ToString("dd.MM.yyyy");
        public string DeadlineDay => Deadline.ToString("dddd", new System.Globalization.CultureInfo("ru-RU"));
    }
}