namespace SQLiteProject
{
    partial class Form1
    {
        /// <summary>
        /// Требуется переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Обязательный метод для поддержки конструктора - не изменяйте
        /// содержимое данного метода при помощи редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.buttonTasks = new System.Windows.Forms.Button();
            this.buttonAddLesson = new System.Windows.Forms.Button();
            this.buttonSettings = new System.Windows.Forms.Button();
            this.dateTimePicker1 = new System.Windows.Forms.DateTimePicker();
            this.panelLessons = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // buttonTasks
            // 
            this.buttonTasks.Location = new System.Drawing.Point(48, 662);
            this.buttonTasks.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.buttonTasks.Name = "buttonTasks";
            this.buttonTasks.Size = new System.Drawing.Size(75, 71);
            this.buttonTasks.TabIndex = 8;
            this.buttonTasks.Text = "Задания";
            this.buttonTasks.UseVisualStyleBackColor = true;
            this.buttonTasks.Click += new System.EventHandler(this.buttonTasks_Click);
            // 
            // buttonAddLesson
            // 
            this.buttonAddLesson.Location = new System.Drawing.Point(203, 662);
            this.buttonAddLesson.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.buttonAddLesson.Name = "buttonAddLesson";
            this.buttonAddLesson.Size = new System.Drawing.Size(75, 71);
            this.buttonAddLesson.TabIndex = 9;
            this.buttonAddLesson.Text = "+";
            this.buttonAddLesson.UseVisualStyleBackColor = true;
            this.buttonAddLesson.Click += new System.EventHandler(this.buttonAddLesson_Click);
            // 
            // buttonSettings
            // 
            this.buttonSettings.Location = new System.Drawing.Point(356, 662);
            this.buttonSettings.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.buttonSettings.Name = "buttonSettings";
            this.buttonSettings.Size = new System.Drawing.Size(75, 71);
            this.buttonSettings.TabIndex = 10;
            this.buttonSettings.Text = "Настройки";
            this.buttonSettings.UseVisualStyleBackColor = true;
            this.buttonSettings.Click += new System.EventHandler(this.buttonSettings_Click);
            // 
            // dateTimePicker1
            // 
            this.dateTimePicker1.Location = new System.Drawing.Point(13, 16);
            this.dateTimePicker1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.dateTimePicker1.Name = "dateTimePicker1";
            this.dateTimePicker1.Size = new System.Drawing.Size(200, 22);
            this.dateTimePicker1.TabIndex = 13;
            this.dateTimePicker1.ValueChanged += new System.EventHandler(this.dateTimePicker1_ValueChanged);
            // 
            // panelLessons
            // 
            this.panelLessons.Location = new System.Drawing.Point(13, 121);
            this.panelLessons.Margin = new System.Windows.Forms.Padding(4);
            this.panelLessons.Name = "panelLessons";
            this.panelLessons.Size = new System.Drawing.Size(451, 506);
            this.panelLessons.TabIndex = 14;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(309, 16);
            this.label1.MinimumSize = new System.Drawing.Size(20, 20);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(20, 20);
            this.label1.TabIndex = 0;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(479, 750);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.panelLessons);
            this.Controls.Add(this.dateTimePicker1);
            this.Controls.Add(this.buttonSettings);
            this.Controls.Add(this.buttonAddLesson);
            this.Controls.Add(this.buttonTasks);
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "Form1";
            this.Text = "Form1";
            this.Shown += new System.EventHandler(this.Form1_Shown);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button buttonTasks;
        private System.Windows.Forms.Button buttonAddLesson;
        private System.Windows.Forms.Button buttonSettings;
        private System.Windows.Forms.DateTimePicker dateTimePicker1;
        private System.Windows.Forms.Panel panelLessons;
        private System.Windows.Forms.Label label1;
    }
}

