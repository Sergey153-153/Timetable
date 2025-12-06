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
            this.lblSubject = new System.Windows.Forms.Label();
            this.lblTime = new System.Windows.Forms.Label();
            this.lblTeacher = new System.Windows.Forms.Label();
            this.lblTasksTitle = new System.Windows.Forms.Label();
            this.btnAddHomework = new System.Windows.Forms.Button();
            this.content_block = new System.Windows.Forms.FlowLayoutPanel();
            this.btnMove = new System.Windows.Forms.Button();
            this.btnDelete = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(134, 538);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(91, 58);
            this.button1.TabIndex = 0;
            this.button1.Text = "расписание";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // lblSubject
            // 
            this.lblSubject.AllowDrop = true;
            this.lblSubject.AutoSize = true;
            this.lblSubject.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lblSubject.Location = new System.Drawing.Point(141, 17);
            this.lblSubject.MaximumSize = new System.Drawing.Size(335, 0);
            this.lblSubject.Name = "lblSubject";
            this.lblSubject.Size = new System.Drawing.Size(59, 25);
            this.lblSubject.TabIndex = 1;
            this.lblSubject.Text = "АИС";
            this.lblSubject.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblSubject.UseWaitCursor = true;
            // 
            // lblTime
            // 
            this.lblTime.AutoSize = true;
            this.lblTime.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lblTime.Location = new System.Drawing.Point(61, 75);
            this.lblTime.Name = "lblTime";
            this.lblTime.Size = new System.Drawing.Size(232, 20);
            this.lblTime.TabIndex = 2;
            this.lblTime.Text = "1 неделя, суббота: 9:00–10:30";
            // 
            // lblTeacher
            // 
            this.lblTeacher.AutoSize = true;
            this.lblTeacher.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lblTeacher.Location = new System.Drawing.Point(82, 104);
            this.lblTeacher.Name = "lblTeacher";
            this.lblTeacher.Size = new System.Drawing.Size(192, 16);
            this.lblTeacher.TabIndex = 3;
            this.lblTeacher.Text = "Пуцко Николай Николаевич";
            // 
            // lblTasksTitle
            // 
            this.lblTasksTitle.AutoSize = true;
            this.lblTasksTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lblTasksTitle.Location = new System.Drawing.Point(96, 143);
            this.lblTasksTitle.Name = "lblTasksTitle";
            this.lblTasksTitle.Size = new System.Drawing.Size(80, 20);
            this.lblTasksTitle.TabIndex = 4;
            this.lblTasksTitle.Text = "ДЗ и Кр:";
            // 
            // btnAddHomework
            // 
            this.btnAddHomework.Location = new System.Drawing.Point(200, 139);
            this.btnAddHomework.Name = "btnAddHomework";
            this.btnAddHomework.Size = new System.Drawing.Size(30, 30);
            this.btnAddHomework.TabIndex = 5;
            this.btnAddHomework.Text = "+";
            this.btnAddHomework.UseVisualStyleBackColor = true;
            this.btnAddHomework.Click += new System.EventHandler(this.btnAddHomework_Click);
            // 
            // content_block
            // 
            this.content_block.AutoScroll = true;
            this.content_block.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.content_block.Location = new System.Drawing.Point(12, 190);
            this.content_block.Name = "content_block";
            this.content_block.Size = new System.Drawing.Size(335, 325);
            this.content_block.TabIndex = 6;
            this.content_block.WrapContents = false;
            // 
            // btnMove
            // 
            this.btnMove.Location = new System.Drawing.Point(34, 538);
            this.btnMove.Name = "btnMove";
            this.btnMove.Size = new System.Drawing.Size(79, 58);
            this.btnMove.TabIndex = 7;
            this.btnMove.Text = "Перенести";
            this.btnMove.UseVisualStyleBackColor = true;
            this.btnMove.Click += new System.EventHandler(this.btnMove_Click);
            // 
            // btnDelete
            // 
            this.btnDelete.Location = new System.Drawing.Point(244, 538);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(79, 58);
            this.btnDelete.TabIndex = 8;
            this.btnDelete.Text = "Удалить";
            this.btnDelete.UseVisualStyleBackColor = true;
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // MoreLesson
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(359, 609);
            this.ControlBox = false;
            this.Controls.Add(this.btnDelete);
            this.Controls.Add(this.btnMove);
            this.Controls.Add(this.content_block);
            this.Controls.Add(this.btnAddHomework);
            this.Controls.Add(this.lblTasksTitle);
            this.Controls.Add(this.lblTeacher);
            this.Controls.Add(this.lblTime);
            this.Controls.Add(this.lblSubject);
            this.Controls.Add(this.button1);
            this.Name = "MoreLesson";
            this.Text = "ш";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label lblSubject;
        private System.Windows.Forms.Label lblTime;
        private System.Windows.Forms.Label lblTeacher;
        private System.Windows.Forms.Label lblTasksTitle;
        private System.Windows.Forms.Button btnAddHomework;
        private System.Windows.Forms.FlowLayoutPanel content_block;
        private System.Windows.Forms.Button btnMove;
        private System.Windows.Forms.Button btnDelete;
    }
}