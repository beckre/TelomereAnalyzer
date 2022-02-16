
namespace TelomereAnalyzer
{
    partial class FormOne
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
            this.ImageBoxOne = new Emgu.CV.UI.ImageBox();
            this.grpBoxSelectDialog = new System.Windows.Forms.GroupBox();
            this.lblPleaseSelectPic = new System.Windows.Forms.Label();
            this.btnPreviousStep = new System.Windows.Forms.Button();
            this.btnNextStep = new System.Windows.Forms.Button();
            this.mnuMainMenu = new System.Windows.Forms.MenuStrip();
            this.toolStrpMnuUpload = new System.Windows.Forms.ToolStripMenuItem();
            this.tIFFToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.telomerImageToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ImageBoxTwo = new Emgu.CV.UI.ImageBox();
            this.lblThreshold = new System.Windows.Forms.Label();
            this.btnNormalize = new System.Windows.Forms.Button();
            this.btnGenerateThreshold = new System.Windows.Forms.Button();
            this.grpBoxSelectOptions = new System.Windows.Forms.GroupBox();
            ((System.ComponentModel.ISupportInitialize)(this.ImageBoxOne)).BeginInit();
            this.grpBoxSelectDialog.SuspendLayout();
            this.mnuMainMenu.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ImageBoxTwo)).BeginInit();
            this.grpBoxSelectOptions.SuspendLayout();
            this.SuspendLayout();
            // 
            // ImageBoxOne
            // 
            this.ImageBoxOne.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ImageBoxOne.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ImageBoxOne.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.ImageBoxOne.Location = new System.Drawing.Point(443, 182);
            this.ImageBoxOne.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.ImageBoxOne.MaximumSize = new System.Drawing.Size(1024, 1024);
            this.ImageBoxOne.MinimumSize = new System.Drawing.Size(1024, 1024);
            this.ImageBoxOne.Name = "ImageBoxOne";
            this.ImageBoxOne.Size = new System.Drawing.Size(1024, 1024);
            this.ImageBoxOne.TabIndex = 5;
            this.ImageBoxOne.TabStop = false;
            // 
            // grpBoxSelectDialog
            // 
            this.grpBoxSelectDialog.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grpBoxSelectDialog.Controls.Add(this.lblPleaseSelectPic);
            this.grpBoxSelectDialog.Controls.Add(this.btnPreviousStep);
            this.grpBoxSelectDialog.Controls.Add(this.btnNextStep);
            this.grpBoxSelectDialog.Location = new System.Drawing.Point(12, 32);
            this.grpBoxSelectDialog.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.grpBoxSelectDialog.Name = "grpBoxSelectDialog";
            this.grpBoxSelectDialog.Padding = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.grpBoxSelectDialog.Size = new System.Drawing.Size(4104, 100);
            this.grpBoxSelectDialog.TabIndex = 12;
            this.grpBoxSelectDialog.TabStop = false;
            // 
            // lblPleaseSelectPic
            // 
            this.lblPleaseSelectPic.AutoSize = true;
            this.lblPleaseSelectPic.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblPleaseSelectPic.Location = new System.Drawing.Point(17, 22);
            this.lblPleaseSelectPic.Name = "lblPleaseSelectPic";
            this.lblPleaseSelectPic.Size = new System.Drawing.Size(370, 29);
            this.lblPleaseSelectPic.TabIndex = 11;
            this.lblPleaseSelectPic.Text = "Please upload a Nuclei .TIFF file ";
            // 
            // btnPreviousStep
            // 
            this.btnPreviousStep.BackColor = System.Drawing.SystemColors.MenuHighlight;
            this.btnPreviousStep.Location = new System.Drawing.Point(1196, 45);
            this.btnPreviousStep.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnPreviousStep.Name = "btnPreviousStep";
            this.btnPreviousStep.Size = new System.Drawing.Size(86, 32);
            this.btnPreviousStep.TabIndex = 10;
            this.btnPreviousStep.Text = "Back";
            this.btnPreviousStep.UseVisualStyleBackColor = false;
            // 
            // btnNextStep
            // 
            this.btnNextStep.BackColor = System.Drawing.SystemColors.MenuHighlight;
            this.btnNextStep.Location = new System.Drawing.Point(1318, 45);
            this.btnNextStep.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnNextStep.Name = "btnNextStep";
            this.btnNextStep.Size = new System.Drawing.Size(88, 32);
            this.btnNextStep.TabIndex = 9;
            this.btnNextStep.Text = "Next";
            this.btnNextStep.UseVisualStyleBackColor = false;
            this.btnNextStep.Click += new System.EventHandler(this.OnClickNext);
            // 
            // mnuMainMenu
            // 
            this.mnuMainMenu.BackColor = System.Drawing.SystemColors.MenuHighlight;
            this.mnuMainMenu.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.mnuMainMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStrpMnuUpload});
            this.mnuMainMenu.Location = new System.Drawing.Point(0, 0);
            this.mnuMainMenu.Name = "mnuMainMenu";
            this.mnuMainMenu.Size = new System.Drawing.Size(3065, 33);
            this.mnuMainMenu.TabIndex = 13;
            this.mnuMainMenu.Text = "menuStrip2";
            // 
            // toolStrpMnuUpload
            // 
            this.toolStrpMnuUpload.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tIFFToolStripMenuItem,
            this.telomerImageToolStripMenuItem});
            this.toolStrpMnuUpload.Name = "toolStrpMnuUpload";
            this.toolStrpMnuUpload.Size = new System.Drawing.Size(91, 29);
            this.toolStrpMnuUpload.Text = "Upload ";
            // 
            // tIFFToolStripMenuItem
            // 
            this.tIFFToolStripMenuItem.BackColor = System.Drawing.SystemColors.MenuHighlight;
            this.tIFFToolStripMenuItem.Name = "tIFFToolStripMenuItem";
            this.tIFFToolStripMenuItem.Size = new System.Drawing.Size(231, 34);
            this.tIFFToolStripMenuItem.Text = "Nuclei Image";
            this.tIFFToolStripMenuItem.Click += new System.EventHandler(this.OnUploadNucleiImage);
            // 
            // telomerImageToolStripMenuItem
            // 
            this.telomerImageToolStripMenuItem.BackColor = System.Drawing.SystemColors.MenuHighlight;
            this.telomerImageToolStripMenuItem.Name = "telomerImageToolStripMenuItem";
            this.telomerImageToolStripMenuItem.Size = new System.Drawing.Size(231, 34);
            this.telomerImageToolStripMenuItem.Text = "Telomer Image";
            this.telomerImageToolStripMenuItem.Click += new System.EventHandler(this.OnUploadTelomereImage);
            // 
            // ImageBoxTwo
            // 
            this.ImageBoxTwo.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ImageBoxTwo.BackColor = System.Drawing.SystemColors.Control;
            this.ImageBoxTwo.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ImageBoxTwo.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.ImageBoxTwo.Location = new System.Drawing.Point(1735, 182);
            this.ImageBoxTwo.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.ImageBoxTwo.MaximumSize = new System.Drawing.Size(1024, 1024);
            this.ImageBoxTwo.MinimumSize = new System.Drawing.Size(1024, 1024);
            this.ImageBoxTwo.Name = "ImageBoxTwo";
            this.ImageBoxTwo.Size = new System.Drawing.Size(1024, 1024);
            this.ImageBoxTwo.TabIndex = 14;
            this.ImageBoxTwo.TabStop = false;
            // 
            // lblThreshold
            // 
            this.lblThreshold.AutoSize = true;
            this.lblThreshold.Location = new System.Drawing.Point(1731, 1224);
            this.lblThreshold.Name = "lblThreshold";
            this.lblThreshold.Size = new System.Drawing.Size(0, 20);
            this.lblThreshold.TabIndex = 15;
            // 
            // btnNormalize
            // 
            this.btnNormalize.BackColor = System.Drawing.SystemColors.MenuHighlight;
            this.btnNormalize.Location = new System.Drawing.Point(50, 45);
            this.btnNormalize.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnNormalize.Name = "btnNormalize";
            this.btnNormalize.Size = new System.Drawing.Size(130, 86);
            this.btnNormalize.TabIndex = 16;
            this.btnNormalize.Text = "Normalize";
            this.btnNormalize.UseVisualStyleBackColor = false;
            this.btnNormalize.Click += new System.EventHandler(this.OnNormalize);
            // 
            // btnGenerateThreshold
            // 
            this.btnGenerateThreshold.BackColor = System.Drawing.SystemColors.MenuHighlight;
            this.btnGenerateThreshold.Location = new System.Drawing.Point(50, 169);
            this.btnGenerateThreshold.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnGenerateThreshold.Name = "btnGenerateThreshold";
            this.btnGenerateThreshold.Size = new System.Drawing.Size(130, 76);
            this.btnGenerateThreshold.TabIndex = 17;
            this.btnGenerateThreshold.Text = "Threshold";
            this.btnGenerateThreshold.UseVisualStyleBackColor = false;
            this.btnGenerateThreshold.Click += new System.EventHandler(this.OnThreshold);
            // 
            // grpBoxSelectOptions
            // 
            this.grpBoxSelectOptions.Controls.Add(this.btnNormalize);
            this.grpBoxSelectOptions.Controls.Add(this.btnGenerateThreshold);
            this.grpBoxSelectOptions.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.grpBoxSelectOptions.Location = new System.Drawing.Point(74, 138);
            this.grpBoxSelectOptions.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.grpBoxSelectOptions.Name = "grpBoxSelectOptions";
            this.grpBoxSelectOptions.Padding = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.grpBoxSelectOptions.Size = new System.Drawing.Size(244, 1026);
            this.grpBoxSelectOptions.TabIndex = 18;
            this.grpBoxSelectOptions.TabStop = false;
            // 
            // FormOne
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(3065, 1351);
            this.Controls.Add(this.grpBoxSelectOptions);
            this.Controls.Add(this.lblThreshold);
            this.Controls.Add(this.ImageBoxTwo);
            this.Controls.Add(this.mnuMainMenu);
            this.Controls.Add(this.grpBoxSelectDialog);
            this.Controls.Add(this.ImageBoxOne);
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Name = "FormOne";
            this.Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)(this.ImageBoxOne)).EndInit();
            this.grpBoxSelectDialog.ResumeLayout(false);
            this.grpBoxSelectDialog.PerformLayout();
            this.mnuMainMenu.ResumeLayout(false);
            this.mnuMainMenu.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ImageBoxTwo)).EndInit();
            this.grpBoxSelectOptions.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Emgu.CV.UI.ImageBox ImageBoxOne;
        private System.Windows.Forms.GroupBox grpBoxSelectDialog;
        private System.Windows.Forms.Label lblPleaseSelectPic;
        private System.Windows.Forms.Button btnPreviousStep;
        private System.Windows.Forms.Button btnNextStep;
        private System.Windows.Forms.MenuStrip mnuMainMenu;
        private System.Windows.Forms.ToolStripMenuItem toolStrpMnuUpload;
        private System.Windows.Forms.ToolStripMenuItem tIFFToolStripMenuItem;
        private Emgu.CV.UI.ImageBox ImageBoxTwo;
        private System.Windows.Forms.Label lblThreshold;
        private System.Windows.Forms.Button btnNormalize;
        private System.Windows.Forms.Button btnGenerateThreshold;
        private System.Windows.Forms.GroupBox grpBoxSelectOptions;
        private System.Windows.Forms.ToolStripMenuItem telomerImageToolStripMenuItem;
    }
}

