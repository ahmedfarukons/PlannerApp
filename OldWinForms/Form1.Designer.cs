namespace StudyPlanner
{
    partial class Form1
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
            this.components = new System.ComponentModel.Container();
            this.panelTop = new System.Windows.Forms.Panel();
            this.btnLoad = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnDelete = new System.Windows.Forms.Button();
            this.btnAdd = new System.Windows.Forms.Button();
            this.txtNotes = new System.Windows.Forms.TextBox();
            this.lblNotes = new System.Windows.Forms.Label();
            this.numDuration = new System.Windows.Forms.NumericUpDown();
            this.lblDuration = new System.Windows.Forms.Label();
            this.dtpDate = new System.Windows.Forms.DateTimePicker();
            this.lblDate = new System.Windows.Forms.Label();
            this.txtSubject = new System.Windows.Forms.TextBox();
            this.lblSubject = new System.Windows.Forms.Label();
            this.listPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.panelTop.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numDuration)).BeginInit();
            this.SuspendLayout();
            // 
            // panelTop
            // 
            this.panelTop.Controls.Add(this.btnLoad);
            this.panelTop.Controls.Add(this.btnSave);
            this.panelTop.Controls.Add(this.btnDelete);
            this.panelTop.Controls.Add(this.btnAdd);
            this.panelTop.Controls.Add(this.txtNotes);
            this.panelTop.Controls.Add(this.lblNotes);
            this.panelTop.Controls.Add(this.numDuration);
            this.panelTop.Controls.Add(this.lblDuration);
            this.panelTop.Controls.Add(this.dtpDate);
            this.panelTop.Controls.Add(this.lblDate);
            this.panelTop.Controls.Add(this.txtSubject);
            this.panelTop.Controls.Add(this.lblSubject);
            this.panelTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelTop.Location = new System.Drawing.Point(0, 0);
            this.panelTop.Name = "panelTop";
            this.panelTop.Size = new System.Drawing.Size(800, 140);
            this.panelTop.TabIndex = 0;
            // 
            // btnLoad
            // 
            this.btnLoad.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnLoad.Location = new System.Drawing.Point(705, 100);
            this.btnLoad.Name = "btnLoad";
            this.btnLoad.Size = new System.Drawing.Size(75, 25);
            this.btnLoad.TabIndex = 11;
            this.btnLoad.Text = "Yükle";
            this.btnLoad.UseVisualStyleBackColor = true;
            this.btnLoad.Click += new System.EventHandler(this.btnLoad_Click);
            // 
            // btnSave
            // 
            this.btnSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSave.Location = new System.Drawing.Point(624, 100);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 25);
            this.btnSave.TabIndex = 10;
            this.btnSave.Text = "Kaydet";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnDelete
            // 
            this.btnDelete.Location = new System.Drawing.Point(93, 100);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(75, 25);
            this.btnDelete.TabIndex = 9;
            this.btnDelete.Text = "Sil";
            this.btnDelete.UseVisualStyleBackColor = true;
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // btnAdd
            // 
            this.btnAdd.Location = new System.Drawing.Point(12, 100);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(75, 25);
            this.btnAdd.TabIndex = 8;
            this.btnAdd.Text = "Ekle";
            this.btnAdd.UseVisualStyleBackColor = true;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // txtNotes
            // 
            this.txtNotes.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtNotes.Location = new System.Drawing.Point(72, 60);
            this.txtNotes.Name = "txtNotes";
            this.txtNotes.Size = new System.Drawing.Size(708, 20);
            this.txtNotes.TabIndex = 7;
            // 
            // lblNotes
            // 
            this.lblNotes.AutoSize = true;
            this.lblNotes.Location = new System.Drawing.Point(12, 63);
            this.lblNotes.Name = "lblNotes";
            this.lblNotes.Size = new System.Drawing.Size(35, 13);
            this.lblNotes.TabIndex = 6;
            this.lblNotes.Text = "Notlar";
            // 
            // numDuration
            // 
            this.numDuration.Location = new System.Drawing.Point(334, 20);
            this.numDuration.Maximum = new decimal(new int[] {
            600,
            0,
            0,
            0});
            this.numDuration.Minimum = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this.numDuration.Name = "numDuration";
            this.numDuration.Size = new System.Drawing.Size(60, 20);
            this.numDuration.TabIndex = 5;
            this.numDuration.Value = new decimal(new int[] {
            30,
            0,
            0,
            0});
            // 
            // lblDuration
            // 
            this.lblDuration.AutoSize = true;
            this.lblDuration.Location = new System.Drawing.Point(255, 23);
            this.lblDuration.Name = "lblDuration";
            this.lblDuration.Size = new System.Drawing.Size(73, 13);
            this.lblDuration.TabIndex = 4;
            this.lblDuration.Text = "Süre (dakika)";
            // 
            // dtpDate
            // 
            this.dtpDate.CustomFormat = "dd.MM.yyyy HH:mm";
            this.dtpDate.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpDate.Location = new System.Drawing.Point(72, 20);
            this.dtpDate.Name = "dtpDate";
            this.dtpDate.Size = new System.Drawing.Size(170, 20);
            this.dtpDate.TabIndex = 3;
            // 
            // lblDate
            // 
            this.lblDate.AutoSize = true;
            this.lblDate.Location = new System.Drawing.Point(12, 23);
            this.lblDate.Name = "lblDate";
            this.lblDate.Size = new System.Drawing.Size(54, 13);
            this.lblDate.TabIndex = 2;
            this.lblDate.Text = "Tarih/Saat";
            // 
            // txtSubject
            // 
            this.txtSubject.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtSubject.Location = new System.Drawing.Point(470, 20);
            this.txtSubject.Name = "txtSubject";
            this.txtSubject.Size = new System.Drawing.Size(310, 20);
            this.txtSubject.TabIndex = 1;
            // 
            // lblSubject
            // 
            this.lblSubject.AutoSize = true;
            this.lblSubject.Location = new System.Drawing.Point(413, 23);
            this.lblSubject.Name = "lblSubject";
            this.lblSubject.Size = new System.Drawing.Size(51, 13);
            this.lblSubject.TabIndex = 0;
            this.lblSubject.Text = "Ders/Konu";
            // 
            // listPanel
            // 
            this.listPanel.AutoScroll = true;
            this.listPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listPanel.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.listPanel.Location = new System.Drawing.Point(0, 140);
            this.listPanel.Name = "listPanel";
            this.listPanel.Padding = new System.Windows.Forms.Padding(8);
            this.listPanel.Size = new System.Drawing.Size(800, 310);
            this.listPanel.TabIndex = 1;
            this.listPanel.WrapContents = false;
            // 
            // Form1
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.listPanel);
            this.Controls.Add(this.panelTop);
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Study Planner";
            this.panelTop.ResumeLayout(false);
            this.panelTop.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numDuration)).EndInit();
            this.ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.Panel panelTop;
        private System.Windows.Forms.Label lblSubject;
        private System.Windows.Forms.TextBox txtSubject;
        private System.Windows.Forms.Label lblDate;
        private System.Windows.Forms.DateTimePicker dtpDate;
        private System.Windows.Forms.Label lblDuration;
        private System.Windows.Forms.NumericUpDown numDuration;
        private System.Windows.Forms.Label lblNotes;
        private System.Windows.Forms.TextBox txtNotes;
        private System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.Button btnDelete;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnLoad;
        private System.Windows.Forms.FlowLayoutPanel listPanel;
    }
}