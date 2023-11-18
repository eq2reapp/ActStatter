namespace ActStatter.UI
{
    partial class StatterPluginTab
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
            this.pnlMain = new System.Windows.Forms.Panel();
            this.grpStatterConfig = new System.Windows.Forms.GroupBox();
            this.grpGeneralOptions = new System.Windows.Forms.GroupBox();
            this.chkParseOnImport = new System.Windows.Forms.CheckBox();
            this.pnlInstructionsContainer = new System.Windows.Forms.Panel();
            this.tabExtra = new System.Windows.Forms.TabControl();
            this.tabInstructions = new System.Windows.Forms.TabPage();
            this.pnlInstructions = new System.Windows.Forms.Panel();
            this.lnkOpenHelp = new System.Windows.Forms.LinkLabel();
            this.tabLogs = new System.Windows.Forms.TabPage();
            this.btnClearLogs = new System.Windows.Forms.Button();
            this.btnRefresh = new System.Windows.Forms.Button();
            this.btnCopyLogs = new System.Windows.Forms.Button();
            this.txtLogs = new System.Windows.Forms.TextBox();
            this.grpTrackedStats = new System.Windows.Forms.GroupBox();
            this.pnlStatDetails = new System.Windows.Forms.FlowLayoutPanel();
            this.btnAdd = new System.Windows.Forms.Button();
            this.chkRestrictChannels = new System.Windows.Forms.CheckBox();
            this.txtRestrictChannels = new System.Windows.Forms.TextBox();
            this.pnlMain.SuspendLayout();
            this.grpStatterConfig.SuspendLayout();
            this.grpGeneralOptions.SuspendLayout();
            this.pnlInstructionsContainer.SuspendLayout();
            this.tabExtra.SuspendLayout();
            this.tabInstructions.SuspendLayout();
            this.pnlInstructions.SuspendLayout();
            this.tabLogs.SuspendLayout();
            this.grpTrackedStats.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlMain
            // 
            this.pnlMain.Controls.Add(this.grpStatterConfig);
            this.pnlMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlMain.Location = new System.Drawing.Point(0, 0);
            this.pnlMain.Name = "pnlMain";
            this.pnlMain.Size = new System.Drawing.Size(992, 685);
            this.pnlMain.TabIndex = 2;
            // 
            // grpStatterConfig
            // 
            this.grpStatterConfig.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grpStatterConfig.Controls.Add(this.grpGeneralOptions);
            this.grpStatterConfig.Controls.Add(this.pnlInstructionsContainer);
            this.grpStatterConfig.Controls.Add(this.grpTrackedStats);
            this.grpStatterConfig.Location = new System.Drawing.Point(13, 11);
            this.grpStatterConfig.Name = "grpStatterConfig";
            this.grpStatterConfig.Size = new System.Drawing.Size(966, 661);
            this.grpStatterConfig.TabIndex = 1;
            this.grpStatterConfig.TabStop = false;
            this.grpStatterConfig.Text = "Statter Configuration";
            // 
            // grpGeneralOptions
            // 
            this.grpGeneralOptions.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.grpGeneralOptions.Controls.Add(this.txtRestrictChannels);
            this.grpGeneralOptions.Controls.Add(this.chkRestrictChannels);
            this.grpGeneralOptions.Controls.Add(this.chkParseOnImport);
            this.grpGeneralOptions.Location = new System.Drawing.Point(18, 455);
            this.grpGeneralOptions.Name = "grpGeneralOptions";
            this.grpGeneralOptions.Size = new System.Drawing.Size(327, 187);
            this.grpGeneralOptions.TabIndex = 15;
            this.grpGeneralOptions.TabStop = false;
            this.grpGeneralOptions.Text = "Options";
            // 
            // chkParseOnImport
            // 
            this.chkParseOnImport.AutoSize = true;
            this.chkParseOnImport.Location = new System.Drawing.Point(24, 25);
            this.chkParseOnImport.Name = "chkParseOnImport";
            this.chkParseOnImport.Size = new System.Drawing.Size(163, 17);
            this.chkParseOnImport.TabIndex = 0;
            this.chkParseOnImport.Text = "Parse stats during log imports";
            this.chkParseOnImport.UseVisualStyleBackColor = true;
            this.chkParseOnImport.CheckedChanged += new System.EventHandler(this.chkParseOnImport_CheckedChanged);
            // 
            // pnlInstructionsContainer
            // 
            this.pnlInstructionsContainer.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlInstructionsContainer.BackColor = System.Drawing.SystemColors.ControlLight;
            this.pnlInstructionsContainer.Controls.Add(this.tabExtra);
            this.pnlInstructionsContainer.Location = new System.Drawing.Point(367, 28);
            this.pnlInstructionsContainer.Name = "pnlInstructionsContainer";
            this.pnlInstructionsContainer.Padding = new System.Windows.Forms.Padding(1);
            this.pnlInstructionsContainer.Size = new System.Drawing.Size(581, 614);
            this.pnlInstructionsContainer.TabIndex = 14;
            // 
            // tabExtra
            // 
            this.tabExtra.Controls.Add(this.tabInstructions);
            this.tabExtra.Controls.Add(this.tabLogs);
            this.tabExtra.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabExtra.Location = new System.Drawing.Point(1, 1);
            this.tabExtra.Name = "tabExtra";
            this.tabExtra.SelectedIndex = 0;
            this.tabExtra.Size = new System.Drawing.Size(579, 612);
            this.tabExtra.TabIndex = 16;
            this.tabExtra.SelectedIndexChanged += new System.EventHandler(this.tabExtra_SelectedIndexChanged);
            // 
            // tabInstructions
            // 
            this.tabInstructions.Controls.Add(this.pnlInstructions);
            this.tabInstructions.Location = new System.Drawing.Point(4, 22);
            this.tabInstructions.Name = "tabInstructions";
            this.tabInstructions.Padding = new System.Windows.Forms.Padding(3);
            this.tabInstructions.Size = new System.Drawing.Size(571, 586);
            this.tabInstructions.TabIndex = 0;
            this.tabInstructions.Text = "Instructions";
            this.tabInstructions.UseVisualStyleBackColor = true;
            // 
            // pnlInstructions
            // 
            this.pnlInstructions.BackColor = System.Drawing.SystemColors.Window;
            this.pnlInstructions.Controls.Add(this.lnkOpenHelp);
            this.pnlInstructions.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlInstructions.Location = new System.Drawing.Point(3, 3);
            this.pnlInstructions.Name = "pnlInstructions";
            this.pnlInstructions.Padding = new System.Windows.Forms.Padding(5);
            this.pnlInstructions.Size = new System.Drawing.Size(565, 580);
            this.pnlInstructions.TabIndex = 13;
            // 
            // lnkOpenHelp
            // 
            this.lnkOpenHelp.ActiveLinkColor = System.Drawing.Color.Blue;
            this.lnkOpenHelp.AutoSize = true;
            this.lnkOpenHelp.Location = new System.Drawing.Point(18, 18);
            this.lnkOpenHelp.Name = "lnkOpenHelp";
            this.lnkOpenHelp.Size = new System.Drawing.Size(166, 13);
            this.lnkOpenHelp.TabIndex = 0;
            this.lnkOpenHelp.TabStop = true;
            this.lnkOpenHelp.Text = "Open a help page in your browser";
            this.lnkOpenHelp.VisitedLinkColor = System.Drawing.Color.Blue;
            this.lnkOpenHelp.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnkOpenHelp_LinkClicked);
            // 
            // tabLogs
            // 
            this.tabLogs.Controls.Add(this.btnClearLogs);
            this.tabLogs.Controls.Add(this.btnRefresh);
            this.tabLogs.Controls.Add(this.btnCopyLogs);
            this.tabLogs.Controls.Add(this.txtLogs);
            this.tabLogs.Location = new System.Drawing.Point(4, 22);
            this.tabLogs.Name = "tabLogs";
            this.tabLogs.Padding = new System.Windows.Forms.Padding(3);
            this.tabLogs.Size = new System.Drawing.Size(571, 586);
            this.tabLogs.TabIndex = 1;
            this.tabLogs.Text = "Logs";
            this.tabLogs.UseVisualStyleBackColor = true;
            // 
            // btnClearLogs
            // 
            this.btnClearLogs.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnClearLogs.Location = new System.Drawing.Point(168, 556);
            this.btnClearLogs.Name = "btnClearLogs";
            this.btnClearLogs.Size = new System.Drawing.Size(75, 23);
            this.btnClearLogs.TabIndex = 3;
            this.btnClearLogs.Text = "Clear";
            this.btnClearLogs.UseVisualStyleBackColor = true;
            this.btnClearLogs.Click += new System.EventHandler(this.btnClearLogs_Click);
            // 
            // btnRefresh
            // 
            this.btnRefresh.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnRefresh.Location = new System.Drawing.Point(6, 556);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(75, 23);
            this.btnRefresh.TabIndex = 2;
            this.btnRefresh.Text = "Refresh";
            this.btnRefresh.UseVisualStyleBackColor = true;
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
            // 
            // btnCopyLogs
            // 
            this.btnCopyLogs.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnCopyLogs.Location = new System.Drawing.Point(87, 556);
            this.btnCopyLogs.Name = "btnCopyLogs";
            this.btnCopyLogs.Size = new System.Drawing.Size(75, 23);
            this.btnCopyLogs.TabIndex = 1;
            this.btnCopyLogs.Text = "Copy";
            this.btnCopyLogs.UseVisualStyleBackColor = true;
            this.btnCopyLogs.Click += new System.EventHandler(this.btnCopyLogs_Click);
            // 
            // txtLogs
            // 
            this.txtLogs.BackColor = System.Drawing.SystemColors.Window;
            this.txtLogs.Location = new System.Drawing.Point(6, 6);
            this.txtLogs.Multiline = true;
            this.txtLogs.Name = "txtLogs";
            this.txtLogs.ReadOnly = true;
            this.txtLogs.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtLogs.Size = new System.Drawing.Size(559, 545);
            this.txtLogs.TabIndex = 0;
            // 
            // grpTrackedStats
            // 
            this.grpTrackedStats.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.grpTrackedStats.Controls.Add(this.pnlStatDetails);
            this.grpTrackedStats.Controls.Add(this.btnAdd);
            this.grpTrackedStats.Location = new System.Drawing.Point(18, 22);
            this.grpTrackedStats.Name = "grpTrackedStats";
            this.grpTrackedStats.Size = new System.Drawing.Size(327, 422);
            this.grpTrackedStats.TabIndex = 10;
            this.grpTrackedStats.TabStop = false;
            this.grpTrackedStats.Text = "Tracked Stats";
            // 
            // pnlStatDetails
            // 
            this.pnlStatDetails.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlStatDetails.AutoScroll = true;
            this.pnlStatDetails.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.pnlStatDetails.Location = new System.Drawing.Point(73, 19);
            this.pnlStatDetails.Name = "pnlStatDetails";
            this.pnlStatDetails.Size = new System.Drawing.Size(248, 397);
            this.pnlStatDetails.TabIndex = 2;
            this.pnlStatDetails.WrapContents = false;
            // 
            // btnAdd
            // 
            this.btnAdd.Location = new System.Drawing.Point(23, 23);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(32, 23);
            this.btnAdd.TabIndex = 1;
            this.btnAdd.Text = "+";
            this.btnAdd.UseVisualStyleBackColor = true;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // chkRestrictChannels
            // 
            this.chkRestrictChannels.AutoSize = true;
            this.chkRestrictChannels.Location = new System.Drawing.Point(24, 48);
            this.chkRestrictChannels.Name = "chkRestrictChannels";
            this.chkRestrictChannels.Size = new System.Drawing.Size(179, 17);
            this.chkRestrictChannels.TabIndex = 1;
            this.chkRestrictChannels.Text = "Only parse stats from channel(s):";
            this.chkRestrictChannels.UseVisualStyleBackColor = true;
            this.chkRestrictChannels.CheckedChanged += new System.EventHandler(this.chkRestrictChannel_CheckedChanged);
            // 
            // txtRestrictChannels
            // 
            this.txtRestrictChannels.Location = new System.Drawing.Point(44, 66);
            this.txtRestrictChannels.Multiline = true;
            this.txtRestrictChannels.Name = "txtRestrictChannels";
            this.txtRestrictChannels.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtRestrictChannels.Size = new System.Drawing.Size(265, 71);
            this.txtRestrictChannels.TabIndex = 2;
            this.txtRestrictChannels.Leave += new System.EventHandler(this.txtRestrictChannels_Leave);
            // 
            // StatterPluginTab
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.pnlMain);
            this.Name = "StatterPluginTab";
            this.Size = new System.Drawing.Size(992, 685);
            this.Load += new System.EventHandler(this.StatterUI_Load);
            this.pnlMain.ResumeLayout(false);
            this.grpStatterConfig.ResumeLayout(false);
            this.grpGeneralOptions.ResumeLayout(false);
            this.grpGeneralOptions.PerformLayout();
            this.pnlInstructionsContainer.ResumeLayout(false);
            this.tabExtra.ResumeLayout(false);
            this.tabInstructions.ResumeLayout(false);
            this.pnlInstructions.ResumeLayout(false);
            this.pnlInstructions.PerformLayout();
            this.tabLogs.ResumeLayout(false);
            this.tabLogs.PerformLayout();
            this.grpTrackedStats.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pnlMain;
        private System.Windows.Forms.GroupBox grpStatterConfig;
        private System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.FlowLayoutPanel pnlStatDetails;
        private System.Windows.Forms.GroupBox grpTrackedStats;
        private System.Windows.Forms.Panel pnlInstructions;
        private System.Windows.Forms.Panel pnlInstructionsContainer;
        private System.Windows.Forms.GroupBox grpGeneralOptions;
        protected internal System.Windows.Forms.CheckBox chkParseOnImport;
        private System.Windows.Forms.TabControl tabExtra;
        private System.Windows.Forms.TabPage tabInstructions;
        private System.Windows.Forms.TabPage tabLogs;
        private System.Windows.Forms.TextBox txtLogs;
        private System.Windows.Forms.Button btnCopyLogs;
        private System.Windows.Forms.Button btnRefresh;
        private System.Windows.Forms.LinkLabel lnkOpenHelp;
        private System.Windows.Forms.Button btnClearLogs;
        private System.Windows.Forms.TextBox txtRestrictChannels;
        protected internal System.Windows.Forms.CheckBox chkRestrictChannels;
    }
}
