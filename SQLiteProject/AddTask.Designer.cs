namespace SQLiteProject
{
    partial class Add_task
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.screen_tasks = new System.Windows.Forms.Panel();
            this.label_add_task = new System.Windows.Forms.Label();
            this.Task_type = new System.Windows.Forms.GroupBox();
            this.Exam = new System.Windows.Forms.RadioButton();
            this.Test = new System.Windows.Forms.RadioButton();
            this.Control = new System.Windows.Forms.RadioButton();
            this.HomeTask = new System.Windows.Forms.RadioButton();
            this.Lesson_type = new System.Windows.Forms.ComboBox();
            this.dateTimePicker1 = new System.Windows.Forms.DateTimePicker();
            this.Add_files = new System.Windows.Forms.Button();
            this.back_tasks = new System.Windows.Forms.Button();
            this.textBox_description = new System.Windows.Forms.TextBox();
            this.labelSubject = new System.Windows.Forms.Label();
            this.labelAbout = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.screen_tasks.SuspendLayout();
            this.Task_type.SuspendLayout();
            this.SuspendLayout();
            // 
            // screen_tasks
            // 
            this.screen_tasks.Controls.Add(this.label_add_task);
            this.screen_tasks.Location = new System.Drawing.Point(24, 12);
            this.screen_tasks.Name = "screen_tasks";
            this.screen_tasks.Size = new System.Drawing.Size(488, 78);
            this.screen_tasks.TabIndex = 1;
            // 
            // label_add_task
            // 
            this.label_add_task.AutoSize = true;
            this.label_add_task.Font = new System.Drawing.Font("Arial Narrow", 16F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label_add_task.Location = new System.Drawing.Point(116, 22);
            this.label_add_task.Name = "label_add_task";
            this.label_add_task.Size = new System.Drawing.Size(258, 37);
            this.label_add_task.TabIndex = 0;
            this.label_add_task.Text = "Добавить задание";
            // 
            // Task_type
            // 
            this.Task_type.Controls.Add(this.Exam);
            this.Task_type.Controls.Add(this.Test);
            this.Task_type.Controls.Add(this.Control);
            this.Task_type.Controls.Add(this.HomeTask);
            this.Task_type.Location = new System.Drawing.Point(24, 95);
            this.Task_type.Name = "Task_type";
            this.Task_type.Size = new System.Drawing.Size(488, 165);
            this.Task_type.TabIndex = 2;
            this.Task_type.TabStop = false;
            this.Task_type.Text = "Выберите тип задания:";
            // 
            // Exam
            // 
            this.Exam.AutoSize = true;
            this.Exam.Location = new System.Drawing.Point(6, 128);
            this.Exam.Name = "Exam";
            this.Exam.Size = new System.Drawing.Size(100, 24);
            this.Exam.TabIndex = 3;
            this.Exam.TabStop = true;
            this.Exam.Text = "Экзамен";
            this.Exam.UseVisualStyleBackColor = true;
            // 
            // Test
            // 
            this.Test.AutoSize = true;
            this.Test.Location = new System.Drawing.Point(6, 97);
            this.Test.Name = "Test";
            this.Test.Size = new System.Drawing.Size(81, 24);
            this.Test.TabIndex = 2;
            this.Test.TabStop = true;
            this.Test.Text = "Зачет";
            this.Test.UseVisualStyleBackColor = true;
            // 
            // Control
            // 
            this.Control.AutoSize = true;
            this.Control.Location = new System.Drawing.Point(6, 68);
            this.Control.Name = "Control";
            this.Control.Size = new System.Drawing.Size(54, 24);
            this.Control.TabIndex = 1;
            this.Control.TabStop = true;
            this.Control.Text = "КР";
            this.Control.UseVisualStyleBackColor = true;
            // 
            // HomeTask
            // 
            this.HomeTask.AutoSize = true;
            this.HomeTask.Location = new System.Drawing.Point(6, 37);
            this.HomeTask.Name = "HomeTask";
            this.HomeTask.Size = new System.Drawing.Size(57, 24);
            this.HomeTask.TabIndex = 0;
            this.HomeTask.TabStop = true;
            this.HomeTask.Text = "ДЗ";
            this.HomeTask.UseVisualStyleBackColor = true;
            // 
            // Lesson_type
            // 
            this.Lesson_type.FormattingEnabled = true;
            this.Lesson_type.Items.AddRange(new object[] {
            "Мат. анализ",
            "ТРПО",
            "АИС",
            "ТОК",
            "История"});
            this.Lesson_type.Location = new System.Drawing.Point(24, 302);
            this.Lesson_type.Name = "Lesson_type";
            this.Lesson_type.Size = new System.Drawing.Size(486, 28);
            this.Lesson_type.TabIndex = 3;
            // 
            // dateTimePicker1
            // 
            this.dateTimePicker1.Location = new System.Drawing.Point(23, 436);
            this.dateTimePicker1.Name = "dateTimePicker1";
            this.dateTimePicker1.Size = new System.Drawing.Size(487, 26);
            this.dateTimePicker1.TabIndex = 4;
            // 
            // Add_files
            // 
            this.Add_files.Location = new System.Drawing.Point(24, 504);
            this.Add_files.Name = "Add_files";
            this.Add_files.Size = new System.Drawing.Size(488, 57);
            this.Add_files.TabIndex = 6;
            this.Add_files.Text = "Добавить файлы";
            this.Add_files.UseVisualStyleBackColor = true;
            this.Add_files.Click += new System.EventHandler(this.Add_files_Click);
            // 
            // back_tasks
            // 
            this.back_tasks.Location = new System.Drawing.Point(162, 578);
            this.back_tasks.Name = "back_tasks";
            this.back_tasks.Size = new System.Drawing.Size(212, 45);
            this.back_tasks.TabIndex = 7;
            this.back_tasks.Text = "Добавить задание";
            this.back_tasks.UseVisualStyleBackColor = true;
            this.back_tasks.Click += new System.EventHandler(this.back_tasks_Click);
            // 
            // textBox_description
            // 
            this.textBox_description.Location = new System.Drawing.Point(24, 371);
            this.textBox_description.Name = "textBox_description";
            this.textBox_description.Size = new System.Drawing.Size(486, 26);
            this.textBox_description.TabIndex = 8;
            // 
            // labelSubject
            // 
            this.labelSubject.AutoSize = true;
            this.labelSubject.Location = new System.Drawing.Point(21, 276);
            this.labelSubject.Name = "labelSubject";
            this.labelSubject.Size = new System.Drawing.Size(160, 20);
            this.labelSubject.TabIndex = 9;
            this.labelSubject.Text = "Выберите предмет:";
            // 
            // labelAbout
            // 
            this.labelAbout.AutoSize = true;
            this.labelAbout.Location = new System.Drawing.Point(21, 346);
            this.labelAbout.Name = "labelAbout";
            this.labelAbout.Size = new System.Drawing.Size(223, 20);
            this.labelAbout.TabIndex = 10;
            this.labelAbout.Text = "Введите описание задания:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(22, 413);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(100, 20);
            this.label3.TabIndex = 11;
            this.label3.Text = "Срок сдачи:";
            // 
            // Add_task
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(538, 638);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.labelAbout);
            this.Controls.Add(this.labelSubject);
            this.Controls.Add(this.textBox_description);
            this.Controls.Add(this.back_tasks);
            this.Controls.Add(this.Add_files);
            this.Controls.Add(this.dateTimePicker1);
            this.Controls.Add(this.Lesson_type);
            this.Controls.Add(this.Task_type);
            this.Controls.Add(this.screen_tasks);
            this.MaximizeBox = false;
            this.Name = "Add_task";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Add_task";
            this.screen_tasks.ResumeLayout(false);
            this.screen_tasks.PerformLayout();
            this.Task_type.ResumeLayout(false);
            this.Task_type.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel screen_tasks;
        private System.Windows.Forms.Label label_add_task;
        private System.Windows.Forms.GroupBox Task_type;
        private System.Windows.Forms.RadioButton Exam;
        private System.Windows.Forms.RadioButton Test;
        private System.Windows.Forms.RadioButton Control;
        private System.Windows.Forms.RadioButton HomeTask;
        private System.Windows.Forms.ComboBox Lesson_type;
        private System.Windows.Forms.DateTimePicker dateTimePicker1;
        private System.Windows.Forms.Button Add_files;
        private System.Windows.Forms.Button back_tasks;
        private System.Windows.Forms.TextBox textBox_description;
        private System.Windows.Forms.Label labelSubject;
        private System.Windows.Forms.Label labelAbout;
        private System.Windows.Forms.Label label3;
    }
}