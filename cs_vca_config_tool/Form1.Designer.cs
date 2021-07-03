
namespace cs_vca_config_tool
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
            this.btnSelectZip = new System.Windows.Forms.Button();
            this.btnDownload = new System.Windows.Forms.Button();
            this.btnAddAndZip = new System.Windows.Forms.Button();
            this.lvTransmissions = new System.Windows.Forms.ListView();
            this.SuspendLayout();
            // 
            // btnSelectZip
            // 
            this.btnSelectZip.Location = new System.Drawing.Point(27, 25);
            this.btnSelectZip.Name = "btnSelectZip";
            this.btnSelectZip.Size = new System.Drawing.Size(100, 23);
            this.btnSelectZip.TabIndex = 0;
            this.btnSelectZip.Text = "Select Zip";
            this.btnSelectZip.UseVisualStyleBackColor = true;
            // 
            // btnDownload
            // 
            this.btnDownload.Location = new System.Drawing.Point(133, 25);
            this.btnDownload.Name = "btnDownload";
            this.btnDownload.Size = new System.Drawing.Size(100, 23);
            this.btnDownload.TabIndex = 1;
            this.btnDownload.Text = "Download";
            this.btnDownload.UseVisualStyleBackColor = true;
            // 
            // btnAddAndZip
            // 
            this.btnAddAndZip.Location = new System.Drawing.Point(239, 25);
            this.btnAddAndZip.Name = "btnAddAndZip";
            this.btnAddAndZip.Size = new System.Drawing.Size(100, 23);
            this.btnAddAndZip.TabIndex = 2;
            this.btnAddAndZip.Text = "Add and Zip File";
            this.btnAddAndZip.UseVisualStyleBackColor = true;
            // 
            // lvTransmissions
            // 
            this.lvTransmissions.CausesValidation = false;
            this.lvTransmissions.HideSelection = false;
            this.lvTransmissions.Location = new System.Drawing.Point(27, 54);
            this.lvTransmissions.Name = "lvTransmissions";
            this.lvTransmissions.Size = new System.Drawing.Size(312, 384);
            this.lvTransmissions.TabIndex = 3;
            this.lvTransmissions.UseCompatibleStateImageBehavior = false;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(384, 451);
            this.Controls.Add(this.lvTransmissions);
            this.Controls.Add(this.btnAddAndZip);
            this.Controls.Add(this.btnDownload);
            this.Controls.Add(this.btnSelectZip);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "Form1";
            this.Text = "VCA Config Tool";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnSelectZip;
        private System.Windows.Forms.Button btnDownload;
        private System.Windows.Forms.Button btnAddAndZip;
        private System.Windows.Forms.ListView lvTransmissions;
    }
}

