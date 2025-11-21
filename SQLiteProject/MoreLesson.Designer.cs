namespace SQLiteProject
{
    partial class MoreLesson
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
            this.button1 = new System.Windows.Forms.Button();
            this.lblSubjectName = new System.Windows.Forms.Label();
            this.lblLessonInfo = new System.Windows.Forms.Label();
            this.lblTeacher = new System.Windows.Forms.Label();
            this.lblTasksTitle = new System.Windows.Forms.Label();
            this.btnAddHomework = new System.Windows.Forms.Button();
            this.flpTasks = new System.Windows.Forms.FlowLayoutPanel();
            this.btnMove = new System.Windows.Forms.Button();
            this.btnDelete = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(144, 582);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 0;
            this.button1.Text = "расписание";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // lblSubjectName
            // 
            this.lblSubjectName.AutoEllipsis = true;
            this.lblSubjectName.AutoSize = true;
            this.lblSubjectName.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lblSubjectName.Location = new System.Drawing.Point(148, 23);
            this.lblSubjectName.Name = "lblSubjectName";
            this.lblSubjectName.Size = new System.Drawing.Size(59, 25);
            this.lblSubjectName.TabIndex = 1;
            this.lblSubjectName.Text = "АИС";
            this.lblSubjectName.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.lblSubjectName.UseWaitCursor = true;
            // 
            // lblLessonInfo
            // 
            this.lblLessonInfo.AutoSize = true;
            this.lblLessonInfo.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lblLessonInfo.Location = new System.Drawing.Point(61, 65);
            this.lblLessonInfo.Name = "lblLessonInfo";
            this.lblLessonInfo.Size = new System.Drawing.Size(232, 20);
            this.lblLessonInfo.TabIndex = 2;
            this.lblLessonInfo.Text = "1 неделя, суббота: 9:00–10:30";
            // 
            // lblTeacher
            // 
            this.lblTeacher.AutoSize = true;
            this.lblTeacher.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lblTeacher.Location = new System.Drawing.Point(82, 102);
            this.lblTeacher.Name = "lblTeacher";
            this.lblTeacher.Size = new System.Drawing.Size(192, 16);
            this.lblTeacher.TabIndex = 3;
            this.lblTeacher.Text = "Пуцко Николай Николаевич";
            // 
            // lblTasksTitle
            // 
            this.lblTasksTitle.AutoSize = true;
            this.lblTasksTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lblTasksTitle.Location = new System.Drawing.Point(96, 141);
            this.lblTasksTitle.Name = "lblTasksTitle";
            this.lblTasksTitle.Size = new System.Drawing.Size(80, 20);
            this.lblTasksTitle.TabIndex = 4;
            this.lblTasksTitle.Text = "ДЗ и Кр:";
            // 
            // btnAddHomework
            // 
            this.btnAddHomework.Location = new System.Drawing.Point(200, 137);
            this.btnAddHomework.Name = "btnAddHomework";
            this.btnAddHomework.Size = new System.Drawing.Size(30, 30);
            this.btnAddHomework.TabIndex = 5;
            this.btnAddHomework.Text = "+";
            this.btnAddHomework.UseVisualStyleBackColor = true;
            // 
            // flpTasks
            // 
            this.flpTasks.AutoScroll = true;
            this.flpTasks.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flpTasks.Location = new System.Drawing.Point(12, 190);
            this.flpTasks.Name = "flpTasks";
            this.flpTasks.Size = new System.Drawing.Size(335, 348);
            this.flpTasks.TabIndex = 6;
            this.flpTasks.WrapContents = false;
            // 
            // btnMove
            // 
            this.btnMove.Location = new System.Drawing.Point(12, 582);
            this.btnMove.Name = "btnMove";
            this.btnMove.Size = new System.Drawing.Size(75, 23);
            this.btnMove.TabIndex = 7;
            this.btnMove.Text = "Перенести";
            this.btnMove.UseVisualStyleBackColor = true;
            this.btnMove.Click += new System.EventHandler(this.btnMove_Click);
            // 
            // btnDelete
            // 
            this.btnDelete.Location = new System.Drawing.Point(272, 582);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(75, 23);
            this.btnDelete.TabIndex = 8;
            this.btnDelete.Text = "Отмена";
            this.btnDelete.UseVisualStyleBackColor = true;
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // Form2
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(359, 628);
            this.Controls.Add(this.btnDelete);
            this.Controls.Add(this.btnMove);
            this.Controls.Add(this.flpTasks);
            this.Controls.Add(this.btnAddHomework);
            this.Controls.Add(this.lblTasksTitle);
            this.Controls.Add(this.lblTeacher);
            this.Controls.Add(this.lblLessonInfo);
            this.Controls.Add(this.lblSubjectName);
            this.Controls.Add(this.button1);
            this.Name = "Form2";
            this.Text = "Экран пары";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label lblSubjectName;
        private System.Windows.Forms.Label lblLessonInfo;
        private System.Windows.Forms.Label lblTeacher;
        private System.Windows.Forms.Label lblTasksTitle;
        private System.Windows.Forms.Button btnAddHomework;
        private System.Windows.Forms.FlowLayoutPanel flpTasks;
        private System.Windows.Forms.Button btnMove;
        private System.Windows.Forms.Button btnDelete;
    }
}