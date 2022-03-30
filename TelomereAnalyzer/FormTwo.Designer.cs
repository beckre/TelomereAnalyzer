
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
            this.button2 = new System.Windows.Forms.Button();
            this.pnlSelectNuclei = new System.Windows.Forms.Panel();
            this.grpBxAddNucleus = new System.Windows.Forms.GroupBox();
            this.btnAddNucleus = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.ImageBoxOneFormTwo)).BeginInit();
            this.grpBxFormTwo.SuspendLayout();
            this.grpBxToolsSelectNucleiFormTwo.SuspendLayout();
            this.grpBxAddNucleus.SuspendLayout();
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
            this.lblInstructionsFormTwo.Size = new System.Drawing.Size(768, 29);
            this.lblInstructionsFormTwo.TabIndex = 0;
            this.lblInstructionsFormTwo.Text = "Please select/deselect and/or draw the Nuclei that you want to analyze.";
            // 
            // grpBxToolsSelectNucleiFormTwo
            // 
            this.grpBxToolsSelectNucleiFormTwo.Controls.Add(this.button2);
            this.grpBxToolsSelectNucleiFormTwo.Controls.Add(this.pnlSelectNuclei);
            this.grpBxToolsSelectNucleiFormTwo.Controls.Add(this.grpBxAddNucleus);
            this.grpBxToolsSelectNucleiFormTwo.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.grpBxToolsSelectNucleiFormTwo.Location = new System.Drawing.Point(1072, 174);
            this.grpBxToolsSelectNucleiFormTwo.Name = "grpBxToolsSelectNucleiFormTwo";
            this.grpBxToolsSelectNucleiFormTwo.Size = new System.Drawing.Size(468, 1025);
            this.grpBxToolsSelectNucleiFormTwo.TabIndex = 8;
            this.grpBxToolsSelectNucleiFormTwo.TabStop = false;
            this.grpBxToolsSelectNucleiFormTwo.Text = "Tools";
            // 
            // button2
            // 
            this.button2.BackColor = System.Drawing.SystemColors.MenuHighlight;
            this.button2.Location = new System.Drawing.Point(168, 901);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(130, 86);
            this.button2.TabIndex = 2;
            this.button2.Text = "Apply";
            this.button2.UseVisualStyleBackColor = false;
            // 
            // pnlSelectNuclei
            // 
            this.pnlSelectNuclei.AutoScroll = true;
            this.pnlSelectNuclei.Location = new System.Drawing.Point(6, 272);
            this.pnlSelectNuclei.Name = "pnlSelectNuclei";
            this.pnlSelectNuclei.Size = new System.Drawing.Size(456, 586);
            this.pnlSelectNuclei.TabIndex = 1;
            // 
            // grpBxAddNucleus
            // 
            this.grpBxAddNucleus.Controls.Add(this.btnAddNucleus);
            this.grpBxAddNucleus.Location = new System.Drawing.Point(6, 25);
            this.grpBxAddNucleus.Name = "grpBxAddNucleus";
            this.grpBxAddNucleus.Size = new System.Drawing.Size(462, 241);
            this.grpBxAddNucleus.TabIndex = 0;
            this.grpBxAddNucleus.TabStop = false;
            // 
            // btnAddNucleus
            // 
            this.btnAddNucleus.BackColor = System.Drawing.SystemColors.MenuHighlight;
            this.btnAddNucleus.Location = new System.Drawing.Point(162, 89);
            this.btnAddNucleus.Name = "btnAddNucleus";
            this.btnAddNucleus.Size = new System.Drawing.Size(130, 86);
            this.btnAddNucleus.TabIndex = 0;
            this.btnAddNucleus.Text = "Add Nucleus";
            this.btnAddNucleus.UseVisualStyleBackColor = false;
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
            this.grpBxToolsSelectNucleiFormTwo.ResumeLayout(false);
            this.grpBxAddNucleus.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        public Emgu.CV.UI.ImageBox ImageBoxOneFormTwo;
        private System.Windows.Forms.GroupBox grpBxFormTwo;
        private System.Windows.Forms.Label lblInstructionsFormTwo;
        private System.Windows.Forms.GroupBox grpBxToolsSelectNucleiFormTwo;
        private System.Windows.Forms.GroupBox grpBxAddNucleus;
        private System.Windows.Forms.Panel pnlSelectNuclei;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button btnAddNucleus;
    }
}