
namespace TelomereAnalyzer
{
    partial class FormTwo
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
            this.ImageBoxOneFormTwo = new Emgu.CV.UI.ImageBox();
            this.grpBxFormTwo = new System.Windows.Forms.GroupBox();
            this.lblInstructionsFormTwo = new System.Windows.Forms.Label();
            this.grpBxToolsSelectNucleiFormTwo = new System.Windows.Forms.GroupBox();
            ((System.ComponentModel.ISupportInitialize)(this.ImageBoxOneFormTwo)).BeginInit();
            this.grpBxFormTwo.SuspendLayout();
            this.SuspendLayout();
            // 
            // ImageBoxOneFormTwo
            // 
            this.ImageBoxOneFormTwo.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ImageBoxOneFormTwo.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.ImageBoxOneFormTwo.Location = new System.Drawing.Point(40, 175);
            this.ImageBoxOneFormTwo.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.ImageBoxOneFormTwo.MaximumSize = new System.Drawing.Size(1024, 1024);
            this.ImageBoxOneFormTwo.Name = "ImageBoxOneFormTwo";
            this.ImageBoxOneFormTwo.Size = new System.Drawing.Size(1024, 1024);
            this.ImageBoxOneFormTwo.TabIndex = 6;
            this.ImageBoxOneFormTwo.TabStop = false;
            this.ImageBoxOneFormTwo.Paint += new System.Windows.Forms.PaintEventHandler(this.OnPaint);
            this.ImageBoxOneFormTwo.MouseDown += new System.Windows.Forms.MouseEventHandler(this.OnMouseDown);
            this.ImageBoxOneFormTwo.MouseMove += new System.Windows.Forms.MouseEventHandler(this.OnMouseMove);
            this.ImageBoxOneFormTwo.MouseUp += new System.Windows.Forms.MouseEventHandler(this.OnMouseUp);
            // 
            // grpBxFormTwo
            // 
            this.grpBxFormTwo.Controls.Add(this.lblInstructionsFormTwo);
            this.grpBxFormTwo.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.grpBxFormTwo.Location = new System.Drawing.Point(13, 13);
            this.grpBxFormTwo.Name = "grpBxFormTwo";
            this.grpBxFormTwo.Size = new System.Drawing.Size(1542, 154);
            this.grpBxFormTwo.TabIndex = 7;
            this.grpBxFormTwo.TabStop = false;
            // 
            // lblInstructionsFormTwo
            // 
            this.lblInstructionsFormTwo.AutoSize = true;
            this.lblInstructionsFormTwo.Location = new System.Drawing.Point(22, 40);
            this.lblInstructionsFormTwo.Name = "lblInstructionsFormTwo";
            this.lblInstructionsFormTwo.Size = new System.Drawing.Size(726, 29);
            this.lblInstructionsFormTwo.TabIndex = 0;
            this.lblInstructionsFormTwo.Text = "Please select/deselect and/or draw the Nuclei that will be analyzed";
            // 
            // grpBxToolsSelectNucleiFormTwo
            // 
            this.grpBxToolsSelectNucleiFormTwo.Location = new System.Drawing.Point(1072, 174);
            this.grpBxToolsSelectNucleiFormTwo.Name = "grpBxToolsSelectNucleiFormTwo";
            this.grpBxToolsSelectNucleiFormTwo.Size = new System.Drawing.Size(468, 1025);
            this.grpBxToolsSelectNucleiFormTwo.TabIndex = 8;
            this.grpBxToolsSelectNucleiFormTwo.TabStop = false;
            this.grpBxToolsSelectNucleiFormTwo.Text = "Tools";
            // 
            // FormTwo
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1552, 1351);
            this.Controls.Add(this.grpBxToolsSelectNucleiFormTwo);
            this.Controls.Add(this.grpBxFormTwo);
            this.Controls.Add(this.ImageBoxOneFormTwo);
            this.Name = "FormTwo";
            this.Text = "FormTwo";
            ((System.ComponentModel.ISupportInitialize)(this.ImageBoxOneFormTwo)).EndInit();
            this.grpBxFormTwo.ResumeLayout(false);
            this.grpBxFormTwo.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        public Emgu.CV.UI.ImageBox ImageBoxOneFormTwo;
        private System.Windows.Forms.GroupBox grpBxFormTwo;
        private System.Windows.Forms.Label lblInstructionsFormTwo;
        private System.Windows.Forms.GroupBox grpBxToolsSelectNucleiFormTwo;
    }
}