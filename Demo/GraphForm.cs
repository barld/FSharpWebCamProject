//using System;
//using System.Collections.Generic;
//using System.ComponentModel;
//using System.Data;
//using System.Drawing;
//using System.Linq;
//using System.Text;
//using System.Windows.Forms;
//using System.Windows.Forms.DataVisualization.Charting;
//using Touchless.Vision.Contracts;

//namespace Demo
//{
//    public partial class GraphForm : Form
//    {
//        public GraphForm()
//        {
//            InitializeComponent();
//        }

//        private void Form1_Load(object sender, EventArgs e)
//        {
//        }

//        Random rnd = new Random();
//        bool bussy = false;

//        public void Update(IFrameSource frameSource, Bitmap bitmap)
//        {
//            if (bussy)
//                return;

//            if (InvokeRequired)
//            {
//                Invoke(new Action<IFrameSource, Bitmap>(Update), frameSource, bitmap);
//                return;
//            }
//            bussy = true;
//            //var data = BitmapConverting.getHHistogramData(bitmap);



//            this.chart.Series.Clear();
//            var series1 = new System.Windows.Forms.DataVisualization.Charting.Series
//            {
//                Name = "Series1",
//                Color = System.Drawing.Color.Green,
//                IsVisibleInLegend = false,
//                IsXValueIndexed = true,
//                ChartType = SeriesChartType.Column
                
//            };

//            this.chart.Series.Add(series1);

//            foreach(var p in data)
//            {
//                series1.Points.AddXY(p.Item1,p.Item2);
//            }
//            chart.ChartAreas[0].AxisX.Minimum = 0;
//            chart.ChartAreas[0].AxisX.Maximum = 36;

//            chart.Invalidate();

//            bussy = false;
//        }
//    }
//}
