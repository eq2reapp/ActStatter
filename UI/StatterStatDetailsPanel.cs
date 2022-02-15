using System;
using System.Windows.Forms;
using ACT_Plugin.Model;

namespace ACT_Plugin.UI
{
    public partial class StatterStatDetailsPanel : UserControl
    {
        public event Action<StatterStatDetailsPanel> StatDeleted;
        private void OnStatDeleted() { if (StatDeleted != null) StatDeleted(this); }

        public event Action<StatterStatDetailsPanel> StatModified;
        private void OnStatModified() { if (StatModified != null) StatModified(this); }

        private StatterStat _stat = null;
        public StatterStat Stat { get { return _stat; } }

        public StatterStatDetailsPanel(StatterStat stat)
        {
            InitializeComponent();

            _stat = stat;
        }

        private void StatDetails_Load(object sender, EventArgs e)
        {
            lblName.Text = _stat.Name;
            pnlColour.BackColor = _stat.Colour;
        }

        private void pnlColour_Click(object sender, EventArgs e)
        {
            dlgColour.Color = pnlColour.BackColor;
            if (dlgColour.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                pnlColour.BackColor = dlgColour.Color;
                if (_stat.Colour != pnlColour.BackColor)
                {
                    _stat.Colour = pnlColour.BackColor;
                    OnStatModified();
                }
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            OnStatDeleted();
        }
    }
}
