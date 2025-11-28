using mySQLite;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SQLite;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SQLiteProject
{
    public partial class LoginForm : Form
    {
        private SQLiteQueries _sqlQueries;

        public LoginForm()
        {
            InitializeComponent();
            _sqlQueries = new SQLiteQueries("Timetable.db");

            try
            {
                _sqlQueries.CreateTables("Timetable.db");
               

                _sqlQueries.AddUser("1234567890");

                // ДОБАВЬ ПРОВЕРКУ СРАЗУ:
                bool check = _sqlQueries.CheckUserExists("1234567890");
               
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при создании БД: {ex.Message}");
            }
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            string phoneNumber = txtPhoneNumber.Text.Trim();

            if (string.IsNullOrEmpty(phoneNumber))
            {
                MessageBox.Show("Пожалуйста, введите номер телефона", "Ошибка",
                              MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }


            if (_sqlQueries.CheckUserExists(phoneNumber))
            {
                MessageBox.Show("Вход выполнен успешно!", "Успех",
                              MessageBoxButtons.OK, MessageBoxIcon.Information);

                Form1 mainForm = new Form1();
                mainForm.Show();
                this.Hide();
            }
            else
            {
                MessageBox.Show("Неверный номер телефона. Попробуйте еще раз.", "Ошибка",
                              MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtPhoneNumber.Focus();
                txtPhoneNumber.SelectAll();
            }
        }
    }
}
