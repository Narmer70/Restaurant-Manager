using System;
using System.IO;
using System.Text;
using PadTai.Classes;
using System.Drawing;
using System.Data.SQLite;
using System.Windows.Forms;
using PadTai.Classes.Others;
using System.Drawing.Printing;
using System.Collections.Generic;
using PadTai.Classes.Databaselink;
using PadTai.Sec_daryfolders.Others;
using PadTai.Sec_daryfolders.Grossmanager;
using PadTai.Sec_daryfolders.Others_Forms;
using System.Linq;


namespace PadTai.Sec_daryfolders.Departmentdata
{
    public partial class Savereceipes : Form
    {
        private List<(int FoodID, string FoodName)> dishes = new List<(int FoodID, string FoodName)>();
        private string sqliteConnectionString = DatabaseConnection.GetSQLiteConnectionString();
        ColorPalette.ColorTrio colors = ColorPalette.GetColorTrio();
        private DataGridViewScroller scroller;
        private Clavieroverlay clavieroverlay;
        private CrudDatabase crudDatabase;
        private FontResizer fontResizer;
        private ControlResizer resizer;
        private Receipt currentReceipt;

        public Savereceipes()
        {
            InitializeComponent();
            InitializeResizing();

            LoadData(); 
            ApplyTheme();
            CenterLabel();
            LocalizeControls();
            label1.Visible = false;
            roundedPanel1.Visible = false;
            dataGridView1.GridColor = colors.Color3;
            dataGridView2.GridColor = colors.Color3;
        }

        private void InitializeResizing()
        {
            dataGridView1.ReadOnly = true;
            rjButton6.Visible = false;

            crudDatabase = new CrudDatabase();  

            fontResizer = new FontResizer();
            fontResizer.AdjustFont(this);            
            
            resizer = new ControlResizer(this.Size);

            resizer.RegisterControl(panel1);
            resizer.RegisterControl(label1);
            resizer.RegisterControl(label2);
            resizer.RegisterControl(label3);
            resizer.RegisterControl(label4);
            resizer.RegisterControl(rjButton1);
            resizer.RegisterControl(rjButton2);
            resizer.RegisterControl(rjButton6);
            resizer.RegisterControl(rjButton7);
            resizer.RegisterControl(dataGridView1);
            resizer.RegisterControl(dataGridView2);
            resizer.RegisterControl(roundedPanel1);
            resizer.RegisterControl(roundedTextbox1);

            scroller = new DataGridViewScroller(this, null, dataGridView1, dataGridView2);

            if (!Properties.Settings.Default.isReadOnlyIngredients)
            {
                dataGridView1.ReadOnly = false;
                rjButton6.Visible = true;
            }
        }

        private void CenterLabel()
        {
            roundedTextbox1.Left = (this.ClientSize.Width - roundedTextbox1.Width) / 2;
        }

        private void LoadData()
        {
            using (var connection = new SQLiteConnection(sqliteConnectionString))
            {
                connection.Open();

                // Load Dishes
                var dishesCommand = new SQLiteCommand("SELECT FoodID, FoodName FROM FoodItems", connection);
                var dishesReader = dishesCommand.ExecuteReader();
                dishes.Clear(); // Clear previous data
                var dishesList = new List<(int FoodID, string FoodName)>();

                while (dishesReader.Read())
                {
                    var dish = (Convert.ToInt32(dishesReader["FoodID"]), dishesReader["FoodName"].ToString());
                    dishesList.Add(dish);
                    dishes.Add(dish); // Store the dish in the class-level variable
                }

                // Load Gross Products
                var productsCommand = new SQLiteCommand("SELECT ProductID, ProductName FROM GrossProducts", connection);
                var productsReader = productsCommand.ExecuteReader();
                var productsList = new List<(int ProductID, string ProductName)>();

                while (productsReader.Read())
                {
                    productsList.Add((Convert.ToInt32(productsReader["ProductID"]), productsReader["ProductName"].ToString()));
                }

                // Clear existing columns and add new ones
                dataGridView1.Columns.Clear();
                foreach (var product in productsList)
                {
                    dataGridView1.Columns.Add(product.ProductName, product.ProductName);
                }

                // Prepare a single query to fetch all amounts in one go
                var dishIds = string.Join(",", dishesList.Select(d => d.FoodID));
                var productIds = string.Join(",", productsList.Select(p => p.ProductID));

                var amountQuery = $@" SELECT DishID, ProductID, AmountInGrams  FROM IngredientMap 
                                             WHERE DishID IN ({dishIds}) AND ProductID IN ({productIds})";

                var amountCommand = new SQLiteCommand(amountQuery, connection);
                var amountReader = amountCommand.ExecuteReader();

                // Create a dictionary to hold amounts for quick access
                var amounts = new Dictionary<(int DishID, int ProductID), double>();

                while (amountReader.Read())
                {
                    var dishId = Convert.ToInt32(amountReader["DishID"]);
                    var productId = Convert.ToInt32(amountReader["ProductID"]);
                    var amount = Convert.ToDouble(amountReader["AmountInGrams"]);
                    amounts[(dishId, productId)] = amount;
                }

                // Populate the DataGridView
                foreach (var dish in dishesList)
                {
                    int rowIndex = dataGridView1.Rows.Add();
                    dataGridView1.Rows[rowIndex].HeaderCell.Value = dish.FoodName;

                    foreach (var product in productsList)
                    {
                        if (amounts.TryGetValue((dish.FoodID, product.ProductID), out double amountValue))
                        {
                            dataGridView1.Rows[rowIndex].Cells[product.ProductName].Value = amountValue;
                        }
                    }
                }
                dgvColumnWidth();
            }
        }

        private void dgvColumnWidth()
        {
            for (int i = 0; i < dataGridView1.Columns.Count; i++)
            {
                dataGridView1.Columns[i].Width = 38;
            }
            for (int i = 0; i < dataGridView1.Columns.Count; i++)
            {
                dataGridView1.Columns[i].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dataGridView1.Columns[i].HeaderCell.Style.Font = new Font("Century Gothic", 10, FontStyle.Regular);
            }
            dataGridView1.ColumnHeadersHeight = 200;
            dataGridView1.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
            dataGridView1.ColumnHeadersDefaultCellStyle.BackColor = dataGridView1.BackgroundColor;
            dataGridView1.ColumnHeadersDefaultCellStyle.SelectionForeColor = dataGridView1.ForeColor;
            dataGridView1.ColumnHeadersDefaultCellStyle.SelectionBackColor = dataGridView1.BackColor;
            dataGridView1.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.EnableResizing;

            for (int i = 0; i < dataGridView1.Rows.Count; i++)
            {
                dataGridView1.Rows[i].Height = 30;
                dataGridView1.Rows[i].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }
            dataGridView1.RowHeadersWidth = 250;
            dataGridView1.DefaultCellStyle.ForeColor = dataGridView1.ForeColor;
            dataGridView1.RowHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
            dataGridView1.RowHeadersDefaultCellStyle.ForeColor = dataGridView1.ForeColor;
            dataGridView1.RowHeadersDefaultCellStyle.BackColor = dataGridView1.BackgroundColor;
            dataGridView1.RowHeadersDefaultCellStyle.SelectionForeColor = dataGridView1.ForeColor;
            dataGridView1.RowHeadersDefaultCellStyle.SelectionBackColor = dataGridView1.BackColor;
            dataGridView1.RowHeadersDefaultCellStyle.Font = new Font("Microsoft PhagsPa", 10, FontStyle.Regular); 
        }

        private void dataGridView1_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.RowIndex == -1 && (e.ColumnIndex >= 0))
            {
                e.PaintBackground(e.CellBounds, true);
                e.Graphics.TranslateTransform(e.CellBounds.Left, e.CellBounds.Bottom);
                e.Graphics.RotateTransform(270);
                
                using (Brush brush = new SolidBrush(this.ForeColor))
                {
                    e.Graphics.DrawString(e.FormattedValue.ToString(), e.CellStyle.Font, brush, 5, 5);
                }
                e.Graphics.ResetTransform();
                e.Handled = true;
            }
        }

        private void saveTableStruct() 
        {
            try
            {
                using (var connection = new SQLiteConnection(sqliteConnectionString))
                {
                    connection.Open();

                    foreach (DataGridViewRow row in dataGridView1.Rows)
                    {
                        if (row.IsNewRow) continue;

                        // Get the dish ID from the row header
                        var dishName = row.HeaderCell.Value.ToString();
                        var dishIDCommand = new SQLiteCommand("SELECT FoodID FROM FoodItems WHERE FoodName = @FoodName", connection);
                        dishIDCommand.Parameters.AddWithValue("@FoodName", dishName);
                        var dishID = dishIDCommand.ExecuteScalar();

                        foreach (DataGridViewCell cell in row.Cells)
                        {
                            // Get the product ID from the column header
                            var productName = cell.OwningColumn.HeaderText;
                            var productIDCommand = new SQLiteCommand("SELECT ProductID FROM GrossProducts WHERE ProductName = @ProductName", connection);
                            productIDCommand.Parameters.AddWithValue("@ProductName", productName);
                            var productID = productIDCommand.ExecuteScalar();

                            if (cell.Value != null)
                            {
                                double amount;
                                try
                                {
                                    amount = Convert.ToDouble(cell.Value);
                                }
                                catch (FormatException)
                                {
                                    label1.Text = string.Empty;
                                    label1.Text = ($"The value '{cell.Value}' in the cell is not a valid integer.");
                                    label1.ForeColor = Color.Tomato;
                                    continue;
                                }
                                catch (InvalidCastException)
                                {
                                    label1.Text = string.Empty;
                                    label1.Text = ($"The value '{cell.Value}' in the cell cannot be converted to an integer.");
                                    label1.ForeColor = Color.Tomato;
                                    continue;
                                }
                                var upsertCommand = new SQLiteCommand(@" INSERT INTO IngredientMap (DishID, ProductID, AmountInGrams) 
                                                                     VALUES (@DishID, @ProductID, @AmountInGrams) ON CONFLICT(DishID, ProductID) 
                                                                     DO UPDATE SET AmountInGrams = @AmountInGrams", connection);

                                upsertCommand.Parameters.AddWithValue("@DishID", dishID);
                                upsertCommand.Parameters.AddWithValue("@ProductID", productID);
                                upsertCommand.Parameters.AddWithValue("@AmountInGrams", amount);
                                upsertCommand.ExecuteNonQuery();
                            }
                            else
                            {
                                var deleteCommand = new SQLiteCommand(@" DELETE FROM IngredientMap WHERE DishID = @DishID AND ProductID = @ProductID", connection);
                                deleteCommand.Parameters.AddWithValue("@DishID", dishID);
                                deleteCommand.Parameters.AddWithValue("@ProductID", productID);
                                deleteCommand.ExecuteNonQuery();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                label1.Text = string.Empty;
                label1.Text = ($"An unexpected error occurred: {ex.Message}");
                label1.ForeColor = Color.Tomato;
            }
        }


        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex == -1 && e.ColumnIndex >= 0)
            {
                roundedPanel1.Visible = true;
                label1.Text = string.Empty;

                string columnHeader = dataGridView1.Columns[e.ColumnIndex].HeaderText;
                label2.Text = columnHeader;

                currentReceipt = new Receipt { Header = columnHeader };

                // Retrieve all dishes associated with the selected product
                using (var connection = new SQLiteConnection(sqliteConnectionString))
                {
                    connection.Open();
                    var productIdCommand = new SQLiteCommand("SELECT ProductID FROM GrossProducts WHERE ProductName = @ProductName", connection);
                    productIdCommand.Parameters.AddWithValue("@ProductName", columnHeader);
                    int productId = Convert.ToInt32(productIdCommand.ExecuteScalar());

                    var dishesCommand = new SQLiteCommand("SELECT FoodID, FoodName, AmountInGrams FROM IngredientMap INNER JOIN FoodItems ON IngredientMap.DishID = FoodItems.FoodID WHERE ProductID = @ProductID", connection);
                    dishesCommand.Parameters.AddWithValue("@ProductID", productId);
                    var dishesReader = dishesCommand.ExecuteReader();

                    // Clear previous data in dataGridView2
                    dataGridView2.Rows.Clear();
                    dataGridView2.Columns.Clear();
                    dataGridView2.Columns.Add("DishName", "     " + LanguageManager.Instance.GetString("SREC-Indishes"));
                    dataGridView2.Columns.Add("AmountInGrams", LanguageManager.Instance.GetString("SREC-Amounting"));
                    dgv2ColumnsWidth();

                    double totalAmountInGrams = 0; // Initialize total amount
                    while (dishesReader.Read())
                    {
                        string dishName = dishesReader["FoodName"].ToString();
                        double amountInGrams = Convert.ToDouble(dishesReader["AmountInGrams"]);
                        dataGridView2.Rows.Add(dishName, amountInGrams + " " + LanguageManager.Instance.GetString("Gramm"));
                        totalAmountInGrams += amountInGrams;

                        currentReceipt.Items.Add(new ReceiptItem { Name = dishName, AmountInGrams = amountInGrams });
                    }
                    currentReceipt.TotalAmount = totalAmountInGrams;
                    label3.Text = totalAmountInGrams + " " + LanguageManager.Instance.GetString("Gramm");
                }
            }
            else if (e.RowIndex >= 0 && e.ColumnIndex == -1) // Row header clicked
            {
                roundedPanel1.Visible = true;
                label1.Text = string.Empty;

                string rowHeader = dataGridView1.Rows[e.RowIndex].HeaderCell.Value?.ToString() ?? LanguageManager.Instance.GetString("Novalue");
                label2.Text = rowHeader;

                currentReceipt = new Receipt { Header = rowHeader };

                // Retrieve all products associated with the selected dish
                using (var connection = new SQLiteConnection(sqliteConnectionString))
                {
                    connection.Open();
                    var dishIdCommand = new SQLiteCommand("SELECT FoodID FROM FoodItems WHERE FoodName = @DishName", connection);
                    dishIdCommand.Parameters.AddWithValue("@DishName", rowHeader);
                    int dishId = Convert.ToInt32(dishIdCommand.ExecuteScalar());

                    var productsCommand = new SQLiteCommand(@" SELECT GrossProducts.ProductID, GrossProducts.ProductName, IngredientMap.AmountInGrams 
                                                               FROM IngredientMap INNER JOIN GrossProducts ON IngredientMap.ProductID = GrossProducts.ProductID 
                                                               WHERE IngredientMap.DishID = @DishID", connection);
                   
                    productsCommand.Parameters.AddWithValue("@DishID", dishId);
                    var productsReader = productsCommand.ExecuteReader();

                    // Clear previous data in dataGridView2
                    dataGridView2.Rows.Clear();
                    dataGridView2.Columns.Clear();
                    dataGridView2.Columns.Add("ProductName", "     " + LanguageManager.Instance.GetString("SREC-Prodname"));
                    dataGridView2.Columns.Add("AmountInGrams", LanguageManager.Instance.GetString("SREC-Amounting"));
                    dgv2ColumnsWidth();

                    double totalAmountInGrams = 0; 
                    while (productsReader.Read())
                    {
                        string productName = productsReader["ProductName"].ToString();
                        double amountInGrams = Convert.ToDouble(productsReader["AmountInGrams"]);
                        dataGridView2.Rows.Add(productName, amountInGrams + " " + LanguageManager.Instance.GetString("Gramm"));
                        totalAmountInGrams += amountInGrams;

                        currentReceipt.Items.Add(new ReceiptItem { Name = productName, AmountInGrams = amountInGrams });

                    }
                    currentReceipt.TotalAmount = totalAmountInGrams;
                    label3.Text = totalAmountInGrams + " " + LanguageManager.Instance.GetString("Gramm");
                }
            }
            else if (e.RowIndex >= 0 && e.ColumnIndex >= 0) 
            {
                label1.Text = string.Empty;
                roundedPanel1.Visible = false;
            }
        }

        private void dgv2ColumnsWidth()
        {
            dataGridView2.Columns[0].Width = (int)(dataGridView2.Width * 0.70);
            dataGridView2.Columns[1].Width = (int)(dataGridView2.Width * 0.30);
            dataGridView2.Columns[0].DefaultCellStyle.Padding = new Padding(20, 0, 0, 0); 
            dataGridView2.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            for (int i = 0; i < dataGridView2.Columns.Count; i++)
            {
                if (i == 0) 
                {
                    dataGridView2.Columns[i].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleLeft;
                }
                else if (i == 1) 
                {
                    dataGridView2.Columns[i].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                }
                dataGridView2.Columns[i].HeaderCell.Style.Font = new Font("Century Gothic", 9, FontStyle.Bold);
            }
            dataGridView2.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
            dataGridView2.ColumnHeadersDefaultCellStyle.ForeColor = dataGridView2.ForeColor;
            dataGridView2.ColumnHeadersDefaultCellStyle.BackColor = dataGridView2.BackgroundColor;
            dataGridView2.ColumnHeadersDefaultCellStyle.SelectionForeColor = dataGridView2.ForeColor;
            dataGridView2.ColumnHeadersDefaultCellStyle.SelectionBackColor = dataGridView2.BackColor;
        }

        private void rjButton2_Click(object sender, EventArgs e)
        {
            saveTableStruct();
        }


        private Dictionary<int, Dictionary<int, double>> amountsCache;
        private void roundedTextbox1_TextChanged(object sender, EventArgs e)
        {
            if (roundedTextbox1.Text.Length >= 1)
            {
                string filterText = roundedTextbox1.Text.ToLower();

                // Clear existing rows
                dataGridView1.Rows.Clear();

                // Load products only once
                var products = LoadProducts();

                // Ensure amounts are loaded
                if (amountsCache == null)
                {
                    LoadAmounts();
                }

                foreach (var dish in dishes)
                {
                    if (dish.FoodName.ToLower().Contains(filterText))
                    {
                        int rowIndex = dataGridView1.Rows.Add();
                        dataGridView1.Rows[rowIndex].HeaderCell.Value = dish.FoodName;

                        foreach (var product in products)
                        {
                            if (amountsCache.TryGetValue(dish.FoodID, out var productAmounts) &&
                                productAmounts.TryGetValue(product.ProductID, out double amount))
                            {
                                dataGridView1.Rows[rowIndex].Cells[product.ProductName].Value = amount;
                            }
                        }
                    }
                }
            }
            else
            {
                LoadData();
            }
        }

        // Method to load products
        private List<(int ProductID, string ProductName)> LoadProducts()
        {
            var products = new List<(int ProductID, string ProductName)>();
            using (var connection = new SQLiteConnection(sqliteConnectionString))
            {
                connection.Open();
                var productsCommand = new SQLiteCommand("SELECT ProductID, ProductName FROM GrossProducts", connection);
                var productsReader = productsCommand.ExecuteReader();
                while (productsReader.Read())
                {
                    products.Add((Convert.ToInt32(productsReader["ProductID"]), productsReader["ProductName"].ToString()));
                }
            }
            return products;
        }

        // Method to load all amounts into a cache
        private void LoadAmounts()
        {
            amountsCache = new Dictionary<int, Dictionary<int, double>>();

            using (var connection = new SQLiteConnection(sqliteConnectionString))
            {
                connection.Open();
                var amountCommand = new SQLiteCommand("SELECT DishID, ProductID, AmountInGrams FROM IngredientMap", connection);
                var amountReader = amountCommand.ExecuteReader();

                while (amountReader.Read())
                {
                    int dishID = Convert.ToInt32(amountReader["DishID"]);
                    int productID = Convert.ToInt32(amountReader["ProductID"]);
                    double amount = Convert.ToDouble(amountReader["AmountInGrams"]);

                    if (!amountsCache.ContainsKey(dishID))
                    {
                        amountsCache[dishID] = new Dictionary<int, double>();
                    }

                    amountsCache[dishID][productID] = amount;
                }
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);
            //saveTableStruct();
        }

        private void PrintReceipt(Receipt receipt)
        {
            PrintDocument printDocument = new PrintDocument();
            printDocument.PrintPage += (sender, e) =>
            {
                Brush brush = Brushes.Black;
                Font font = new Font("Arial", 10);
                Font hfont = new Font("Century Gothic", 12, FontStyle.Bold);
                float centerX = e.Graphics.VisibleClipBounds.Width / 2;
                float discountValueX = e.Graphics.VisibleClipBounds.Width - 70;
                float currentYPos = 0;

                // Define margins
                float leftMargin = 20;
                float rightMargin = 20;
                float maxWidth = e.Graphics.VisibleClipBounds.Width - leftMargin - rightMargin;

                // Print header
                string receiptIdText = $"{receipt.Header}";
                string[] words = receiptIdText.Split(' '); // Split the header into words for wrapping
                StringBuilder currentLine = new StringBuilder();
                float lineHeight = hfont.GetHeight(e.Graphics);

                foreach (var word in words)
                {
                    // Check if adding the next word exceeds the max width
                    string testLine = currentLine.Length > 0 ? currentLine + " " + word : word;
                    SizeF testLineSize = e.Graphics.MeasureString(testLine, hfont);

                    if (testLineSize.Width > maxWidth)
                    {
                        // Draw the current line
                        e.Graphics.DrawString(currentLine.ToString(), hfont, brush, centerX - (testLineSize.Width / 2), currentYPos);
                        currentYPos += lineHeight; // Move to the next line
                        currentLine.Clear(); // Clear the current line
                        currentLine.Append(word); // Start a new line with the current word
                    }
                    else
                    {
                        currentLine.Append(currentLine.Length > 0 ? " " + word : word);
                    }
                }

                // Draw the last line if there's any text left
                if (currentLine.Length > 0)
                {
                    SizeF lastLineSize = e.Graphics.MeasureString(currentLine.ToString(), hfont);
                    e.Graphics.DrawString(currentLine.ToString(), hfont, brush, centerX - (lastLineSize.Width / 2), currentYPos);
                }

                float yPos = currentYPos + 20;
                e.Graphics.DrawString("---------------------------------------------------", new Font("Century Gothic", 12, FontStyle.Bold), brush, new PointF(5, yPos));

                yPos += 20;
                // Print items
                foreach (var item in receipt.Items)
                {
                    e.Graphics.DrawString(item.Name, font, brush, new PointF(10, yPos));
                    e.Graphics.DrawString($"{item.AmountInGrams} {LanguageManager.Instance.GetString("Gramm")}", font, brush, new PointF(discountValueX, yPos));
                    yPos += 20;
                }

                e.Graphics.DrawString("---------------------------------------------------", new Font("Century Gothic", 12, FontStyle.Bold), brush, new PointF(5, yPos));

                yPos += 20;
                // Print total amount
                e.Graphics.DrawString(LanguageManager.Instance.GetString("Receiptprinttotal"), new Font("Arial", 12, FontStyle.Bold), brush, new PointF(10, yPos));
                e.Graphics.DrawString($"{receipt.TotalAmount} {LanguageManager.Instance.GetString("Gramm")}", new Font("Arial", 12, FontStyle.Bold), brush, new PointF(discountValueX, yPos));
              
                yPos += 20;
                e.Graphics.DrawString("---------------------------------------------------", new Font("Century Gothic", 12, FontStyle.Bold), brush, new PointF(5, yPos));
             
                yPos += 40;
                SizeF restaurantAddressSize = e.Graphics.MeasureString(receipt.ClientAddress, font);
                e.Graphics.DrawString(receipt.ClientAddress, font, brush, centerX - (restaurantAddressSize.Width / 2), yPos);
                
                yPos += 20;
                string Pointname = receipt.RestaurantName.ToUpper() + ":" + receipt.ClientName.ToUpper();
                SizeF restaurantNameSize = e.Graphics.MeasureString(Pointname, font);
                e.Graphics.DrawString(Pointname, font, brush, centerX - (restaurantNameSize.Width / 2), yPos);

                if (Properties.Settings.Default.printBrandLogo)
                {
                    // Draw the restaurant logo if available
                    if (receipt.RestaurantLogo != null)
                    {
                        yPos += 20;

                        using (MemoryStream ms = new MemoryStream(receipt.RestaurantLogo))
                        {
                            Image logo = Image.FromStream(ms);

                            // Define the desired width and height for the logo
                            int desiredWidth = 80; // Set your desired width
                            int desiredHeight = 80; // Set your desired height

                            // Calculate the aspect ratio
                            float aspectRatio = (float)logo.Width / logo.Height;

                            // Adjust dimensions to maintain aspect ratio
                            if (logo.Width > logo.Height)
                            {
                                desiredHeight = (int)(desiredWidth / aspectRatio);
                            }
                            else
                            {
                                desiredWidth = (int)(desiredHeight * aspectRatio);
                            }

                            // Calculate the position to center the logo
                            int centerPic = (int)(e.Graphics.VisibleClipBounds.Width - desiredWidth) / 2;

                            // Draw the logo with the specified dimensions
                            e.Graphics.DrawImage(logo, new Rectangle(centerPic, (int)yPos, desiredWidth, desiredHeight));
                        }
                    }
                }
            };

            printDocument.Print();
        }

        private void Alert(string msg, Alertform.enmType type)
        {
            Alertform alertform = new Alertform();
            alertform.showAlert(msg, type);
        }

        private void label4_Click(object sender, EventArgs e)
        {
            if (currentReceipt != null)
            {
                var detailsFetcher = new RestaurantDetails();
                var (restaurantName, restaurantLogo, clientName, clientAddress) = detailsFetcher.GetDetails();

                currentReceipt.RestaurantName = restaurantName;
                currentReceipt.RestaurantLogo = restaurantLogo;
                currentReceipt.ClientName = clientName;
                currentReceipt.ClientAddress = clientAddress;

                if (currentReceipt.Items.Count > 0)
                {
                    PrintReceipt(currentReceipt);
                }
                else
                {
                    this.Alert("No ingredients to print", Alertform.enmType.Warning);
                }
            }
        }

        private void rjButton1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void roundedTextbox1_Enter(object sender, EventArgs e)
        {
            roundedPanel1.Visible = false;

            if (Properties.Settings.Default.showKeyboard)
            {
                clavieroverlay = new Clavieroverlay(roundedTextbox1);
                clavieroverlay.boardLocationBottom();
                clavieroverlay.Show();
            }
        }

        private void roundedTextbox1_Leave(object sender, EventArgs e)
        {
            if (clavieroverlay != null && clavieroverlay.Visible == true)
            { 
                clavieroverlay.Visible = false;
            }
            clavieroverlay?.Hide();
        }


        public void AdduserControl(UserControl UserControl)
        {
            UserControl.Dock = DockStyle.Fill;
            panel1.Controls.Clear();
            panel1.Controls.Add(UserControl);
            UserControl.BringToFront();
        }

        private void rjButton6_Click(object sender, EventArgs e)
        {
            rjButton2.Visible = false;
            roundedPanel1.Visible = false;
            roundedTextbox1.Visible = false;

            panel1.Controls.Clear();

            Modifyproduct addproduct = new Modifyproduct();
            AdduserControl(addproduct);
        }

        private void rjButton7_Click(object sender, EventArgs e)
        {
            panel1.Controls.Clear();
            panel1.Controls.Add(dataGridView1);
            panel1.Controls.Add(roundedPanel1);
            roundedTextbox1.Visible=true;
            rjButton2.Visible=true; 
            LoadData();

            foreach (DataGridViewColumn column in dataGridView1.Columns)
            {
                column.SortMode = DataGridViewColumnSortMode.NotSortable;
            }
        }

        private void Merchandisemanagment_Load(object sender, EventArgs e)
        {
            foreach (DataGridViewColumn column in dataGridView1.Columns)
            {
                column.SortMode = DataGridViewColumnSortMode.NotSortable;
            }
            if (resizer != null)
            {
                resizer.ResizeControls(this);
                fontResizer.AdjustFont(this);
            }
        }

        private void Merchandisemanagment_Resize(object sender, EventArgs e)
        {
            if (resizer != null)
            {
                resizer.ResizeControls(this);
                fontResizer.AdjustFont(this);
                CenterLabel();

                if (dataGridView1.RowCount > 0)
                {                
                    dgvColumnWidth();
                }
            }
        }

        private void Savereceipes_Shown(object sender, EventArgs e)
        {
            dataGridView1.Focus();
        }

        private void Savereceipes_Click(object sender, EventArgs e)
        {
            roundedPanel1.Visible = false;

            if (clavieroverlay != null && clavieroverlay.Visible == true)
            {
                clavieroverlay.Visible = false;
                dataGridView1.Focus();
            }
        }

        private void dataGridView1_Click(object sender, EventArgs e)
        {
            if (clavieroverlay != null && clavieroverlay.Visible == true)
            {
                clavieroverlay.Visible = false;
                dataGridView1.Focus();
            }
        }

        private void label4_MouseEnter(object sender, EventArgs e)
        {
            Cursor = Cursors.Hand;
            label4.ForeColor = SystemColors.Highlight;
            label4.Font = new Font(label4.Font, FontStyle.Underline);
        }

        private void label4_MouseLeave(object sender, EventArgs e)
        {
            Cursor = Cursors.Default;
            label4.ForeColor = Color.DodgerBlue;
            label4.Font = new Font(label4.Font, FontStyle.Bold);
        }


        public void LocalizeControls()
        {
            label4.Text = LanguageManager.Instance.GetString("SREC-lbl4");
            rjButton1.Text = LanguageManager.Instance.GetString("Btn-close");
            rjButton2.Text = LanguageManager.Instance.GetString("Btn-save");
            rjButton6.Text = LanguageManager.Instance.GetString("SREC-rbtn6");
            rjButton7.Text = LanguageManager.Instance.GetString("SREC-rbtn7");
            //button5.Text = LanguageManager.Instance.GetString("MF-btn5");
        }

        public void ApplyTheme()
        {
            this.BackColor = colors.Color1;
            this.ForeColor = colors.Color2;

            roundedPanel1.BackColor = colors.Color3;
            roundedTextbox1.BorderColor = colors.Color3;
            dataGridView1.BackgroundColor = colors.Color1;
            dataGridView2.BackgroundColor = colors.Color3;
            roundedTextbox1.BackColorRounded = colors.Color3;
            dataGridView1.DefaultCellStyle.BackColor = colors.Color1;
            dataGridView2.DefaultCellStyle.BackColor = colors.Color3;
       
            roundedPanel1.ForeColor = this.ForeColor;
            dataGridView1.ForeColor = this.ForeColor;
            dataGridView2.ForeColor = this.ForeColor;
            roundedTextbox1.ForeColor = this.ForeColor;
            dataGridView1.DefaultCellStyle.ForeColor = this.ForeColor;
            dataGridView2.DefaultCellStyle.ForeColor = this.ForeColor;
            label4.Font = new Font("Century Gothic", 10, FontStyle.Bold);
            if (colors.Bool1 && Properties.Settings.Default.showScrollbars) showScrollBars();
        }

        private void showScrollBars() 
        {
            dataGridView1.ScrollBars = ScrollBars.Both;
            dataGridView2.ScrollBars = ScrollBars.Vertical;
        }
    }

  

    public class Receipt
    {
        public string Header { get; set; }
        public List<ReceiptItem> Items { get; set; } = new List<ReceiptItem>();
        public double TotalAmount { get; set; }
        public string RestaurantName { get; set; }
        public byte[] RestaurantLogo { get; set; }
        public string ClientName { get; set; }
        public string ClientAddress { get; set; }
    }

    public class ReceiptItem
    {
        public string Name { get; set; }
        public double AmountInGrams { get; set; }
    }
}
