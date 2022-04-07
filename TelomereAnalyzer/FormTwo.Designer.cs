
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
            this.grpBxFormTwo = new System.Windows.Forms.GroupBox();
            this.lblInstructionsFormTwo = new System.Windows.Forms.Label();
            this.grpBxToolsSelectNucleiFormTwo = new System.Windows.Forms.GroupBox();
            this.button2 = new System.Windows.Forms.Button();
            this.pnlSelectNuclei = new System.Windows.Forms.Panel();
            this.grpBxAddNucleus = new System.Windows.Forms.GroupBox();
            this.btnAddNucleus = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.ImageBoxOneFormTwo = new Emgu.CV.UI.ImageBox();
            this.label1 = new System.Windows.Forms.Label();
            this.grpBxFormTwo.SuspendLayout();
            this.grpBxToolsSelectNucleiFormTwo.SuspendLayout();
            this.grpBxAddNucleus.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ImageBoxOneFormTwo)).BeginInit();
            this.SuspendLayout();
            // 
            // grpBxFormTwo
            // 
            this.grpBxFormTwo.Controls.Add(this.label1);
            this.grpBxFormTwo.Controls.Add(this.lblInstructionsFormTwo);
            this.grpBxFormTwo.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.grpBxFormTwo.Location = new System.Drawing.Point(12, 10);
            this.grpBxFormTwo.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.grpBxFormTwo.Name = "grpBxFormTwo";
            this.grpBxFormTwo.Padding = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.grpBxFormTwo.Size = new System.Drawing.Size(1371, 123);
            this.grpBxFormTwo.TabIndex = 7;
            this.grpBxFormTwo.TabStop = false;
            // 
            // lblInstructionsFormTwo
            // 
            this.lblInstructionsFormTwo.AutoSize = true;
            this.lblInstructionsFormTwo.Location = new System.Drawing.Point(20, 32);
            this.lblInstructionsFormTwo.Name = "lblInstructionsFormTwo";
            this.lblInstructionsFormTwo.Size = new System.Drawing.Size(625, 25);
            this.lblInstructionsFormTwo.TabIndex = 0;
            this.lblInstructionsFormTwo.Text = "Please select/deselect and/or draw the Nuclei that you want to analyze.";
            // 
            // grpBxToolsSelectNucleiFormTwo
            // 
            this.grpBxToolsSelectNucleiFormTwo.Controls.Add(this.button2);
            this.grpBxToolsSelectNucleiFormTwo.Controls.Add(this.pnlSelectNuclei);
            this.grpBxToolsSelectNucleiFormTwo.Controls.Add(this.grpBxAddNucleus);
            this.grpBxToolsSelectNucleiFormTwo.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.grpBxToolsSelectNucleiFormTwo.Location = new System.Drawing.Point(953, 139);
            this.grpBxToolsSelectNucleiFormTwo.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.grpBxToolsSelectNucleiFormTwo.Name = "grpBxToolsSelectNucleiFormTwo";
            this.grpBxToolsSelectNucleiFormTwo.Padding = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.grpBxToolsSelectNucleiFormTwo.Size = new System.Drawing.Size(416, 820);
            this.grpBxToolsSelectNucleiFormTwo.TabIndex = 8;
            this.grpBxToolsSelectNucleiFormTwo.TabStop = false;
            this.grpBxToolsSelectNucleiFormTwo.Text = "Tools";
            // 
            // button2
            // 
            this.button2.BackColor = System.Drawing.SystemColors.MenuHighlight;
            this.button2.Location = new System.Drawing.Point(149, 721);
            this.button2.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(116, 69);
            this.button2.TabIndex = 2;
            this.button2.Text = "Apply";
            this.button2.UseVisualStyleBackColor = false;
            this.button2.Click += new System.EventHandler(this.OnApply);
            // 
            // pnlSelectNuclei
            // 
            this.pnlSelectNuclei.AutoScroll = true;
            this.pnlSelectNuclei.Location = new System.Drawing.Point(5, 218);
            this.pnlSelectNuclei.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.pnlSelectNuclei.Name = "pnlSelectNuclei";
            this.pnlSelectNuclei.Size = new System.Drawing.Size(405, 469);
            this.pnlSelectNuclei.TabIndex = 1;
            // 
            // grpBxAddNucleus
            // 
            this.grpBxAddNucleus.Controls.Add(this.btnAddNucleus);
            this.grpBxAddNucleus.Location = new System.Drawing.Point(5, 20);
            this.grpBxAddNucleus.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.grpBxAddNucleus.Name = "grpBxAddNucleus";
            this.grpBxAddNucleus.Padding = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.grpBxAddNucleus.Size = new System.Drawing.Size(411, 193);
            this.grpBxAddNucleus.TabIndex = 0;
            this.grpBxAddNucleus.TabStop = false;
            // 
            // btnAddNucleus
            // 
            this.btnAddNucleus.BackColor = System.Drawing.SystemColors.MenuHighlight;
            this.btnAddNucleus.Location = new System.Drawing.Point(144, 71);
            this.btnAddNucleus.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnAddNucleus.Name = "btnAddNucleus";
            this.btnAddNucleus.Size = new System.Drawing.Size(116, 69);
            this.btnAddNucleus.TabIndex = 0;
            this.btnAddNucleus.Text = "Add Nucleus";
            this.btnAddNucleus.UseVisualStyleBackColor = false;
            this.btnAddNucleus.Click += new System.EventHandler(this.OnAddNucleus);
            // 
            // panel1
            // 
            this.panel1.AutoScroll = true;
            this.panel1.AutoSize = true;
            this.panel1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.panel1.Location = new System.Drawing.Point(36, 140);
            this.panel1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(910, 819);
            this.panel1.TabIndex = 9;
            // 
            // ImageBoxOneFormTwo
            // 
            this.ImageBoxOneFormTwo.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ImageBoxOneFormTwo.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.ImageBoxOneFormTwo.Location = new System.Drawing.Point(36, 140);
            this.ImageBoxOneFormTwo.Margin = new System.Windows.Forms.Padding(4);
            this.ImageBoxOneFormTwo.MaximumSize = new System.Drawing.Size(910, 820);
            this.ImageBoxOneFormTwo.Name = "ImageBoxOneFormTwo";
            this.ImageBoxOneFormTwo.Size = new System.Drawing.Size(910, 820);
            this.ImageBoxOneFormTwo.TabIndex = 6;
            this.ImageBoxOneFormTwo.TabStop = false;
            this.ImageBoxOneFormTwo.Paint += new System.Windows.Forms.PaintEventHandler(this.OnPaint);
            this.ImageBoxOneFormTwo.MouseDown += new System.Windows.Forms.MouseEventHandler(this.OnMouseDown);
            this.ImageBoxOneFormTwo.MouseMove += new System.Windows.Forms.MouseEventHandler(this.OnMouseMove);
            this.ImageBoxOneFormTwo.MouseUp += new System.Windows.Forms.MouseEventHandler(this.OnMouseUp);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(803, 47);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(64, 25);
            this.label1.TabIndex = 1;
            this.label1.Text = "label1";
            // 
            // FormTwo
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1380, 844);
            this.Controls.Add(this.ImageBoxOneFormTwo);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.grpBxToolsSelectNucleiFormTwo);
            this.Controls.Add(this.grpBxFormTwo);
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Name = "FormTwo";
            this.Text = "FormTwo";
            this.grpBxFormTwo.ResumeLayout(false);
            this.grpBxFormTwo.PerformLayout();
            this.grpBxToolsSelectNucleiFormTwo.ResumeLayout(false);
            this.grpBxAddNucleus.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.ImageBoxOneFormTwo)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.GroupBox grpBxFormTwo;
        private System.Windows.Forms.Label lblInstructionsFormTwo;
        private System.Windows.Forms.GroupBox grpBxToolsSelectNucleiFormTwo;
        private System.Windows.Forms.GroupBox grpBxAddNucleus;
        private System.Windows.Forms.Panel pnlSelectNuclei;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button btnAddNucleus;
        private System.Windows.Forms.Panel panel1;
        public Emgu.CV.UI.ImageBox ImageBoxOneFormTwo;
        private System.Windows.Forms.Label label1;
    }
}