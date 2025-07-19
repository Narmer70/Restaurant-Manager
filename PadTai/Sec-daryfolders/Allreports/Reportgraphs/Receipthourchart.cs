using System;
using System.Data;
using System.Linq;
using System.Text;
using PadTai.Classes;
using System.Drawing;
using System.Data.SQLite;
using System.Windows.Forms;
using System.ComponentModel;
using System.Data.SqlClient;
using PadTai.Classes.Others;
using System.Collections.Generic;
using System.Windows.Forms.DataVisualization.Charting;
using PadTai.Classes.Databaselink;


namespace PadTai.Sec_daryfolders.Allreports.Reportgraphs
{
    public partial class Receipthourchart : UserControl
    {
        private Random random = new Random();
        private CrudDatabase crudDatabase;  
        private FontResizer fontResizer;
        private ControlResizer resizer;

        public Receipthourchart()
        {
            InitializeComponent();
            InitializeControlResizer();
            crudDatabase = new CrudDatabase();

            LocalizeControls();
            ApplyTheme();

            chart1.BackColor = roundedPanel1.BackColor;
            chart2.BackColor = roundedPanel1.BackColor;

            LoadChart();
        }

        private void InitializeControlResizer()
        {
            fontResizer = new FontResizer();
            fontResizer.AdjustFont(this);           
            
            resizer = new ControlResizer(this.Size);
            resizer.RegisterControl(chart1);
            resizer.RegisterControl(chart2);
            resizer.RegisterControl(label1);
            resizer.RegisterControl(label2);
            resizer.RegisterControl(label3);
            resizer.RegisterControl(label4);
            resizer.RegisterControl(label5);
            resizer.RegisterControl(label6);
            resizer.RegisterControl(label7);
            resizer.RegisterControl(label8);
            resizer.RegisterControl(label9);
            resizer.RegisterControl(pictureBox1);
            resizer.RegisterControl(pictureBox2);
            resizer.RegisterControl(pictureBox3);
            resizer.RegisterControl(roundedPanel1);
            resizer.RegisterControl(roundedPanel2);
            resizer.RegisterControl(roundedPanel3);
            resizer.RegisterControl(roundedPanel4);
            resizer.RegisterControl(roundedPanel5);
            resizer.RegisterControl(roundedPanel6);
            resizer.RegisterControl(roundedPanel7);
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            if (resizer != null)
            {
                resizer.ResizeControls(this);
            }
        }


        private void LoadChart()
        {
            int clientId;
            if (!int.TryParse(Properties.Settings.Default.SelectedClientId, out clientId))
            {
                MessageBox.Show("Invalid Client ID.");
                return;
            }

            List<(int Hour, int ReceiptCount)> data = new List<(int, int)>();

            string query = @"SELECT strftime('%H', OrderDateTime) AS Hour, COUNT(ReceiptID) AS ReceiptCount 
                 FROM Receipts WHERE ClientID = @ClientID GROUP BY Hour ORDER BY Hour;";

            var parameters = new Dictionary<string, object>
            {
                { "@ClientID", clientId }
             };

            try
            {
                DataTable resultData = crudDatabase.FetchDataFromDatabase(query, parameters);

                foreach (DataRow row in resultData.Rows)
                {
                    int hour = Convert.ToInt32(row["Hour"]);
                    int receiptCount = Convert.ToInt32(row["ReceiptCount"]);
                    data.Add((hour, receiptCount));
                }
            }
            catch (SqlException Sqlex)
            {
                MessageBox.Show("Error: " + Sqlex.Message);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Mistake: " + ex.Message);
            }


            chart2.Series.Clear();
            configureChart1();
            configureChart2();

            Series columnSeries = chart2.Series.FindByName("Series1") ?? new Series
            {
                Name = "Series1", 
                ChartType = SeriesChartType.Column,
                Color = System.Drawing.Color.DodgerBlue,
            };

            chart2.Series.Add(columnSeries);

            if (data.Count == 0)
            {
                return;
            }
            else if (data.Count == 1)
            {
                var entry = data.First();
                int receiptCount = entry.ReceiptCount;
                columnSeries.LabelForeColor = this.ForeColor;
                columnSeries.Points.AddXY($"{entry.Hour}{LanguageManager.Instance.GetString("Hour")}", receiptCount);

                columnSeries.Points[0].IsValueShownAsLabel = true;
                columnSeries.Points[0].Label = receiptCount.ToString();

                //targetValues = new double[] { receiptCount };
            }
            else
            {
                int minHour = data.Min(x => x.Hour);
                int maxHour = data.Max(x => x.Hour);

                //targetValues = new double[maxHour - minHour + 1];

                for (int hour = minHour; hour <= maxHour; hour++)
                {
                    var entry = data.FirstOrDefault(x => x.Hour == hour);
                    int receiptCount = entry != default ? entry.ReceiptCount : 0;

                    columnSeries.Points.AddXY($"{hour}{LanguageManager.Instance.GetString("Hour")}", receiptCount);
                    columnSeries.LabelForeColor = this.ForeColor;

                    //targetValues[hour - minHour] = receiptCount;

                    if (receiptCount > 0)
                    {
                        columnSeries.Points[columnSeries.Points.Count - 1].IsValueShownAsLabel = true;
                        columnSeries.Points[columnSeries.Points.Count - 1].Label = receiptCount.ToString();
                    }
                    else
                    {
                        columnSeries.Points[columnSeries.Points.Count - 1].IsValueShownAsLabel = false;
                    }
                }
            }

            // Prepare the donut series for the chart
            Series donutSeries = chart1.Series.FindByName("Series1") ?? new Series
            {
                Name = "Series1",
                ChartType = SeriesChartType.Doughnut, 
            };

            if (chart1.Series.Contains(donutSeries))
            {
                donutSeries.Points.Clear();
            }
            else
            {
                chart1.Series.Add(donutSeries);
            }

            if (data.Count == 0)
            {
                return;
            }
            else
            {
                double totalCount = data.Sum(entry => entry.ReceiptCount);
                foreach (var entry in data)
                {
                    donutSeries.Points.AddXY($"{entry.Hour}{LanguageManager.Instance.GetString("Hour")}", entry.ReceiptCount);
                    var lastPoint = donutSeries.Points[donutSeries.Points.Count - 1];

                    lastPoint.Color = GetRandomColor();
                    lastPoint.BorderColor = chart1.BackColor;
                    double percentage = (double)entry.ReceiptCount / totalCount * 100;
                    lastPoint.Label = $"{entry.ReceiptCount}";
                    lastPoint.LegendText = $"{entry.Hour}{LanguageManager.Instance.GetString("Hour")}: {entry.ReceiptCount} ({percentage:N1}%)";
                    lastPoint.LabelForeColor = this.ForeColor;
                }
            }
        }

        private void configureChart1()
        {
            ChartArea chartArea = chart1.ChartAreas.FirstOrDefault(ca => ca.Name == "ChartArea1");
            if (chartArea == null)
            {
                chartArea = new ChartArea("ChartArea1");
                chart1.ChartAreas.Add(chartArea);
            }
            chartArea.BackColor = chart1.BackColor;

            Legend legend = chart1.Legends.FirstOrDefault(l => l.Name == "Legend1");
            if (legend != null)
            {
                legend.BackColor = chart1.BackColor;
                legend.ForeColor = this.ForeColor;
            }
            else
            {
            }

            Title title = chart1.Titles.FirstOrDefault(t => t.Name == "Title1");
            if (title != null)
            {
                title.BackColor = chart1.BackColor;
                title.ForeColor = this.ForeColor;
            }
            else
            {
            }
            chart1.Invalidate();
        }

        private void configureChart2()
        {
            chart2.ChartAreas[0].AxisX.LabelStyle.ForeColor = this.ForeColor;
            chart2.ChartAreas[0].AxisY.LabelStyle.ForeColor = this.ForeColor;
            chart2.ChartAreas[0].AxisY.MajorGrid.LineColor = this.BackColor;

            ChartArea chartArea = chart2.ChartAreas.FirstOrDefault(ca => ca.Name == "ChartArea1");
            if (chartArea == null)
            {
                chartArea = new ChartArea("ChartArea1");
                chart2.ChartAreas.Add(chartArea);
            }
            chartArea.BackColor = chart2.BackColor;

            Legend legend = chart2.Legends.FirstOrDefault(l => l.Name == "Legend1");
            if (legend != null)
            {
                legend.BackColor = chart2.BackColor;
                legend.ForeColor = this.ForeColor;
            }
            else
            {
            }

            Title title = chart2.Titles.FirstOrDefault(t => t.Name == "Title1");
            if (title != null)
            {
                title.BackColor = chart2.BackColor;
                title.ForeColor = this.ForeColor;
            }
            else
            {
            }
            chart2.Invalidate();
        }


        //private void timer1_Tick(object sender, EventArgs e)
        //{
        //    if (this.IsDisposed || this.Disposing || this.IsHandleCreated == false)
        //        return;

        //    if (animationStep < animationSteps)
        //    {
        //        try
        //        {
        //            if (chart2 != null && chart2.Series.Count > 0)
        //            {
        //                var series = chart2.Series[0];
        //                if (series.Points.Count == targetValues.Length)
        //                {
        //                    for (int i = 0; i < series.Points.Count; i++)
        //                    {
        //                        var currentPoint = series.Points[i];
        //                        currentPoint.YValues[0] = targetValues[i] * (animationStep / (double)animationSteps);
        //                    }
        //                    chart2.Invalidate();
        //                    animationStep++;
        //                }
        //                else
        //                {
        //                }
        //            }
        //            else
        //            {
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            Console.WriteLine($"An error occurred: {ex.Message}");
        //        }
        //    }
        //    else
        //    {
        //        animationTimer.Stop();
        //    }
        //}

        private System.Drawing.Color GetRandomColor()
        {
            // Generate random RGB values
            int r = random.Next(256);
            int g = random.Next(256);
            int b = random.Next(256);
            return System.Drawing.Color.FromArgb(r, g, b);
        }

        public void LocalizeControls()
        {
            var title1 = chart1.Titles[0];
            title1.Text = LanguageManager.Instance.GetString("Dcharts-receiptt1");

            var title2 = chart2.Titles[0];
            title2.Text = LanguageManager.Instance.GetString("Dcharts-receiptt2");

            //button1.Text = LanguageManager.Instance.GetString("MF-btn1");
            //button2.Text = LanguageManager.Instance.GetString("MF-btn2");
            //button3.Text = LanguageManager.Instance.GetString("MF-btn3");
            //button4.Text = LanguageManager.Instance.GetString("MF-btn4");
            //button5.Text = LanguageManager.Instance.GetString("MF-btn5");
        }

        public void ApplyTheme()
        {
            ColorPalette.ColorTrio colors = ColorPalette.GetColorTrio();

            this.BackColor = colors.Color1;
            this.ForeColor = colors.Color2;

            roundedPanel1.BackColor = colors.Color3;
            roundedPanel2.BackColor = colors.Color3;
            roundedPanel3.BackColor = colors.Color3;
            roundedPanel4.BackColor = colors.Color3;
            roundedPanel5.BackColor = colors.Color3;
            roundedPanel6.BackColor = colors.Color3;
            roundedPanel7.BackColor = colors.Color3;
            //dataGridView1.BackgroundColor = colors.Color3;
            //dataGridView1.DefaultCellStyle.BackColor = colors.Color3;

            label1.ForeColor = this.ForeColor;
            label2.ForeColor = this.ForeColor;
            label3.ForeColor = this.ForeColor;
            label4.ForeColor = this.ForeColor;
            label5.ForeColor = this.ForeColor;
            label6.ForeColor = this.ForeColor;
            label7.ForeColor = this.ForeColor;
            label8.ForeColor = this.ForeColor;
            label9.ForeColor = this.ForeColor;
            chart1.ForeColor = this.ForeColor;
            chart2.ForeColor = this.ForeColor;
            roundedPanel1.ForeColor = this.ForeColor;
            roundedPanel2.ForeColor = this.ForeColor;
            roundedPanel3.ForeColor = this.ForeColor;
            roundedPanel4.ForeColor = this.ForeColor;
            roundedPanel5.ForeColor = this.ForeColor;
            roundedPanel6.ForeColor = this.ForeColor;
            roundedPanel7.ForeColor = this.ForeColor;
            //dataGridView1.DefaultCellStyle.ForeColor = this.ForeColor;
            //dataGridView1.DefaultCellStyle.SelectionForeColor = this.ForeColor;
        }
    }
}
