namespace ACT_Plugin.UI
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
            this.dgStats = new StatterFastDataGrid();
            this.ColStat = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColMin = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColMax = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.pnlGraph = new System.Windows.Forms.Panel();
            this.statGraph = new StatterStatGraph();
            this.pnlGraphControls = new System.Windows.Forms.Panel();
            ((System.ComponentModel.ISupportInitialize)(this.dgStats)).BeginInit();
            this.pnlGraph.SuspendLayout();
            this.SuspendLayout();
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
            this.ColMax});
            this.dgStats.GridColor = System.Drawing.SystemColors.ControlLight;
            this.dgStats.Location = new System.Drawing.Point(6, 6);
            this.dgStats.Name = "dgStats";
            this.dgStats.ReadOnly = true;
            this.dgStats.RowHeadersVisible = false;
            this.dgStats.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgStats.ShowCellErrors = false;
            this.dgStats.ShowCellToolTips = false;
            this.dgStats.ShowEditingIcon = false;
            this.dgStats.ShowRowErrors = false;
            this.dgStats.Size = new System.Drawing.Size(268, 697);
            this.dgStats.TabIndex = 1;
            this.dgStats.SelectionChanged += new System.EventHandler(this.dgStats_SelectionChanged);
            // 
            // ColStat
            // 
            this.ColStat.DataPropertyName = "Stat";
            this.ColStat.HeaderText = "Stat";
            this.ColStat.Name = "ColStat";
            this.ColStat.ReadOnly = true;
            this.ColStat.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.ColStat.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // ColMin
            // 
            this.ColMin.DataPropertyName = "Min";
            this.ColMin.FillWeight = 50F;
            this.ColMin.HeaderText = "Min";
            this.ColMin.Name = "ColMin";
            this.ColMin.ReadOnly = true;
            this.ColMin.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.ColMin.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // ColMax
            // 
            this.ColMax.DataPropertyName = "Max";
            this.ColMax.FillWeight = 50F;
            this.ColMax.HeaderText = "Max";
            this.ColMax.Name = "ColMax";
            this.ColMax.ReadOnly = true;
            this.ColMax.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.ColMax.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // pnlGraph
            // 
            this.pnlGraph.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlGraph.Controls.Add(this.statGraph);
            this.pnlGraph.Controls.Add(this.pnlGraphControls);
            this.pnlGraph.Location = new System.Drawing.Point(280, 0);
            this.pnlGraph.Name = "pnlGraph";
            this.pnlGraph.Padding = new System.Windows.Forms.Padding(5);
            this.pnlGraph.Size = new System.Drawing.Size(774, 710);
            this.pnlGraph.TabIndex = 2;
            // 
            // statGraph
            // 
            this.statGraph.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.statGraph.Dock = System.Windows.Forms.DockStyle.Fill;
            this.statGraph.Location = new System.Drawing.Point(5, 5);
            this.statGraph.Name = "statGraph";
            this.statGraph.ShowSteppedStatLines = true;
            this.statGraph.Size = new System.Drawing.Size(764, 700);
            this.statGraph.TabIndex = 1;
            // 
            // pnlGraphControls
            // 
            this.pnlGraphControls.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlGraphControls.Location = new System.Drawing.Point(5, 5);
            this.pnlGraphControls.Name = "pnlGraphControls";
            this.pnlGraphControls.Size = new System.Drawing.Size(764, 0);
            this.pnlGraphControls.TabIndex = 0;
            // 
            // DlgViewStats
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1054, 710);
            this.Controls.Add(this.pnlGraph);
            this.Controls.Add(this.dgStats);
            this.Name = "DlgViewStats";
            this.ShowIcon = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "View Stats";
            this.Load += new System.EventHandler(this.ViewStats_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgStats)).EndInit();
            this.pnlGraph.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private StatterFastDataGrid dgStats;
        private System.Windows.Forms.Panel pnlGraph;
        private System.Windows.Forms.Panel pnlGraphControls;
        private StatterStatGraph statGraph;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColStat;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColMin;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColMax;
    }
}