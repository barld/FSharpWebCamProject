using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using Touchless.Vision.Contracts;

namespace Demo
{
    public partial class GraphForm : Form
    {
        public GraphForm()
        {
            InitializeComponent();
        }

        private double f(int i)
        {
            var f1 = 59894 - (8128 * i) + (262 * i * i) - (1.6 * i * i * i);
            return f1;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            chart.Series.Clear();
            var series1 = new System.Windows.Forms.DataVisualization.Charting.Series
            {
                Name = "Series1",
                Color = System.Drawing.Color.Green,
                IsVisibleInLegend = false,
                IsXValueIndexed = true,
                ChartType = SeriesChartType.Line
            };

            this.chart.Series.Add(series1);

            for (int i = 0; i < 100; i++)
            {
                series1.Points.AddXY(i, f(i));
            }
            chart.Invalidate();
        }

        Random rnd = new Random();
        bool bussy = false;

        public void Update(IFrameSource frameSource, Bitmap bitmap)
        {
            if (bussy)
                return;

            if (InvokeRequired)
            {
                Invoke(new Action<IFrameSource, Bitmap>(Update), frameSource, bitmap);
                return;
            }
            bussy = true;
            var data = BitmapConverting.getHHistogramData(bitmap);



            this.chart.Series.Clear();
            var series1 = new System.Windows.Forms.DataVisualization.Charting.Series
            {
                Name = "Series1",
                Color = System.Drawing.Color.Green,
                IsVisibleInLegend = false,
                IsXValueIndexed = true,
                ChartType = SeriesChartType.Column
            };

            this.chart.Series.Add(series1);

            foreach(var p in data)
            {
                series1.Points.AddXY(p.Item1,p.Item2);
            }
            chart.Invalidate();

            bussy = false;
        }
    }
}
