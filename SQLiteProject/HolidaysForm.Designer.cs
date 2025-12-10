namespace SQLiteProject
{
    partial class HolidaysForm
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
            this.back = new System.Windows.Forms.Button();
            this.task_schelude = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label_task = new System.Windows.Forms.Label();
            this.screen_tasks = new System.Windows.Forms.Panel();
            this.screen_tasks.SuspendLayout();
            this.SuspendLayout();
            // 
            // back
            // 
            this.back.Font = new System.Drawing.Font("Arial Narrow", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.back.Location = new System.Drawing.Point(224, 538);
            this.back.Margin = new System.Windows.Forms.Padding(2);
            this.back.Name = "back";
            this.back.Size = new System.Drawing.Size(79, 58);
            this.back.TabIndex = 4;
            this.back.Text = "Назад";
            this.back.UseVisualStyleBackColor = true;
            this.back.Click += new System.EventHandler(this.back_Click);
            // 
            // task_schelude
            // 
            this.task_schelude.Font = new System.Drawing.Font("Arial Narrow", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.task_schelude.Location = new System.Drawing.Point(54, 538);
            this.task_schelude.Margin = new System.Windows.Forms.Padding(2);
            this.task_schelude.Name = "task_schelude";
            this.task_schelude.Size = new System.Drawing.Size(79, 58);
            this.task_schelude.TabIndex = 5;
            this.task_schelude.Text = "Добавить";
            this.task_schelude.UseVisualStyleBackColor = true;
            this.task_schelude.Click += new System.EventHandler(this.add_schelude_Click);
            // 
            // panel1
            // 
            this.panel1.AutoScroll = true;
            this.panel1.Location = new System.Drawing.Point(8, 87);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(343, 428);
            this.panel1.TabIndex = 7;
            // 
            // label_task
            // 
            this.label_task.Font = new System.Drawing.Font("Arial Narrow", 16F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label_task.Location = new System.Drawing.Point(0, 13);
            this.label_task.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label_task.Name = "label_task";
            this.label_task.Size = new System.Drawing.Size(343, 26);
            this.label_task.TabIndex = 0;
            this.label_task.Text = "Праздники";
            this.label_task.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // screen_tasks
            // 
            this.screen_tasks.Controls.Add(this.label_task);
            this.screen_tasks.Location = new System.Drawing.Point(8, 11);
            this.screen_tasks.Margin = new System.Windows.Forms.Padding(2);
            this.screen_tasks.Name = "screen_tasks";
            this.screen_tasks.Size = new System.Drawing.Size(343, 51);
            this.screen_tasks.TabIndex = 6;
            // 
            // HolidaysForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(359, 609);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.screen_tasks);
            this.Controls.Add(this.task_schelude);
            this.Controls.Add(this.back);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "HolidaysForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Список праздников";
            this.screen_tasks.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button back;
        private System.Windows.Forms.Button task_schelude;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label_task;
        private System.Windows.Forms.Panel screen_tasks;
    }
}