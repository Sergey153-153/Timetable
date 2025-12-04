namespace SQLiteProject
{
    partial class AddTask
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
            this.groupBoxTaskType = new System.Windows.Forms.GroupBox();
            this.radioButtonExam = new System.Windows.Forms.RadioButton();
            this.radioButtonTest = new System.Windows.Forms.RadioButton();
            this.radioButtonKR = new System.Windows.Forms.RadioButton();
            this.radioButtonDZ = new System.Windows.Forms.RadioButton();
            this.labelSubject = new System.Windows.Forms.Label();
            this.dateTimePicker1 = new System.Windows.Forms.DateTimePicker();
            this.labelDate = new System.Windows.Forms.Label();
            this.textBoxAbout = new System.Windows.Forms.TextBox();
            this.labelAbout = new System.Windows.Forms.Label();
            this.buttonAddFile = new System.Windows.Forms.Button();
            this.buttonAddTask = new System.Windows.Forms.Button();
            this.groupBoxTaskType.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBoxTaskType
            // 
            this.groupBoxTaskType.Controls.Add(this.radioButtonExam);
            this.groupBoxTaskType.Controls.Add(this.radioButtonTest);
            this.groupBoxTaskType.Controls.Add(this.radioButtonKR);
            this.groupBoxTaskType.Controls.Add(this.radioButtonDZ);
            this.groupBoxTaskType.Location = new System.Drawing.Point(12, 12);
            this.groupBoxTaskType.Name = "groupBoxTaskType";
            this.groupBoxTaskType.Size = new System.Drawing.Size(363, 146);
            this.groupBoxTaskType.TabIndex = 0;
            this.groupBoxTaskType.TabStop = false;
            this.groupBoxTaskType.Text = "Выберите тип задания:";
            // 
            // radioButtonExam
            // 
            this.radioButtonExam.AutoSize = true;
            this.radioButtonExam.Location = new System.Drawing.Point(6, 113);
            this.radioButtonExam.Name = "radioButtonExam";
            this.radioButtonExam.Size = new System.Drawing.Size(100, 24);
            this.radioButtonExam.TabIndex = 3;
            this.radioButtonExam.TabStop = true;
            this.radioButtonExam.Text = "Экзамен";
            this.radioButtonExam.UseVisualStyleBackColor = true;
            // 
            // radioButtonTest
            // 
            this.radioButtonTest.AutoSize = true;
            this.radioButtonTest.Location = new System.Drawing.Point(6, 85);
            this.radioButtonTest.Name = "radioButtonTest";
            this.radioButtonTest.Size = new System.Drawing.Size(81, 24);
            this.radioButtonTest.TabIndex = 2;
            this.radioButtonTest.TabStop = true;
            this.radioButtonTest.Text = "Зачет";
            this.radioButtonTest.UseVisualStyleBackColor = true;
            // 
            // radioButtonKR
            // 
            this.radioButtonKR.AutoSize = true;
            this.radioButtonKR.Location = new System.Drawing.Point(6, 55);
            this.radioButtonKR.Name = "radioButtonKR";
            this.radioButtonKR.Size = new System.Drawing.Size(54, 24);
            this.radioButtonKR.TabIndex = 1;
            this.radioButtonKR.TabStop = true;
            this.radioButtonKR.Text = "КР";
            this.radioButtonKR.UseVisualStyleBackColor = true;
            // 
            // radioButtonDZ
            // 
            this.radioButtonDZ.AutoSize = true;
            this.radioButtonDZ.Location = new System.Drawing.Point(6, 25);
            this.radioButtonDZ.Name = "radioButtonDZ";
            this.radioButtonDZ.Size = new System.Drawing.Size(57, 24);
            this.radioButtonDZ.TabIndex = 0;
            this.radioButtonDZ.TabStop = true;
            this.radioButtonDZ.Text = "ДЗ";
            this.radioButtonDZ.UseVisualStyleBackColor = true;
            // 
            // labelSubject
            // 
            this.labelSubject.AutoSize = true;
            this.labelSubject.Location = new System.Drawing.Point(12, 176);
            this.labelSubject.Name = "labelSubject";
            this.labelSubject.Size = new System.Drawing.Size(160, 20);
            this.labelSubject.TabIndex = 2;
            this.labelSubject.Text = "Выберите предмет:";
            // 
            // dateTimePicker1
            // 
            this.dateTimePicker1.Location = new System.Drawing.Point(12, 263);
            this.dateTimePicker1.Name = "dateTimePicker1";
            this.dateTimePicker1.Size = new System.Drawing.Size(363, 26);
            this.dateTimePicker1.TabIndex = 3;
            // 
            // labelDate
            // 
            this.labelDate.AutoSize = true;
            this.labelDate.Location = new System.Drawing.Point(12, 240);
            this.labelDate.Name = "labelDate";
            this.labelDate.Size = new System.Drawing.Size(100, 20);
            this.labelDate.TabIndex = 4;
            this.labelDate.Text = "Срок сдачи:";
            // 
            // textBoxAbout
            // 
            this.textBoxAbout.Location = new System.Drawing.Point(12, 331);
            this.textBoxAbout.Name = "textBoxAbout";
            this.textBoxAbout.Size = new System.Drawing.Size(362, 26);
            this.textBoxAbout.TabIndex = 5;
            // 
            // labelAbout
            // 
            this.labelAbout.AutoSize = true;
            this.labelAbout.Location = new System.Drawing.Point(12, 308);
            this.labelAbout.Name = "labelAbout";
            this.labelAbout.Size = new System.Drawing.Size(87, 20);
            this.labelAbout.TabIndex = 6;
            this.labelAbout.Text = "Описание:";
            // 
            // buttonAddFile
            // 
            this.buttonAddFile.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.buttonAddFile.Location = new System.Drawing.Point(12, 380);
            this.buttonAddFile.Name = "buttonAddFile";
            this.buttonAddFile.Size = new System.Drawing.Size(190, 48);
            this.buttonAddFile.TabIndex = 7;
            this.buttonAddFile.Text = "Прикрепить файл";
            this.buttonAddFile.UseVisualStyleBackColor = true;
            // 
            // buttonAddTask
            // 
            this.buttonAddTask.Location = new System.Drawing.Point(151, 638);
            this.buttonAddTask.Name = "buttonAddTask";
            this.buttonAddTask.Size = new System.Drawing.Size(203, 50);
            this.buttonAddTask.TabIndex = 8;
            this.buttonAddTask.Text = "Добавить задание";
            this.buttonAddTask.UseVisualStyleBackColor = true;
            // 
            // AddTask
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(538, 731);
            this.Controls.Add(this.buttonAddTask);
            this.Controls.Add(this.buttonAddFile);
            this.Controls.Add(this.labelAbout);
            this.Controls.Add(this.textBoxAbout);
            this.Controls.Add(this.labelDate);
            this.Controls.Add(this.dateTimePicker1);
            this.Controls.Add(this.labelSubject);
            this.Controls.Add(this.groupBoxTaskType);
            this.Name = "AddTask";
            this.Text = "AddTask";
            this.groupBoxTaskType.ResumeLayout(false);
            this.groupBoxTaskType.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBoxTaskType;
        private System.Windows.Forms.RadioButton radioButtonExam;
        private System.Windows.Forms.RadioButton radioButtonTest;
        private System.Windows.Forms.RadioButton radioButtonKR;
        private System.Windows.Forms.RadioButton radioButtonDZ;
        private System.Windows.Forms.Label labelSubject;
        private System.Windows.Forms.DateTimePicker dateTimePicker1;
        private System.Windows.Forms.Label labelDate;
        private System.Windows.Forms.TextBox textBoxAbout;
        private System.Windows.Forms.Label labelAbout;
        private System.Windows.Forms.Button buttonAddFile;
        private System.Windows.Forms.Button buttonAddTask;
    }
}