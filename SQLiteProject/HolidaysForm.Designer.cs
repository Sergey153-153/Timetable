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
            this.listBoxHolidays = new System.Windows.Forms.ListBox();
            this.SuspendLayout();
            // 
            // listBoxHolidays
            // 
            this.listBoxHolidays.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listBoxHolidays.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.listBoxHolidays.FormattingEnabled = true;
            this.listBoxHolidays.ItemHeight = 16;
            this.listBoxHolidays.Location = new System.Drawing.Point(0, 0);
            this.listBoxHolidays.Name = "listBoxHolidays";
            this.listBoxHolidays.Size = new System.Drawing.Size(394, 341);
            this.listBoxHolidays.TabIndex = 0;
            // 
            // HolidaysForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(394, 341);
            this.Controls.Add(this.listBoxHolidays);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "HolidaysForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Список праздников";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListBox listBoxHolidays;
    }
}