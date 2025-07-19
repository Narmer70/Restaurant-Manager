using System;
using QRCoder;
using System.IO;
using System.Data;
using System.Linq;
using System.Drawing;
using System.Diagnostics;
using System.Data.SQLite;
using System.Globalization;
using System.Windows.Forms;
using System.Data.SqlClient;
using PadTai.Fastcheckfiles;
using PadTai.Classes.Others;
using System.Drawing.Printing;
using System.Collections.Generic;
using PadTai.Classes.Databaselink;
using PadTai.Sec_daryfolders.Updates;
using PadTai.Classes.Fastcheckmodifiers;


namespace PadTai
{
    public class FoodItemManager
    {
        public event Action<decimal> TotalPriceUpdated;
        private CrudDatabase crudDatabase;

        public FoodItemManager()
        {
            crudDatabase = new CrudDatabase();
        }

        public void AddFoodItemToGrid(string foodId, DataGridView dataGridView1, Dictionary<int, DiscountConfig> discounts)
        {
            string query = $"SELECT FoodID, FoodName, Price, FooditemtypeID FROM FoodItems WHERE FoodID = '{foodId}'";

            // Fetch data using the existing method
            DataTable foodItemData = crudDatabase.FetchDataFromDatabase(query);

            if (foodItemData.Rows.Count > 0)
            {
                DataRow row = foodItemData.Rows[0];
                int foodid = Convert.ToInt32(row["FoodID"]);
                string foodName = row["FoodName"].ToString();
                decimal price = Convert.ToDecimal(row["Price"]);
                int foodtypeid = Convert.ToInt32(row["FooditemtypeID"]);

                if (!string.IsNullOrEmpty(foodName) && foodtypeid > 0)
                {
                    string foodNameSpaced = "    " + foodName;
                    dataGridView1.Rows.Add(foodNameSpaced, price, foodtypeid, foodid);
                }
            }

            ApplyIndividualDiscounts(dataGridView1, discounts);
        }


        public void ApplyIndividualDiscounts(DataGridView dataGridView1, Dictionary<int, DiscountConfig> discounts)
        {
            if (discounts == null || discounts.Count == 0) return;

            // Group items by FoodID to count occurrences
            var groupedItems = new Dictionary<int, List<DataGridViewRow>>();
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                int foodID = (int)row.Cells[3].Value;
                if (!groupedItems.ContainsKey(foodID))
                {
                    groupedItems[foodID] = new List<DataGridViewRow>();
                }
                groupedItems[foodID].Add(row);
            }

            foreach (var group in groupedItems)
            {

                var foodID = group.Key;
                var occurrences = group.Value.Count;

                if (discounts.TryGetValue(foodID, out var discountConfig) && occurrences >= discountConfig.OccurrencesRequired)
                {

                    foreach (var row in group.Value)
                    {
                        var foodName = row.Cells[0].Value.ToString();

                        if (!foodName.Contains($"[-{discountConfig.DiscountPercentage}%"))
                        {
                            decimal price = decimal.Parse(row.Cells[1].Value.ToString());
                            decimal discountAmount = price * (decimal)discountConfig.DiscountPercentage / 100;
                            decimal discountedPrice = price - discountAmount;
                            row.Cells[0].Value = $"{foodName} [-{discountConfig.DiscountPercentage}%]"; /*  {price}-{discountAmount}*/
                            row.Cells[1].Value = discountedPrice.ToString("0.00");
                        }
                    }
                }
                else
                {
                    foreach (var row in group.Value)
                    {
                        var foodName = row.Cells[0].Value.ToString();
                        if (foodName.Contains("["))
                            foodName = foodName.Substring(0, foodName.IndexOf(" ["));

                        // Use string interpolation to create the query
                        string query = $"SELECT Price FROM FoodItems WHERE FoodID = '{group.Key}'";

                        // Fetch data using the existing method
                        DataTable priceData = crudDatabase.FetchDataFromDatabase(query);

                        if (priceData.Rows.Count > 0)
                        {
                            decimal price = Convert.ToDecimal(priceData.Rows[0]["Price"]);
                            row.Cells[0].Value = $"{foodName}";
                            row.Cells[1].Value = price.ToString("0.00");
                        }
                    }
                }
            }
        }

        public void paymentsystem(string paymentid, DataGridView dataGridView2)
        {
            string paymentname = string.Empty;

            // Use string interpolation to create the query
            string query = $"SELECT PaymenttypeName FROM PaymentTypes WHERE PaymentypeID = '{paymentid}'";

            // Fetch data using the existing method
            DataTable paymentData = crudDatabase.FetchDataFromDatabase(query);

            if (paymentData.Rows.Count > 0)
            {
                paymentname = paymentData.Rows[0]["PaymenttypeName"].ToString();
            }

            if (!string.IsNullOrEmpty(paymentname))
            {
                while (dataGridView2.Rows.Count <= 1)
                {
                    dataGridView2.Rows.Add();
                }
                try
                {
                    dataGridView2.Rows[1].Cells[0].Value = LanguageManager.Instance.GetString("FIM-paytype");
                    dataGridView2.Rows[1].Cells[1].Value = paymentname;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"An error occurred: {ex.Message}");
                }
            }
        }

        public void Wheretoeat(string placetoeatid, DataGridView dataGridView2)
        {
            string placetoeatname = string.Empty;

            // Use string interpolation to create the query
            string query = $"SELECT Thedelivery FROM Delivery WHERE DeliveryID = '{placetoeatid}'";

            // Fetch data using the existing method
            DataTable deliveryData = crudDatabase.FetchDataFromDatabase(query);

            if (deliveryData.Rows.Count > 0)
            {
                placetoeatname = deliveryData.Rows[0]["Thedelivery"].ToString();
            }

            if (!string.IsNullOrEmpty(placetoeatname))
            {
                while (dataGridView2.Rows.Count <= 2)
                {
                    dataGridView2.Rows.Add();
                }
                try
                {
                    dataGridView2.Rows[2].Cells[0].Value = LanguageManager.Instance.GetString("FIM-delivery");
                    dataGridView2.Rows[2].Cells[1].Value = placetoeatname;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"An error occurred: {ex.Message}");
                }
            }
        }

        public void AddDiscount(string discountID, DataGridView dataGridView2)
        {
            string TheDiscount = string.Empty;

            // Use string interpolation to create the query
            string query = $"SELECT Thediscount FROM Discounts WHERE DiscountID = '{discountID}'";

            // Fetch data using the existing method
            DataTable discountData = crudDatabase.FetchDataFromDatabase(query);

            if (discountData.Rows.Count > 0)
            {
                TheDiscount = discountData.Rows[0]["Thediscount"].ToString();
            }

            if (!string.IsNullOrEmpty(TheDiscount))
            {
                while (dataGridView2.Rows.Count <= 3)
                {
                    dataGridView2.Rows.Add();
                }
                try
                {
                    dataGridView2.Rows[3].Cells[0].Value = LanguageManager.Instance.GetString("FIM-discount");
                    dataGridView2.Rows[3].Cells[1].Value = TheDiscount;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"An error occurred: {ex.Message}");
                }

            }
        }

        public void AddTable(string tableID, Label label8)
        {
            string Tablenumber = string.Empty;

            // Use string interpolation to create the query
            string query = $"SELECT Thetablenumber FROM Tablenumber WHERE TableID = '{tableID}'";

            // Fetch data using the existing method
            DataTable tableData = crudDatabase.FetchDataFromDatabase(query);

            if (tableData.Rows.Count > 0)
            {
                Tablenumber = tableData.Rows[0]["Thetablenumber"].ToString();
            }

            if (!string.IsNullOrEmpty(Tablenumber))
            {
                try
                {
                    label8.Text = Tablenumber;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"An error occurred: {ex.Message}");
                }
            }
        }


        public void UpdateTotalPrice(DataGridView dataGridView1, DataGridView dataGridView2)
        {
            decimal totalPrice = 0.00m;

            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                if (!row.IsNewRow && row.Cells[1].Value != null)
                {
                    if (decimal.TryParse(row.Cells[1].Value.ToString(), out decimal cellValue))
                    {
                        totalPrice += cellValue;
                    }
                    else
                    {

                    }
                }
                if (totalPrice != 0.00m)
                {
                    while (dataGridView2.Rows.Count < 1)
                    {
                        dataGridView2.Rows.Add();
                    }
                    try
                    {
                        dataGridView2.Rows[0].Cells[0].Value = LanguageManager.Instance.GetString("FIM-totalprice");
                        dataGridView2.Rows[0].Cells[1].Value = totalPrice;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"An error occurred: {ex.Message}");
                    }
                }

                TotalPriceUpdated?.Invoke(totalPrice);
            }
        }

        public void SaveReceipts(DataGridView dataGridView1, DataGridView dataGridView2, Label label8, Label label1)
        {
            List<FoodItem> foodItems = GetFoodItemsFromDataGridView(dataGridView1);
            var (totalPrice, paymentTypeName, discount, placeToEat) = GetSummaryFromDataGridView(dataGridView2);
            var (tableNumber, buyerID, buyerName) = GetOtherReceiptsData(label8, label1);

            // Default PlaceToEat if empty
            if (string.IsNullOrEmpty(placeToEat))
            {
                placeToEat = "NULL";
            }

            if (string.IsNullOrEmpty(discount))
            {
                discount = "0";
            }

            // Concatenate food items and prices into single strings
            string foodItemsConcatenated = string.Join(", ", foodItems.Select(fi => fi.FoodName));
            string foodPricesConcatenated = string.Join(", ", foodItems.Select(fi => fi.Price.ToString("F2", CultureInfo.InvariantCulture))); // Format to 2 decimal places
            string FoodItemidConcatenated = string.Join(", ", foodItems.Select(fi => fi.FooditemtypeID.ToString("F0")));
            string FoodIdConcatenated = string.Join(", ", foodItems.Select(fi => fi.FoodID.ToString()));

            try
            {
                int newReceiptId;
                int clientId;

                // Step 1: Get the next ReceiptId for the ClientID
                string receiptIdQuery = @"SELECT LastReceiptId FROM Receiptcount WHERE ClientID = @ClientID LIMIT 1;";

                if (!int.TryParse(Properties.Settings.Default.SelectedClientId, out clientId))
                {
                    throw new Exception("Invalid Client ID.");
                }

                // Fetch the last receipt ID
                var parameters = new Dictionary<string, object>
                {
                    { "@ClientID", clientId }
                };

                DataTable lastReceiptIdTable = crudDatabase.FetchDataFromDatabase(receiptIdQuery, parameters);

                if (lastReceiptIdTable.Rows.Count > 0)
                {
                    // If the record exists, update LastReceiptId
                    int updatedLastReceiptId = Convert.ToInt32(lastReceiptIdTable.Rows[0]["LastReceiptId"]) + 1;

                    string updateQuery = @"UPDATE Receiptcount SET LastReceiptId = @NewLastReceiptId WHERE ClientID = @ClientID;";

                    parameters = new Dictionary<string, object>
                    {
                        { "@NewLastReceiptId", updatedLastReceiptId },
                        { "@ClientID", clientId }
                    };
                    crudDatabase.ExecuteNonQuery(updateQuery, parameters);

                    newReceiptId = updatedLastReceiptId; // Use the updated LastReceiptId
                }
                else
                {
                    // If the record does not exist, insert a new record
                    string insertQuery = @"INSERT INTO Receiptcount (ClientID, LastReceiptId) VALUES (@ClientID, 1);";

                    parameters = new Dictionary<string, object>
                    {
                        { "@ClientID", clientId }
                    };
                    crudDatabase.ExecuteNonQuery(insertQuery, parameters);

                    newReceiptId = 1; // Set newReceiptId to 1 for the new record
                }


                // Start building the base query
                string receiptQuery = "INSERT INTO Receipts (ReceiptId, FoodName, FoodPrice, OrderDateTime, PaymenttypeName, PlacetoEatName, Thediscount, TotalPrice, FooditemtypeID, FoodID";

                // Add columns conditionally
                if (!string.IsNullOrEmpty(tableNumber))
                {
                    receiptQuery += ", Ordertable";
                }
                if (!string.IsNullOrEmpty(buyerID))
                {
                    receiptQuery += ", BuyerID";
                }

                receiptQuery += ", ClientID) VALUES (@ReceiptId, @FoodName, @FoodPrice, @OrderDateTime, @PaymenttypeName, @PlacetoEatName, @Thediscount, @TotalPrice, @FooditemtypeID, @FoodID";

                // Add values conditionally
                if (!string.IsNullOrEmpty(tableNumber))
                {
                    receiptQuery += ", @Ordertable";
                }
                if (!string.IsNullOrEmpty(buyerID))
                {
                    receiptQuery += ", @BuyerID";
                }

                receiptQuery += ", @ClientID)";

                // Add values conditionally
                var receiptParameters = new Dictionary<string, object>
                {
                     { "@ReceiptId", newReceiptId },
                     { "@FoodName", foodItemsConcatenated },
                     { "@FoodPrice", foodPricesConcatenated },
                     { "@OrderDateTime", DateTime.Now },
                     { "@PaymenttypeName", paymentTypeName },
                     { "@PlacetoEatName", placeToEat },
                     { "@Thediscount", discount },
                     { "@TotalPrice", totalPrice.ToString("F2", CultureInfo.InvariantCulture) },
                     { "@FooditemtypeID", FoodItemidConcatenated },
                     { "@FoodID", FoodIdConcatenated },
                     { "@ClientID", clientId }
                };

                // Add parameters conditionally
                if (!string.IsNullOrEmpty(tableNumber))
                {
                    receiptParameters.Add("@Ordertable", tableNumber);
                }
                if (!string.IsNullOrEmpty(buyerID))
                {
                    receiptParameters.Add("@BuyerID", buyerID);
                }

                // Execute the query
                crudDatabase.ExecuteNonQuery(receiptQuery, receiptParameters);

                if (Properties.Settings.Default.Automaticprint)
                {
                    var detailsFetcher = new RestaurantDetails();
                    var (restaurantName, restaurantLogo, clientName, clientAddress) = detailsFetcher.GetDetails();

                    // Create a receipt object
                    Receipt receipt = new Receipt
                    {
                        ReceiptID = newReceiptId,
                        DateTime = DateTime.Now,
                        RestaurantLocation = clientName,
                        RestaurantName = restaurantName,
                        RestaurantAddress = clientAddress,
                        RestaurantLogo = restaurantLogo,
                        FoodItems = foodItems,
                        TotalPrice = totalPrice,
                        PaymentTypeName = paymentTypeName,
                        Discount = discount,
                        PlaceToEat = placeToEat,
                        tableNumber = tableNumber,
                        buyerName = buyerName
                    };

                    // Print the receipt
                    PrintReceipt(receipt);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error saving receipts: " + ex.Message);
            }
        }

        private List<FoodItem> GetFoodItemsFromDataGridView(DataGridView dataGridView)
        {
            List<FoodItem> foodItems = new List<FoodItem>();

            for (int i = 0; i < dataGridView.Rows.Count; i++)
            {
                var row = dataGridView.Rows[i];
                if (row.IsNewRow) continue; // Skip the new row placeholder

                // Check if both FoodName and Price cells are non-empty
                if (row.Cells.Count >= 3)
                {
                    string foodName = row.Cells[0].Value?.ToString();
                    string priceValue = row.Cells[1].Value?.ToString();
                    string fooditemtypevalue = row.Cells[2].Value?.ToString();
                    string foodidvalue = row.Cells[3].Value?.ToString();

                    if (!string.IsNullOrEmpty(foodName) && decimal.TryParse(priceValue, out decimal price) && int.TryParse(fooditemtypevalue, out int fooditemtypeid) && int.TryParse(foodidvalue, out int foodid))
                    {
                        var foodItem = new FoodItem
                        {
                            FoodName = foodName,
                            Price = price,
                            FooditemtypeID = fooditemtypeid,
                            FoodID = foodid,
                        };
                        foodItems.Add(foodItem);
                    }
                }
            }
            return foodItems;
        }

        private (decimal TotalPrice, string PaymenttypeName, string Discount, string PlaceToEat) GetSummaryFromDataGridView(DataGridView dataGridView)
        {
            decimal totalPrice = 0;
            string paymentTypeName = null;
            string placeToEat = null;
            string discount = null;

            if (dataGridView.Rows.Count > 0)
            {
                // Ensure there are enough rows before accessing them
                if (dataGridView.Rows.Count >= 6 && dataGridView.Rows[5].Cells.Count > 1 && dataGridView.Rows[5].Cells[1].Value != null)
                {
                    // Fetch value from the 6th row, second cell
                    totalPrice = (decimal)dataGridView.Rows[5].Cells[1].Value;
                }
                else if (dataGridView.Rows.Count >= 1 && dataGridView.Rows[0].Cells.Count > 1 && dataGridView.Rows[0].Cells[1].Value != null)
                {
                    // Fallback to the 1st row, first cell
                    totalPrice = (decimal)dataGridView.Rows[0].Cells[1].Value;
                }
                // PaymentTypeName in the second row, cell 0
                if (dataGridView.Rows.Count > 1 && dataGridView.Rows[1].Cells.Count > 0 && dataGridView.Rows[1].Cells[0].Value != null)
                {
                    paymentTypeName = dataGridView.Rows[1].Cells[1].Value.ToString();
                }
                // PlaceToEat in the third row, cell 0
                if (dataGridView.Rows.Count > 2 && dataGridView.Rows[2].Cells.Count > 0 && dataGridView.Rows[2].Cells[0].Value != null)
                {
                    placeToEat = dataGridView.Rows[2].Cells[1].Value.ToString();
                }
                //  Discount in the fourth row, cell 0
                if (dataGridView.Rows.Count > 3 && dataGridView.Rows[3].Cells.Count > 0 && dataGridView.Rows[3].Cells[0].Value != null)
                {
                    discount = dataGridView.Rows[3].Cells[1].Value.ToString();
                }
            }

            return (totalPrice, paymentTypeName, discount, placeToEat);
        }

        private (string tableNumber, string buyerID, string buyerName) GetOtherReceiptsData(Label label8, Label label1)
        {
            string tableNumber = null;
            string buyerID = null;
            string buyerName = null;

            if (label8 != null)
            {
                tableNumber = label8.Text; 
            }

            if (label1 != null)
            {
                if (label1.Tag != null)
                {
                    buyerID = label1.Tag.ToString(); 
                }
               
                buyerName = label1.Text;
            }

            return (tableNumber, buyerID, buyerName);
        }


        private void PrintReceipt(Receipt receipt)
        {
            PrintDocument printDocument = new PrintDocument();
            printDocument.PrintPage += (sender, e) =>
            {
                // Set up the font and brush
                Brush brush = Brushes.Black;
                Font font = new Font("Arial", 10);
                Font thefont = new Font("Arial", 10, FontStyle.Italic);
                Font otherfont = new Font("Arial", 10, FontStyle.Bold);

                // Center the restaurant name and address
                float discountValueX = e.Graphics.VisibleClipBounds.Width - 70;
                float discountValueY = e.Graphics.VisibleClipBounds.Width - 25;
                float centerX = e.Graphics.VisibleClipBounds.Width / 2;
                float discountLabelX = 10;
                decimal subtotal = 0; 
                float yPos = 70;

                // Draw receipt ID centered
                string receiptIdText = $"{receipt.ReceiptID}";
                SizeF receiptIdSize = e.Graphics.MeasureString(receiptIdText, new Font("Century Gothic", 14, FontStyle.Bold));
                e.Graphics.DrawString(receiptIdText, new Font("Century Gothic", 14, FontStyle.Bold), brush, centerX - (receiptIdSize.Width / 2), 0);

                // Draw DateTime centered
                string DatetimeText = $"{receipt.DateTime}";
                SizeF DatetimeSize = e.Graphics.MeasureString(DatetimeText, font);
                e.Graphics.DrawString(DatetimeText, font, brush, centerX - (DatetimeSize.Width / 2), 30);
               
                if (receipt.PlaceToEat != "NULL")
                {
                    string PlacetoEatText = $"{receipt.PlaceToEat}";
                    SizeF PlacetoEatSize = e.Graphics.MeasureString(PlacetoEatText, font);
                    e.Graphics.DrawString(PlacetoEatText, font, brush, centerX - (PlacetoEatSize.Width / 2), 50);
                   
                    e.Graphics.DrawString("---------------------------------------------------", new Font("Century Gothic", 12, FontStyle.Bold), brush, new PointF(5, 60));
                   
                    if ( receipt.buyerName != null && receipt.buyerName != string.Empty || receipt.tableNumber != null && receipt.tableNumber != string.Empty) 
                    {
                        if (receipt.buyerName != null && receipt.buyerName != string.Empty) 
                        {
                            string Clientname = LanguageManager.Instance.GetString("Receiptprintclient") + ":" + $" {receipt.buyerName}";
                            e.Graphics.DrawString(Clientname, font, brush, new PointF(discountLabelX, 80));
                        }
                        if (receipt.tableNumber != null && receipt.tableNumber != string.Empty) 
                        {
                            string Ordertable = LanguageManager.Instance.GetString("Receiptprinttable") + ":" + $" {receipt.tableNumber}";
                            e.Graphics.DrawString(Ordertable, font, brush, new PointF(discountValueX, 80));
                        }

                        e.Graphics.DrawString("---------------------------------------------------", new Font("Century Gothic", 12, FontStyle.Bold), brush, new PointF(5, 90));
                        yPos = 120;
                    }
                    else 
                    {
                        yPos = 90;
                    }
                }
                else
                {
                    e.Graphics.DrawString("---------------------------------------------------", new Font("Century Gothic", 12, FontStyle.Bold), brush, new PointF(5, 40));

                    if (receipt.buyerName != null && receipt.buyerName != string.Empty || receipt.tableNumber != null && receipt.tableNumber != string.Empty)
                    {
                        if (receipt.buyerName != null && receipt.buyerName != string.Empty)
                        {
                            string Clientname = LanguageManager.Instance.GetString("Receiptprintclient") + $" {receipt.buyerName}";
                            e.Graphics.DrawString(Clientname, font, brush, new PointF(discountLabelX, 60));
                        }
                        if (receipt.tableNumber != null && receipt.tableNumber != string.Empty)
                        {
                            string Ordertable = LanguageManager.Instance.GetString("Receiptprinttable") + ":" + $" {receipt.tableNumber}";
                            e.Graphics.DrawString(Ordertable, font, brush, new PointF(discountValueX, 60));
                        }

                        e.Graphics.DrawString("---------------------------------------------------", new Font("Century Gothic", 12, FontStyle.Bold), brush, new PointF(5, 70));
                        yPos = 100;
                    }
                    else
                    {
                        yPos = 70;
                    }
                }

                if (!Properties.Settings.Default.printToNewLine) 
                {
                    foreach (var item in receipt.FoodItems)
                    {
                        string foodItemText = $"{item.FoodName}";
                        // Truncate the food name to a maximum of 30 characters
                        if (foodItemText.Length > 28)
                        {
                            foodItemText = foodItemText.Substring(0, 28) + "..."; // Add ellipsis to indicate truncation
                        }

                        string priceText = item.Price.ToString("F2", CultureInfo.InvariantCulture);
                        float priceX = e.Graphics.VisibleClipBounds.Width - 60; // Fixed position for price

                        e.Graphics.DrawString(foodItemText.Trim(), font, brush, new PointF(10, yPos));
                        e.Graphics.DrawString(priceText, font, brush, new PointF(priceX, yPos));

                        subtotal += item.Price;
                        yPos += 20; // Move down for the next item
                    }
                }
                else 
                {
                    foreach (var item in receipt.FoodItems)
                    {
                        string foodItemText = $"{item.FoodName}";
                        string priceText = item.Price.ToString("F2", CultureInfo.InvariantCulture);
                        float priceX = e.Graphics.VisibleClipBounds.Width - 60; // Fixed position for price

                        // Check if the food item text exceeds 30 characters
                        if (foodItemText.Length <= 30 && !Properties.Settings.Default.isPlustonewLine)
                        {
                            // Directly draw the food item text if it's 30 characters or less
                            e.Graphics.DrawString(foodItemText.Trim(), font, brush, new PointF(10, yPos));
                        }
                        else
                        {
                            // Split the food item text into words
                            string[] words = foodItemText.Split(' ');
                            string currentLine = "";
                            float lineHeight = font.GetHeight(e.Graphics); // Get the height of the font for line spacing

                            foreach (var word in words)
                            {
                                // Check if the word is a '+' and if the setting is enabled
                                if (word == "+" && Properties.Settings.Default.isPlustonewLine)
                                {
                                    // If there's any current line, draw it before starting a new line
                                    if (!string.IsNullOrEmpty(currentLine))
                                    {
                                        e.Graphics.DrawString(currentLine.Trim(), font, brush, new PointF(10, yPos));
                                        yPos += lineHeight; // Move down for the next line
                                        currentLine = ""; // Reset current line
                                    }
                                    // Start a new line with the '+' sign
                                    currentLine = "+"; // Start the new line with the plus sign
                                    continue; // Skip to the next iteration
                                }

                                // Measure the width of the current line plus the new word
                                string testLine = currentLine + (currentLine.Length > 0 ? " " : "") + word;
                                SizeF size = e.Graphics.MeasureString(testLine, font);

                                // Check if the width exceeds the available width
                                if (size.Width > (e.Graphics.VisibleClipBounds.Width - 70)) // Adjust for margins
                                {
                                    // Draw the current line and move down
                                    e.Graphics.DrawString(currentLine.Trim(), font, brush, new PointF(10, yPos));
                                    yPos += lineHeight; // Move down for the next line
                                    currentLine = word; // Start a new line with the current word
                                }
                                else
                                {
                                    currentLine = testLine; // Update the current line
                                }
                            }

                            // Draw any remaining text in the current line
                            if (!string.IsNullOrEmpty(currentLine))
                            {
                                e.Graphics.DrawString(currentLine.Trim(), font, brush, new PointF(10, yPos));
                                // yPos += lineHeight; // Move down for the next item
                            }
                        }

                        // Draw the price
                        e.Graphics.DrawString(priceText, font, brush, new PointF(priceX, yPos));

                        subtotal += item.Price;
                        yPos += 20; // Move down for the next item
                    }
                }

                e.Graphics.DrawString("...................................................", new Font("Century Gothic", 14, FontStyle.Regular), brush, new PointF(5, yPos));
                yPos += 30;
                e.Graphics.DrawString(LanguageManager.Instance.GetString("Receiptprintsubtotal") + ":", thefont, brush, new PointF(discountLabelX, yPos));
                e.Graphics.DrawString(subtotal.ToString("F2", CultureInfo.InvariantCulture), font, brush, new PointF(discountValueX, yPos));
                yPos += 20;

                if (receipt.Discount != "0") 
                {
                    e.Graphics.DrawString(LanguageManager.Instance.GetString("FCK-lbl4"), thefont, brush, new PointF(discountLabelX, yPos));
                    e.Graphics.DrawString(receipt.Discount.ToString() + "%", font, brush, new PointF(discountValueX, yPos));
                    yPos += 20;
                }

                //e.Graphics.DrawString($"Tax:", font, brush, new PointF(discountLabelX, yPos));
                //e.Graphics.DrawString(tax.ToString("F2", CultureInfo.InvariantCulture), font, brush, new PointF(discountValueX, yPos);
                //yPos += 20; 

                e.Graphics.DrawString(LanguageManager.Instance.GetString("Receiptprintpaymethod"), thefont, brush, new PointF(discountLabelX, yPos));
                SizeF textSize = e.Graphics.MeasureString(receipt.PaymentTypeName.ToString(), font);
                float startX = discountValueY - textSize.Width;
                e.Graphics.DrawString(receipt.PaymentTypeName.ToString(), font, brush, new PointF(startX, yPos));           
                yPos += 10;       
                
                e.Graphics.DrawString("...................................................", new Font("Century Gothic", 14, FontStyle.Regular), brush, new PointF(5, yPos));
                yPos += 30;

                // Draw total price, payment type, discount, and place to eat at fixed positions
                e.Graphics.DrawString(LanguageManager.Instance.GetString("Receiptprinttotal"), otherfont, brush, new PointF(discountLabelX, yPos));
                e.Graphics.DrawString(receipt.TotalPrice.ToString("F2", CultureInfo.InvariantCulture), otherfont, brush, new PointF(discountValueX, yPos));
                
                if (CurrencyService.Instance.GetCurrencySymbol() != "")
                {
                     yPos += 20;
                    e.Graphics.DrawString(LanguageManager.Instance.GetString("Receiptprintcurrency"), thefont, brush, new PointF(discountLabelX, yPos));
                    e.Graphics.DrawString(CurrencyService.Instance.GetCurrencySymbol(), font, brush, new PointF(discountValueX, yPos));
                }
                yPos += 50;

                string ThanksMessage = LanguageManager.Instance.GetString("Receiptprintthanks");
                SizeF ThanksMessageSize = e.Graphics.MeasureString(ThanksMessage, font);
                e.Graphics.DrawString(ThanksMessage, font, brush, centerX - (ThanksMessageSize.Width / 2), yPos);
                yPos += 20;
                SizeF restaurantAddressSize = e.Graphics.MeasureString(receipt.RestaurantAddress, font);
                e.Graphics.DrawString(receipt.RestaurantAddress, font, brush, centerX - (restaurantAddressSize.Width / 2), yPos);
                yPos += 20;
                string Pointname = receipt.RestaurantName.ToUpper() + ":" + receipt.RestaurantLocation.ToUpper();
                SizeF restaurantNameSize = e.Graphics.MeasureString(Pointname, font);
                e.Graphics.DrawString(Pointname, font, brush, centerX - (restaurantNameSize.Width / 2), yPos);

                if (Properties.Settings.Default.printBrandLogo && !Properties.Settings.Default.printQRcode) 
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
                else if (Properties.Settings.Default.printBrandLogo && Properties.Settings.Default.printQRcode) 
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

                            // Draw the logo with the specified dimensions
                            e.Graphics.DrawImage(logo, new Rectangle(10, (int)yPos, desiredWidth, desiredHeight));
                        }
                    }

                    // Generate QR Code for the receipt
                    string qrText = LanguageManager.Instance.GetString("Receiptprintqrcode"); // Replace with actual data
                    QRCodeGenerator qrGenerator = new QRCodeGenerator();
                    QRCodeData qrCodeData = qrGenerator.CreateQrCode(qrText, QRCodeGenerator.ECCLevel.Q);
                    QRCode qrCode = new QRCode(qrCodeData);
                    Bitmap qrCodeImage = qrCode.GetGraphic(20); // You can adjust the size here

                    // Draw the QR Code next to the logo
                    int qrCodeWidth = 80;
                    int qrCodeHeight = 80;
                    float qrCodeX = e.Graphics.VisibleClipBounds.Width - 100;
                    int qrCodeY = (int)yPos; // Align with the logo's y position

                    e.Graphics.DrawImage(qrCodeImage, new Rectangle((int)qrCodeX, qrCodeY, qrCodeWidth, qrCodeHeight));
                }
                else if (!Properties.Settings.Default.printBrandLogo && Properties.Settings.Default.printQRcode) 
                {
                    yPos += 20;
                   
                    // Generate QR Code for the receipt
                    string qrText = LanguageManager.Instance.GetString("Receiptprintqrcode"); // Replace with actual data
                    QRCodeGenerator qrGenerator = new QRCodeGenerator();
                    QRCodeData qrCodeData = qrGenerator.CreateQrCode(qrText, QRCodeGenerator.ECCLevel.Q);
                    QRCode qrCode = new QRCode(qrCodeData);
                    Bitmap qrCodeImage = qrCode.GetGraphic(20); // You can adjust the size here

                    // Draw the QR Code next to the logo
                    int qrCodeWidth = 80;
                    int qrCodeHeight = 80;
                    int qrCodeX = (int)(e.Graphics.VisibleClipBounds.Width - qrCodeWidth) / 2;
                    int qrCodeY = (int)yPos; // Align with the logo's y position

                    e.Graphics.DrawImage(qrCodeImage, new Rectangle(qrCodeX, qrCodeY, qrCodeWidth, qrCodeHeight));
                }
                else 
                {
                }
            };

            printDocument.PrinterSettings.Copies = (short)Properties.Settings.Default.NumberOfCopies;
            printDocument.Print();
        }

        public class FoodItem
        {
            public string FoodName { get; set; }
            public decimal Price { get; set; }
            public decimal TotalPrice { get; set; }
            public string PaymenttypeName { get; set; }
            public string Discount { get; set; }
            public string PlaceToEat { get; set; }
            public int FooditemtypeID { get; set; }
            public int FoodID { get; set; }
        }

        public class Receipt
        {
            public int ReceiptID {  get; set; }
            public DateTime DateTime { get; set; }
            public string RestaurantName { get; set; }
            public string RestaurantAddress { get; set; }
            public byte[] RestaurantLogo { get; set; }
            public List<FoodItem> FoodItems { get; set; }
            public decimal TotalPrice { get; set; }
            public string PaymentTypeName { get; set; }
            public string Discount { get; set; }
            public string PlaceToEat { get; set; }
            public string RestaurantLocation { get; set; }
            public string tableNumber { get; set; }
            public string buyerName { get; set; }
        }
    }
}

