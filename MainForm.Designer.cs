using System;

namespace Hackground
{
    partial class MainForm
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
            this.ColorButton = new System.Windows.Forms.Button();
            this.ImageButton = new System.Windows.Forms.Button();
            this.StopButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // ColorButton
            // 
            this.ColorButton.Location = new System.Drawing.Point(12, 12);
            this.ColorButton.Name = "ColorButton";
            this.ColorButton.Size = new System.Drawing.Size(260, 50);
            this.ColorButton.TabIndex = 0;
            this.ColorButton.Text = "Farve";
            this.ColorButton.UseVisualStyleBackColor = true;
            this.ColorButton.Click += new System.EventHandler(this.ColorButton_Click);
            // 
            // ImageButton
            // 
            this.ImageButton.Location = new System.Drawing.Point(12, 68);
            this.ImageButton.Name = "ImageButton";
            this.ImageButton.Size = new System.Drawing.Size(260, 50);
            this.ImageButton.TabIndex = 1;
            this.ImageButton.Text = "Billede";
            this.ImageButton.UseVisualStyleBackColor = true;
            this.ImageButton.Click += new System.EventHandler(this.ImageButton_Click);
            // 
            // StopButton
            // 
            this.StopButton.Location = new System.Drawing.Point(12, 126);
            this.StopButton.Name = "StopButton";
            this.StopButton.Size = new System.Drawing.Size(260, 50);
            this.StopButton.TabIndex = 2;
            this.StopButton.Text = "Anuller";
            this.StopButton.UseVisualStyleBackColor = true;
            this.StopButton.Click += new System.EventHandler(this.StopButton_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(284, 188);
            this.Controls.Add(this.StopButton);
            this.Controls.Add(this.ImageButton);
            this.Controls.Add(this.ColorButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.Text = "Hackground";
            this.Load += new System.EventHandler(this.HackgroundGui_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button ColorButton;
        private System.Windows.Forms.Button ImageButton;
        private System.Windows.Forms.Button StopButton;
    }
}