using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Collections.Generic;
using PadTai.Classes.Controlsdesign;
using System.Runtime.Serialization.Formatters.Binary;


namespace PadTai.Classes.Fastcheckmodifiers
{
    internal class DraftReceiptSerializer
    {
        public static void SaveDraft(DataGridView dgv1, DataGridView dgv2, Label label1, Label label2, RoundedTextbox textBox, string filePath)
        {
            DraftReceiptData draftData = new DraftReceiptData
            {
                Label1Tag = label1.Tag, 
                Label1Text = label1.Text,
                Label2Text = label2.Text,
                TextBoxText = textBox.Text
            };

            // Save DataGridView 1 texts
            foreach (DataGridViewRow row in dgv1.Rows)
            {
                List<object> rowData = new List<object>();
                foreach (DataGridViewCell cell in row.Cells)
                {
                    rowData.Add(cell.Value); // Store the value directly
                }
                draftData.DataGridView1Texts.Add(rowData);
            }

            // Save DataGridView 2 texts
            foreach (DataGridViewRow row in dgv2.Rows)
            {
                List<object> rowData = new List<object>();
                foreach (DataGridViewCell cell in row.Cells)
                {
                    rowData.Add(cell.Value); // Store the value directly
                }
                draftData.DataGridView2Texts.Add(rowData);
            }

            using (FileStream fs = new FileStream(filePath, FileMode.Create))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(fs, draftData);
            }
        }

        public static void LoadDraft(DataGridView dgv1, DataGridView dgv2, Label label1, Label label2, RoundedTextbox textBox, string filePath)
        {
            if (!File.Exists(filePath)) return;

            using (FileStream fs = new FileStream(filePath, FileMode.Open))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                DraftReceiptData draftData = (DraftReceiptData)formatter.Deserialize(fs);

                // Load Label texts
                label1.Tag = draftData.Label1Tag; 
                label1.Text = draftData.Label1Text;
                label2.Text = draftData.Label2Text;
                textBox.Text = draftData.TextBoxText;

                // Load DataGridView 1 texts
                dgv1.Rows.Clear();
                foreach (var rowData in draftData.DataGridView1Texts)
                {
                    int rowIndex = dgv1.Rows.Add();
                    for (int i = 0; i < rowData.Count && i < dgv1.Columns.Count; i++)
                    {
                        dgv1.Rows[rowIndex].Cells[i].Value = rowData[i]; // Assign the object directly
                    }
                }

                // Load DataGridView 2 texts
                dgv2.Rows.Clear();
                foreach (var rowData in draftData.DataGridView2Texts)
                {
                    int rowIndex = dgv2.Rows.Add();
                    for (int i = 0; i < rowData.Count && i < dgv2.Columns.Count; i++)
                    {
                        dgv2.Rows[rowIndex].Cells[i].Value = rowData[i]; // Assign the object directly
                    }
                }
            }
        }
    }
}

[Serializable]
public class DraftReceiptData
{
    public List<List<object>> DataGridView1Texts { get; set; } = new List<List<object>>();
    public List<List<object>> DataGridView2Texts { get; set; } = new List<List<object>>();
    public object Label1Tag { get; set; } 
    public string Label1Text { get; set; }
    public string Label2Text { get; set; }
    public string TextBoxText { get; set; }
}