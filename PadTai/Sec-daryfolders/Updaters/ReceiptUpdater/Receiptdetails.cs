using System;
using QRCoder;
using System.IO;
using System.Data;
using System.Linq;
using System.Text;
using System.Drawing;
using PadTai.Classes;
using System.Data.SQLite;
using System.Drawing.Text;
using System.Globalization;
using System.Windows.Forms;
using PadTai.Fastcheckfiles;
using System.Data.SqlClient;
using PadTai.Classes.Others;
using System.Reflection.Emit;
using System.Threading.Tasks;
using System.Drawing.Printing;
using System.Collections.Generic;
using PadTai.Classes.Databaselink;
using PadTai.Sec_daryfolders.Updaters.FoodUpdater;


namespace PadTai.Sec_daryfolders.Updates
{
    public partial class Receiptdetails : Form
    {
        public IndividualDiscount individualDiscount;
        private DataGridViewScroller scroller;
        private CrudDatabase crudDatabase;
        private bool isKeyPressed = false;

        public int ReceiptId { get; set; }
        private ControlResizer resizer;
        private FontResizer fontResizer;
        private DraggableForm draggableForm;
        private Displayreceipts displayreceipts;


        public Receiptdetails(Displayreceipts display)
        {
            InitializeComponent();

            crudDatabase = new CrudDatabase();
            draggableForm = new DraggableForm();
            draggableForm.EnableDragging(this, this);
            fontResizer = new FontResizer();
            fontResizer.AdjustFont(this);

            resizer = new ControlResizer(this.Size);

            resizer.RegisterControl(button1);
            resizer.RegisterControl(button2);
            resizer.RegisterControl(button3);
            resizer.RegisterControl(button4);
            resizer.RegisterControl(button5);
            resizer.RegisterControl(button6);
            resizer.RegisterControl(dataGridView1);
            resizer.RegisterControl(dataGridView2);
            resizer.RegisterControl(dataGridView3);

            scroller = new DataGridViewScroller(this, null, dataGridView1, dataGridView2, dataGridView3);
            this.KeyDown += new KeyEventHandler(Receiptdetails_KeyDown);
            this.KeyUp += new KeyEventHandler(Receiptdetails_KeyUp);
            this.displayreceipts = display;
            this.KeyPreview = true;

            LocalizeControls();;
            ApplyTheme();
        }


        public void LoadReceiptDetails(int receiptId)
        {
            var receiptDetails = LoadReceiptData(receiptId);
            
            if (receiptDetails == null)
            {
                return; 
            
            }

            // Clear existing rows in the DataGridViews
            dataGridView1.Rows.Clear();
            dataGridView2.Rows.Clear();
            dataGridView3.Rows.Clear();

            // Populate dataGridView1 with ReceiptId and OrderDateTime
            dataGridView1.Rows.Add(receiptId.ToString());
            dataGridView1.Rows.Add(receiptDetails.OrderDateTime.ToString());

            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                row.Cells[0].Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }

            // Populate dataGridView2 with food items
            foreach (var foodItem in receiptDetails.FoodItems)
            {
                string foodNameWithSpace = "    " + foodItem.FoodName.Trim();
                dataGridView2.Rows.Add(foodNameWithSpace, foodItem.Price.Trim() + $" {CurrencyService.Instance.GetCurrencySymbol()}", foodItem.FoodItemTypeID);
            }

            // Format and add total price to dataGridView3
            object formattedPrice = receiptDetails.TotalPrice.ToString("n2") + $" {CurrencyService.Instance.GetCurrencySymbol()}";
            dataGridView3.Rows.Add($"    {LanguageManager.Instance.GetString("Receiptprinttotal")}", formattedPrice);
            dataGridView3.Rows.Add($"    {LanguageManager.Instance.GetString("Receiptprintpaymethod")}", receiptDetails.PaymentTypeName);
            dataGridView3.Rows.Add($"    {LanguageManager.Instance.GetString("Printdelivery")}", receiptDetails.PlaceToEatName);
            dataGridView3.Rows.Add($"    {LanguageManager.Instance.GetString("FCK-lbl4")}", receiptDetails.Discount + "%");

            // Check for BuyerID and Ordertable
            if (!string.IsNullOrEmpty(receiptDetails.BuyerID) || !string.IsNullOrEmpty(receiptDetails.TableNumber))
            {
                string buyerName = GetUsernameById(receiptDetails.BuyerID);
                dataGridView3.Rows.Add($"    {buyerName}", receiptDetails.TableNumber);
            }

            setcolumnWidths();
        }

        private void setcolumnWidths() 
        {
            dataGridView2.Columns[2].Visible = false;
            dataGridView1.Columns[0].Width = (int)(dataGridView1.Width * 1); 
            dataGridView2.Columns[0].Width = (int)(dataGridView1.Width * 0.80); 
            dataGridView2.Columns[1].Width = (int)(dataGridView1.Width * 0.20); 
            dataGridView2.Columns[2].Width = (int)(dataGridView1.Width * 0.00); 
            dataGridView3.Columns[0].Width = (int)(dataGridView1.Width * 0.80); 
            dataGridView3.Columns[1].Width = (int)(dataGridView1.Width * 0.20); 
        }


        public Receipt LoadReceiptData(int receiptId)
        {
            // Validate Client ID
            if (!int.TryParse(Properties.Settings.Default.SelectedClientId, out int clientId))
            {
                return null; 
            }

            // Load receipt details
            string query = "SELECT FoodName, Foodprice, FooditemtypeID, OrderDateTime, TotalPrice, PaymenttypeName, PlacetoEatName, Thediscount, Ordertable, BuyerID " +
                           "FROM Receipts WHERE ReceiptId = @ReceiptId AND ClientID = @ClientID";

            var parameters = new Dictionary<string, object>
            {
                { "@ReceiptId", receiptId },
                { "@ClientID", clientId }
            };

            // Fetch data from the database
            DataTable receiptTable = crudDatabase.FetchDataFromDatabase(query, parameters);

            if (receiptTable != null && receiptTable.Rows.Count > 0)
            {
                DataRow row = receiptTable.Rows[0]; // Get the first row

                // Create and populate a new Receipt object
                var receipt = new Receipt
                {
                    ReceiptId = receiptId,
                    Discount = row["Thediscount"].ToString(),
                    PaymentTypeName = row["PaymenttypeName"].ToString(),
                    TableNumber = row["Ordertable"].ToString(),
                    BuyerID = row["BuyerID"].ToString(),
                    PlaceToEatName = row["PlacetoEatName"].ToString(),
                    TotalPrice = row["TotalPrice"] != DBNull.Value ? Convert.ToDecimal(row["TotalPrice"]) : 0,
                    OrderDateTime = row["OrderDateTime"] != DBNull.Value ? Convert.ToDateTime(row["OrderDateTime"]) : DateTime.MinValue,
                    FoodItems = new List<FoodItem>()
                };

                // Assuming FoodName and Foodprice are stored as comma-separated values
                string foodNamesString = row["FoodName"].ToString();
                string foodPricesString = row["Foodprice"].ToString();
                string foodItemTypesString = row["FooditemtypeID"].ToString();

                // Split the food items into arrays
                string[] foodNames = foodNamesString.Split(',');
                string[] foodPrices = foodPricesString.Split(',');
                string[] foodItemTypes = foodItemTypesString.Split(',');

                // Create FoodItem objects and add them to the list
                for (int i = 0; i < Math.Min(foodNames.Length, Math.Min(foodPrices.Length, foodItemTypes.Length)); i++)
                {
                    var foodItem = new FoodItem
                    {
                        FoodName = foodNames[i].Trim(),
                        Price = foodPrices[i].Trim(),
                        FoodItemTypeID = int.TryParse(foodItemTypes[i].Trim(), out int foodItemTypeId) ? foodItemTypeId : 0
                    };

                    receipt.FoodItems.Add(foodItem);
                }

                return receipt; // Return the populated Receipt object
            }
            else
            {
               // MessageBox.Show("No receipt found.");
                return null; 
            }
        }


        private void deleteReceipt()
        {
            int clientId;

            if (!int.TryParse(Properties.Settings.Default.SelectedClientId, out clientId))
            {
                return;
            }

            string query = "DELETE FROM Receipts WHERE ReceiptId = @ReceiptId AND ClientID = @ClientID";
          
            var parameters = new Dictionary<string, object>
            {
                { "@ReceiptId", ReceiptId },
                { "@ClientID", clientId }
            };

            bool success = crudDatabase.ExecuteNonQuery(query, parameters);

            if (success)
            {
                updateforms();
                this.Close();
            }
            else
            {
                MessageBox.Show("Failed to delete the receipt.");
            }
        }

        public string GetUsernameById(string userId)
        {
            if (!int.TryParse(userId, out int userIdInt))
            {
                // Invalid User ID format
                return null;
            }

            string fullName = null;

            string query = "SELECT Username, Secondname FROM Userdata WHERE UserID = @UserID";
            var parameters = new Dictionary<string, object>
            {
                { "@UserID", userIdInt }
            };

            DataTable resultTable = crudDatabase.FetchDataFromDatabase(query, parameters);

            if (resultTable != null && resultTable.Rows.Count > 0)
            {
                DataRow row = resultTable.Rows[0];
                string username = row["Username"] as string;
                string secondName = row["Secondname"] as string;

                fullName = $"{username} {secondName}".Trim();
            }

            return fullName;
        }

        public void PrintReceipt(int ReceiptId)
        {
            var detailsFetcher = new RestaurantDetails();
            // Fetch restaurant and client details
            var (restaurantName, restaurantLogo, clientName, clientAddress) = detailsFetcher.GetDetails();

            // Fetch receipt details
            var receiptDetails = LoadReceiptData(ReceiptId);
        
            string buyerName = null; 

            if (receiptDetails.BuyerID != null)
            {            
                buyerName = GetUsernameById(receiptDetails.BuyerID);
            }

            if (receiptDetails.FoodItems == null || !receiptDetails.FoodItems.Any())
            {
              //  MessageBox.Show("Receipt not found.");
                return;
            }

            // Create a new PrintDocument
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
                float yPos;

                // Draw receipt ID centered
                string receiptIdText = $"{ReceiptId}";
                SizeF receiptIdSize = e.Graphics.MeasureString(receiptIdText, new Font("Century Gothic", 14, FontStyle.Bold));
                e.Graphics.DrawString(receiptIdText, new Font("Century Gothic", 14, FontStyle.Bold), brush, centerX - (receiptIdSize.Width / 2), 0);

                // Draw order date centered
                string DatetimeText = $"{receiptDetails.OrderDateTime}";
                SizeF DatetimeSize = e.Graphics.MeasureString(DatetimeText, font);
                e.Graphics.DrawString(DatetimeText, font, brush, centerX - (DatetimeSize.Width / 2), 30);

                if (receiptDetails.PlaceToEatName != "NULL")
                {
                    string PlacetoEatText = $"{receiptDetails.PlaceToEatName}";
                    SizeF PlacetoEatSize = e.Graphics.MeasureString(PlacetoEatText, font);
                    e.Graphics.DrawString(PlacetoEatText, font, brush, centerX - (PlacetoEatSize.Width / 2), 50);

                    e.Graphics.DrawString("---------------------------------------------------", new Font("Century Gothic", 12, FontStyle.Bold), brush, new PointF(5, 60));

                    if (buyerName != null && buyerName != string.Empty || receiptDetails.TableNumber != null && receiptDetails.TableNumber != string.Empty)
                    {
                        if (buyerName != null && buyerName != string.Empty)
                        {
                            string Clientname = LanguageManager.Instance.GetString("Receiptprintclient") + $" {buyerName}";
                            e.Graphics.DrawString(Clientname, font, brush, new PointF(discountLabelX, 80));
                        }
                        if (receiptDetails.TableNumber != null && receiptDetails.TableNumber != string.Empty)
                        {
                            string Ordertable = LanguageManager.Instance.GetString("Receiptprinttable") + ":" + $" {receiptDetails.TableNumber}";
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

                    if (buyerName != null && buyerName != string.Empty || receiptDetails.TableNumber != null && receiptDetails.TableNumber != string.Empty)
                    {
                        if (buyerName != null && buyerName != string.Empty)
                        {
                            string Clientname = LanguageManager.Instance.GetString("Receiptprintclient") + $" {buyerName}";
                            e.Graphics.DrawString(Clientname, font, brush, new PointF(discountLabelX, 60));
                        }
                        if (receiptDetails.TableNumber != null && receiptDetails.TableNumber != string.Empty)
                        {
                            string Ordertable = LanguageManager.Instance.GetString("Receiptprinttable") + ":" + $" {receiptDetails.TableNumber}";
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
                    foreach (var item in receiptDetails.FoodItems)
                    {
                        string foodItemText = $"{item.FoodName}";
                        // Truncate the food name to a maximum of 30 characters
                        if (foodItemText.Length > 28)
                        {
                            foodItemText = foodItemText.Substring(0, 28) + "..."; // Add ellipsis to indicate truncation
                        }

                        string priceText = item.Price; // Get the price as a string
                        float priceX = e.Graphics.VisibleClipBounds.Width - 60; // Fixed position for price

                        e.Graphics.DrawString(foodItemText.Trim(), font, brush, new PointF(10, yPos));
                        e.Graphics.DrawString(priceText, font, brush, new PointF(priceX, yPos));

                        // Change dot to comma in the price string
                        string priceWithComma = priceText.Replace('.', ',');

                        // Convert Price to decimal before adding to subtotal
                        if (decimal.TryParse(priceWithComma, out decimal price))
                        {
                            subtotal += price;
                        }
                        else
                        {
                            //Handle the case where Price cannot be parsed to decimal
                            //MessageBox.Show($"Failed to parse price: {priceWithComma}", "Parsing Error");
                        }

                        yPos += 20; // Move down for the next item
                    }
                }
                else
                {
                    foreach (var item in receiptDetails.FoodItems)
                    {
                        string foodItemText = $"{item.FoodName}";
                        string priceText = item.Price.ToString(CultureInfo.InvariantCulture);
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

                        // Change dot to comma in the price string
                        string priceWithComma = priceText.Replace('.', ',');

                        // Convert Price to decimal before adding to subtotal
                        if (decimal.TryParse(priceWithComma, out decimal price))
                        {
                            subtotal += price;
                        }
                        else
                        {
                            //MessageBox.Show($"Failed to parse price: {item.Price}", "Parsing Error");
                        }

                        yPos += 20; // Move down for the next item
                    }
                }

                e.Graphics.DrawString("...................................................", new Font("Century Gothic", 14, FontStyle.Regular), brush, new PointF(5, yPos));
                yPos += 30;
                e.Graphics.DrawString(LanguageManager.Instance.GetString("Receiptprintsubtotal") + ":", thefont, brush, new PointF(discountLabelX, yPos));
                e.Graphics.DrawString(subtotal.ToString("F2", CultureInfo.InvariantCulture), font, brush, new PointF(discountValueX, yPos));
                yPos += 20;

                if (receiptDetails.Discount != "0")
                {
                    e.Graphics.DrawString(LanguageManager.Instance.GetString("FCK-lbl4"), thefont, brush, new PointF(discountLabelX, yPos));
                    e.Graphics.DrawString(receiptDetails.Discount.ToString() + "%", font, brush, new PointF(discountValueX, yPos));
                    yPos += 20;
                }

                //e.Graphics.DrawString($"Tax:", font, brush, new PointF(discountLabelX, yPos));
                //e.Graphics.DrawString(tax.ToString("F2", CultureInfo.InvariantCulture), font, brush, new PointF(discountValueX, yPos);
                //yPos += 20; 

                e.Graphics.DrawString(LanguageManager.Instance.GetString("Receiptprintpaymethod"), thefont, brush, new PointF(discountLabelX, yPos));
                SizeF textSize = e.Graphics.MeasureString(receiptDetails.PaymentTypeName.ToString(), font);
                float startX = discountValueY - textSize.Width;
                e.Graphics.DrawString(receiptDetails.PaymentTypeName.ToString(), font, brush, new PointF(startX, yPos));
                yPos += 10;

                e.Graphics.DrawString("...................................................", new Font("Century Gothic", 14, FontStyle.Regular), brush, new PointF(5, yPos));
                yPos += 30;

                // Draw total price, payment type, discount, and place to eat at fixed positions
                e.Graphics.DrawString(LanguageManager.Instance.GetString("Receiptprinttotal"), otherfont, brush, new PointF(discountLabelX, yPos));
                e.Graphics.DrawString(receiptDetails.TotalPrice.ToString("F2", CultureInfo.InvariantCulture), otherfont, brush, new PointF(discountValueX, yPos));

                if (CurrencyService.Instance.GetCurrencySymbol() != "")
                {
                    yPos += 20;
                    e.Graphics.DrawString(LanguageManager.Instance.GetString("Receiptprintcurrency"), thefont, brush, new PointF(discountLabelX, yPos));
                    e.Graphics.DrawString(CurrencyService.Instance.GetCurrencySymbol(), font, brush, new PointF(discountValueX, yPos));
                }

                yPos += 40;

                // Thank you message
                string thanksMessage = LanguageManager.Instance.GetString("Receiptprintthanks");
                SizeF thanksMessageSize = e.Graphics.MeasureString(thanksMessage, font);
                e.Graphics.DrawString(thanksMessage, font, brush, centerX - (thanksMessageSize.Width / 2), yPos);
                yPos += 20;
                // Draw restaurant address
                SizeF restaurantAddressSize = e.Graphics.MeasureString(clientAddress, font); 
                e.Graphics.DrawString(clientAddress, font, brush, centerX - (restaurantAddressSize.Width / 2), yPos);
                yPos += 20;
                // Draw restaurant name and location
                string Pointname = restaurantName.ToUpper() + ":" + clientName.ToUpper();
                SizeF restaurantNameSize = e.Graphics.MeasureString(Pointname, font);
                e.Graphics.DrawString(Pointname, font, brush, centerX - (restaurantNameSize.Width / 2), yPos);

                yPos += 30;
                string warningMessage = LanguageManager.Instance.GetString("Receiptprintwarning");
                SizeF warningMessageSize = e.Graphics.MeasureString(warningMessage, new Font("Arial", 8, FontStyle.Regular));
                e.Graphics.DrawString(warningMessage, new Font("Arial", 8, FontStyle.Regular), brush, centerX - (warningMessageSize.Width / 2), yPos);

                if (Properties.Settings.Default.printBrandLogo && !Properties.Settings.Default.printQRcode)
                {
                    // Draw the restaurant logo if available
                    if (restaurantLogo != null)
                    {
                        yPos += 20;

                        using (MemoryStream ms = new MemoryStream(restaurantLogo))
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
                    if (restaurantLogo != null)
                    {
                        yPos += 20;

                        using (MemoryStream ms = new MemoryStream(restaurantLogo))
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
                            // int centerPic = (int)(e.Graphics.VisibleClipBounds.Width - desiredWidth) / 2;

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

            printDocument.Print();
        } 

        private void button3_Click(object sender, EventArgs e)
        {
            PrintReceipt(ReceiptId);
        }


        public void updateforms()
        {
            displayreceipts.LoadData();
            displayreceipts.UpdateControlVisibility();
            this.Close();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Receiptdetails_Load(object sender, EventArgs e)
        {
            //var (_, totalCount) = displayreceipts.HasMoreThanOneClient();
            //_totalReceipts = totalCount;    

            if (resizer != null)
            {
                resizer.ResizeControls(this);
                fontResizer.AdjustFont(this);
            }

            if (ReceiptId >= 1)
            {
                LoadReceiptDetails(ReceiptId);
            }
        }

        private void Receiptdetails_Resize(object sender, EventArgs e)
        {
            if (resizer != null)
            {
                resizer.ResizeControls(this);
                fontResizer.AdjustFont(this);
                setcolumnWidths();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                if (ReceiptId >= 1)
                {
                    using (Deletedishconfirm DDC = new Deletedishconfirm())
                    {
                        FormHelper.ShowFormWithOverlay(this.FindForm(), DDC);

                        // Check the DialogResult after the dialog is closed
                        if (DDC.DialogResult == DialogResult.OK)
                        {
                            deleteReceipt();
                            DDC.Close();
                        }
                        else if (DDC.DialogResult == DialogResult.Cancel)
                        {
                            DDC.Close();
                        }
                    }
                }
            }
            catch 
            {
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (ReceiptId >= 1)
            {
                ReceiptId--;
                LoadReceiptDetails(ReceiptId);
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            if (ReceiptId >= 0)
            {
                ReceiptId++;
                LoadReceiptDetails(ReceiptId);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                if (ReceiptId >= 1)
                {
                    Properties.Settings.Default.isUpdatepage = true;
                    Properties.Settings.Default.Save();

                    Fastcheck fastcheck = new Fastcheck(this)
                    {
                        UpdreceiptId = ReceiptId
                    };

                    fastcheck.insertreceipt();

                    using (Fastcheck fcheck = new Fastcheck(this))
                    {
                        fastcheck.disablecontrols();
                        FormHelper.ShowFormWithOverlay(fcheck, fastcheck);
                    }
                }
            }
            catch 
            {
               // MessageBox.Show("No receipt to update");
            }
        }

        private void Receiptdetails_KeyUp(object sender, KeyEventArgs e)
        {
            // Reset the flag when the key is released
            if (e.KeyCode == Keys.Up || e.KeyCode == Keys.Down)
            {
                isKeyPressed = false;
            }
        }

        private void Receiptdetails_KeyDown(object sender, KeyEventArgs e)
        {
            // Check if the Up Arrow key is pressed
            if (e.KeyCode == Keys.Up && !isKeyPressed)
            {
                isKeyPressed = true;
                if (ReceiptId >= 0)
                {
                    ReceiptId++;
                    LoadReceiptDetails(ReceiptId);
                }
            }
            // Check if the Down Arrow key is pressed
            else if (e.KeyCode == Keys.Down && !isKeyPressed)
            {
                isKeyPressed = true;
                if (ReceiptId >= 1)
                {
                    ReceiptId--;
                    LoadReceiptDetails(ReceiptId);
                }
            }
        }

        private void dataGridView1_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex == 0 && e.ColumnIndex == 0)
            {
                e.CellStyle.Font = new System.Drawing.Font("Segoe UI", 15, FontStyle.Bold);
                e.CellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }
            else if (e.RowIndex == 1 && e.ColumnIndex == 0)
            {
                e.CellStyle.ForeColor = System.Drawing.Color.DodgerBlue;
                e.CellStyle.SelectionForeColor = System.Drawing.Color.DodgerBlue;
                e.CellStyle.Font = new System.Drawing.Font("Segoe UI", 9, FontStyle.Regular);
            }
        }

        private void dataGridView3_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex == 0 && e.ColumnIndex == 1)
            {
                e.CellStyle.ForeColor = System.Drawing.Color.Green;
                e.CellStyle.SelectionForeColor = System.Drawing.Color.Green;
                e.CellStyle.Font = new System.Drawing.Font("Segoe UI", 12, FontStyle.Bold);

            }
            if (e.RowIndex == 1 && e.ColumnIndex == 1)
            {
                e.CellStyle.ForeColor = System.Drawing.Color.Chocolate;
                e.CellStyle.SelectionForeColor = System.Drawing.Color.Chocolate;
                e.CellStyle.Font = new System.Drawing.Font("Segoe UI", 10, FontStyle.Regular);

            }
        }


        public void LocalizeControls()
        {
            button4.Text = LanguageManager.Instance.GetString("Btn-close");
            //button1.Text = LanguageManager.Instance.GetString("MF-btn1");
            //button2.Text = LanguageManager.Instance.GetString("MF-btn2");
            //button3.Text = LanguageManager.Instance.GetString("MF-btn3");
            //button4.Text = LanguageManager.Instance.GetString("MF-btn4");
            //button5.Text = LanguageManager.Instance.GetString("MF-btn5");
            //button6.Text = LanguageManager.Instance.GetString("MF-btn6");
            //button7.Text = LanguageManager.Instance.GetString("MF-btn7");
        }

        public void ApplyTheme()
        {
            ColorPalette.ColorTrio colors = ColorPalette.GetColorTrio();

            this.BackColor = colors.Color1;
            this.ForeColor = colors.Color2;

            dataGridView1.BackgroundColor = colors.Color3;
            dataGridView2.BackgroundColor = colors.Color3;
            dataGridView3.BackgroundColor = colors.Color3;
            dataGridView1.DefaultCellStyle.BackColor = colors.Color3;
            dataGridView2.DefaultCellStyle.BackColor = colors.Color3;
            dataGridView3.DefaultCellStyle.BackColor = colors.Color3;
            dataGridView1.DefaultCellStyle.SelectionBackColor = colors.Color3;
            dataGridView2.DefaultCellStyle.SelectionBackColor = colors.Color3;
            dataGridView3.DefaultCellStyle.SelectionBackColor = colors.Color3;

            dataGridView1.ForeColor = this.ForeColor;
            dataGridView2.ForeColor = this.ForeColor;
            dataGridView3.ForeColor = this.ForeColor;
            dataGridView1.DefaultCellStyle.ForeColor = this.ForeColor;
            dataGridView2.DefaultCellStyle.ForeColor = this.ForeColor;
            dataGridView3.DefaultCellStyle.ForeColor = this.ForeColor;
            dataGridView1.DefaultCellStyle.SelectionForeColor = this.ForeColor;
            dataGridView2.DefaultCellStyle.SelectionForeColor = this.ForeColor;
            dataGridView3.DefaultCellStyle.SelectionForeColor = this.ForeColor;
            if (colors.Bool1 && Properties.Settings.Default.showScrollbars) showScrollBars();
        }

        private void showScrollBars()
        {
            dataGridView2.ScrollBars = ScrollBars.Vertical;
            dataGridView3.ScrollBars = ScrollBars.Vertical;
        }

        public class FoodItem
        {
            public string FoodName { get; set; }
            public string Price { get; set; }
            public int FoodItemTypeID { get; set; }
        }

        public class Receipt
        {
            public int ReceiptId { get; set; }
            public List<FoodItem> FoodItems { get; set; }
            public string Discount { get; set; }
            public string PaymentTypeName { get; set; }
            public decimal TotalPrice { get; set; }
            public string BuyerID { get; set; }
            public string TableNumber { get; set; }
            public string PlaceToEatName { get; set; }
            public DateTime OrderDateTime { get; set; }
        }
    }
}
