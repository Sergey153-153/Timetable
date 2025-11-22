using mySQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SQLiteProject
{
    static class Program
    {
        /// <summary>
        /// Главная точка входа для приложения.
        /// </summary>
        [STAThread]
        static void Main()
        {
            SQLiteQueries sql = new SQLiteQueries("mydb.sqlite");
            sql.CreateTables("mydb.sqlite");

            if (sql.GetSchedulesCount() == 0)
            {
                ScheduleInitializer.CreateDefaultSchedules(sql);
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }
}
