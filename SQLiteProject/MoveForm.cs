using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SQLiteProject
{
    public partial class MoveForm : Form
    {
        private int lessonId;
        // Новый конструктор с аргументом
        public MoveForm(int currentLessonId)
        {
            InitializeComponent();
            lessonId = currentLessonId;

            // здесь можно сразу что-то сделать с lessonId, если нужно
            // например, загрузить данные урока
        }
    }
}
