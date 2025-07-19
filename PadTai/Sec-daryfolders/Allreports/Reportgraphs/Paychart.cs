using System;
using System.Data;
using System.Linq;
using System.Text;
using System.Drawing;
using PadTai.Classes;
using System.Data.SQLite;
using System.Windows.Forms;
using PadTai.Classes.Others;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Windows.Forms.DataVisualization.Charting;
using UserControl = System.Windows.Forms.UserControl;
using PadTai.Classes.Databaselink;
using System.Data.SqlClient;


namespace PadTai.Sec_daryfolders.Allreports.Reportgraphs
{
    public partial class Paychart : UserControl
    {
        private Dictionary<string, int> paymentTypeData;
        private Random random = new Random();
        private CrudDatabase crudDatabase;  
        private FontResizer fontResizer;
        private ControlResizer resizer;

        public Paychart()
        {
            InitializeComponent();
            InitializeControlResizer();
            crudDatabase = new CrudDatabase();

            LocalizeControls();
            ApplyTheme();

            chart1.BackColor = roundedPanel1.BackColor;
            chart2.BackColor = roundedPanel1.BackColor;

            LoadDonutChart();
            LoadLineChart();        
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

        private void LoadDonutChart()
        {
            int clientId;
            if (!int.TryParse(Properties.Settings.Default.SelectedClientId, out clientId))
            {
                throw new Exception("Invalid Client ID. Please ensure a valid Client ID is selected.");
            }

            string query = @"SELECT PaymentTypeName, COUNT(ReceiptID) AS OccurrenceCount
                 FROM Receipts WHERE ClientID = @ClientID GROUP BY PaymentTypeName;";

            var parameters = new Dictionary<string, object>
            {
                { "@ClientID", clientId }
            };

            try
            {
                DataTable paymentTypeDataTable = crudDatabase.FetchDataFromDatabase(query, parameters);
                paymentTypeData = new Dictionary<string, int>();

                foreach (DataRow row in paymentTypeDataTable.Rows)
                {
                    string paymentTypeName = row["PaymentTypeName"].ToString();

                    if (row["OccurrenceCount"] != DBNull.Value)
                    {
                        int occurrenceCount = Convert.ToInt32(row["OccurrenceCount"]);
                        paymentTypeData[paymentTypeName] = occurrenceCount;
                    }
                }
            }
            catch (SqlException Sqlex)
            {
                MessageBox.Show(LanguageManager.Instance.GetString("Unexpectederror") + Sqlex.Message);
            }
            catch (Exception ex)
            {
                MessageBox.Show(LanguageManager.Instance.GetString("Unexpectederror") + ex.Message);
            }


            configureChart1();
            Series donutSeries = chart1.Series.FindByName("Series1") ?? new Series
            {
                Name = "Series1",
            };

            if (chart1.Series.Contains(donutSeries))
            {
                donutSeries.Points.Clear();
            }
            else
            {
                chart1.Series.Add(donutSeries);
            }

            double totalCount = paymentTypeData.Values.Sum();

            foreach (var paymentType in paymentTypeData)
            {
                donutSeries.Points.AddXY(paymentType.Key, paymentType.Value);
                var lastPoint = donutSeries.Points[donutSeries.Points.Count - 1];

                lastPoint.Color = GetRandomColor();
                lastPoint.BorderColor = chart1.BackColor;
                double percentage = (double)paymentType.Value / totalCount * 100;
                donutSeries.Points[donutSeries.Points.Count - 1].Label = $"{paymentType.Value}"; 
                donutSeries.Points[donutSeries.Points.Count - 1].LegendText = $"{paymentType.Key}: {paymentType.Value} ({percentage:N1}%)";
                lastPoint.LabelForeColor = this.ForeColor;
            }
        }



        private void LoadLineChart()
        {
            int clientId;
            if (!int.TryParse(Properties.Settings.Default.SelectedClientId, out clientId))
            {
                throw new Exception("Invalid Client ID. Please ensure a valid Client ID is selected.");
            }

            string query = @"SELECT strftime('%H', OrderDateTime) AS Hour, COUNT(ReceiptID) AS OccurrenceCount, 
                 PaymentTypeName FROM Receipts WHERE ClientID = @ClientID GROUP BY Hour, PaymentTypeName ORDER BY Hour;";

            var parameters = new Dictionary<string, object>
            {
                { "@ClientID", clientId }
            };

            var paymentTypeData = new Dictionary<string, Dictionary<int, int>>();

            try
            {
                DataTable paymentTypeDataTable = crudDatabase.FetchDataFromDatabase(query, parameters);

                foreach (DataRow row in paymentTypeDataTable.Rows)
                {
                    string paymentType = row["PaymentTypeName"].ToString();
                    int hour = Convert.ToInt32(row["Hour"]);
                    int occurrenceCount = Convert.ToInt32(row["OccurrenceCount"]);

                    if (!paymentTypeData.ContainsKey(paymentType))
                    {
                        paymentTypeData[paymentType] = new Dictionary<int, int>();
                    }

                    paymentTypeData[paymentType][hour] = occurrenceCount;
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
            configureChart2();

            var uniqueHours = paymentTypeData.Values.SelectMany(x => x.Keys).Distinct().ToList();

            if (uniqueHours.Count == 0)
            {
                return;
            }
            else if (uniqueHours.Count == 1)
            {
                int singleHour = uniqueHours.First();

                foreach (var paymentType in paymentTypeData)
                {
                    if (paymentType.Value.ContainsKey(singleHour))
                    {
                        Series lineSeries = chart2.Series.FindByName(paymentType.Key) ?? new Series
                        {
                            Name = paymentType.Key,
                            Color = GetRandomColor(),
                            ChartType = SeriesChartType.Column,
                        };

                        lineSeries.Points.AddXY($"{singleHour}{LanguageManager.Instance.GetString("Hour")}", paymentType.Value[singleHour]);
                        lineSeries.LabelForeColor = this.ForeColor;

                        foreach (var point in lineSeries.Points)
                        {
                            if (point.YValues[0] != 0)
                            {
                                point.Label = point.YValues[0].ToString();
                                point.LabelForeColor = this.ForeColor;
                            }
                        }
                        chart2.Series.Add(lineSeries);
                    }
                }
            }

            else
            {
                int minHour = paymentTypeData.Values.SelectMany(x => x.Keys).Min();
                int maxHour = paymentTypeData.Values.SelectMany(x => x.Keys).Max();

                foreach (var paymentType in paymentTypeData)
                {
                    Series columnSeries = new Series
                    {
                        Name = paymentType.Key,
                        Color = GetRandomColor(),
                        ChartType = SeriesChartType.Column,
                    };

                    for (int hour = minHour; hour <= maxHour; hour++)
                    {
                        int receiptCount = paymentType.Value.ContainsKey(hour) ? paymentType.Value[hour] : 0;
                        columnSeries.Points.AddXY($"{hour}{LanguageManager.Instance.GetString("Hour")}", receiptCount);
                        columnSeries.LabelForeColor = this.ForeColor;

                        if (receiptCount != 0)
                        {
                            var point = columnSeries.Points[columnSeries.Points.Count - 1];
                            point.Font = new Font("Century Gothic", 8, FontStyle.Regular);
                            point.Label = receiptCount.ToString();
                            point.LabelForeColor = this.ForeColor;
                        }
                    }

                    chart2.Series.Add(columnSeries);
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
            title1.Text = LanguageManager.Instance.GetString("Dcharts-payt1");

            var title2 = chart2.Titles[0];
            title2.Text = LanguageManager.Instance.GetString("Dcharts-payt2");

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