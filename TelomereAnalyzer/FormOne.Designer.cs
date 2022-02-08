
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
            this.ImageBox = new Emgu.CV.UI.ImageBox();
            this.grpBoxSelectDialog = new System.Windows.Forms.GroupBox();
            this.lblPleaseSelectPic = new System.Windows.Forms.Label();
            this.btnPreviousStep = new System.Windows.Forms.Button();
            this.btnNextStep = new System.Windows.Forms.Button();
            this.mnuMainMenu = new System.Windows.Forms.MenuStrip();
            this.toolStrpMnuUpload = new System.Windows.Forms.ToolStripMenuItem();
            this.tIFFToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ImageBoxTwo = new Emgu.CV.UI.ImageBox();
            ((System.ComponentModel.ISupportInitialize)(this.ImageBox)).BeginInit();
            this.grpBoxSelectDialog.SuspendLayout();
            this.mnuMainMenu.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ImageBoxTwo)).BeginInit();
            this.SuspendLayout();
            // 
            // ImageBox
            // 
            this.ImageBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ImageBox.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ImageBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.ImageBox.Location = new System.Drawing.Point(232, 140);
            this.ImageBox.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.ImageBox.MaximumSize = new System.Drawing.Size(1024, 1024);
            this.ImageBox.MinimumSize = new System.Drawing.Size(1024, 1024);
            this.ImageBox.Name = "ImageBox";
            this.ImageBox.Size = new System.Drawing.Size(1024, 1024);
            this.ImageBox.TabIndex = 5;
            this.ImageBox.TabStop = false;
            // 
            // grpBoxSelectDialog
            // 
            this.grpBoxSelectDialog.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grpBoxSelectDialog.Controls.Add(this.lblPleaseSelectPic);
            this.grpBoxSelectDialog.Controls.Add(this.btnPreviousStep);
            this.grpBoxSelectDialog.Controls.Add(this.btnNextStep);
            this.grpBoxSelectDialog.Location = new System.Drawing.Point(12, 32);
            this.grpBoxSelectDialog.Name = "grpBoxSelectDialog";
            this.grpBoxSelectDialog.Size = new System.Drawing.Size(2958, 100);
            this.grpBoxSelectDialog.TabIndex = 12;
            this.grpBoxSelectDialog.TabStop = false;
            // 
            // lblPleaseSelectPic
            // 
            this.lblPleaseSelectPic.AutoSize = true;
            this.lblPleaseSelectPic.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblPleaseSelectPic.Location = new System.Drawing.Point(17, 22);
            this.lblPleaseSelectPic.Name = "lblPleaseSelectPic";
            this.lblPleaseSelectPic.Size = new System.Drawing.Size(330, 29);
            this.lblPleaseSelectPic.TabIndex = 11;
            this.lblPleaseSelectPic.Text = "Please upload a .TIFF picture";
            // 
            // btnPreviousStep
            // 
            this.btnPreviousStep.BackColor = System.Drawing.SystemColors.MenuHighlight;
            this.btnPreviousStep.Location = new System.Drawing.Point(1196, 45);
            this.btnPreviousStep.Name = "btnPreviousStep";
            this.btnPreviousStep.Size = new System.Drawing.Size(86, 33);
            this.btnPreviousStep.TabIndex = 10;
            this.btnPreviousStep.Text = "Back";
            this.btnPreviousStep.UseVisualStyleBackColor = false;
            // 
            // btnNextStep
            // 
            this.btnNextStep.BackColor = System.Drawing.SystemColors.MenuHighlight;
            this.btnNextStep.Location = new System.Drawing.Point(1319, 45);
            this.btnNextStep.Name = "btnNextStep";
            this.btnNextStep.Size = new System.Drawing.Size(88, 33);
            this.btnNextStep.TabIndex = 9;
            this.btnNextStep.Text = "Next";
            this.btnNextStep.UseVisualStyleBackColor = false;
            this.btnNextStep.Click += new System.EventHandler(this.OnClickNext);
            // 
            // mnuMainMenu
            // 
            this.mnuMainMenu.BackColor = System.Drawing.SystemColors.MenuHighlight;
            this.mnuMainMenu.GripMargin = new System.Windows.Forms.Padding(2, 2, 0, 2);
            this.mnuMainMenu.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.mnuMainMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStrpMnuUpload});
            this.mnuMainMenu.Location = new System.Drawing.Point(0, 0);
            this.mnuMainMenu.Name = "mnuMainMenu";
            this.mnuMainMenu.Size = new System.Drawing.Size(3011, 33);
            this.mnuMainMenu.TabIndex = 13;
            this.mnuMainMenu.Text = "menuStrip2";
            // 
            // toolStrpMnuUpload
            // 
            this.toolStrpMnuUpload.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tIFFToolStripMenuItem});
            this.toolStrpMnuUpload.Name = "toolStrpMnuUpload";
            this.toolStrpMnuUpload.Size = new System.Drawing.Size(91, 29);
            this.toolStrpMnuUpload.Text = "Upload ";
            // 
            // tIFFToolStripMenuItem
            // 
            this.tIFFToolStripMenuItem.BackColor = System.Drawing.SystemColors.MenuHighlight;
            this.tIFFToolStripMenuItem.Name = "tIFFToolStripMenuItem";
            this.tIFFToolStripMenuItem.Size = new System.Drawing.Size(150, 34);
            this.tIFFToolStripMenuItem.Text = ".TIFF";
            this.tIFFToolStripMenuItem.Click += new System.EventHandler(this.OnUploadTIFF);
            // 
            // ImageBoxTwo
            // 
            this.ImageBoxTwo.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ImageBoxTwo.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ImageBoxTwo.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.ImageBoxTwo.Location = new System.Drawing.Point(1507, 140);
            this.ImageBoxTwo.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.ImageBoxTwo.MaximumSize = new System.Drawing.Size(1024, 1024);
            this.ImageBoxTwo.MinimumSize = new System.Drawing.Size(1024, 1024);
            this.ImageBoxTwo.Name = "ImageBoxTwo";
            this.ImageBoxTwo.Size = new System.Drawing.Size(1024, 1024);
            this.ImageBoxTwo.TabIndex = 14;
            this.ImageBoxTwo.TabStop = false;
            // 
            // FormOne
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(3011, 1406);
            this.Controls.Add(this.ImageBoxTwo);
            this.Controls.Add(this.mnuMainMenu);
            this.Controls.Add(this.grpBoxSelectDialog);
            this.Controls.Add(this.ImageBox);
            this.Name = "FormOne";
            this.Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)(this.ImageBox)).EndInit();
            this.grpBoxSelectDialog.ResumeLayout(false);
            this.grpBoxSelectDialog.PerformLayout();
            this.mnuMainMenu.ResumeLayout(false);
            this.mnuMainMenu.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ImageBoxTwo)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Emgu.CV.UI.ImageBox ImageBox;
        private System.Windows.Forms.GroupBox grpBoxSelectDialog;
        private System.Windows.Forms.Label lblPleaseSelectPic;
        private System.Windows.Forms.Button btnPreviousStep;
        private System.Windows.Forms.Button btnNextStep;
        private System.Windows.Forms.MenuStrip mnuMainMenu;
        private System.Windows.Forms.ToolStripMenuItem toolStrpMnuUpload;
        private System.Windows.Forms.ToolStripMenuItem tIFFToolStripMenuItem;
        private Emgu.CV.UI.ImageBox ImageBoxTwo;
    }
}

