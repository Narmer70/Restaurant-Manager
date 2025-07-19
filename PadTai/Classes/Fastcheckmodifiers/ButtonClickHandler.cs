using System;
using PadTai.Classes;
using System.Windows.Forms;
using PadTai.Fastcheckfiles;
using System.Collections.Generic;


namespace PadTai
{
    public class ButtonClickHandler
    {
        private readonly UserControl _userControl;
        private FoodItemManager manager;

        public ButtonClickHandler(UserControl userControl)
        {
            _userControl = userControl;
            manager = new FoodItemManager();
        }

        public void HandleButtonClick(object sender)
        {
            if (sender is Button button)
            {
                if (button.Tag == null)
                {
                    return;
                }
                string foodId = button.Tag.ToString();
                var panelReceptacle = _userControl.Parent as Panel;
                var fastCheckForm = panelReceptacle?.Parent as Fastcheck;

                if (fastCheckForm != null)
                {
                    try
                    {
                        var discounts = ConvertDiscounts(fastCheckForm.individualDiscount.GetDiscountConfigurations());
                        manager.AddFoodItemToGrid(foodId, fastCheckForm.dataGridView1, discounts);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error adding food item: {ex.Message}");
                    }
                }
                else
                {
                    MessageBox.Show("Parent form not found. Please ensure you're in the correct context.");
                }
            }
            else
            {
                MessageBox.Show("The sender is not a button.");
            }
        }

        private Dictionary<int, DiscountConfig> ConvertDiscounts(Dictionary<int, PadTai.DiscountConfig> originalDiscounts)
        {
            var newDiscounts = new Dictionary<int, DiscountConfig>();

            if (originalDiscounts != null)
            {
                foreach (var item in originalDiscounts)
                {
                    var config = new DiscountConfig
                    {
                        DiscountPercentage = item.Value.DiscountPercentage,
                        OccurrencesRequired = item.Value.OccurrencesRequired
                    };
                    newDiscounts.Add(item.Key, config);
                }
            }
            return newDiscounts;
        }

        public void HandleButtonClickP(object sender)
        {
            if (sender is Button button)
            {
                if (button.Tag == null)
                {
                    return;
                }
                string paymentid = button.Tag.ToString();
                var panelReceptacle = _userControl.Parent as Panel;
                var fastCheckForm = panelReceptacle?.Parent as Fastcheck;

                if (fastCheckForm != null)
                {
                    try
                    {
                        manager.paymentsystem(paymentid, fastCheckForm.dataGridView2);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error adding payment type: {ex.Message}");
                    }
                }
                else
                {
                    MessageBox.Show("Parent form not found. Please ensure you're in the correct context.");
                }
            }
            else
            {
                MessageBox.Show("The sender is not a button.");
            }
        }

        public void HandleButtonClickWTE(object sender)
        {
            if (sender is Button button)
            {
                if (button.Tag == null)
                {
                    return;
                }
                string wheretoeatid = button.Tag.ToString();
                var panelReceptacle = _userControl.Parent as Panel;
                var fastCheckForm = panelReceptacle?.Parent as Fastcheck;

                if (fastCheckForm != null)
                {
                    try
                    {
                        manager.Wheretoeat(wheretoeatid, fastCheckForm.dataGridView2);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error adding delivery type: {ex.Message}");
                    }
                }
                else
                {
                    MessageBox.Show("Parent form not found. Please ensure you're in the correct context.");
                }
            }
            else
            {
                MessageBox.Show("The sender is not a button.");
            }
        }

        public void HandleDiscount(object sender)
        {
            if (sender is Button button)
            {
                if (button.Tag == null)
                {
                    return;
                }
                string discountID = button.Tag.ToString();
                var panelReceptacle = _userControl.Parent as Panel;
                var fastCheckForm = panelReceptacle?.Parent as Fastcheck;

                if (fastCheckForm != null)
                {
                    try
                    {
                        manager.AddDiscount(discountID, fastCheckForm.dataGridView2);

                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error adding discount: {ex.Message}");
                    }
                }
                else
                {
                    MessageBox.Show("Parent form not found. Please ensure you're in the correct context.");
                }
            }
            else
            {
                MessageBox.Show("The sender is not a button.");
            }
        }

        public void AddTablenumber(object sender)
        {
            if (sender is Button button)
            {
                if (button.Tag == null)
                {
                    return;
                }
                string tableID = button.Tag.ToString();
                var panelReceptacle = _userControl.Parent as Panel;
                var fastCheckForm = panelReceptacle?.Parent as Fastcheck;

                if (fastCheckForm != null)
                {
                    try
                    {
                        manager.AddTable(tableID, fastCheckForm.label8);

                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error adding table: {ex.Message}");
                    }
                }
                else
                {
                    MessageBox.Show("Parent form not found. Please ensure you're in the correct context.");
                }
            }
            else
            {
                MessageBox.Show("The sender is not a button.");
            }
        }
    }
}

