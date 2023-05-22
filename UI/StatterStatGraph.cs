using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Advanced_Combat_Tracker;
using ActStatter.Model;
using ActStatter.Util;

namespace ActStatter.UI
{
    public partial class StatterStatGraph : UserControl
    {
        private class LineOptions
        {
            public Color Color;
            public float Width;
            public int Alpha = 255;
        }
        private class GraphPin
        {
            public Point Point;
            public string Label;
        }

        private StatterSettings _settings = new StatterSettings();
        private List<GraphPin> _pins = new List<GraphPin>();

        // Global margins surrounding the graph area (used for labels)
        private const int MARGIN_DATA_X_LEFT = 55;
        private const int MARGIN_DATA_X_RIGHT = 55;
        private const int MARGIN_DATA_Y_TOP = 30;
        private const int MARGIN_DATA_Y_BOTTOM = 30;

        // Line constants
        private const float LINE_SHADOW_WIDTH = 5.0f;
        private const float LINE_OC_WIDTH = 8.0f;
        private const float LINE_NON_OC_WIDTH = 2.0f;
        private readonly Color LINE_SHADOW_COLOUR = Color.FromArgb(128, 200, 200, 200);
        private const int LINE_SHADOW_ALPHA = 160;

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

        // Data for rendering enc dpc overlay
        private Dictionary<int, double> _encDps = new Dictionary<int, double>();
        private double _maxDps = double.MinValue;
        private double _maxDpsScale = 3;

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

            if (_sfNearNear != null) _sfNearNear.Dispose();
            if (_sfFarNear != null) _sfFarNear.Dispose();
            if (_sfNearMid != null) _sfNearMid.Dispose();
            if (_sfFarMid != null) _sfFarMid.Dispose();
            if (_sfMidFar != null) _sfMidFar.Dispose();
            if (_sfMidNear != null) _sfMidNear.Dispose();

            if (_pCrosshair != null) _pCrosshair.Dispose();
            if (_pTicks != null) _pTicks.Dispose();
            if (_pOutline != null) _pOutline.Dispose();
            if (_bLabels != null) _bLabels.Dispose();
            if (_pAvg != null) _pAvg.Dispose();
            if (_bAvgFill != null) _bAvgFill.Dispose();
        }

        public void UseSettings(StatterSettings settings)
        {
            _settings = settings;
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

                    gBuff.DrawString(string.Format("{0} ({1})", 
                            timeAtPos.ToString("h':'mm':'ss"), 
                            GetOffsetString(timeAtPos)),
                        this.Font, SystemBrushes.Highlight, x, DataTop - 5, _sfMidFar);
                }
                if (y >= DataTop && y <= DataBottom)
                {
                    gBuff.DrawLine(_pCrosshair, DataLeft, y, DataRight, y);
                    string readableVal = Formatters.GetReadableNumber(GetValueAt(boundedLocation));
                    gBuff.DrawString(readableVal, this.Font, SystemBrushes.Highlight, DataRight + 5, y, _sfNearMid);
                }

                gBase.DrawImage(bmpBuff, 0, 0);
            }
        }

        private void picMain_MouseUp(object sender, MouseEventArgs e)
        {
            if (this.Width < 1 || this.Height < 1) return;

            int x = Math.Min(DataRight + 1, Math.Max(DataLeft - 1, e.X));
            int y = Math.Min(DataBottom + 1, Math.Max(DataTop - 1, e.Y));
            if (e.Button == MouseButtons.Left && y >= DataTop && y <= DataBottom)
            {
                Point boundedLocation = new Point(x, y);
                string readableVal = Formatters.GetReadableNumber(GetValueAt(boundedLocation));
                _pins.Add(new GraphPin()
                {
                    Point = boundedLocation,
                    Label = readableVal
                });
                UpdateGraph();
            }
            else
            {
                _pins.Clear();
                UpdateGraph();
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

                g.DrawString(Formatters.GetReadableNumber(_maxVal), this.Font, _bLabels, DataLeft - 8, DataTop + 3, _sfFarMid);
                g.DrawString(Formatters.GetReadableNumber(_minVal), this.Font, _bLabels, DataLeft - 8, DataBottom - 3, _sfFarMid);

                // Draw a box representing the average of the stat (note that this only applies
                // when a single stat is being shown)
                if (_settings.GraphShowAverage && _valueAverage >= 0)
                {
                    Point pTopLeft = GetDataPoint(_startTime, _valueAverage);
                    Point pTopRight = GetDataPoint(_endTime, _valueAverage);
                    g.FillRectangle(_bAvgFill, DataLeft, pTopLeft.Y, DataWidth, DataBottom - pTopLeft.Y);
                    g.DrawLine(_pAvg, pTopLeft, pTopRight);
                }

                // Draw the encDps line
                if (_settings.GraphShowEncDps && _encDps.Count > 0)
                {
                    List<Tuple<Point, Point>> endDpsLine = new List<Tuple<Point, Point>>();
                    Point prevDpsPoint = _encDps.Count > 0 ? GetDpsPoint(0, _encDps[0]) : GetDpsPoint(0, 0);
                    foreach (int tTime in _encDps.Keys)
                    {
                        Point curDpsPoint = GetDpsPoint(tTime, _encDps[tTime]);
                        endDpsLine.Add(new Tuple<Point, Point>(prevDpsPoint, curDpsPoint));
                        prevDpsPoint = curDpsPoint;
                    }
                    RenderLine(g, endDpsLine, Color.Black, 1f, 160);
                }

                // Now render the actual stat value lines
                Dictionary<List<Tuple<Point, Point>>, LineOptions> shadowLines = new Dictionary<List<Tuple<Point, Point>>, LineOptions>();
                Dictionary<List<Tuple<Point, Point>>, LineOptions> measurementLines = new Dictionary<List<Tuple<Point, Point>>, LineOptions>();
                LineOptions shadowLineOpts = new LineOptions() { Color = LINE_SHADOW_COLOUR, Width = LINE_SHADOW_WIDTH, Alpha = LINE_SHADOW_ALPHA };
                for (int i = 0; i < _stats.Count; i++)
                    if (_stats[i].Readings.Count > 0)
                    {
                        StatterEncounterStat encStat = _stats[i];
                        var readings = encStat.Readings;
                        StatterStatReading curReading;
                        StatterStatReading prevReading;
                        Point prevPoint;
                        Point wayPoint;
                        Point curPoint;
                        List<Tuple<Point, Point>> verticalLines = new List<Tuple<Point, Point>>();
                        List<Tuple<Point, Point>> nonOcLines = new List<Tuple<Point, Point>>();
                        List<Tuple<Point, Point>> ocLines = new List<Tuple<Point, Point>>();

                        // Iterate through the readings for this stat, and build collections
                        // of points that we can use to draw lines for the stat graph. We'll
                        // build each point and use the previous point to add the line points.
                        prevReading = readings[0];
                        for (int j = 0; j < readings.Count; j++)
                        {
                            // Since this stat can start after the fight T0, handle the first point specially
                            if (j == 0)
                                prevPoint = GetDataPoint(_startTime, prevReading.Value);
                            else
                                prevPoint = GetDataPoint(prevReading.Time, prevReading.Value);

                            // Need a horizontal line from the last reading to the current time
                            curReading = readings[j];
                            wayPoint = GetDataPoint(curReading.Time, prevReading.Value);
                            if (prevReading.Overcap)
                                ocLines.Add(new Tuple<Point, Point>(prevPoint, wayPoint));
                            else
                                nonOcLines.Add(new Tuple<Point, Point>(prevPoint, wayPoint));

                            // Now the vertical line
                            curPoint = GetDataPoint(curReading.Time, curReading.Value);
                            verticalLines.Add(new Tuple<Point, Point>(wayPoint, curPoint));

                            prevReading = curReading;
                        }
                        // Add a final horizontal line to close out the duration, since it's unlikely the
                        // reading occured at exactly the end of the encounter
                        prevPoint = GetDataPoint(prevReading.Time, prevReading.Value);
                        curPoint = GetDataPoint(_endTime, prevReading.Value);
                        if (prevReading.Overcap)
                            ocLines.Add(new Tuple<Point, Point>(prevPoint, curPoint));
                        else
                            nonOcLines.Add(new Tuple<Point, Point>(prevPoint, curPoint));

                        shadowLines.Add(ShiftLines(ocLines, 2, 2), shadowLineOpts);
                        shadowLines.Add(ShiftLines(nonOcLines, 2, 2), shadowLineOpts);
                        shadowLines.Add(ShiftLines(verticalLines, 2, 2), shadowLineOpts);
                        measurementLines.Add(StretchLines(ocLines, 1, 0), new LineOptions() { Color = encStat.Stat.Colour, Width = LINE_OC_WIDTH });
                        measurementLines.Add(ShiftLines(nonOcLines, 0, 0), new LineOptions() { Color = encStat.Stat.Colour, Width = LINE_NON_OC_WIDTH });
                        measurementLines.Add(ShiftLines(verticalLines, 0, 0), new LineOptions() { Color = encStat.Stat.Colour, Width = LINE_NON_OC_WIDTH });
                    }

                RenderLines(g, measurementLines);

                // Render pins
                foreach (var pin in _pins)
                {
                    g.DrawLine(_pCrosshair, DataLeft, pin.Point.Y, DataRight, pin.Point.Y);
                    g.DrawString(pin.Label, this.Font, SystemBrushes.Highlight, DataRight + 5, pin.Point.Y, _sfNearMid);
                }
            }

            SwapBuff(buff);
        }

        // Extract all points to an array, optionally shifting them by xOffset or yOffset
        private List<Tuple<Point, Point>> ShiftLines(List<Tuple<Point, Point>> lines, int xOffset, int yOffset)
        {
            List<Tuple<Point, Point>> newLines = new List<Tuple<Point, Point>>();

            foreach (var line in lines)
            {
                newLines.Add(new Tuple<Point, Point>(
                    new Point(line.Item1.X + xOffset, line.Item1.Y + yOffset),
                    new Point(line.Item2.X + xOffset, line.Item2.Y + yOffset)
                ));
            }

            return newLines;
        }

        private List<Tuple<Point, Point>> StretchLines(List<Tuple<Point, Point>> lines, int xStretch, int yStretch)
        {
            List<Tuple<Point, Point>> newLines = new List<Tuple<Point, Point>>();

            foreach (var line in lines)
            {
                newLines.Add(new Tuple<Point, Point>(
                    new Point(line.Item1.X - xStretch, line.Item1.Y - yStretch),
                    new Point(line.Item2.X + xStretch, line.Item2.Y + yStretch)
                ));
            }

            return newLines;
        }

        private void RenderLines(Graphics g, Dictionary<List<Tuple<Point, Point>>, LineOptions> lines)
        {
            foreach (var lineList in lines.Keys)
                RenderLine(g, lineList, lines[lineList].Color, lines[lineList].Width, lines[lineList].Alpha);
        }

        private void RenderLine(Graphics g, List<Tuple<Point, Point>> lines, Color baseColour, float lineWidth, int alpha)
        {
            using (Pen pLine = new Pen(ShiftAlpha(baseColour, alpha), lineWidth))
            {
                foreach (var line in lines)
                {
                    g.DrawLine(pLine, line.Item1, line.Item2);
                }
            }
        }

        private void RenderPoints(Graphics g, Point[] points, Color baseColour, bool overcap)
        {
            using (SolidBrush bPoint = new SolidBrush(baseColour))
            {
                int pntSize = 4;
                foreach (Point p in points)
                    g.FillEllipse(bPoint, p.X - pntSize, p.Y - pntSize, pntSize * 2, pntSize * 2);
            }
        }

        // Get a point inside the stat drawing area that corresponds to the given time and dps value
        private Point GetDpsPoint(int timeOffsetSeconds, double dps)
        {
            int x = 0, y = 0;

            if (_totalSeconds <= 0)
                x = DataLeft + (DataWidth / 2);
            else
                x = DataLeft + (int)((timeOffsetSeconds / _totalSeconds) * DataWidth);

            if (_valueSpread <= 0)
                y = DataBottom - (DataHeight / 2);
            else
                y = DataBottom - (int)((dps / (_maxDps * _maxDpsScale)) * DataHeight);

            return new Point(x, y);
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

            // Append duration in format h:mm:ss
            if (tsOffset.TotalSeconds > 60)
            {
                sb.Append("+");
                if (tsOffset.Hours > 0)
                    sb.Append(tsOffset.Hours).Append(":");
                if (tsOffset.Minutes > 0)
                    sb.Append(sb.Length > 0 ? tsOffset.Minutes.ToString().PadLeft(2, '0') : tsOffset.Minutes.ToString()).Append(":");
                sb.Append(sb.Length > 0 ? tsOffset.Seconds.ToString().PadLeft(2, '0') : tsOffset.Seconds.ToString());
                sb.Append(" / ");
            }

            // Also append duration in total elapsed seconds
            sb.Append("+" + Math.Floor(tsOffset.TotalSeconds).ToString("0"));

            return sb.ToString();
        }

        private Color ShiftAlpha(Color baseColour, int newAlpha)
        {
            return Color.FromArgb(Math.Min(255, Math.Max(0, newAlpha)), baseColour.R, baseColour.G, baseColour.B);
        }

        // This is the main draw routine. Calculate some state values, then render and display the buffer
        public void DrawStats(List<StatterEncounterStat> stats, DateTime startTime, DateTime endTime, EncounterData encData)
        {
            // Don't clear th pins if the stats are the same
            var curStatKeys = _stats.Aggregate("", (acc, x) => acc += x.Stat.Key);
            var newStatKeys = stats.Aggregate("", (acc, x) => acc += x.Stat.Key);
            if (curStatKeys != newStatKeys)
                _pins.Clear();

            _stats = stats;
            _startTime = startTime;
            _endTime = endTime;
            _totalSeconds = (_endTime - _startTime).TotalSeconds;
            if (_totalSeconds < 0) _totalSeconds = 0;

            // Find the min and max values accross all stats to draw the bounds
            _minVal = double.MaxValue;
            _maxVal = double.MinValue;
            foreach (StatterEncounterStat stat in _stats)
            {
                if (stat.Readings.Count > 0)
                {
                    if (stat.MinReading.Value < _minVal) _minVal = stat.MinReading.Value;
                    if (stat.MaxReading.Value > _maxVal) _maxVal = stat.MaxReading.Value;
                }
            }
            _valueSpread = _minVal == double.MaxValue ? 0 : _maxVal - _minVal;

            // Now calculate the average value if that concept applies
            if (stats.Count == 1 && stats[0].Readings.Count > 1 && _totalSeconds > 0)
            {
                _valueAverage = stats[0].AvgReading.Value;
            }
            else
                _valueAverage = -1;

            // Calculate enc dps
            _encDps.Clear();
            _maxDps = 0;
            if (_settings.GraphShowEncDps)
            {
                // To generate the dps graph, just sum up the damage done each second
                // over the duration of the enncounter. Then pass back over and accumulate
                // damage done in the last n seconds, where n is controlled by the resoltion
                // slider. ACT uses 10 sec in its own graphs (which is the max duration we use,
                // corresponding to the "lowest" resolution.
                // Convert the resolution slider (min 1, max 10) to the accumulation period:
                int accumulateSeconds = 1 + (10 - _settings.EncDpsResolution);
                List<MasterSwing> attacks = null;
                foreach (var combatant in encData.Items)
                    if (combatant.Key.Equals(encData.CharName.ToUpper()))
                        foreach (var combatItem in combatant.Value.Items)
                            if (combatItem.Key.Equals("Outgoing Damage"))
                                foreach (var subItem in combatItem.Value.Items)
                                    if (subItem.Key.Equals("All"))
                                        attacks = subItem.Value.Items;
                if (attacks != null)
                {
                    int encDurationSec = Convert.ToInt32(_totalSeconds);
                    double[] damageAtSecond = new double[encDurationSec];
                    foreach (var attack in attacks)
                    {
                        int t = Math.Min(encDurationSec - 1, Convert.ToInt32((attack.Time - _startTime).TotalSeconds));
                        damageAtSecond[t] += attack.Damage.Number;
                    }
                    for (int i = 0; i < encDurationSec; i++)
                    {
                        double damageInRollingPeriod = 0;
                        for (int j = i; j > (i - accumulateSeconds) && j >= 0; j--)
                            damageInRollingPeriod += damageAtSecond[j];
                        double dps = damageInRollingPeriod / (accumulateSeconds * 1.0);
                        if (dps > _maxDps)
                            _maxDps = dps;
                        _encDps.Add(i, dps);
                    }
                }
            }

            UpdateGraph();
        }

        public void UpdateGraph()
        {
            Render();
            Refresh();
        }
    }
}
