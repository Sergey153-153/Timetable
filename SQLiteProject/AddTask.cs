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
    public partial class AddTask : Form
    {
        public AddTask()
        {
            InitializeComponent();
            InitializeCustomComponents();
        }
        private void InitializeCustomComponents()
        {
            // Настройка формы
            this.Text = "Добавить задачу";
            this.Size = new Size(375, 500);
        }

    }
}
