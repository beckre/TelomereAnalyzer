
namespace TelomereAnalyzer
{
    partial class FormThree
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
            this.btnSaveThresholdImage = new System.Windows.Forms.Button();
            this.grpBxSaveImages = new System.Windows.Forms.GroupBox();
            this.btnExportExcel = new System.Windows.Forms.Button();
            this.btnSaveMergedNucleiTelomereImage = new System.Windows.Forms.Button();
            this.lblNucleiTelomereImageMerged = new System.Windows.Forms.Label();
            this.btnSaveDetectedAndDrawnNucleiImage = new System.Windows.Forms.Button();
            this.lblDetectedAndDrawnNucleiImage = new System.Windows.Forms.Label();
            this.btnSaveDetectedNucleiImage = new System.Windows.Forms.Button();
            this.lblDetectedNucleiImage = new System.Windows.Forms.Label();
            this.btnSaveThresholdTelomereOverlayNucleiImage = new System.Windows.Forms.Button();
            this.lblThresholdTemlomereOverlayNucleiImage = new System.Windows.Forms.Label();
            this.lblThresholdImage = new System.Windows.Forms.Label();
            this.grpBx = new System.Windows.Forms.GroupBox();
            this.lblInstructionsFormTwo = new System.Windows.Forms.Label();
            this.lblAutoLevelImage = new System.Windows.Forms.Label();
            this.btnSaveAutoLevelNucleiImage = new System.Windows.Forms.Button();
            this.lblAutoLevelTelomereImage = new System.Windows.Forms.Label();
            this.btnSaveAutoLevelTelomereImage = new System.Windows.Forms.Button();
            this.grpBxSaveImages.SuspendLayout();
            this.grpBx.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnSaveThresholdImage
            // 
            this.btnSaveThresholdImage.BackColor = System.Drawing.SystemColors.MenuHighlight;
            this.btnSaveThresholdImage.Location = new System.Drawing.Point(495, 225);
            this.btnSaveThresholdImage.Name = "btnSaveThresholdImage";
            this.btnSaveThresholdImage.Size = new System.Drawing.Size(102, 39);
            this.btnSaveThresholdImage.TabIndex = 1;
            this.btnSaveThresholdImage.Text = "Save";
            this.btnSaveThresholdImage.UseVisualStyleBackColor = false;
            this.btnSaveThresholdImage.Click += new System.EventHandler(this.OnSaveThresholdTelomereImage);
            // 
            // grpBxSaveImages
            // 
            this.grpBxSaveImages.Controls.Add(this.btnSaveAutoLevelTelomereImage);
            this.grpBxSaveImages.Controls.Add(this.lblAutoLevelTelomereImage);
            this.grpBxSaveImages.Controls.Add(this.btnSaveAutoLevelNucleiImage);
            this.grpBxSaveImages.Controls.Add(this.lblAutoLevelImage);
            this.grpBxSaveImages.Controls.Add(this.btnExportExcel);
            this.grpBxSaveImages.Controls.Add(this.btnSaveMergedNucleiTelomereImage);
            this.grpBxSaveImages.Controls.Add(this.lblNucleiTelomereImageMerged);
            this.grpBxSaveImages.Controls.Add(this.btnSaveDetectedAndDrawnNucleiImage);
            this.grpBxSaveImages.Controls.Add(this.lblDetectedAndDrawnNucleiImage);
            this.grpBxSaveImages.Controls.Add(this.btnSaveDetectedNucleiImage);
            this.grpBxSaveImages.Controls.Add(this.lblDetectedNucleiImage);
            this.grpBxSaveImages.Controls.Add(this.btnSaveThresholdTelomereOverlayNucleiImage);
            this.grpBxSaveImages.Controls.Add(this.lblThresholdTemlomereOverlayNucleiImage);
            this.grpBxSaveImages.Controls.Add(this.lblThresholdImage);
            this.grpBxSaveImages.Controls.Add(this.btnSaveThresholdImage);
            this.grpBxSaveImages.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.grpBxSaveImages.Location = new System.Drawing.Point(12, 139);
            this.grpBxSaveImages.Name = "grpBxSaveImages";
            this.grpBxSaveImages.Size = new System.Drawing.Size(829, 1200);
            this.grpBxSaveImages.TabIndex = 2;
            this.grpBxSaveImages.TabStop = false;
            // 
            // btnExportExcel
            // 
            this.btnExportExcel.BackColor = System.Drawing.SystemColors.MenuHighlight;
            this.btnExportExcel.Location = new System.Drawing.Point(337, 819);
            this.btnExportExcel.Name = "btnExportExcel";
            this.btnExportExcel.Size = new System.Drawing.Size(156, 93);
            this.btnExportExcel.TabIndex = 11;
            this.btnExportExcel.Text = "Export Excel";
            this.btnExportExcel.UseVisualStyleBackColor = false;
            this.btnExportExcel.Click += new System.EventHandler(this.OnExportExcel);
            // 
            // btnSaveMergedNucleiTelomereImage
            // 
            this.btnSaveMergedNucleiTelomereImage.BackColor = System.Drawing.SystemColors.MenuHighlight;
            this.btnSaveMergedNucleiTelomereImage.Location = new System.Drawing.Point(495, 615);
            this.btnSaveMergedNucleiTelomereImage.Name = "btnSaveMergedNucleiTelomereImage";
            this.btnSaveMergedNucleiTelomereImage.Size = new System.Drawing.Size(102, 39);
            this.btnSaveMergedNucleiTelomereImage.TabIndex = 10;
            this.btnSaveMergedNucleiTelomereImage.Text = "Save";
            this.btnSaveMergedNucleiTelomereImage.UseVisualStyleBackColor = false;
            this.btnSaveMergedNucleiTelomereImage.Click += new System.EventHandler(this.OnSave);
            // 
            // lblNucleiTelomereImageMerged
            // 
            this.lblNucleiTelomereImageMerged.AutoSize = true;
            this.lblNucleiTelomereImageMerged.Location = new System.Drawing.Point(31, 622);
            this.lblNucleiTelomereImageMerged.Name = "lblNucleiTelomereImageMerged";
            this.lblNucleiTelomereImageMerged.Size = new System.Drawing.Size(375, 25);
            this.lblNucleiTelomereImageMerged.TabIndex = 9;
            this.lblNucleiTelomereImageMerged.Text = "Detected and drawn Nuclei Image merged";
            // 
            // btnSaveDetectedAndDrawnNucleiImage
            // 
            this.btnSaveDetectedAndDrawnNucleiImage.BackColor = System.Drawing.SystemColors.MenuHighlight;
            this.btnSaveDetectedAndDrawnNucleiImage.Location = new System.Drawing.Point(495, 519);
            this.btnSaveDetectedAndDrawnNucleiImage.Name = "btnSaveDetectedAndDrawnNucleiImage";
            this.btnSaveDetectedAndDrawnNucleiImage.Size = new System.Drawing.Size(102, 39);
            this.btnSaveDetectedAndDrawnNucleiImage.TabIndex = 8;
            this.btnSaveDetectedAndDrawnNucleiImage.Text = "Save";
            this.btnSaveDetectedAndDrawnNucleiImage.UseVisualStyleBackColor = false;
            this.btnSaveDetectedAndDrawnNucleiImage.Click += new System.EventHandler(this.OnSaveDetectedAndDrawnNucleiImage);
            // 
            // lblDetectedAndDrawnNucleiImage
            // 
            this.lblDetectedAndDrawnNucleiImage.AutoSize = true;
            this.lblDetectedAndDrawnNucleiImage.Location = new System.Drawing.Point(31, 519);
            this.lblDetectedAndDrawnNucleiImage.Name = "lblDetectedAndDrawnNucleiImage";
            this.lblDetectedAndDrawnNucleiImage.Size = new System.Drawing.Size(304, 25);
            this.lblDetectedAndDrawnNucleiImage.TabIndex = 7;
            this.lblDetectedAndDrawnNucleiImage.Text = "Detected and drawn Nuclei Image";
            // 
            // btnSaveDetectedNucleiImage
            // 
            this.btnSaveDetectedNucleiImage.BackColor = System.Drawing.SystemColors.MenuHighlight;
            this.btnSaveDetectedNucleiImage.Location = new System.Drawing.Point(495, 418);
            this.btnSaveDetectedNucleiImage.Name = "btnSaveDetectedNucleiImage";
            this.btnSaveDetectedNucleiImage.Size = new System.Drawing.Size(102, 39);
            this.btnSaveDetectedNucleiImage.TabIndex = 6;
            this.btnSaveDetectedNucleiImage.Text = "Save";
            this.btnSaveDetectedNucleiImage.UseVisualStyleBackColor = false;
            this.btnSaveDetectedNucleiImage.Click += new System.EventHandler(this.OnSaveDetectedNucleiImage);
            // 
            // lblDetectedNucleiImage
            // 
            this.lblDetectedNucleiImage.AutoSize = true;
            this.lblDetectedNucleiImage.Location = new System.Drawing.Point(31, 425);
            this.lblDetectedNucleiImage.Name = "lblDetectedNucleiImage";
            this.lblDetectedNucleiImage.Size = new System.Drawing.Size(208, 25);
            this.lblDetectedNucleiImage.TabIndex = 5;
            this.lblDetectedNucleiImage.Text = "Detected Nuclei Image";
            // 
            // btnSaveThresholdTelomereOverlayNucleiImage
            // 
            this.btnSaveThresholdTelomereOverlayNucleiImage.BackColor = System.Drawing.SystemColors.MenuHighlight;
            this.btnSaveThresholdTelomereOverlayNucleiImage.Location = new System.Drawing.Point(495, 320);
            this.btnSaveThresholdTelomereOverlayNucleiImage.Name = "btnSaveThresholdTelomereOverlayNucleiImage";
            this.btnSaveThresholdTelomereOverlayNucleiImage.Size = new System.Drawing.Size(102, 39);
            this.btnSaveThresholdTelomereOverlayNucleiImage.TabIndex = 4;
            this.btnSaveThresholdTelomereOverlayNucleiImage.Text = "Save";
            this.btnSaveThresholdTelomereOverlayNucleiImage.UseVisualStyleBackColor = false;
            this.btnSaveThresholdTelomereOverlayNucleiImage.Click += new System.EventHandler(this.OnSaveThresholdTelomereOverlayNucleiImage);
            // 
            // lblThresholdTemlomereOverlayNucleiImage
            // 
            this.lblThresholdTemlomereOverlayNucleiImage.AutoSize = true;
            this.lblThresholdTemlomereOverlayNucleiImage.Location = new System.Drawing.Point(27, 334);
            this.lblThresholdTemlomereOverlayNucleiImage.Name = "lblThresholdTemlomereOverlayNucleiImage";
            this.lblThresholdTemlomereOverlayNucleiImage.Size = new System.Drawing.Size(379, 25);
            this.lblThresholdTemlomereOverlayNucleiImage.TabIndex = 3;
            this.lblThresholdTemlomereOverlayNucleiImage.Text = "Threshold Telomere Overlay Nuclei Image";
            // 
            // lblThresholdImage
            // 
            this.lblThresholdImage.AutoSize = true;
            this.lblThresholdImage.Location = new System.Drawing.Point(31, 232);
            this.lblThresholdImage.Name = "lblThresholdImage";
            this.lblThresholdImage.Size = new System.Drawing.Size(247, 25);
            this.lblThresholdImage.TabIndex = 2;
            this.lblThresholdImage.Text = "Threshold Telomere Image";
            // 
            // grpBx
            // 
            this.grpBx.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grpBx.Controls.Add(this.lblInstructionsFormTwo);
            this.grpBx.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.grpBx.Location = new System.Drawing.Point(12, 11);
            this.grpBx.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.grpBx.Name = "grpBx";
            this.grpBx.Padding = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.grpBx.Size = new System.Drawing.Size(829, 123);
            this.grpBx.TabIndex = 8;
            this.grpBx.TabStop = false;
            // 
            // lblInstructionsFormTwo
            // 
            this.lblInstructionsFormTwo.AutoSize = true;
            this.lblInstructionsFormTwo.Location = new System.Drawing.Point(22, 40);
            this.lblInstructionsFormTwo.Name = "lblInstructionsFormTwo";
            this.lblInstructionsFormTwo.Size = new System.Drawing.Size(705, 29);
            this.lblInstructionsFormTwo.TabIndex = 0;
            this.lblInstructionsFormTwo.Text = "Please save the Images and Export the Excel File of the Analysis.";
            // 
            // lblAutoLevelImage
            // 
            this.lblAutoLevelImage.AutoSize = true;
            this.lblAutoLevelImage.Location = new System.Drawing.Point(31, 36);
            this.lblAutoLevelImage.Name = "lblAutoLevelImage";
            this.lblAutoLevelImage.Size = new System.Drawing.Size(225, 25);
            this.lblAutoLevelImage.TabIndex = 12;
            this.lblAutoLevelImage.Text = "Auto-Level Nuclei Image";
            // 
            // btnSaveAutoLevelNucleiImage
            // 
            this.btnSaveAutoLevelNucleiImage.BackColor = System.Drawing.SystemColors.MenuHighlight;
            this.btnSaveAutoLevelNucleiImage.Location = new System.Drawing.Point(495, 29);
            this.btnSaveAutoLevelNucleiImage.Name = "btnSaveAutoLevelNucleiImage";
            this.btnSaveAutoLevelNucleiImage.Size = new System.Drawing.Size(102, 39);
            this.btnSaveAutoLevelNucleiImage.TabIndex = 13;
            this.btnSaveAutoLevelNucleiImage.Text = "Save";
            this.btnSaveAutoLevelNucleiImage.UseVisualStyleBackColor = false;
            this.btnSaveAutoLevelNucleiImage.Click += new System.EventHandler(this.OnSaveAutoLevelNucleiImage);
            // 
            // lblAutoLevelTelomereImage
            // 
            this.lblAutoLevelTelomereImage.AutoSize = true;
            this.lblAutoLevelTelomereImage.Location = new System.Drawing.Point(24, 132);
            this.lblAutoLevelTelomereImage.Name = "lblAutoLevelTelomereImage";
            this.lblAutoLevelTelomereImage.Size = new System.Drawing.Size(254, 25);
            this.lblAutoLevelTelomereImage.TabIndex = 14;
            this.lblAutoLevelTelomereImage.Text = "Auto-Level Telomere Image";
            // 
            // btnSaveAutoLevelTelomereImage
            // 
            this.btnSaveAutoLevelTelomereImage.BackColor = System.Drawing.SystemColors.MenuHighlight;
            this.btnSaveAutoLevelTelomereImage.Location = new System.Drawing.Point(495, 125);
            this.btnSaveAutoLevelTelomereImage.Name = "btnSaveAutoLevelTelomereImage";
            this.btnSaveAutoLevelTelomereImage.Size = new System.Drawing.Size(102, 39);
            this.btnSaveAutoLevelTelomereImage.TabIndex = 15;
            this.btnSaveAutoLevelTelomereImage.Text = "Save";
            this.btnSaveAutoLevelTelomereImage.UseVisualStyleBackColor = false;
            this.btnSaveAutoLevelTelomereImage.Click += new System.EventHandler(this.OnSaveAutoLevelTelomereImage);
            // 
            // FormThree
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(853, 1386);
            this.Controls.Add(this.grpBx);
            this.Controls.Add(this.grpBxSaveImages);
            this.Name = "FormThree";
            this.Text = "FormThree";
            this.grpBxSaveImages.ResumeLayout(false);
            this.grpBxSaveImages.PerformLayout();
            this.grpBx.ResumeLayout(false);
            this.grpBx.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Button btnSaveThresholdImage;
        private System.Windows.Forms.GroupBox grpBxSaveImages;
        private System.Windows.Forms.GroupBox grpBx;
        private System.Windows.Forms.Label lblInstructionsFormTwo;
        private System.Windows.Forms.Label lblThresholdImage;
        private System.Windows.Forms.Button btnSaveThresholdTelomereOverlayNucleiImage;
        private System.Windows.Forms.Label lblThresholdTemlomereOverlayNucleiImage;
        private System.Windows.Forms.Button btnSaveDetectedNucleiImage;
        private System.Windows.Forms.Label lblDetectedNucleiImage;
        private System.Windows.Forms.Button btnSaveDetectedAndDrawnNucleiImage;
        private System.Windows.Forms.Label lblDetectedAndDrawnNucleiImage;
        private System.Windows.Forms.Button btnSaveMergedNucleiTelomereImage;
        private System.Windows.Forms.Label lblNucleiTelomereImageMerged;
        private System.Windows.Forms.Button btnExportExcel;
        private System.Windows.Forms.Button btnSaveAutoLevelTelomereImage;
        private System.Windows.Forms.Label lblAutoLevelTelomereImage;
        private System.Windows.Forms.Button btnSaveAutoLevelNucleiImage;
        private System.Windows.Forms.Label lblAutoLevelImage;
    }
}