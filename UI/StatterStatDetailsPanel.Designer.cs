using Advanced_Combat_Tracker;

namespace ActStatter.UI
{
    partial class StatterStatDetailsPanel
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.pnlColour = new System.Windows.Forms.Panel();
            this.lblName = new System.Windows.Forms.Label();
            this.dlgColour = new System.Windows.Forms.ColorDialog();
            this.pnlMain = new System.Windows.Forms.Panel();
            this.btnDelete = new ButtonPainted();
            this.pnlMain.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlColour
            // 
            this.pnlColour.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlColour.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.pnlColour.Location = new System.Drawing.Point(140, 4);
            this.pnlColour.Margin = new System.Windows.Forms.Padding(0);
            this.pnlColour.Name = "pnlColour";
            this.pnlColour.Size = new System.Drawing.Size(29, 22);
            this.pnlColour.TabIndex = 1;
            this.pnlColour.Visible = false;
            this.pnlColour.Click += new System.EventHandler(this.pnlColour_Click);
            // 
            // lblName
            // 
            this.lblName.AutoSize = true;
            this.lblName.ForeColor = System.Drawing.SystemColors.WindowText;
            this.lblName.Location = new System.Drawing.Point(6, 8);
            this.lblName.Name = "lblName";
            this.lblName.Size = new System.Drawing.Size(35, 13);
            this.lblName.TabIndex = 2;
            this.lblName.Text = "Name";
            // 
            // dlgColour
            // 
            this.dlgColour.AnyColor = true;
            this.dlgColour.FullOpen = true;
            // 
            // pnlMain
            // 
            this.pnlMain.BackColor = System.Drawing.SystemColors.Window;
            this.pnlMain.Controls.Add(this.btnDelete);
            this.pnlMain.Controls.Add(this.lblName);
            this.pnlMain.Controls.Add(this.pnlColour);
            this.pnlMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlMain.Location = new System.Drawing.Point(1, 1);
            this.pnlMain.Name = "pnlMain";
            this.pnlMain.Size = new System.Drawing.Size(218, 29);
            this.pnlMain.TabIndex = 4;
            // 
            // btnDelete
            // 
            this.btnDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnDelete.Location = new System.Drawing.Point(183, 3);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(32, 23);
            this.btnDelete.TabIndex = 3;
            this.btnDelete.Text = "-";
            this.btnDelete.UseVisualStyleBackColor = true;
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // StatterStatDetailsPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ControlLight;
            this.Controls.Add(this.pnlMain);
            this.Margin = new System.Windows.Forms.Padding(1);
            this.Name = "StatterStatDetailsPanel";
            this.Padding = new System.Windows.Forms.Padding(1);
            this.Size = new System.Drawing.Size(220, 31);
            this.Load += new System.EventHandler(this.StatDetails_Load);
            this.pnlMain.ResumeLayout(false);
            this.pnlMain.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pnlColour;
        private System.Windows.Forms.Label lblName;
        private System.Windows.Forms.ColorDialog dlgColour;
        private System.Windows.Forms.Panel pnlMain;
        private ButtonPainted btnDelete;
    }
}
