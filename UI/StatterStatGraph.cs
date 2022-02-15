using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using ACT_Plugin.Model;

namespace ACT_Plugin.UI
{
    public partial class StatterStatGraph : UserControl
    {
        // Global margins surrounding the graph area (used for labels)
        private const int MARGIN_DATA_X_LEFT = 55;
        private const int MARGIN_DATA_X_RIGHT = 55;
        private const int MARGIN_DATA_Y_TOP = 30;
        private const int MARGIN_DATA_Y_BOTTOM = 30;

        // Bounds for the graph drawing area
        private int DataWidth { get { return this.Width - MARGIN_DATA_X_LEFT - MARGIN_DATA_X_RIGHT; } }
        private int DataHeight { get { return this.Height - MARGIN_DATA_Y_TOP - MARGIN_DATA_Y_BOTTOM; } }
        private int DataLeft { get { return MARGIN_DATA_X_LEFT; } }
        private int DataRight { get { return this.Width - MARGIN_DATA_X_RIGHT; } }
        private int DataTop { get { return MARGIN_DATA_Y_TOP; } }
        private int DataBottom { get { return this.Height - MARGIN_DATA_Y_BOTTOM; } }

        // Calculated values used for rendering data points and lines
        private List<StatterEncounterStat> _stats = new List<StatterEncounterStat>();
        private DateTime _startTime = DateTime.MinValue;
        private DateTime _endTime = DateTime.MinValue;
        private double _totalSeconds = -1;
        private double _minVal = double.MaxValue;
        private double _maxVal = double.MinValue;
        private double _valueSpread = -1;
        private double _valueAverage = -1;

        // The off-screen drawing buffer where renders are prepared
        private Bitmap _buff = null;

        // Some shorthands for rendering text
        private StringFormat _sfNearNear = new StringFormat();
        private StringFormat _sfFarNear = new StringFormat();
        private StringFormat _sfNearMid = new StringFormat();
        private StringFormat _sfFarMid = new StringFormat();
        private StringFormat _sfMidFar = new StringFormat();
        private StringFormat _sfMidNear = new StringFormat();

        // Some pens and brushes that are re-used
        private Pen _pCrosshair = null;
        private Pen _pTicks = null;
        private Pen _pOutline = null;
        private SolidBrush _bLabels = null;
        private Pen _pAvg = null;
        private SolidBrush _bAvgFill = null;

        // If true, the stat line graphs will be rendered as steps rather than direct path
        public bool ShowSteppedStatLines { get; set; }

        public StatterStatGraph()
        {
            InitializeComponent();

            _sfNearNear.Alignment = StringAlignment.Near;
            _sfNearNear.LineAlignment = StringAlignment.Near;
            _sfFarNear.Alignment = StringAlignment.Far;
            _sfFarNear.LineAlignment = StringAlignment.Near;
            _sfNearMid.Alignment = StringAlignment.Near;
            _sfNearMid.LineAlignment = StringAlignment.Center;
            _sfFarMid.Alignment = StringAlignment.Far;
            _sfFarMid.LineAlignment = StringAlignment.Center;
            _sfMidFar.Alignment = StringAlignment.Center;
            _sfMidFar.LineAlignment = StringAlignment.Far;
            _sfMidNear.Alignment = StringAlignment.Center;
            _sfMidNear.LineAlignment = StringAlignment.Near;

            _pCrosshair = new Pen(SystemColors.Highlight);
            _pTicks = new Pen(Color.FromArgb(235, 235, 235));
            _pOutline = new Pen(Color.FromArgb(220, 220, 220));
            _bLabels = new SolidBrush(Color.FromArgb(128, 128, 128));
            _pAvg = new Pen(Color.FromArgb(128, 192, 192, 255), 2f);
            _bAvgFill = new SolidBrush(Color.FromArgb(64, 192, 192, 255));
        }

        private void CustomDispose()
        {
            if (_buff != null) _buff.Dispose();

            _sfNearNear.Dispose();
            _sfFarNear.Dispose();
            _sfNearMid.Dispose();
            _sfFarMid.Dispose();
            _sfMidFar.Dispose();
            _sfMidNear.Dispose();

            _pCrosshair.Dispose();
            _pTicks.Dispose();
            _pOutline.Dispose();
            _bLabels.Dispose();
            _pAvg.Dispose();
            _bAvgFill.Dispose();
        }

        private void StatGraph_Load(object sender, System.EventArgs e)
        {
            // Render an empty graph with ticks and labels
            UpdateGraph();
        }

        private void picMain_SizeChanged(object sender, System.EventArgs e)
        {
            UpdateGraph();
        }

        // Draw some context lines and labels when the user moves the mouse around the graph
        private void picMain_MouseMove(object sender, MouseEventArgs e)
        {
            if (this.Width < 1 || this.Height < 1) return;

            int x = Math.Min(DataRight + 1, Math.Max(DataLeft - 1, e.X));
            int y = Math.Min(DataBottom + 1, Math.Max(DataTop - 1, e.Y));
            Point boundedLocation = new Point(x, y);

            using (Bitmap bmpBuff = new Bitmap(_buff))
            using (Graphics gBuff = Graphics.FromImage(bmpBuff))
            using (Graphics gBase = picMain.CreateGraphics())
            {
                _pCrosshair.DashStyle = System.Drawing.Drawing2D.DashStyle.DashDot;

                gBuff.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;

                if (x >= DataLeft && x <= DataRight)
                {
                    gBuff.DrawLine(_pCrosshair, x, DataTop, x, DataBottom);
                    DateTime timeAtPos = GetTimeAt(boundedLocation);
                    gBuff.DrawString(string.Format("{0} (+{1})", timeAtPos.ToString("h':'mm':'ss"), GetOffsetString(timeAtPos)), this.Font, SystemBrushes.Highlight, x, DataTop - 5, _sfMidFar);
                }
                if (y >= DataTop && y <= DataBottom)
                {
                    gBuff.DrawLine(_pCrosshair, DataLeft, y, DataRight, y);
                    gBuff.DrawString(GetValueAt(boundedLocation).ToString("0"), this.Font, SystemBrushes.Highlight, DataRight + 5, y, _sfNearMid);
                }

                gBase.DrawImage(bmpBuff, 0, 0);
            }
        }

        private void picMain_Paint(object sender, PaintEventArgs e)
        {
            if (_buff != null)
                e.Graphics.DrawImage(_buff, 0, 0);
        }

        // Safe helper to release previous buffer
        private void SwapBuff(Bitmap newBuff)
        {
            if (_buff != null)
                _buff.Dispose();

            _buff = newBuff;
        }

        // The main render routine - creates the buffer that will be drawn to screen
        private void Render()
        {
            if (this.Width < 1 || this.Height < 1) return;

            Bitmap buff = new Bitmap(this.Width, this.Height);
            using (Graphics g = Graphics.FromImage(buff))
            {
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                g.Clear(SystemColors.Window);

                // Draw the grid and labels
                int spacing = 20;
                for (int x = DataLeft + spacing; x < DataRight; x += spacing)
                    g.DrawLine(_pTicks, x, DataTop, x, DataBottom);
                for (int y = DataTop + spacing; y < DataBottom; y += spacing)
                    g.DrawLine(_pTicks, DataLeft, y, DataRight, y);

                g.DrawLine(_pOutline, DataLeft - 4, DataTop, DataRight, DataTop);
                g.DrawLine(_pOutline, DataLeft - 4, DataBottom, DataRight, DataBottom);
                g.DrawLine(_pOutline, DataLeft, DataTop, DataLeft, DataBottom + 4);
                g.DrawLine(_pOutline, DataRight, DataTop, DataRight, DataBottom + 4);

                g.DrawString(_startTime.ToString("h':'mm':'ss"), this.Font, _bLabels, DataLeft - 20, DataBottom + 8, _sfNearNear);
                g.DrawString(_endTime.ToString("h':'mm':'ss"), this.Font, _bLabels, DataRight + 20, DataBottom + 8, _sfFarNear);

                g.DrawString(_maxVal.ToString("0"), this.Font, _bLabels, DataLeft - 8, DataTop + 3, _sfFarMid);
                g.DrawString(_minVal.ToString("0"), this.Font, _bLabels, DataLeft - 8, DataBottom - 3, _sfFarMid);

                // Draw a box representing the average of the stat (note that this only applies
                // when a single stat is being shown)
                if (_valueAverage >= 0)
                {
                    Point pTopLeft = GetDataPoint(_startTime, _valueAverage);
                    Point pTopRight = GetDataPoint(_endTime, _valueAverage);
                    g.FillRectangle(_bAvgFill, DataLeft, pTopLeft.Y, DataWidth, DataBottom - pTopLeft.Y);
                    g.DrawLine(_pAvg, pTopLeft, pTopRight);
                }

                // Now render the actual stat value lines
                Dictionary<Point[], Color> shadowLines = new Dictionary<Point[], Color>();
                Dictionary<Point[], Color> measurementLines = new Dictionary<Point[], Color>();
                for (int i = 0; i < _stats.Count; i++)
                    if (_stats[i].Readings.Count > 0)
                    {
                        StatterEncounterStat encStat = _stats[i];
                        List<Point> points = new List<Point>();

                        points.Add(GetDataPoint(_startTime, encStat.Readings[0].Value));
                        for (int j = 0; j < encStat.Readings.Count; j++)
                        {
                            // Insert an extra point if steps are being used
                            if (ShowSteppedStatLines && j > 0 && encStat.Readings[j].Value != encStat.Readings[j - 1].Value)
                                points.Add(GetDataPoint(encStat.Readings[j].Time, encStat.Readings[j - 1].Value));

                            points.Add(GetDataPoint(encStat.Readings[j].Time, encStat.Readings[j].Value));
                        }
                        points.Add(GetDataPoint(_endTime, encStat.Readings[encStat.Readings.Count - 1].Value));

                        shadowLines.Add(ToPointArray(points, 2, 2), Color.FromArgb(128, 200, 200, 200));
                        measurementLines.Add(ToPointArray(points, 0, 0), encStat.Stat.Colour);
                    }

                RenderLines(g, shadowLines, 5.0f);
                RenderLines(g, measurementLines, 3.0f);
            }

            SwapBuff(buff);
        }

        private Point[] ToPointArray(List<Point> points, int xOffset, int yOffset)
        {
            Point[] pointArray = points.ToArray();

            if (xOffset != 0)
                for (int i = 0; i < pointArray.Length - 1; i++)
                    pointArray[i].X += xOffset;

            if (yOffset != 0)
                for (int i = 0; i < pointArray.Length - 1; i++)
                    pointArray[i].Y += yOffset;

            return pointArray;
        }

        private void RenderLine(Graphics g, Point[] points, Color baseColour, float lineWidth)
        {
            using (SolidBrush bPoint = new SolidBrush(baseColour))
            {
                int pntSize = 4;
                foreach (Point p in points)
                    g.FillEllipse(bPoint, p.X - pntSize, p.Y - pntSize, pntSize * 2, pntSize * 2);
            }
            using (Pen pLine = new Pen(ShiftAlpha(baseColour, 160), lineWidth))
            {
                g.DrawLines(pLine, points);
            }
        }

        private void RenderLines(Graphics g, Dictionary<Point[], Color> lines, float lineWidth)
        {
            foreach (Point[] key in lines.Keys)
                RenderLine(g, key, lines[key], lineWidth);
        }

        private void RenderLines(Graphics g, Dictionary<Point[], Color> lines)
        {
            RenderLines(g, lines, 2.0f);
        }

        // Get a point inside the stat drawing area that corresponds to the given time and value
        private Point GetDataPoint(DateTime time, double value)
        {
            int x = 0, y = 0;

            if (_totalSeconds <= 0)
                x = DataLeft + (DataWidth / 2);
            else
                x = DataLeft + (int)(((time - _startTime).TotalSeconds / _totalSeconds) * DataWidth);

            if (_valueSpread <= 0)
                y = DataBottom - (DataHeight / 2);
            else
                y = DataBottom - (int)(((value - _minVal) / _valueSpread) * DataHeight);

            return new Point(x, y);
        }

        private DateTime GetTimeAt(Point p)
        {
            DateTime timeAtPoint = _startTime.AddSeconds(Math.Max(0, (((p.X - DataLeft) / (1.0 * DataWidth)) * _totalSeconds)));
            return timeAtPoint;
        }

        private double GetValueAt(Point p)
        {
            return _maxVal - ((((p.Y - DataTop) / (1.0 * DataHeight)) * _valueSpread));
        }

        private string GetOffsetString(DateTime time)
        {
            TimeSpan tsOffset = time - _startTime;
            StringBuilder sb = new StringBuilder();
            if (tsOffset.Hours > 0) sb.Append(tsOffset.Hours).Append(":");
            if (tsOffset.Minutes > 0) sb.Append(sb.Length > 0 ? tsOffset.Minutes.ToString().PadLeft(2, '0') : tsOffset.Minutes.ToString()).Append(":");
            sb.Append(sb.Length > 0 ? tsOffset.Seconds.ToString().PadLeft(2, '0') : tsOffset.Seconds.ToString());
            return sb.ToString();
        }

        private Color ShiftAlpha(Color baseColour, int newAlpha)
        {
            return Color.FromArgb(Math.Min(255, Math.Max(0, newAlpha)), baseColour.R, baseColour.G, baseColour.B);
        }

        // This is the main draw routine. Calculate some state values, then render and display the buffer
        public void DrawStats(List<StatterEncounterStat> stats, DateTime startTime, DateTime endTime)
        {
            _stats = stats;
            _startTime = startTime;
            _endTime = endTime;
            _totalSeconds = (_endTime - _startTime).TotalSeconds;
            if (_totalSeconds < 0) _totalSeconds = 0;

            // Find the min and max values accross all stats to draw the bounds
            _minVal = double.MaxValue;
            _maxVal = double.MinValue;
            foreach (StatterEncounterStat stat in _stats)
                if (stat.Readings.Count > 0)
                {
                    if (stat.MinReading.Value < _minVal) _minVal = stat.MinReading.Value;
                    if (stat.MaxReading.Value > _maxVal) _maxVal = stat.MaxReading.Value;
                }
            _valueSpread = _minVal == double.MaxValue ? 0 : _maxVal - _minVal;

            // Now calculate the average value if that concept applies
            if (stats.Count == 1 && stats[0].Readings.Count > 1 && _totalSeconds > 0)
            {
                double sum = 0;

                // Add value from start of fight until first reading - since we don't know what 
                // the value was prior to the first reading, use the first reading
                sum += (stats[0].Readings[0].Time - startTime).TotalSeconds * stats[0].Readings[0].Value;

                // Now add all intermediary values
                for (int i = 1; i < stats[0].Readings.Count; i++)
                    sum += (stats[0].Readings[i].Time - stats[0].Readings[i - 1].Time).TotalSeconds * stats[0].Readings[i - 1].Value;

                // Finally, add the value after the last reading, up until the end of the encounter
                sum += (endTime - stats[0].Readings[stats[0].Readings.Count - 1].Time ).TotalSeconds * stats[0].Readings[stats[0].Readings.Count - 1].Value;

                _valueAverage = sum / _totalSeconds;
            }
            else
                _valueAverage = -1;

            UpdateGraph();
        }

        public void UpdateGraph()
        {
            Render();
            Refresh();
        }
    }
}
