using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Data;
using PadTai.Classes;
using System.Drawing;
using System.Data.SQLite;
using System.Windows.Forms;
using System.Data.SqlClient;
using PadTai.Classes.Others;
using System.Collections.Generic;
using PadTai.Classes.Databaselink;


namespace PadTai.Fastcheckfiles
{
    public partial class DishGroupControl : UserControl
    {
        Fastcheck FCH;
        public int _pageNumber = 0; 
        private ControlResizer resizer;
        public int? _currentGroupId;
        public int? _currentSubgroupId;
        private int _totalItemCount = 0;
        private FontResizer fontResizer;
        private CrudDatabase crudDatabase;
        public int? _currentSubsubgroupId;
        private const int MaxStackSize = 5;
        private const int ItemsPerPage = 19; 
        private const int GroupItemsPerPage = 20; 
        public Stack<NavigationState> _navigationStack;
        private ButtonClickHandler _buttonClickHandler;

        public DishGroupControl(Fastcheck fCH, Stack<NavigationState> navigationStack, int? groupId = null, int? subgroupId = null, int? subsubgroupId = null, int pageNumber = 0)
        {
            InitializeComponent();
            this.DoubleBuffered = true;
            crudDatabase = new CrudDatabase();
            fontResizer = new FontResizer();
            fontResizer.AdjustFont(this);

            _currentGroupId = groupId;
            _currentSubgroupId = subgroupId;
            _currentSubsubgroupId = subsubgroupId;
            _navigationStack = navigationStack ?? new Stack<NavigationState>();
            _pageNumber = pageNumber;
            LoadGroupsOrDishes();

            this.FCH = fCH;

            _buttonClickHandler = new ButtonClickHandler(this);

            LocalizeControls();
            ApplyTheme();
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
           
            if (resizer != null)
            {
                resizer.ResizeControls(this);
            }
        }

        public void LoadGroupsOrDishes()
        {
            this.Controls.Clear();
            resizer = new ControlResizer(this.Size);

            // Create a TableLayoutPanel for better button structure
            TableLayoutPanel tableLayoutPanel1 = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                AutoScroll = false,
                AutoSize = false,
                ColumnCount = 3,
                RowCount = 7 
            };

            float[] rowPercentages = new float[7] { 14F, 14F, 14F, 14F, 14F, 14F, 14F };

            foreach (float percentage in rowPercentages)
            {
                tableLayoutPanel1.RowStyles.Add(new ColumnStyle(SizeType.Percent, percentage));
            }

            float[] columnPercentages = new float[3] { 33.3F, 33.3F, 33.3F };

            foreach (float percentage in columnPercentages)
            {
                tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, percentage));
            }
            resizer.RegisterControl(tableLayoutPanel1);
            this.Controls.Add(tableLayoutPanel1);

            // Add Back button
            RJButton backButton = new RJButton
            {
                Text = "◀",
                Width = 145,
                Height = 75,
                BorderRadius = 2,
                Dock = DockStyle.Fill,
                Margin = new Padding(2),
                BackColor = Color.DodgerBlue,
                Visible = _navigationStack.Count > 0,
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };
            resizer.RegisterControl(backButton);
            backButton.Click += BackButton_Click;
            tableLayoutPanel1.Controls.Add(backButton, 0, 0); 
            tableLayoutPanel1.SetColumnSpan(backButton, 1);


            int currentRow = 0;
            int maxRowCount = 20;
            int itemsPerPage = _currentGroupId.HasValue && !_currentSubgroupId.HasValue && !_currentSubsubgroupId.HasValue ? GroupItemsPerPage : ItemsPerPage;


            // Fetch subgroups or dishes based on the current group and subgroup IDs
            if (_currentSubsubgroupId.HasValue)
            {
                // Load dishes if we are at the subsubgroup level
                List<Dish> dishes = GetDishesBySubsubgroupId(_currentSubsubgroupId);
                _totalItemCount = dishes.Count;
                dishes = dishes.Skip(_pageNumber * itemsPerPage).Take(itemsPerPage).ToList();
                foreach (var dish in dishes)
                {
                    bool isChecked = GetIsCheckedStatus(dish.DishID);
                    Image foodImage = GetFoodPicture(dish.DishID);

                    if (currentRow >= maxRowCount) break;
                    RJButton dishButton = new RJButton
                    {
                        Text = dish.DishName,
                        Tag = dish.DishID,
                        Width = 145,
                        Height = 75,
                        BorderRadius = 2,
                        Dock = DockStyle.Fill,
                        Margin = new Padding(2),
                        BackColor = Color.DodgerBlue,
                        TextAlign = ContentAlignment.MiddleCenter,
                        Font = new Font("Segoe UI", 10, FontStyle.Regular),
                        Enabled = !isChecked // Enable or disable based on IsChecked status

                    };

                    if (foodImage != null && Properties.Settings.Default.dishpicEnabled)
                    {
                        Image resizedImage = ImageResizer.ResizeImage(foodImage, dishButton.Width, dishButton.Height);
                        dishButton.Image = resizedImage;
                        //dishButton.ForeColor = Color.Black;
                        dishButton.ImageAlign = ContentAlignment.MiddleCenter;
                        dishButton.TextImageRelation = TextImageRelation.ImageAboveText;
                    }

                    resizer.RegisterControl(dishButton);
                    dishButton.Click += DishButton_Click;
                    tableLayoutPanel1.Controls.Add(dishButton);
                    tableLayoutPanel1.RowCount++;
                    currentRow++;
                }
            }
            else if (_currentSubgroupId.HasValue)
            {
                // Load subsubgroups if we are at the subgroup level
                List<Subsubgroup> subsubgroups = GetSubsubgroupsBySubgroupId(_currentSubgroupId);
                _totalItemCount = subsubgroups.Count;
                subsubgroups = subsubgroups.Skip(_pageNumber * itemsPerPage).Take(itemsPerPage).ToList();
                foreach (var subsubgroup in subsubgroups)
                {
                    if (currentRow >= maxRowCount) break;
                    RJButton subsubgroupButton = new RJButton
                    {
                        Text = subsubgroup.SubsubgroupName,
                        Tag = subsubgroup.SubsubgroupID,
                        Width = 145,
                        Height = 75,
                        BorderRadius = 2,
                        Dock = DockStyle.Fill,
                        Margin = new Padding(2),
                        BackColor = Color.DodgerBlue,
                        TextAlign = ContentAlignment.MiddleCenter,
                        Font = new Font("Segoe UI", 10, FontStyle.Regular)
                    };
                    resizer.RegisterControl(subsubgroupButton);
                    subsubgroupButton.Click += SubsubgroupButton_Click;
                    tableLayoutPanel1.Controls.Add(subsubgroupButton);
                    tableLayoutPanel1.RowCount++;
                    currentRow++;
                };

                // Load dishes if there are no subsubgroups
                List<Dish> dishes = GetDishesBySubgroupId(_currentSubgroupId);
                _totalItemCount += dishes.Count;
                dishes = dishes.Skip(_pageNumber * itemsPerPage).Take(itemsPerPage).ToList();
                foreach (var dish in dishes)
                {
                    bool isChecked = GetIsCheckedStatus(dish.DishID);
                    Image foodImage = GetFoodPicture(dish.DishID); 


                    if (currentRow >= maxRowCount) break;
                    RJButton dishButton = new RJButton
                    {
                        Text = dish.DishName,
                        Tag = dish.DishID,
                        Width = 145,
                        Height = 75,
                        BorderRadius = 2,
                        Dock = DockStyle.Fill,
                        Margin = new Padding(2),
                        BackColor = Color.DodgerBlue,
                        TextAlign = ContentAlignment.MiddleCenter,
                        Font = new Font("Segoe UI", 10, FontStyle.Regular),
                        Enabled = !isChecked // Enable or disable based on IsChecked status
                    };

                    if (foodImage != null && Properties.Settings.Default.dishpicEnabled)
                    {
                        Image resizedImage = ImageResizer.ResizeImage(foodImage, dishButton.Width, dishButton.Height);
                        dishButton.Image = resizedImage;
                        // dishButton.Image = foodImage;
                        //dishButton.ForeColor = Color.Black;
                        dishButton.ImageAlign = ContentAlignment.MiddleCenter;
                        dishButton.TextImageRelation = TextImageRelation.ImageAboveText;
                    }

                    resizer.RegisterControl(dishButton);
                    dishButton.Click += DishButton_Click;
                    tableLayoutPanel1.Controls.Add(dishButton);
                    tableLayoutPanel1.RowCount++;
                    currentRow++;
                }
            }
            else if (_currentGroupId.HasValue)
            {
                // Load subgroups if we are at the group level
                List<Subgroup> subgroups = GetSubgroupsByGroupId(_currentGroupId);
                _totalItemCount = subgroups.Count;
                subgroups = subgroups.Skip(_pageNumber * itemsPerPage).Take(itemsPerPage).ToList();

                foreach (var subgroup in subgroups)
                {
                    if (currentRow >= maxRowCount) break;
                    RJButton subgroupButton = new RJButton
                    {
                        Text = subgroup.SubgroupName,
                        Tag = subgroup.SubgroupID,
                        Width = 145,
                        Height = 75,
                        BorderRadius = 2,
                        Dock = DockStyle.Fill,
                        Margin = new Padding(2),
                        BackColor = Color.DodgerBlue,
                        TextAlign = ContentAlignment.MiddleCenter,
                        Font = new Font("Segoe UI", 10, FontStyle.Regular)
                    };
                    resizer.RegisterControl(subgroupButton);
                    subgroupButton.Click += SubgroupButton_Click;
                    tableLayoutPanel1.Controls.Add(subgroupButton);
                    tableLayoutPanel1.RowCount++;
                    currentRow++;
                }


                // Load dishes if there are no subgroups
                List<Dish> dishes = GetDishesByGroupId(_currentGroupId);
                _totalItemCount += dishes.Count;
                dishes = dishes.Skip(_pageNumber * itemsPerPage).Take(itemsPerPage).ToList();

                foreach (var dish in dishes)
                {
                    bool isChecked = GetIsCheckedStatus(dish.DishID);
                    Image foodImage = GetFoodPicture(dish.DishID);

                    if (currentRow >= maxRowCount) break;
                    RJButton dishButton = new RJButton
                    {
                        Text = dish.DishName,
                        Tag = dish.DishID,
                        Width = 145,
                        Height = 75,
                        BorderRadius = 2,
                        Dock = DockStyle.Fill,
                        Margin = new Padding(2),
                        BackColor = Color.DodgerBlue,
                        TextAlign = ContentAlignment.MiddleCenter,
                        Font = new Font("Segoe UI", 10, FontStyle.Regular),
                        Enabled = !isChecked // Enable or disable based on IsChecked status
                    };

                    if (foodImage != null && Properties.Settings.Default.dishpicEnabled)
                    {
                        Image resizedImage = ImageResizer.ResizeImage(foodImage, dishButton.Width, dishButton.Height);
                        dishButton.Image = resizedImage;
                        // dishButton.Image = foodImage;
                        //dishButton.ForeColor = Color.Black;
                        dishButton.ImageAlign = ContentAlignment.MiddleCenter;
                        dishButton.TextImageRelation = TextImageRelation.ImageAboveText;
                    }

                    resizer.RegisterControl(dishButton);
                    dishButton.Click += DishButton_Click;
                    tableLayoutPanel1.Controls.Add(dishButton);
                    tableLayoutPanel1.RowCount++;
                    currentRow++;
                }

            }
            else
            {
                // Load groups if no current group is set
                List<Group> groups = GetAllGroups();
                _totalItemCount = groups.Count;
                groups = groups.Skip(_pageNumber * GroupItemsPerPage).Take(GroupItemsPerPage).ToList();
                foreach (var group in groups)
                {
                    if (currentRow >= maxRowCount) break;
                    RJButton groupButton = new RJButton
                    {
                        Text = group.GroupName,
                        Tag = group.GroupID,
                        Width = 145,
                        Height = 75,
                        BorderRadius = 2,
                        Dock = DockStyle.Fill,
                        Margin = new Padding(2),
                        BackColor = Color.DodgerBlue,
                        TextAlign = ContentAlignment.MiddleCenter,
                        Font = new Font("Segoe UI", 10, FontStyle.Regular)
                    };
                    resizer.RegisterControl(groupButton);
                    groupButton.Click += GroupButton_Click;
                    tableLayoutPanel1.Controls.Add(groupButton);
                    tableLayoutPanel1.RowCount++;
                    currentRow++;
                }
            }


            if (_totalItemCount > (_pageNumber + 1) * itemsPerPage || (_currentGroupId.HasValue && !_currentSubgroupId.HasValue && !_currentSubsubgroupId.HasValue && _totalItemCount > (_pageNumber + 1) * GroupItemsPerPage))
            {
                // Add the forward button
                RJButton forwardButton = new RJButton
                {
                    Text = "▶",
                    Width = 145,
                    Height = 75,
                    BorderRadius = 2,
                    Dock = DockStyle.Fill,
                    Margin = new Padding(2),
                    BackColor = Color.DodgerBlue,
                    TextAlign = ContentAlignment.MiddleCenter,
                    Font = new Font("Segoe UI", 10, FontStyle.Bold)
                };
                resizer.RegisterControl(forwardButton);
                forwardButton.Click += ForwardButton_Click;
                tableLayoutPanel1.Controls.Add(forwardButton, 2, 6); 
                tableLayoutPanel1.SetColumnSpan(forwardButton, 1);
                //  Visible = _totalItemCount.PageNumber > ItemsPerPage,
            }
        }


        private void ForwardButton_Click(object sender, EventArgs e)
        {
            LoadDishGroupControl(_currentGroupId, _currentSubgroupId, _currentSubsubgroupId, _pageNumber + 1);
           // ShowDebugInfo();
        }

        private void BackButton_Click(object sender, EventArgs e)
        {
            if (_navigationStack.Count > 0)
            {
                NavigationState previousState = _navigationStack.Pop();
                if (/*previousState.GroupId == _currentGroupId && previousState.SubgroupId == _currentSubgroupId && previousState.SubsubgroupId == _currentSubsubgroupId &&*/ _pageNumber >= 1)
                {
                    LoadDishGroupControl(_currentGroupId, _currentSubgroupId, _currentSubsubgroupId, _pageNumber - 1);
                    // LoadDishGroupControl(previousState.GroupId, previousState.SubgroupId, previousState.SubsubgroupId, previousState.PageNumber);
                    //ShowDebugInfo();
                }
                else
                {
                    //_navigationStack.Push(previousState);
                    //NavigationState previousHierachy = _navigationStack.Pop();
                    //LoadDishGroupControl(previousHierachy.GroupId, previousHierachy.SubgroupId, previousHierachy.SubsubgroupId, 0);
                   
                    LoadDishGroupControl(previousState.GroupId, previousState.SubgroupId, previousState.SubsubgroupId, previousState.PageNumber);
                  //  ShowDebugInfo();
                }

            }
            else
            {
                MessageBox.Show("No previous group to go back to.");
            }
        }

        private void GroupButton_Click(object sender, EventArgs e)
        {
            RJButton button = sender as RJButton;
            int groupId = (int)button.Tag;

            // Push the current state onto the stack before navigating
            if (_navigationStack.Count >= MaxStackSize)
            {
                _navigationStack.Pop();
            }

            _navigationStack.Push(new NavigationState { GroupId = _currentGroupId, SubgroupId = _currentSubgroupId, SubsubgroupId = _currentSubsubgroupId, PageNumber = _pageNumber });

            LoadDishGroupControl(groupId);
          //  ShowDebugInfo();
        }

        private void SubgroupButton_Click(object sender, EventArgs e)
        {
            RJButton button = sender as RJButton;
            int subgroupId = (int)button.Tag;

            // Push the current state onto the stack before navigating
            if (_navigationStack.Count >= MaxStackSize)
            {
                _navigationStack.Pop();
            }
            _navigationStack.Push(new NavigationState { GroupId = _currentGroupId, SubgroupId = _currentSubgroupId, SubsubgroupId = _currentSubsubgroupId, PageNumber = _pageNumber });

            LoadDishGroupControl(_currentGroupId, subgroupId);
          //  ShowDebugInfo();
        }


        private void SubsubgroupButton_Click(object sender, EventArgs e)
        {
            RJButton button = sender as RJButton;
            int subsubgroupId = (int)button.Tag;

            // Push the current state onto the stack before navigating
            if (_navigationStack.Count >= MaxStackSize)
            {
                _navigationStack.Pop();
            }

            _navigationStack.Push(new NavigationState { GroupId = _currentGroupId, SubgroupId = _currentSubgroupId, SubsubgroupId = _currentSubsubgroupId, PageNumber = _pageNumber });

            LoadDishGroupControl(_currentGroupId, _currentSubgroupId, subsubgroupId);
           // ShowDebugInfo();
        }

        private void DishButton_Click(object sender, EventArgs e)
        {
            RJButton button = sender as RJButton;
            int dishId = (int)button.Tag;
            _buttonClickHandler.HandleButtonClick(sender);
        }

        public List<Group> GetAllGroups()
        {
            string query = "SELECT GroupID, GroupName FROM Groups";

            return crudDatabase.FetchDataToList(query, reader => new Group
            {
                GroupID = reader.GetInt32(0),
                GroupName = reader.GetString(1)
            });
        }

        private List<Subgroup> GetSubgroupsByGroupId(int? groupId)
        {
            string query = $"SELECT SubgroupID, SubgroupName FROM Subgroups WHERE GroupID = {groupId}";

            return crudDatabase.FetchDataToList(query, reader => new Subgroup
            {
                SubgroupID = reader.GetInt32(0),
                SubgroupName = reader.GetString(1)
            });
        }

        private List<Subsubgroup> GetSubsubgroupsBySubgroupId(int? subgroupId)
        {
            string query = $"SELECT SubsubgroupID, SubsubgroupName FROM Subsubgroups WHERE SubgroupID = {subgroupId}";

            return crudDatabase.FetchDataToList(query, reader => new Subsubgroup
            {
                SubsubgroupID = reader.GetInt32(0),
                SubsubgroupName = reader.GetString(1)
            });
        }

        private List<Dish> GetDishesBySubgroupId(int? subgroupId)
        {
            string query = $"SELECT FoodID, FoodName, GroupID, SubgroupID, SubsubgroupID FROM FoodItems WHERE SubgroupID = {subgroupId}";

            return crudDatabase.FetchDataToList(query, reader => new Dish
            {
                DishID = reader.GetInt32(0),
                DishName = reader.GetString(1),
                GroupID = reader.IsDBNull(2) ? (int?)null : reader.GetInt32(2),
                SubgroupID = reader.IsDBNull(3) ? (int?)null : reader.GetInt32(3),
                SubsubgroupID = reader.IsDBNull(4) ? (int?)null : reader.GetInt32(4)
            });
        }

        private List<Dish> GetDishesBySubsubgroupId(int? subsubgroupId)
        {
            string query = $"SELECT FoodID, FoodName, GroupID, SubgroupID, SubsubgroupID FROM FoodItems WHERE SubsubgroupID = {subsubgroupId}";

            return crudDatabase.FetchDataToList(query, reader => new Dish
            {
                DishID = reader.GetInt32(0),
                DishName = reader.GetString(1),
                GroupID = reader.IsDBNull(2) ? (int?)null : reader.GetInt32(2),
                SubgroupID = reader.IsDBNull(3) ? (int?)null : reader.GetInt32(3),
                SubsubgroupID = reader.IsDBNull(4) ? (int?)null : reader.GetInt32(4)
            });
        }


        private List<Dish> GetDishesByGroupId(int? groupId)
        {
            string query = $"SELECT FoodID, FoodName, GroupID, SubgroupID, SubsubgroupID FROM FoodItems WHERE GroupID = {groupId}";

            return crudDatabase.FetchDataToList(query, reader => new Dish
            {
                DishID = reader.GetInt32(0),
                DishName = reader.GetString(1),
                GroupID = reader.IsDBNull(2) ? (int?)null : reader.GetInt32(2),
                SubgroupID = reader.IsDBNull(3) ? (int?)null : reader.GetInt32(3),
                SubsubgroupID = reader.IsDBNull(4) ? (int?)null : reader.GetInt32(4)
            });
        }

        public void LoadDishGroupControl(int? groupId, int? subgroupId = null, int? subsubgroupId = null, int pageNumber = 0)
        {
            DishGroupControl dishGroupControl = new DishGroupControl(FCH, _navigationStack, groupId, subgroupId, subsubgroupId, pageNumber);
            FCH.Panelreceptacle.Controls.Clear();
            dishGroupControl.Dock = DockStyle.Fill;
            FCH.Panelreceptacle.Controls.Add(dishGroupControl);
           
            _currentGroupId = groupId;
            _currentSubgroupId = subgroupId;
            _currentSubsubgroupId = subsubgroupId;
            _pageNumber = pageNumber;


            //FCH.Panelreceptacle.Controls.Clear();
            //// We could create a new instance here if needed, but let's update the existing one!
            //this._currentGroupId = groupId;
            //this._currentSubgroupId = subgroupId;
            //this._currentSubsubgroupId = subsubgroupId;
            //this._pageNumber = pageNumber;
            //LoadGroupsOrDishes();

            ////This was not in the original code sample, and should be here to ensure it is added to the form
            //this.Dock = DockStyle.Fill;
            //FCH.Panelreceptacle.Controls.Add(this);
        }

        private bool GetIsCheckedStatus(int foodID)
        {
            string query = $"SELECT IsChecked FROM FoodItems WHERE FoodID = {foodID}";

            // Fetch data using the existing method
            DataTable resultTable = crudDatabase.FetchDataFromDatabase(query);

            // Check if any rows were returned and return the IsChecked status
            if (resultTable != null && resultTable.Rows.Count > 0)
            {
                return Convert.ToBoolean(resultTable.Rows[0]["IsChecked"]);
            }

            return false; // Default return value if no data is found
        }

        private Image GetFoodPicture(int foodID)
        {
            string query = $"SELECT FoodPicture FROM FoodItems WHERE FoodID = {foodID}";

            // Fetch data using the existing method
            DataTable resultTable = crudDatabase.FetchDataFromDatabase(query);

            // Check if any rows were returned and return the FoodPicture
            if (resultTable != null && resultTable.Rows.Count > 0)
            {
                object result = resultTable.Rows[0]["FoodPicture"];
                if (result != null && result != DBNull.Value)
                {
                    byte[] imageData = (byte[])result;
                    using (MemoryStream ms = new MemoryStream(imageData))
                    {
                        return Image.FromStream(ms);
                    }
                }
            }

            return null; // Return null if no image is found
        }

        public class Group
        {
            public int GroupID { get; set; }
            public string GroupName { get; set; }
        }

        public class Subgroup
        {
            public int SubgroupID { get; set; }
            public string SubgroupName { get; set; }
        }

        public class Subsubgroup
        {
            public int SubsubgroupID { get; set; }
            public string SubsubgroupName { get; set; }
        }

        public class Dish
        {
            public int DishID { get; set; }
            public string DishName { get; set; }
            public int? GroupID { get; set; }
            public int? SubgroupID { get; set; }
            public int? SubsubgroupID { get; set; }
        }

        // Class to represent a navigation state
        public class NavigationState
        {
            public int? GroupId { get; set; }
            public int? SubgroupId { get; set; }
            public int? SubsubgroupId { get; set; }
            public int PageNumber { get; set; }
        }

        private void ShowDebugInfo()
        {
            string message = $"Current Group ID: {_currentGroupId}\n" +
                            $"Current Subgroup ID: {_currentSubgroupId}\n" +
                            $"Current Subsubgroup ID: {_currentSubsubgroupId}\n" +
                           $"Page Number: {_pageNumber}\n" +
                            $"Navigation Stack Count: {_navigationStack.Count}\n" +
                           $"Navigation Stack Contents:";
            foreach (var item in _navigationStack)
            {
                message += $"\nGroupId: {item.GroupId}, SubgroupId: {item.SubgroupId}, SubsubgroupId: {item.SubsubgroupId}, PageNumber: {item.PageNumber}";
            }

            MessageBox.Show(message, "Debug Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        public void LocalizeControls()
        {
            //button1.Text = LanguageManager.Instance.GetString("MF-btn1");
            //button2.Text = LanguageManager.Instance.GetString("MF-btn2");
            //button3.Text = LanguageManager.Instance.GetString("MF-btn3");
        }

        public void ApplyTheme()
        {
            ColorPalette.ColorTrio colors = ColorPalette.GetColorTrio();

            this.BackColor = colors.Color1;
            this.ForeColor = colors.Color2;
        }
    }
}