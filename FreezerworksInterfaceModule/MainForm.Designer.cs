namespace FreezerworksInterfaceModule {
	partial class MainForm {
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() {
			this.lbl_Status = new System.Windows.Forms.Label();
			this.btn_Exit = new System.Windows.Forms.Button();
			this.btn_Export = new System.Windows.Forms.Button();
			this.btn_Import = new System.Windows.Forms.Button();
			this.TwoDScannerBackgroundWorker = new System.ComponentModel.BackgroundWorker();
			this.txt_RackID = new System.Windows.Forms.TextBox();
			this.btn_ProcessBacklog = new System.Windows.Forms.Button();
			this.lbl_Instructions = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// lbl_Status
			// 
			this.lbl_Status.AutoSize = true;
			this.lbl_Status.BackColor = System.Drawing.SystemColors.Control;
			this.lbl_Status.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte) (0)));
			this.lbl_Status.Location = new System.Drawing.Point(55, 55);
			this.lbl_Status.Name = "lbl_Status";
			this.lbl_Status.Size = new System.Drawing.Size(143, 29);
			this.lbl_Status.TabIndex = 0;
			this.lbl_Status.Text = "Status Text";
			// 
			// btn_Exit
			// 
			this.btn_Exit.BackColor = System.Drawing.SystemColors.Control;
			this.btn_Exit.FlatAppearance.BorderColor = System.Drawing.Color.White;
			this.btn_Exit.Font = new System.Drawing.Font("Microsoft Sans Serif", 20F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte) (0)));
			this.btn_Exit.ForeColor = System.Drawing.SystemColors.ControlText;
			this.btn_Exit.Location = new System.Drawing.Point(588, 345);
			this.btn_Exit.Name = "btn_Exit";
			this.btn_Exit.Size = new System.Drawing.Size(130, 73);
			this.btn_Exit.TabIndex = 1;
			this.btn_Exit.Text = "Exit";
			this.btn_Exit.UseVisualStyleBackColor = false;
			this.btn_Exit.Click += new System.EventHandler(this.btn_Exit_Click);
			// 
			// btn_Export
			// 
			this.btn_Export.BackColor = System.Drawing.SystemColors.Control;
			this.btn_Export.FlatAppearance.BorderColor = System.Drawing.Color.White;
			this.btn_Export.Font = new System.Drawing.Font("Microsoft Sans Serif", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte) (0)));
			this.btn_Export.ForeColor = System.Drawing.SystemColors.ControlText;
			this.btn_Export.Location = new System.Drawing.Point(423, 345);
			this.btn_Export.Name = "btn_Export";
			this.btn_Export.Size = new System.Drawing.Size(130, 73);
			this.btn_Export.TabIndex = 2;
			this.btn_Export.Text = "Export";
			this.btn_Export.UseVisualStyleBackColor = false;
			this.btn_Export.Click += new System.EventHandler(this.btn_Export_Click);
			// 
			// btn_Import
			// 
			this.btn_Import.BackColor = System.Drawing.SystemColors.Control;
			this.btn_Import.FlatAppearance.BorderColor = System.Drawing.Color.White;
			this.btn_Import.Font = new System.Drawing.Font("Microsoft Sans Serif", 20F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte) (0)));
			this.btn_Import.ForeColor = System.Drawing.SystemColors.ControlText;
			this.btn_Import.Location = new System.Drawing.Point(258, 345);
			this.btn_Import.Name = "btn_Import";
			this.btn_Import.Size = new System.Drawing.Size(130, 73);
			this.btn_Import.TabIndex = 3;
			this.btn_Import.Text = "Import";
			this.btn_Import.UseVisualStyleBackColor = false;
			this.btn_Import.Click += new System.EventHandler(this.btn_Import_Click);
			// 
			// TwoDScannerBackgroundWorker
			// 
			this.TwoDScannerBackgroundWorker.WorkerReportsProgress = true;
			this.TwoDScannerBackgroundWorker.WorkerSupportsCancellation = true;
			this.TwoDScannerBackgroundWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.TwoDScannerBackgroundWorker_DoWork);
			this.TwoDScannerBackgroundWorker.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.TwoDScannerBackgroundWorker_ProgressChanged);
			this.TwoDScannerBackgroundWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.TwoDScannerBackgroundWorker_RunWorkerCompleted);
			// 
			// txt_RackID
			// 
			this.txt_RackID.AcceptsReturn = true;
			this.txt_RackID.Font = new System.Drawing.Font("Microsoft Sans Serif", 20F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte) (0)));
			this.txt_RackID.Location = new System.Drawing.Point(55, 133);
			this.txt_RackID.Name = "txt_RackID";
			this.txt_RackID.Size = new System.Drawing.Size(367, 38);
			this.txt_RackID.TabIndex = 5;
			this.txt_RackID.Visible = false;
			this.txt_RackID.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txt_RackID_KeyDown);
			// 
			// btn_ProcessBacklog
			// 
			this.btn_ProcessBacklog.BackColor = System.Drawing.SystemColors.Control;
			this.btn_ProcessBacklog.FlatAppearance.BorderColor = System.Drawing.Color.White;
			this.btn_ProcessBacklog.Font = new System.Drawing.Font("Microsoft Sans Serif", 20F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte) (0)));
			this.btn_ProcessBacklog.ForeColor = System.Drawing.SystemColors.ControlText;
			this.btn_ProcessBacklog.Location = new System.Drawing.Point(85, 345);
			this.btn_ProcessBacklog.Name = "btn_ProcessBacklog";
			this.btn_ProcessBacklog.Size = new System.Drawing.Size(138, 73);
			this.btn_ProcessBacklog.TabIndex = 7;
			this.btn_ProcessBacklog.Text = "Process Backlog";
			this.btn_ProcessBacklog.UseVisualStyleBackColor = false;
			this.btn_ProcessBacklog.Click += new System.EventHandler(this.btn_ProcessBacklog_Click);
			// 
			// lbl_Instructions
			// 
			this.lbl_Instructions.AutoSize = true;
			this.lbl_Instructions.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.lbl_Instructions.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte) (0)));
			this.lbl_Instructions.Location = new System.Drawing.Point(55, 98);
			this.lbl_Instructions.Name = "lbl_Instructions";
			this.lbl_Instructions.Size = new System.Drawing.Size(177, 31);
			this.lbl_Instructions.TabIndex = 9;
			this.lbl_Instructions.Text = "Instruction Text";
			// 
			// MainForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.SystemColors.Control;
			this.ClientSize = new System.Drawing.Size(822, 468);
			this.Controls.Add(this.lbl_Instructions);
			this.Controls.Add(this.btn_ProcessBacklog);
			this.Controls.Add(this.txt_RackID);
			this.Controls.Add(this.btn_Import);
			this.Controls.Add(this.btn_Export);
			this.Controls.Add(this.btn_Exit);
			this.Controls.Add(this.lbl_Status);
			this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte) (0)));
			this.Name = "MainForm";
			this.Text = "Freezerworks Interface Module";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label lbl_Status;
		private System.Windows.Forms.Button btn_Exit;
		private System.Windows.Forms.Button btn_Export;
		private System.Windows.Forms.Button btn_Import;
		private System.ComponentModel.BackgroundWorker TwoDScannerBackgroundWorker;
		private System.Windows.Forms.TextBox txt_RackID;
		private System.Windows.Forms.Button btn_ProcessBacklog;
		private System.Windows.Forms.Label lbl_Instructions;
	}
}