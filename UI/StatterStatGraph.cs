using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Advanced_Combat_Tracker;
using ActStatter.Model;
using ActStatter.Util;
using System.Runtime.CompilerServices;

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
        private class AreaOptions
        {
            public Color Color;
        }
        private class GraphPin
        {
            public Point Point;
            public string Label;
        }

        private StatterSettings _settings = new StatterSettings();
        private List<GraphPin> _pins = new List<GraphPin>();
        private Color _fg = Color.Black;
        private Color _bg = Color.White;

        // Global margins surrounding the graph area (used for labels)
        private const int MARGIN_DATA_X_LEFT = 140;
        private const int MARGIN_DATA_X_RIGHT = 55;
        private const int MARGIN_DATA_Y_TOP = 30;
        private const int MARGIN_DATA_Y_BOTTOM = 30;
        private const int MARGIN_DPS_DATA_BUFFER = 20;

        // Line constants
        private const float LINE_OC_WIDTH = 8.0f;
        private const float LINE_NON_OC_WIDTH = 2.0f;

        // Colors for rendered stat lines. Everything after the first
        // few will be derived.
        private Color[] _statLineColours =
        {
            Color.FromArgb(255, 104, 164, 98),
            Color.FromArgb(255, 242, 175, 88),
            Color.FromArgb(255, 53, 108, 154),
            Color.FromArgb(255, 209, 65, 63),
            Color.FromArgb(255, 134, 81, 137),
            Color.White,
            Color.White,
            Color.White,
            Color.White,
            Color.White,
            Color.White,
            Color.White,
            Color.White,
            Color.White,
            Color.White,
            Color.White,
            Color.White,
            Color.White,
            Color.White,
            Color.White,
            Color.White,
            Color.White,
            Color.White,
            Color.White,
        };
        private Dictionary<string, Color> _statPlayerLineColourMap = new Dictionary<string, Color>();

        // Bounds for the graph drawing area
        private bool ShowingDps { get { return _settings.GraphShowEncDps && _playersEncSwings.Count > 0; } }
        private bool ShowingHps { get { return _settings.GraphShowEncHps && _playersEncSwings.Count > 0; } }
        private bool ShowingAuto { get { return _settings.GraphShowEncAuto && _playersEncSwings.Count > 0; } }
        private bool ShowingSkills { get { return _settings.GraphShowEncSkills && _playersEncSwings.Count > 0; } }
        private bool ShowingAny { get { return ShowingDps || ShowingHps || ShowingAuto || ShowingSkills; } }
        private bool ShowSwings { get { return _settings.GraphShowEncDps || _settings.GraphShowEncHps || _settings.GraphShowEncAuto || _settings.GraphShowEncSkills; } }
        private bool ShowingRanges { get { return _settings.GraphShowRanges; } }
        private double DpsAreaRatio = 0.33;
        private int GraphWidth { get { return this.Width - MARGIN_DATA_X_LEFT - MARGIN_DATA_X_RIGHT; } }
        private int GraphHeight { get { return this.Height - MARGIN_DATA_Y_TOP - MARGIN_DATA_Y_BOTTOM; } }
        private int GraphLeft { get { return MARGIN_DATA_X_LEFT; } }
        private int GraphRight { get { return this.Width - MARGIN_DATA_X_RIGHT; } }
        private int GraphTop { get { return MARGIN_DATA_Y_TOP; } }
        private int GraphBottom { get { return this.Height - MARGIN_DATA_Y_BOTTOM; } }
        private int DataWidth { get { return GraphWidth; } }
        private int DataHeight { get { return (ShowingAny) ? (int)Math.Floor((GraphHeight - MARGIN_DPS_DATA_BUFFER) * (1 - DpsAreaRatio)) : GraphHeight; } }
        private int DataLeft { get { return GraphLeft; } }
        private int DataRight { get { return GraphRight; } }
        private int DataTop { get { return GraphTop; } }
        private int DataBottom { get { return DataTop + DataHeight; } }
        private int DpsWidth { get { return GraphWidth; } }
        private int DpsHeight { get { return (ShowingAny) ? (int)Math.Floor((GraphHeight - MARGIN_DPS_DATA_BUFFER) * (DpsAreaRatio)) : 0; } }
        private int DpsLeft { get { return GraphLeft; } }
        private int DpsRight { get { return GraphRight; } }
        private int DpsTop { get { return GraphBottom - DpsHeight; } }
        private int DpsBottom { get { return GraphBottom; } }
        private int LegendTop { get { return GraphTop; } }
        private int LegendLeft { get { return 10; } }

        // Calculated values used for rendering data points and lines
        private List<StatterEncounterStat> _stats = new List<StatterEncounterStat>();
        private DateTime _startTime = DateTime.MinValue;
        private DateTime _endTime = DateTime.MinValue;
        private double _totalSeconds = -1;
        private double _minVal = 0;
        private double _maxVal = 0;
        private double _valueSpread = -1;
        private double _valueAverage = -1;

        // Data for rendering enc dps/hps overlays
        private Dictionary<string, Dictionary<int, double>> _playersEncSwings = new Dictionary<string, Dictionary<int, double>>();
        private double _maxSwing = double.MinValue;

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
        private SolidBrush _bHighlight = null;

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

            SetColours();

            // Auto-generate extra line colours based on the first 5
            for (int i = 5; i < _statLineColours.Length; i++)
            {
                _statLineColours[i] = ChangeColorBrightness(_statLineColours[i - 5], 1.2f);
            }
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
            if (_bHighlight != null) _bHighlight.Dispose();
        }

        private void SetColours()
        {
            if (_pCrosshair != null) _pCrosshair.Dispose();
            if (_pTicks != null) _pTicks.Dispose();
            if (_pOutline != null) _pOutline.Dispose();
            if (_bLabels != null) _bLabels.Dispose();
            if (_pAvg != null) _pAvg.Dispose();
            if (_bAvgFill != null) _bAvgFill.Dispose();
            if (_bHighlight != null) _bHighlight.Dispose();

            Color highlight = Color.FromArgb(255, 0, 128, 255);
            _pCrosshair = new Pen(highlight);
            _bHighlight = new SolidBrush(highlight);

            _pTicks = new Pen(Color.FromArgb(16, _fg));
            _pOutline = new Pen(Color.FromArgb(192, _fg));
            _bLabels = new SolidBrush(Color.FromArgb(192, _fg));
            _pAvg = new Pen(Color.FromArgb(128, 192, 192, 255), 2f);
            _bAvgFill = new SolidBrush(Color.FromArgb(64, 192, 192, 255));
        }

        private List<string> GetStatPlayerKeys()
        {
            List<string> playerKeys = new List<string>();
            foreach (var stat in _stats)
                if (!playerKeys.Contains(stat.PlayerKey))
                    playerKeys.Add(stat.PlayerKey);
            if (playerKeys.Contains(StatterStatReading.DEFAULT_PLAYER_KEY))
            {
                playerKeys.Remove(StatterStatReading.DEFAULT_PLAYER_KEY);
                playerKeys.Insert(0, StatterStatReading.DEFAULT_PLAYER_KEY);
            }
            return playerKeys;
        }

        private Color ChangeColorBrightness(Color color, float correctionFactor)
        {
            float red = (float)color.R;
            float green = (float)color.G;
            float blue = (float)color.B;

            if (correctionFactor < 0)
            {
                correctionFactor = 1 + correctionFactor;
                red *= correctionFactor;
                green *= correctionFactor;
                blue *= correctionFactor;
            }
            else
            {
                red *= correctionFactor;
                green *= correctionFactor;
                blue *= correctionFactor;
            }

            return Color.FromArgb(color.A, Math.Min(255, (int)red), Math.Min(255, (int)green), Math.Min(255, (int)blue));
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
            _pins.Clear();
            UpdateGraph();
        }

        // Draw some context lines and labels when the user moves the mouse around the graph
        private void picMain_MouseMove(object sender, MouseEventArgs e)
        {
            if (this.Width < 1 || this.Height < 1) return;

            int x = Math.Min(GraphRight + 1, Math.Max(GraphLeft - 1, e.X));
            int y = Math.Min(GraphBottom + 1, Math.Max(GraphTop - 1, e.Y));
            Point boundedLocation = new Point(x, y);

            using (Bitmap bmpBuff = new Bitmap(_buff))
            using (Graphics gBuff = Graphics.FromImage(bmpBuff))
            using (Graphics gBase = picMain.CreateGraphics())
            {
                _pCrosshair.DashStyle = System.Drawing.Drawing2D.DashStyle.DashDot;

                gBuff.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;

                // Draw time line
                if (x >= GraphLeft && x <= GraphRight)
                {
                    gBuff.DrawLine(_pCrosshair, x, GraphTop, x, GraphBottom);
                    DateTime timeAtPos = GetTimeAt(boundedLocation);

                    gBuff.DrawString(string.Format("{0} ({1})", 
                            timeAtPos.ToString("h':'mm':'ss"), 
                            GetOffsetString(timeAtPos)),
                        this.Font, _bHighlight, x, DataTop - 5, _sfMidFar);
                }

                // Draw stat line
                if (y >= DataTop && y <= DataBottom)
                {
                    gBuff.DrawLine(_pCrosshair, DataLeft, y, DataRight, y);
                    string readableVal = Formatters.GetReadableNumber(GetValueAt(boundedLocation));
                    gBuff.DrawString(readableVal, this.Font, _bHighlight, DataRight + 5, y, _sfNearMid);
                }

                // Draw dps/hps line
                if (y >= DpsTop && y <= DpsBottom)
                {
                    gBuff.DrawLine(_pCrosshair, DpsLeft, y, DpsRight, y);

                    string readableVal = Formatters.GetReadableNumber(GetEncValAt(boundedLocation, _maxSwing));
                    gBuff.DrawString(readableVal, this.Font, _bHighlight, DpsRight + 5, y, _sfNearMid);
                }

                gBase.DrawImage(bmpBuff, 0, 0);
            }
        }

        private void picMain_MouseUp(object sender, MouseEventArgs e)
        {
            if (this.Width < 1 || this.Height < 1) return;

            int x = Math.Min(GraphRight + 1, Math.Max(GraphLeft - 1, e.X));
            int y = Math.Min(GraphBottom + 1, Math.Max(GraphTop - 1, e.Y));
            Point boundedLocation = new Point(x, y);

            if (e.Button == MouseButtons.Left && y >= DataTop && y <= DataBottom)
            {
                string readableVal = Formatters.GetReadableNumber(GetValueAt(boundedLocation));
                _pins.Add(new GraphPin()
                {
                    Point = boundedLocation,
                    Label = readableVal
                });
                UpdateGraph();
            }
            else if (e.Button == MouseButtons.Left && y >= DpsTop && y <= DpsBottom)
            {
                string readableVal = Formatters.GetReadableNumber(GetEncValAt(boundedLocation, _maxSwing));
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

            try
            {
                Bitmap buff = new Bitmap(this.Width, this.Height);
                using (Graphics g = Graphics.FromImage(buff))
                {
                    g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                    g.Clear(_bg);

                    // Draw the grids
                    int spacing = 20;
                    for (int x = DataLeft + spacing; x < DataRight; x += spacing)
                        g.DrawLine(_pTicks, x, DataTop, x, DataBottom);
                    for (int y = DataTop + spacing; y < DataBottom; y += spacing)
                        g.DrawLine(_pTicks, DataLeft, y, DataRight, y);
                    g.DrawLine(_pOutline, DataLeft - 4, DataTop, DataRight, DataTop);
                    g.DrawLine(_pOutline, DataLeft - 4, DataBottom, DataRight, DataBottom);
                    g.DrawLine(_pOutline, DataLeft, DataTop, DataLeft, DataBottom + 4);
                    g.DrawLine(_pOutline, DataRight, DataTop, DataRight, DataBottom + 4);
                    g.DrawString(Formatters.GetReadableNumber(_maxVal), this.Font, _bLabels, DataLeft - 8, DataTop + 3, _sfFarMid);
                    g.DrawString(Formatters.GetReadableNumber(_minVal), this.Font, _bLabels, DataLeft - 8, DataBottom - 3, _sfFarMid);

                    if (ShowingAny)
                    {
                        for (int x = DpsLeft + spacing; x < DpsRight; x += spacing)
                            g.DrawLine(_pTicks, x, DpsTop, x, DpsBottom);
                        for (int y = DpsTop + spacing; y < DpsBottom; y += spacing)
                            g.DrawLine(_pTicks, DpsLeft, y, DpsRight, y);
                        g.DrawLine(_pOutline, DpsLeft - 4, DpsTop, DpsRight, DpsTop);
                        g.DrawLine(_pOutline, DpsLeft - 4, DpsBottom, DpsRight, DpsBottom);
                        g.DrawLine(_pOutline, DpsLeft, DpsTop, DpsLeft, DpsBottom + 4);
                        g.DrawLine(_pOutline, DpsRight, DpsTop, DpsRight, DpsBottom + 4);

                        string label = "Label";
                        double maxVal = _maxSwing;
                        if (ShowingDps)
                        {
                            label = "EncDPS";
                        }
                        else if (ShowingHps)
                        {
                            label = "EncHPS";
                        }
                        else if (ShowingAuto) {
                            label = "EncAuto";
                        }
                        else if (ShowingSkills) {
                            label = "EncSkills";
                        }
                        g.DrawString(Formatters.GetReadableNumber(maxVal), this.Font, _bLabels, DpsLeft - 8, DpsTop + 3, _sfFarMid);
                        g.DrawString(Formatters.GetReadableNumber(0), this.Font, _bLabels, DpsLeft - 8, DpsBottom - 3, _sfFarMid);
                        g.DrawString(label, this.Font, _bLabels, DpsLeft - 8, DpsTop + (DpsHeight / 2), _sfFarMid);
                    }

                    // Draw the time labels
                    g.DrawString(_startTime.ToString("h':'mm':'ss"), this.Font, _bLabels, GraphLeft - 20, GraphBottom + 8, _sfNearNear);
                    g.DrawString(_endTime.ToString("h':'mm':'ss"), this.Font, _bLabels, GraphRight + 20, GraphBottom + 8, _sfFarNear);
                    g.DrawString($"Period: {_settings.EncDpsResolution}s", this.Font, _bLabels, GraphLeft + ((GraphRight - GraphLeft) / 2), GraphBottom + 8, _sfMidNear);

                    // Draw a legend
                    List<string> playerKeys = GetStatPlayerKeys();
                    int yLast = LegendTop;
                    foreach (var playerKey in playerKeys)
                    {
                        using (var pen = new Pen(GetPlayerColour(playerKey), 2))
                            g.DrawLine(pen, LegendLeft, yLast, LegendLeft + 15, yLast);
                        g.DrawString(playerKey, this.Font, _bLabels, LegendLeft + 20, yLast, _sfNearMid);
                        yLast += 20;
                    }

                    // Draw a box representing the average of the stat (note that this only applies
                    // when a single stat is being shown)
                    if (_settings.GraphShowAverage && _valueAverage >= 0)
                    {
                        Point pTopLeft = GetDataPoint(_startTime, _valueAverage);
                        Point pTopRight = GetDataPoint(_endTime, _valueAverage);
                        g.FillRectangle(_bAvgFill, DataLeft, pTopLeft.Y, DataWidth, DataBottom - pTopLeft.Y);
                        g.DrawLine(_pAvg, pTopLeft, pTopRight);
                    }

                    // Draw the encDps/encHps line
                    if (ShowingAny)
                    {
                        foreach (string player in _playersEncSwings.Keys)
                        {
                            Dictionary<int, double> statPoints = _playersEncSwings[player];
                            Color lineColour = GetPlayerColour(player);
                            List<Tuple<Point, Point>> encLine = new List<Tuple<Point, Point>>();
                            Point prevPoint = statPoints.Count > 0 ? GetEncValPoint(0, statPoints[0], _maxSwing) : GetEncValPoint(0, 0, _maxSwing);
                            Point curPoint;
                            foreach (int tTime in statPoints.Keys)
                            {
                                curPoint = GetEncValPoint(tTime, statPoints[tTime], _maxSwing);
                                encLine.Add(new Tuple<Point, Point>(prevPoint, curPoint));
                                prevPoint = curPoint;
                            }
                            // Add the last point
                            curPoint = GetEncValPoint((int)Math.Floor(_totalSeconds), statPoints.Values.Last(), _maxSwing);
                            encLine.Add(new Tuple<Point, Point>(prevPoint, curPoint));
                            RenderLine(g, encLine, lineColour, 2f, 255);
                        }
                    }

                    // Now render the actual stat value lines
                    Dictionary<List<Tuple<Point, Point>>, LineOptions> measurementLines = new Dictionary<List<Tuple<Point, Point>>, LineOptions>();
                    Dictionary<List<Tuple<Point, Point>>, AreaOptions> measurementAreas = new Dictionary<List<Tuple<Point, Point>>, AreaOptions>();
                    for (int i = 0; i < _stats.Count; i++)
                        if (_stats[i].Readings.Count > 0)
                        {
                            StatterEncounterStat encStat = _stats[i];
                            Color measurementLineColour = GetPlayerColour(encStat.PlayerKey);
                            var readings = encStat.Readings;
                            StatterStatReading curReading;
                            StatterStatReading prevReading;
                            Point prevPoint;
                            Point wayPoint;
                            Point curPoint;
                            List<Tuple<Point, Point>> verticalLines = new List<Tuple<Point, Point>>();
                            List<Tuple<Point, Point>> nonOcLines = new List<Tuple<Point, Point>>();
                            List<Tuple<Point, Point>> ocLines = new List<Tuple<Point, Point>>();
                            List<Tuple<Point, Point>> areas = new List<Tuple<Point, Point>>();

                            // Iterate through the readings for this stat, and build collections
                            // of points that we can use to draw lines for the stat graph. We'll
                            // build each point and use the previous point to add the line points.
                            prevReading = readings[0];
                            for (int j = 0; j < readings.Count; j++)
                            {
                                curReading = readings[j];

                                // Since this stat can start after the fight T0, handle the first point specially
                                if (j == 0)
                                    prevPoint = GetDataPoint(_startTime, prevReading.Value);
                                else
                                    prevPoint = GetDataPoint(prevReading.Time, prevReading.Value);

                                // Need a horizontal line from the last reading to the current time
                                wayPoint = GetDataPoint(curReading.Time, prevReading.Value);
                                if (prevReading.Overcap)
                                    ocLines.Add(new Tuple<Point, Point>(prevPoint, wayPoint));
                                else
                                    nonOcLines.Add(new Tuple<Point, Point>(prevPoint, wayPoint));

                                // Now the vertical line
                                curPoint = GetDataPoint(curReading.Time, curReading.Value);
                                verticalLines.Add(new Tuple<Point, Point>(wayPoint, curPoint));

                                // If the stat has a secondary value, do the same thing for a second line
                                if (ShowingRanges && encStat.Stat.HasSecondary)
                                {
                                    if (j == 0)
                                        prevPoint = GetDataPoint(_startTime, prevReading.SecondaryValue);
                                    else
                                        prevPoint = GetDataPoint(prevReading.Time, prevReading.SecondaryValue);

                                    // Need a horizontal line from the last reading to the current time
                                    wayPoint = GetDataPoint(curReading.Time, prevReading.SecondaryValue);
                                    if (prevReading.Overcap)
                                        ocLines.Add(new Tuple<Point, Point>(prevPoint, wayPoint));
                                    else
                                        nonOcLines.Add(new Tuple<Point, Point>(prevPoint, wayPoint));

                                    // Now the vertical line
                                    curPoint = GetDataPoint(curReading.Time, curReading.SecondaryValue);
                                    verticalLines.Add(new Tuple<Point, Point>(wayPoint, curPoint));

                                    if (j == 0)
                                        prevPoint = GetDataPoint(_startTime, prevReading.SecondaryValue);
                                    else
                                        prevPoint = GetDataPoint(prevReading.Time, prevReading.SecondaryValue);

                                    curPoint = GetDataPoint(curReading.Time, prevReading.Value);
                                    areas.Add(new Tuple<Point, Point>(prevPoint, curPoint));
                                }

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
                            if (ShowingRanges && encStat.Stat.HasSecondary)
                            {
                                prevPoint = GetDataPoint(prevReading.Time, prevReading.SecondaryValue);
                                curPoint = GetDataPoint(_endTime, prevReading.SecondaryValue);
                                if (prevReading.Overcap)
                                    ocLines.Add(new Tuple<Point, Point>(prevPoint, curPoint));
                                else
                                    nonOcLines.Add(new Tuple<Point, Point>(prevPoint, curPoint));

                                curPoint = GetDataPoint(_endTime, prevReading.Value);
                                areas.Add(new Tuple<Point, Point>(prevPoint, curPoint));
                            }

                            measurementLines.Add(StretchLines(ocLines, 1, 0), new LineOptions() { Color = measurementLineColour, Width = LINE_OC_WIDTH });
                            measurementLines.Add(ShiftLines(nonOcLines, 0, 0), new LineOptions() { Color = measurementLineColour, Width = LINE_NON_OC_WIDTH });
                            measurementLines.Add(ShiftLines(verticalLines, 0, 0), new LineOptions() { Color = measurementLineColour, Width = LINE_NON_OC_WIDTH });

                            measurementAreas.Add(areas, new AreaOptions() { Color = measurementLineColour });

                            // Draw the stat label on the axis
                            string statLabel = encStat.Stat.Name.Replace(" ", Environment.NewLine);
                            g.DrawString(statLabel, this.Font, _bLabels, DataLeft - 8, DataTop + (DataHeight / 2), _sfFarMid);
                        }

                    RenderAreas(g, measurementAreas);
                    RenderLines(g, measurementLines);

                    // Render pins
                    foreach (var pin in _pins)
                    {
                        g.DrawLine(_pCrosshair, DataLeft, pin.Point.Y, DataRight, pin.Point.Y);
                        g.DrawString(pin.Label, this.Font, _bHighlight, DataRight + 5, pin.Point.Y, _sfNearMid);
                    }
                }

                SwapBuff(buff);
            }
            catch { }
        }

        private Color GetPlayerColour(string player)
        {
            Color colour = Color.Gray;
            if (_statPlayerLineColourMap.ContainsKey(player) && _statPlayerLineColourMap[player] != null)
                colour = _statPlayerLineColourMap[player];

            return colour;
        }

        // Extract all points to an array, optionally shifting them by xOffset or yOffset
        private List<Tuple<Point, Point>> ShiftLines(List<Tuple<Point, Point>> lines, int xOffset, int yOffset)
        {
            List<Tuple<Point, Point>> newLines = new List<Tuple<Point, Point>>();

            foreach (var line in lines)
                newLines.Add(new Tuple<Point, Point>(
                    new Point(line.Item1.X + xOffset, line.Item1.Y + yOffset),
                    new Point(line.Item2.X + xOffset, line.Item2.Y + yOffset)
                ));

            return newLines;
        }

        private List<Tuple<Point, Point>> StretchLines(List<Tuple<Point, Point>> lines, int xStretch, int yStretch)
        {
            List<Tuple<Point, Point>> newLines = new List<Tuple<Point, Point>>();

            foreach (var line in lines)
                newLines.Add(new Tuple<Point, Point>(
                    new Point(line.Item1.X - xStretch, line.Item1.Y - yStretch),
                    new Point(line.Item2.X + xStretch, line.Item2.Y + yStretch)
                ));

            return newLines;
        }

        private void RenderLines(Graphics g, Dictionary<List<Tuple<Point, Point>>, LineOptions> lines)
        {
            foreach (var lineList in lines.Keys)
                RenderLine(g, lineList, lines[lineList].Color, lines[lineList].Width, lines[lineList].Alpha);
        }

        private void RenderLine(Graphics g, List<Tuple<Point, Point>> lines, Color baseColour, float lineWidth, int alpha)
        {
            using (var pLine = new Pen(ShiftAlpha(baseColour, alpha), lineWidth))
                foreach (var line in lines)
                    g.DrawLine(pLine, line.Item1, line.Item2);
        }

        private void RenderAreas(Graphics g, Dictionary<List<Tuple<Point, Point>>, AreaOptions> areas)
        {
            foreach (var areaList in areas.Keys)
                using (var bFill = new SolidBrush(ShiftAlpha(areas[areaList].Color, 16)))
                    foreach (Tuple<Point, Point> coords in areaList)
                        g.FillRectangle(bFill, coords.Item1.X, coords.Item2.Y, coords.Item2.X - coords.Item1.X, coords.Item1.Y - coords.Item2.Y);
        }

        // Get a point inside the stat drawing area that corresponds to the given time and dps/hps value
        private Point GetEncValPoint(int timeOffsetSeconds, double encVal, double maxEncVal)
        {
            int x = 0, y = 0;

            if (_totalSeconds <= 0)
                x = DpsLeft + (DpsWidth / 2);
            else
                x = DpsLeft + (int)((timeOffsetSeconds / _totalSeconds) * DpsWidth);

            y = maxEncVal <= 0 ? 0 : DpsBottom - (int)((encVal / maxEncVal) * DpsHeight);

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
            DateTime timeAtPoint = _startTime.AddSeconds(Math.Max(0, (((p.X - GraphLeft) / (1.0 * GraphWidth)) * _totalSeconds)));
            return timeAtPoint;
        }

        private double GetValueAt(Point p)
        {
            return _maxVal - ((((p.Y - DataTop) / (1.0 * DataHeight)) * _valueSpread));
        }

        private double GetEncValAt(Point p, double maxVal)
        {
            return maxVal - ((((p.Y - DpsTop) / (1.0 * DpsHeight)) * maxVal));
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
            if (baseColour == null)
                return Color.Gray;

            return Color.FromArgb(Math.Min(255, Math.Max(0, newAlpha)), baseColour.R, baseColour.G, baseColour.B);
        }

        // This is the main draw routine. Calculate some state values, then render and display the buffer
        public void DrawStats(List<StatterEncounterStat> stats, DateTime startTime, DateTime endTime, EncounterData encData, Color fg, Color bg)
        {
            _pins.Clear();
            _fg = fg;
            _bg = bg;
            SetColours();

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

                    if (ShowingRanges && stat.Stat.HasSecondary)
                    {
                        if (stat.MinReading.SecondaryValue < _minVal) _minVal = stat.MinReading.SecondaryValue;
                        if (stat.MaxReading.SecondaryValue > _maxVal) _maxVal = stat.MaxReading.SecondaryValue;
                    }
                }
            }
            bool minIsZero = _settings.YMin.Equals("0");
            if (minIsZero)
                _minVal = 0;
            _minVal = _minVal == double.MaxValue ? 0 : _minVal;
            _maxVal = _maxVal == double.MinValue ? 0 : _maxVal;
            _valueSpread = _maxVal - _minVal;

            // Now calculate the average value if that concept applies
            if (_stats.Count == 1 && _stats[0].Readings.Count > 1 && _totalSeconds > 0)
            {
                _valueAverage = _stats[0].AvgReading.Value;
            }
            else
                _valueAverage = -1;

            // Calculate enc dps/hps
            _playersEncSwings.Clear();
            _maxSwing = 0;
            if (_stats.Count > 0 && ShowSwings)
                foreach (var playerKey in GetStatPlayerKeys())
                {
                    var playerName = playerKey.Equals(StatterStatReading.DEFAULT_PLAYER_KEY) ? encData.CharName : playerKey;

                    // To generate the dps/hps graph, just sum up the damage done each second
                    // over the duration of the enncounter. Then pass back over and accumulate
                    // damage done in the last n seconds, where n is defined by the resolution
                    // slider. ACT uses 10 sec in its own graphs (which is the max duration we use,
                    // corresponding to the "lowest" resolution).
                    int accumulateSeconds = _settings.EncDpsResolution;
                    List<MasterSwing> swings = null;
                    if (playerName != null)
                        foreach (var combatant in encData.Items)
                            if (combatant.Key.Equals(playerName.ToUpper()))
                                foreach (var combatItem in combatant.Value.Items)
                                {
                                    if ((_settings.GraphShowEncDps && combatItem.Key.Equals("Outgoing Damage")) ||
                                        (_settings.GraphShowEncHps && combatItem.Key.Equals("Healed (Out)")) ||
                                        (_settings.GraphShowEncAuto && combatItem.Key.Equals("Auto-Attack (Out)")) ||
                                        (_settings.GraphShowEncSkills && combatItem.Key.Equals("Skill/Ability (Out)")))
                                        foreach (var subItem in combatItem.Value.Items)
                                            if (subItem.Key.Equals("All"))
                                                swings = subItem.Value.Items;
                                }
                    if (swings != null)
                    {
                        int encDurationSec = Convert.ToInt32(_totalSeconds);
                        double[] valueAtSecond = new double[encDurationSec];
                        foreach (var swing in swings)
                        {
                            int t = Math.Min(encDurationSec - 1, Convert.ToInt32((swing.Time - _startTime).TotalSeconds));
                            valueAtSecond[t] += swing.Damage.Number;
                        }
                        var encSwings = new Dictionary<int, double>();
                        for (int i = 0; i < encDurationSec; i++)
                        {
                            double sumInRollingPeriod = 0;
                            for (int j = i; j > (i - accumulateSeconds) && j >= 0; j--)
                                sumInRollingPeriod += valueAtSecond[j];
                            double valPerSec = sumInRollingPeriod / (accumulateSeconds * 1.0);
                            if (valPerSec > _maxSwing)
                                _maxSwing = valPerSec;
                            encSwings.Add(i, valPerSec);
                        }
                        _playersEncSwings.Add(playerKey, encSwings);
                    }
                }

            // Assign colours to the stat players. "You" should always exclusively use the same (first) colour
            _statPlayerLineColourMap.Clear();
            stats.Sort((x, y) => x.PlayerKey.CompareTo(y.PlayerKey));
            stats.ForEach(stat =>
            {
                if (StatterStatReading.IsFirstPersonKey(stat.PlayerKey))
                    _statPlayerLineColourMap.Add(stat.PlayerKey, _statLineColours[0]);
            });
            int defaultPlayerOffset = _statPlayerLineColourMap.Count;
            stats.ForEach(stat =>
            {
                if (!StatterStatReading.IsFirstPersonKey(stat.PlayerKey))
                    _statPlayerLineColourMap.Add(stat.PlayerKey, _statLineColours[1 + (_statPlayerLineColourMap.Count - defaultPlayerOffset)]);
            });

            UpdateGraph();
        }

        public void UpdateGraph()
        {
            Render();
            Refresh();
        }
    }
}
