using System;
using System.Data;
using System.Linq;
using System.Text;
using PadTai.Classes;
using System.Drawing;
using System.Data.SQLite;
using System.Windows.Forms;
using PadTai.Classes.Others;
using System.Data.SqlClient;
using System.Threading.Tasks;
using System.Collections.Generic;
using PadTai.Classes.Databaselink;
using System.Windows.Forms.DataVisualization.Charting;


namespace PadTai.Sec_daryfolders.Allreports.Reportgraphs
{
    public partial class Foodgroupchart : UserControl
    {
        private Dictionary<int, string> foodItemNames;
        private Dictionary<int,int > foodItemCounts;
        private Random random = new Random();
        private CrudDatabase crudDatabase;
        private FontResizer fontResizer;
        private ControlResizer resizer;

        public Foodgroupchart()
        {
            InitializeComponent();
            InitializeControlResizer();
            crudDatabase = new CrudDatabase();

            LocalizeControls();
            ApplyTheme();

            chart1.BackColor = roundedPanel1.BackColor;
            chart2.BackColor = roundedPanel1.BackColor;
            
            LoadFoodItemGraphs();
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

        private void LoadFoodItemGraphs()
        {
            int clientId;
            foodItemNames = new Dictionary<int, string>();

            if (!int.TryParse(Properties.Settings.Default.SelectedClientId, out clientId))
            {
                throw new Exception("Invalid Client ID. Please ensure a valid Client ID is selected.");
            }

            // Retrieve food item types from the FoodItemsTypes table
            string foodItemTypesQuery = "SELECT FooditemtypeID, FooditemtypeName FROM FoodItemsTypes";

            try
            {
                DataTable foodItemTypesData = crudDatabase.FetchDataFromDatabase(foodItemTypesQuery);

                foreach (DataRow row in foodItemTypesData.Rows)
                {
                    int id = Convert.ToInt32(row["FooditemtypeID"]);
                    string name = row["FooditemtypeName"].ToString();
                    foodItemNames[id] = name; // Populate the dictionary
                }

                string query = "SELECT FooditemtypeID FROM Receipts WHERE ClientID = @ClientID";
                var parameters = new Dictionary<string, object>
                {
                    { "@ClientID", clientId }
                };

                DataTable receiptData = crudDatabase.FetchDataFromDatabase(query, parameters);
                foodItemCounts = new Dictionary<int, int>();
                configureChart1();

                foreach (DataRow receiptRow in receiptData.Rows)
                {
                    // Split concatenated FoodItemTypeID values
                    string[] types = receiptRow["FooditemtypeID"].ToString().Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                    foreach (var type in types)
                    {
                        if (int.TryParse(type.Trim(), out int id))
                        {
                            if (foodItemCounts.ContainsKey(id))
                            {
                                foodItemCounts[id]++;
                            }
                            else
                            {
                                foodItemCounts[id] = 1;
                            }
                        }
                    }
                }

                int totalCount = foodItemCounts.Values.Sum();

                Series pieSeries = chart1.Series.FindByName("Series1") ?? new Series
                {
                    Name = "Series1",
                };

                if (chart1.Series.Contains(pieSeries))
                {
                    pieSeries.Points.Clear();
                }
                else
                {
                    chart1.Series.Add(pieSeries);
                }

                foreach (var kvp in foodItemCounts)
                {
                    string foodItemName;
                    double percentage = (double)kvp.Value / totalCount * 100;
                    Color randomColor = GenerateRandomColor();

                    if (foodItemNames.TryGetValue(kvp.Key, out foodItemName))
                    {
                        pieSeries.Points.Add(new DataPoint
                        {
                            YValues = new double[] { kvp.Value },
                            Label = $"{foodItemName}",
                            Color = randomColor,
                        });
                    }
                    else
                    {
                        pieSeries.Points.Add(new DataPoint
                        {
                            YValues = new double[] { kvp.Value },
                            Label = $"NULL ({kvp.Key})",
                            Color = randomColor,
                        });
                    }

                    var lastPoint = pieSeries.Points[pieSeries.Points.Count - 1];
                    lastPoint.LegendText = $"{foodItemName}: {kvp.Value} ({percentage:F1}%)";
                    lastPoint.BorderColor = chart1.BackColor;
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
        }



        private void LoadLineChart()
        {
            int clientId;
            if (!int.TryParse(Properties.Settings.Default.SelectedClientId, out clientId))
            {
                throw new Exception("Invalid Client ID. Please ensure a valid Client ID is selected.");
            }

            string query = @"SELECT strftime('%H', OrderDateTime) AS Hour, FooditemtypeID FROM Receipts WHERE ClientID = @ClientID;";
            var parameters = new Dictionary<string, object>
            {
                { "@ClientID", clientId }
            };

            var foodItemData = new Dictionary<string, Dictionary<int, int>>();

            try
            {
                DataTable receiptData = crudDatabase.FetchDataFromDatabase(query, parameters);

                foreach (DataRow row in receiptData.Rows)
                {
                    int hour = Convert.ToInt32(row["Hour"]);
                    string foodItemTypes = row["FooditemtypeID"].ToString();

                    // Split the food item types by your delimiter (e.g., comma)
                    var foodItems = foodItemTypes.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                    foreach (var foodItem in foodItems)
                    {
                        string trimmedFoodItem = foodItem.Trim();

                        if (!foodItemData.ContainsKey(trimmedFoodItem))
                        {
                            foodItemData[trimmedFoodItem] = new Dictionary<int, int>();
                        }

                        if (!foodItemData[trimmedFoodItem].ContainsKey(hour))
                        {
                            foodItemData[trimmedFoodItem][hour] = 0;
                        }

                        foodItemData[trimmedFoodItem][hour]++;
                    }
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

            var uniqueHours = foodItemData.Values.SelectMany(x => x.Keys).Distinct().ToList();

            if (uniqueHours.Count == 0)
            {
                return;
            }
            else if (uniqueHours.Count == 1)
            {
                int singleHour = uniqueHours.First();

                foreach (var fooditem in foodItemData)
                {
                    if (int.TryParse(fooditem.Key, out int foodItemTypeId))
                    {
                        string foodItemName = foodItemNames.ContainsKey(foodItemTypeId) ? foodItemNames[foodItemTypeId] : fooditem.Key;

                        Series lineSeries = chart2.Series.FindByName(fooditem.Key) ?? new Series
                        {
                            Name = foodItemName,
                            Color = GenerateRandomColor(),
                            ChartType = SeriesChartType.Column,
                        };

                        lineSeries.Points.AddXY($"{singleHour}{LanguageManager.Instance.GetString("Hour")}", fooditem.Value[singleHour]);
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

                int minHour = uniqueHours.Min();
                int maxHour = uniqueHours.Max();

                foreach (var fooditemType in foodItemData)
                {
                    if (int.TryParse(fooditemType.Key, out int foodItemTypeId))
                    {
                        string foodItemName = foodItemNames.ContainsKey(foodItemTypeId) ? foodItemNames[foodItemTypeId] : fooditemType.Key;

                        Series columnSeries = new Series
                        {
                            BorderWidth = 2,
                            Name = foodItemName,
                            Color = GenerateRandomColor(),
                            ChartType = SeriesChartType.Column,
                        };

                        for (int hour = minHour; hour <= maxHour; hour++)
                        {
                            int receiptCount = fooditemType.Value.ContainsKey(hour) ? fooditemType.Value[hour] : 0;
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
                    else
                    {
                        Console.WriteLine($"Failed to parse Fooditemtype.Key: {fooditemType.Key}");
                    }
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

        private Color GenerateRandomColor()
        {
            int r = random.Next(0, 256);
            int g = random.Next(0, 256);
            int b = random.Next(0, 256);
            return Color.FromArgb(255, (byte)r, (byte)g, (byte)b);
        }


        public void LocalizeControls()
        {
            var title1 = chart1.Titles[0];
            title1.Text = LanguageManager.Instance.GetString("Dcharts-disht1");

            var title2 = chart2.Titles[0];
            title2.Text = LanguageManager.Instance.GetString("Dcharts-disht2");

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