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
            this.buttonSchelude = new System.Windows.Forms.Button();
            this.buttonSetting = new System.Windows.Forms.Button();
            this.buttonAddTask = new System.Windows.Forms.Button();
            this.content_block = new System.Windows.Forms.Panel();
            this.SuspendLayout();
            // 
            // buttonSchelude
            // 
            this.buttonSchelude.Location = new System.Drawing.Point(51, 828);
            this.buttonSchelude.Name = "buttonSchelude";
            this.buttonSchelude.Size = new System.Drawing.Size(118, 89);
            this.buttonSchelude.TabIndex = 1;
            this.buttonSchelude.Text = "Расписание";
            this.buttonSchelude.UseVisualStyleBackColor = true;
            this.buttonSchelude.Click += new System.EventHandler(this.buttonSchelude_Click);
            // 
            // buttonSetting
            // 
            this.buttonSetting.Location = new System.Drawing.Point(366, 828);
            this.buttonSetting.Name = "buttonSetting";
            this.buttonSetting.Size = new System.Drawing.Size(118, 89);
            this.buttonSetting.TabIndex = 2;
            this.buttonSetting.Text = "Настройки";
            this.buttonSetting.UseVisualStyleBackColor = true;
            this.buttonSetting.Click += new System.EventHandler(this.buttonSetting_Click);
            // 
            // buttonAddTask
            // 
            this.buttonAddTask.Font = new System.Drawing.Font("Microsoft Sans Serif", 20F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.buttonAddTask.Location = new System.Drawing.Point(228, 828);
            this.buttonAddTask.Name = "buttonAddTask";
            this.buttonAddTask.Size = new System.Drawing.Size(84, 89);
            this.buttonAddTask.TabIndex = 3;
            this.buttonAddTask.Text = "+";
            this.buttonAddTask.UseVisualStyleBackColor = true;
            // 
            // content_block
            // 
            this.content_block.Location = new System.Drawing.Point(9, 11);
            this.content_block.Name = "content_block";
            this.content_block.Size = new System.Drawing.Size(518, 789);
            this.content_block.TabIndex = 4;
            // 
            // Tasks
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(538, 937);
            this.Controls.Add(this.content_block);
            this.Controls.Add(this.buttonAddTask);
            this.Controls.Add(this.buttonSetting);
            this.Controls.Add(this.buttonSchelude);
            this.Name = "Tasks";
            this.Text = "Tasks";
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Button buttonSchelude;
        private System.Windows.Forms.Button buttonSetting;
        private System.Windows.Forms.Button buttonAddTask;
        private System.Windows.Forms.Panel content_block;
    }
}