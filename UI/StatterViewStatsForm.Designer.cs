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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(StatterViewStatsForm));
            this.pnlGraph = new System.Windows.Forms.Panel();
            this.statGraph = new ActStatter.UI.StatterStatGraph();
            this.pnlGraphControls = new System.Windows.Forms.Panel();
            this.lblNotes = new System.Windows.Forms.Label();
            this.chkShowEncDps = new System.Windows.Forms.CheckBox();
            this.dgStats = new ActStatter.UI.StatterFastDataGrid();
            this.ColStat = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColMin = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColMax = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Avg = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.OC = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.chkShowAverage = new System.Windows.Forms.CheckBox();
            this.pnlGraph.SuspendLayout();
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
            this.pnlGraph.Location = new System.Drawing.Point(347, 6);
            this.pnlGraph.Name = "pnlGraph";
            this.pnlGraph.Padding = new System.Windows.Forms.Padding(5);
            this.pnlGraph.Size = new System.Drawing.Size(650, 677);
            this.pnlGraph.TabIndex = 2;
            // 
            // statGraph
            // 
            this.statGraph.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.statGraph.Dock = System.Windows.Forms.DockStyle.Fill;
            this.statGraph.Location = new System.Drawing.Point(5, 5);
            this.statGraph.Name = "statGraph";
            this.statGraph.Size = new System.Drawing.Size(640, 667);
            this.statGraph.TabIndex = 1;
            // 
            // pnlGraphControls
            // 
            this.pnlGraphControls.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlGraphControls.Location = new System.Drawing.Point(5, 5);
            this.pnlGraphControls.Name = "pnlGraphControls";
            this.pnlGraphControls.Size = new System.Drawing.Size(640, 0);
            this.pnlGraphControls.TabIndex = 0;
            // 
            // lblNotes
            // 
            this.lblNotes.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblNotes.Location = new System.Drawing.Point(12, 512);
            this.lblNotes.Name = "lblNotes";
            this.lblNotes.Size = new System.Drawing.Size(329, 201);
            this.lblNotes.TabIndex = 3;
            this.lblNotes.Text = resources.GetString("lblNotes.Text");
            // 
            // chkShowEncDps
            // 
            this.chkShowEncDps.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.chkShowEncDps.AutoSize = true;
            this.chkShowEncDps.Checked = true;
            this.chkShowEncDps.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkShowEncDps.Location = new System.Drawing.Point(483, 689);
            this.chkShowEncDps.Name = "chkShowEncDps";
            this.chkShowEncDps.Size = new System.Drawing.Size(97, 17);
            this.chkShowEncDps.TabIndex = 4;
            this.chkShowEncDps.Text = "Show EncDPS";
            this.chkShowEncDps.UseVisualStyleBackColor = true;
            this.chkShowEncDps.CheckedChanged += new System.EventHandler(this.chkShowEncDps_CheckedChanged);
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
            this.ColMin,
            this.ColMax,
            this.Avg,
            this.OC});
            this.dgStats.GridColor = System.Drawing.SystemColors.ControlLight;
            this.dgStats.Location = new System.Drawing.Point(6, 6);
            this.dgStats.MultiSelect = false;
            this.dgStats.Name = "dgStats";
            this.dgStats.ReadOnly = true;
            this.dgStats.RowHeadersVisible = false;
            this.dgStats.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgStats.ShowCellErrors = false;
            this.dgStats.ShowCellToolTips = false;
            this.dgStats.ShowEditingIcon = false;
            this.dgStats.ShowRowErrors = false;
            this.dgStats.Size = new System.Drawing.Size(335, 493);
            this.dgStats.TabIndex = 1;
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
            this.Avg.HeaderText = "Average";
            this.Avg.Name = "Avg";
            this.Avg.ReadOnly = true;
            this.Avg.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.Avg.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // OC
            // 
            this.OC.DataPropertyName = "OC";
            this.OC.FillWeight = 20F;
            this.OC.HeaderText = "Overcap";
            this.OC.Name = "OC";
            this.OC.ReadOnly = true;
            this.OC.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.OC.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // chkShowAverage
            // 
            this.chkShowAverage.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.chkShowAverage.AutoSize = true;
            this.chkShowAverage.Checked = true;
            this.chkShowAverage.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkShowAverage.Location = new System.Drawing.Point(361, 689);
            this.chkShowAverage.Name = "chkShowAverage";
            this.chkShowAverage.Size = new System.Drawing.Size(96, 17);
            this.chkShowAverage.TabIndex = 5;
            this.chkShowAverage.Text = "Show Average";
            this.chkShowAverage.UseVisualStyleBackColor = true;
            this.chkShowAverage.CheckedChanged += new System.EventHandler(this.chkShowAverage_CheckedChanged);
            // 
            // StatterViewStatsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(997, 718);
            this.Controls.Add(this.chkShowAverage);
            this.Controls.Add(this.chkShowEncDps);
            this.Controls.Add(this.lblNotes);
            this.Controls.Add(this.pnlGraph);
            this.Controls.Add(this.dgStats);
            this.MinimumSize = new System.Drawing.Size(512, 256);
            this.Name = "StatterViewStatsForm";
            this.ShowIcon = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "View Stats";
            this.Load += new System.EventHandler(this.ViewStats_Load);
            this.pnlGraph.ResumeLayout(false);
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
        private System.Windows.Forms.DataGridViewTextBoxColumn ColStat;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColMin;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColMax;
        private System.Windows.Forms.DataGridViewTextBoxColumn Avg;
        private System.Windows.Forms.DataGridViewTextBoxColumn OC;
        private System.Windows.Forms.CheckBox chkShowEncDps;
        private System.Windows.Forms.CheckBox chkShowAverage;
    }
}