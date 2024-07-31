namespace ActStatter.UI
{
    partial class StatterViewStatsForm
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
            this.pnlGraph = new System.Windows.Forms.Panel();
            this.statGraph = new ActStatter.UI.StatterStatGraph();
            this.pnlGraphControls = new System.Windows.Forms.Panel();
            this.lblNotes = new System.Windows.Forms.Label();
            this.chkShowAverage = new System.Windows.Forms.CheckBox();
            this.sliderEncDpsResolution = new System.Windows.Forms.TrackBar();
            this.lblResolution = new System.Windows.Forms.Label();
            this.cmbShowValues = new System.Windows.Forms.ComboBox();
            this.lblShowVals = new System.Windows.Forms.Label();
            this.btnHelp = new System.Windows.Forms.Button();
            this.lblPlayer = new System.Windows.Forms.Label();
            this.btnCreateData = new System.Windows.Forms.Button();
            this.lbPlayers = new System.Windows.Forms.CheckedListBox();
            this.pnlExtraControls = new System.Windows.Forms.Panel();
            this.dgStats = new ActStatter.UI.StatterFastDataGrid();
            this.ColStat = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Player = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColMin = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColMax = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Avg = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.OC = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.label1 = new System.Windows.Forms.Label();
            this.cmbYAxis = new System.Windows.Forms.ComboBox();
            this.chkShowRange = new System.Windows.Forms.CheckBox();
            this.pnlGraph.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.sliderEncDpsResolution)).BeginInit();
            this.pnlExtraControls.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgStats)).BeginInit();
            this.SuspendLayout();
            // 
            // pnlGraph
            // 
            this.pnlGraph.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlGraph.Controls.Add(this.statGraph);
            this.pnlGraph.Controls.Add(this.pnlGraphControls);
            this.pnlGraph.Location = new System.Drawing.Point(347, 22);
            this.pnlGraph.Name = "pnlGraph";
            this.pnlGraph.Padding = new System.Windows.Forms.Padding(5);
            this.pnlGraph.Size = new System.Drawing.Size(637, 604);
            this.pnlGraph.TabIndex = 2;
            // 
            // statGraph
            // 
            this.statGraph.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.statGraph.Dock = System.Windows.Forms.DockStyle.Fill;
            this.statGraph.Location = new System.Drawing.Point(5, 5);
            this.statGraph.Name = "statGraph";
            this.statGraph.Size = new System.Drawing.Size(627, 594);
            this.statGraph.TabIndex = 1;
            // 
            // pnlGraphControls
            // 
            this.pnlGraphControls.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlGraphControls.Location = new System.Drawing.Point(5, 5);
            this.pnlGraphControls.Name = "pnlGraphControls";
            this.pnlGraphControls.Size = new System.Drawing.Size(627, 0);
            this.pnlGraphControls.TabIndex = 0;
            // 
            // lblNotes
            // 
            this.lblNotes.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblNotes.Location = new System.Drawing.Point(12, 455);
            this.lblNotes.Name = "lblNotes";
            this.lblNotes.Size = new System.Drawing.Size(329, 204);
            this.lblNotes.TabIndex = 3;
            this.lblNotes.Text = "lblNotes is modified on form load\r\n";
            // 
            // chkShowAverage
            // 
            this.chkShowAverage.AutoSize = true;
            this.chkShowAverage.Location = new System.Drawing.Point(513, 7);
            this.chkShowAverage.Name = "chkShowAverage";
            this.chkShowAverage.Size = new System.Drawing.Size(96, 17);
            this.chkShowAverage.TabIndex = 5;
            this.chkShowAverage.Text = "Show Average";
            this.chkShowAverage.UseVisualStyleBackColor = true;
            this.chkShowAverage.CheckedChanged += new System.EventHandler(this.chkShowAverage_CheckedChanged);
            // 
            // sliderEncDpsResolution
            // 
            this.sliderEncDpsResolution.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.sliderEncDpsResolution.AutoSize = false;
            this.sliderEncDpsResolution.Location = new System.Drawing.Point(133, 4);
            this.sliderEncDpsResolution.Minimum = 1;
            this.sliderEncDpsResolution.Name = "sliderEncDpsResolution";
            this.sliderEncDpsResolution.Size = new System.Drawing.Size(104, 30);
            this.sliderEncDpsResolution.TabIndex = 6;
            this.sliderEncDpsResolution.Value = 5;
            this.sliderEncDpsResolution.Scroll += new System.EventHandler(this.sliderEncDpsResolution_Scroll);
            // 
            // lblResolution
            // 
            this.lblResolution.AutoSize = true;
            this.lblResolution.Location = new System.Drawing.Point(231, 8);
            this.lblResolution.Name = "lblResolution";
            this.lblResolution.Size = new System.Drawing.Size(116, 13);
            this.lblResolution.TabIndex = 7;
            this.lblResolution.Text = "Period (1 - 10 seconds)";
            // 
            // cmbShowValues
            // 
            this.cmbShowValues.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbShowValues.FormattingEnabled = true;
            this.cmbShowValues.Items.AddRange(new object[] {
            "None",
            "DPS",
            "HPS"});
            this.cmbShowValues.Location = new System.Drawing.Point(65, 5);
            this.cmbShowValues.Name = "cmbShowValues";
            this.cmbShowValues.Size = new System.Drawing.Size(70, 21);
            this.cmbShowValues.TabIndex = 9;
            this.cmbShowValues.SelectedIndexChanged += new System.EventHandler(this.cmbShowValues_SelectedIndexChanged);
            // 
            // lblShowVals
            // 
            this.lblShowVals.AutoSize = true;
            this.lblShowVals.Location = new System.Drawing.Point(4, 8);
            this.lblShowVals.Name = "lblShowVals";
            this.lblShowVals.Size = new System.Drawing.Size(56, 13);
            this.lblShowVals.TabIndex = 10;
            this.lblShowVals.Text = "Show Enc";
            // 
            // btnHelp
            // 
            this.btnHelp.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnHelp.Location = new System.Drawing.Point(943, 628);
            this.btnHelp.Name = "btnHelp";
            this.btnHelp.Size = new System.Drawing.Size(29, 23);
            this.btnHelp.TabIndex = 12;
            this.btnHelp.Text = "?";
            this.btnHelp.UseVisualStyleBackColor = true;
            this.btnHelp.Click += new System.EventHandler(this.btnHelp_Click);
            // 
            // lblPlayer
            // 
            this.lblPlayer.AutoSize = true;
            this.lblPlayer.Location = new System.Drawing.Point(5, 8);
            this.lblPlayer.Name = "lblPlayer";
            this.lblPlayer.Size = new System.Drawing.Size(87, 13);
            this.lblPlayer.TabIndex = 13;
            this.lblPlayer.Text = "Include player(s):";
            // 
            // btnCreateData
            // 
            this.btnCreateData.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnCreateData.Location = new System.Drawing.Point(2, 636);
            this.btnCreateData.Name = "btnCreateData";
            this.btnCreateData.Size = new System.Drawing.Size(75, 23);
            this.btnCreateData.TabIndex = 15;
            this.btnCreateData.Text = "Create Data";
            this.btnCreateData.UseVisualStyleBackColor = true;
            this.btnCreateData.Click += new System.EventHandler(this.btnCreateData_Click);
            // 
            // lbPlayers
            // 
            this.lbPlayers.CheckOnClick = true;
            this.lbPlayers.FormattingEnabled = true;
            this.lbPlayers.IntegralHeight = false;
            this.lbPlayers.Location = new System.Drawing.Point(6, 27);
            this.lbPlayers.Name = "lbPlayers";
            this.lbPlayers.Size = new System.Drawing.Size(335, 141);
            this.lbPlayers.TabIndex = 16;
            this.lbPlayers.SelectedIndexChanged += new System.EventHandler(this.lbPlayers_SelectedIndexChanged);
            this.lbPlayers.MouseUp += new System.Windows.Forms.MouseEventHandler(this.lbPlayers_MouseUp);
            // 
            // pnlExtraControls
            // 
            this.pnlExtraControls.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.pnlExtraControls.Controls.Add(this.lblResolution);
            this.pnlExtraControls.Controls.Add(this.cmbShowValues);
            this.pnlExtraControls.Controls.Add(this.lblShowVals);
            this.pnlExtraControls.Controls.Add(this.sliderEncDpsResolution);
            this.pnlExtraControls.Location = new System.Drawing.Point(347, 625);
            this.pnlExtraControls.Name = "pnlExtraControls";
            this.pnlExtraControls.Size = new System.Drawing.Size(466, 38);
            this.pnlExtraControls.TabIndex = 17;
            // 
            // dgStats
            // 
            this.dgStats.AllowUserToAddRows = false;
            this.dgStats.AllowUserToDeleteRows = false;
            this.dgStats.AllowUserToResizeColumns = false;
            this.dgStats.AllowUserToResizeRows = false;
            this.dgStats.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.dgStats.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgStats.BackgroundColor = System.Drawing.SystemColors.Window;
            this.dgStats.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dgStats.ColumnHeadersHeight = 24;
            this.dgStats.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.dgStats.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ColStat,
            this.Player,
            this.ColMin,
            this.ColMax,
            this.Avg,
            this.OC});
            this.dgStats.GridColor = System.Drawing.SystemColors.ControlLight;
            this.dgStats.Location = new System.Drawing.Point(6, 174);
            this.dgStats.Name = "dgStats";
            this.dgStats.ReadOnly = true;
            this.dgStats.RowHeadersVisible = false;
            this.dgStats.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.dgStats.ShowCellErrors = false;
            this.dgStats.ShowCellToolTips = false;
            this.dgStats.ShowEditingIcon = false;
            this.dgStats.ShowRowErrors = false;
            this.dgStats.Size = new System.Drawing.Size(335, 268);
            this.dgStats.TabIndex = 1;
            this.dgStats.DataBindingComplete += new System.Windows.Forms.DataGridViewBindingCompleteEventHandler(this.dgStats_DataBindingComplete);
            this.dgStats.SelectionChanged += new System.EventHandler(this.dgStats_SelectionChanged);
            // 
            // ColStat
            // 
            this.ColStat.DataPropertyName = "Stat";
            this.ColStat.FillWeight = 35F;
            this.ColStat.HeaderText = "Stat";
            this.ColStat.Name = "ColStat";
            this.ColStat.ReadOnly = true;
            this.ColStat.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.ColStat.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // Player
            // 
            this.Player.DataPropertyName = "Player";
            this.Player.FillWeight = 30F;
            this.Player.HeaderText = "Player";
            this.Player.Name = "Player";
            this.Player.ReadOnly = true;
            this.Player.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.Player.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // ColMin
            // 
            this.ColMin.DataPropertyName = "Min";
            this.ColMin.FillWeight = 20F;
            this.ColMin.HeaderText = "Min";
            this.ColMin.Name = "ColMin";
            this.ColMin.ReadOnly = true;
            this.ColMin.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.ColMin.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // ColMax
            // 
            this.ColMax.DataPropertyName = "Max";
            this.ColMax.FillWeight = 20F;
            this.ColMax.HeaderText = "Max";
            this.ColMax.Name = "ColMax";
            this.ColMax.ReadOnly = true;
            this.ColMax.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.ColMax.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // Avg
            // 
            this.Avg.DataPropertyName = "Avg";
            this.Avg.FillWeight = 20F;
            this.Avg.HeaderText = "Avg";
            this.Avg.Name = "Avg";
            this.Avg.ReadOnly = true;
            this.Avg.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.Avg.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // OC
            // 
            this.OC.DataPropertyName = "OC";
            this.OC.FillWeight = 20F;
            this.OC.HeaderText = "OC";
            this.OC.Name = "OC";
            this.OC.ReadOnly = true;
            this.OC.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.OC.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(351, 8);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(72, 13);
            this.label1.TabIndex = 18;
            this.label1.Text = "Start Y-axis at";
            // 
            // cmbYAxis
            // 
            this.cmbYAxis.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbYAxis.FormattingEnabled = true;
            this.cmbYAxis.Items.AddRange(new object[] {
            "0",
            "Min Val"});
            this.cmbYAxis.Location = new System.Drawing.Point(429, 4);
            this.cmbYAxis.Name = "cmbYAxis";
            this.cmbYAxis.Size = new System.Drawing.Size(58, 21);
            this.cmbYAxis.TabIndex = 19;
            this.cmbYAxis.SelectedIndexChanged += new System.EventHandler(this.cmbYAxis_SelectedIndexChanged);
            // 
            // chkShowRange
            // 
            this.chkShowRange.AutoSize = true;
            this.chkShowRange.Location = new System.Drawing.Point(624, 6);
            this.chkShowRange.Name = "chkShowRange";
            this.chkShowRange.Size = new System.Drawing.Size(93, 17);
            this.chkShowRange.TabIndex = 20;
            this.chkShowRange.Text = "Show Ranges";
            this.chkShowRange.UseVisualStyleBackColor = true;
            this.chkShowRange.CheckedChanged += new System.EventHandler(this.chkShowRange_CheckedChanged);
            // 
            // StatterViewStatsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(984, 661);
            this.Controls.Add(this.chkShowRange);
            this.Controls.Add(this.chkShowAverage);
            this.Controls.Add(this.cmbYAxis);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.pnlExtraControls);
            this.Controls.Add(this.lbPlayers);
            this.Controls.Add(this.btnCreateData);
            this.Controls.Add(this.lblPlayer);
            this.Controls.Add(this.btnHelp);
            this.Controls.Add(this.lblNotes);
            this.Controls.Add(this.pnlGraph);
            this.Controls.Add(this.dgStats);
            this.KeyPreview = true;
            this.MinimumSize = new System.Drawing.Size(880, 480);
            this.Name = "StatterViewStatsForm";
            this.ShowIcon = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "View Stats";
            this.Load += new System.EventHandler(this.StatterViewStatsForm_Load);
            this.Shown += new System.EventHandler(this.StatterViewStatsForm_Shown);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.StatterViewStatsForm_KeyDown);
            this.Move += new System.EventHandler(this.StatterViewStatsForm_SizeOrLocationChanged);
            this.Resize += new System.EventHandler(this.StatterViewStatsForm_SizeOrLocationChanged);
            this.pnlGraph.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.sliderEncDpsResolution)).EndInit();
            this.pnlExtraControls.ResumeLayout(false);
            this.pnlExtraControls.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgStats)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private StatterFastDataGrid dgStats;
        private System.Windows.Forms.Panel pnlGraph;
        private System.Windows.Forms.Panel pnlGraphControls;
        private StatterStatGraph statGraph;
        private System.Windows.Forms.Label lblNotes;
        private System.Windows.Forms.CheckBox chkShowAverage;
        private System.Windows.Forms.TrackBar sliderEncDpsResolution;
        private System.Windows.Forms.Label lblResolution;
        private System.Windows.Forms.ComboBox cmbShowValues;
        private System.Windows.Forms.Label lblShowVals;
        private System.Windows.Forms.Button btnHelp;
        private System.Windows.Forms.Label lblPlayer;
        private System.Windows.Forms.Button btnCreateData;
        private System.Windows.Forms.CheckedListBox lbPlayers;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColStat;
        private System.Windows.Forms.DataGridViewTextBoxColumn Player;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColMin;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColMax;
        private System.Windows.Forms.DataGridViewTextBoxColumn Avg;
        private System.Windows.Forms.DataGridViewTextBoxColumn OC;
        private System.Windows.Forms.Panel pnlExtraControls;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cmbYAxis;
        private System.Windows.Forms.CheckBox chkShowRange;
    }
}