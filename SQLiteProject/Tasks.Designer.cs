namespace SQLiteProject
{
    partial class Tasks
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
            this.label_task = new System.Windows.Forms.Label();
            this.task_schelude = new System.Windows.Forms.Button();
            this.task_add = new System.Windows.Forms.Button();
            this.task_settings = new System.Windows.Forms.Button();
            this.content_block = new System.Windows.Forms.Panel();
            this.screen_tasks.SuspendLayout();
            this.SuspendLayout();
            // 
            // screen_tasks
            // 
            this.screen_tasks.Controls.Add(this.label_task);
            this.screen_tasks.Location = new System.Drawing.Point(8, 11);
            this.screen_tasks.Margin = new System.Windows.Forms.Padding(2);
            this.screen_tasks.Name = "screen_tasks";
            this.screen_tasks.Size = new System.Drawing.Size(343, 51);
            this.screen_tasks.TabIndex = 0;
            // 
            // label_task
            // 
            this.label_task.AutoSize = true;
            this.label_task.Font = new System.Drawing.Font("Arial Narrow", 16F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label_task.Location = new System.Drawing.Point(132, 15);
            this.label_task.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label_task.Name = "label_task";
            this.label_task.Size = new System.Drawing.Size(80, 26);
            this.label_task.TabIndex = 0;
            this.label_task.Text = "ДЗ и КР";
            // 
            // task_schelude
            // 
            this.task_schelude.Font = new System.Drawing.Font("Arial Narrow", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.task_schelude.Location = new System.Drawing.Point(34, 538);
            this.task_schelude.Margin = new System.Windows.Forms.Padding(2);
            this.task_schelude.Name = "task_schelude";
            this.task_schelude.Size = new System.Drawing.Size(79, 58);
            this.task_schelude.TabIndex = 1;
            this.task_schelude.Text = "Расписание";
            this.task_schelude.UseVisualStyleBackColor = true;
            this.task_schelude.Click += new System.EventHandler(this.task_schelude_Click);
            // 
            // task_add
            // 
            this.task_add.Font = new System.Drawing.Font("Microsoft Sans Serif", 20F);
            this.task_add.Location = new System.Drawing.Point(152, 538);
            this.task_add.Margin = new System.Windows.Forms.Padding(2);
            this.task_add.Name = "task_add";
            this.task_add.Size = new System.Drawing.Size(56, 58);
            this.task_add.TabIndex = 2;
            this.task_add.Text = "+";
            this.task_add.UseVisualStyleBackColor = true;
            this.task_add.Click += new System.EventHandler(this.task_add_Click);
            // 
            // task_settings
            // 
            this.task_settings.Font = new System.Drawing.Font("Arial Narrow", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.task_settings.Location = new System.Drawing.Point(244, 538);
            this.task_settings.Margin = new System.Windows.Forms.Padding(2);
            this.task_settings.Name = "task_settings";
            this.task_settings.Size = new System.Drawing.Size(79, 58);
            this.task_settings.TabIndex = 3;
            this.task_settings.Text = "Настройки";
            this.task_settings.UseVisualStyleBackColor = true;
            this.task_settings.Click += new System.EventHandler(this.task_settings_Click);
            // 
            // content_block
            // 
            this.content_block.AutoScroll = true;
            this.content_block.Font = new System.Drawing.Font("Arial Narrow", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.content_block.Location = new System.Drawing.Point(8, 74);
            this.content_block.Margin = new System.Windows.Forms.Padding(7, 6, 7, 6);
            this.content_block.Name = "content_block";
            this.content_block.Size = new System.Drawing.Size(343, 444);
            this.content_block.TabIndex = 4;
            // 
            // Tasks
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(359, 609);
            this.Controls.Add(this.screen_tasks);
            this.Controls.Add(this.content_block);
            this.Controls.Add(this.task_settings);
            this.Controls.Add(this.task_add);
            this.Controls.Add(this.task_schelude);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.MaximizeBox = false;
            this.Name = "Tasks";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Tasks";
            this.screen_tasks.ResumeLayout(false);
            this.screen_tasks.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel screen_tasks;
        private System.Windows.Forms.Label label_task;
        private System.Windows.Forms.Button task_schelude;
        private System.Windows.Forms.Button task_add;
        private System.Windows.Forms.Button task_settings;
        private System.Windows.Forms.Panel content_block;
    }
}