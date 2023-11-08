using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using NbrbAPI.Models;
using Telerik.WinControls.Export;

namespace CourseScreen
{
    public partial class Form2 : Form
    {
        List<Rate> rates = new List<Rate>();
        public Form2(List<Rate> listRates)
        {
            rates = listRates;
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                SaveFileDialog saveFileDialog1 = new SaveFileDialog();

                saveFileDialog1.Filter = "xlsx files (*.xlsx)|*.xlsx";
                if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    GridViewSpreadExport spreadExporter = new GridViewSpreadExport(this.radGridView1);
                    spreadExporter.ExportChildRowsGrouped = true;
                    SpreadExportRenderer exportRenderer = new SpreadExportRenderer();
                    spreadExporter.RunExport(saveFileDialog1.FileName, exportRenderer);
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, "Ошибка");
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void radGridView1_CellClick(object sender, Telerik.WinControls.UI.GridViewCellEventArgs e)
        {
            if (radGridView1.SelectedRows[0].Cells[5].Value != null)
            {
                int curId = (int)radGridView1.SelectedRows[0].Cells[0].Value;
                List<Rate> selectedCur = rates.Where(r => r.Cur_ID == curId).OrderBy(r => r.Date).ToList();
                double minValue = (double)selectedCur.Min(s => s.Cur_OfficialRate).Value;

                chart1.ChartAreas[0].AxisX.MajorGrid.Enabled = false;
                chart1.ChartAreas[0].AxisY.Minimum = minValue;
                chart1.Series[0].Points.Clear();


                int i = 0;
                foreach (var item in selectedCur)
                {
                    chart1.Series[0].Points.AddXY(++i, item.Cur_OfficialRate);
                }
            }
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            radGridView1.DataSource = rates.OrderBy(r => r.Date);
        }
    }
}
