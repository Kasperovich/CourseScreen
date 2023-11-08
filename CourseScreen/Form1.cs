using System;
using System.Windows.Forms;
using System.Net.Http;
using System.Collections.Generic;
using NbrbAPI.Models;
using System.Text.Json;
using Telerik.WinControls.Export;
using System.Threading.Tasks;

namespace CourseScreen
{
    public partial class Form1 : Form
    {
        List<Currency> listCurrencyes = new List<Currency>();
        List<Rate> rates = new List<Rate>();
        public Form1()
        {
            InitializeComponent();
        }
        private void button2_Click(object sender, EventArgs e)
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
        private void Form1_Load(object sender, EventArgs e)
        {
            GetCurrencies();
            radGridView1.DataSource = listCurrencyes;
        }

        private async void GetCurrencies()
        {
            string URLstring = "https://www.nbrb.by/api/exrates/currencies";
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    using (HttpResponseMessage res = client.GetAsync(URLstring).Result)
                    {
                        using (HttpContent content = res.Content)
                        {
                            var data = await content.ReadAsStringAsync();
                            if(data != null)
                            {
                                listCurrencyes =  JsonSerializer.Deserialize<List<Currency>>(data);
                            }
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, "Ошибка");
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            rates.Clear();
            GetAllRateByPeriod();

            Form2 form2 = new Form2(rates);

            if(form2.ShowDialog() == DialogResult.OK)
            {

            }
        }

        private void GetAllRateByPeriod()
        {
            List<Rate> selectedRates = GetSelectedRates();

            foreach (var item in selectedRates)
            {
                List<Rate> currRateForPeriod = GetRatesDynamics(dateTimePicker1.Value.ToString("yyyy-MM-dd"),
                    dateTimePicker2.Value.ToString("yyyy-MM-dd"), item.Cur_ID.ToString()).Result;

                if(currRateForPeriod.Count == 0)
                {
                    rates.Add(item);
                }
                else
                {
                    foreach (var rate in currRateForPeriod)
                    {
                        rate.Cur_Abbreviation = item.Cur_Abbreviation;
                        rate.Cur_Name = item.Cur_Name;
                    }

                    rates.AddRange(currRateForPeriod);
                }
            }
        }

        private List<Rate> GetSelectedRates()
        {
            List<Rate> selectedRates = new List<Rate>();

            foreach (var item in radGridView1.SelectedRows)
            {
                selectedRates.Add(
                    new Rate
                    {
                        Cur_ID = (int)item.Cells[0].Value,
                        Cur_Abbreviation = item.Cells[4].Value.ToString(),
                        Cur_Name = item.Cells[5].Value.ToString()
                    });
            }

            return selectedRates;
        }

        private async Task<List<Rate>> GetRatesDynamics(string startDate, string endDate, string cur_Id)
        {
            string URLstring = $"https://www.nbrb.by/API/ExRates/Rates/Dynamics/" +
                $"{cur_Id}?startDate={startDate}&endDate={endDate}";

            try
            {
                using (HttpClient client = new HttpClient())
                {
                    using (HttpResponseMessage res = client.GetAsync(URLstring).Result)
                    {
                        using (HttpContent content = res.Content)
                        {
                            var data = await content.ReadAsStringAsync();
                            return JsonSerializer.Deserialize<List<Rate>>(data);
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, "Ошибка");
                return null;
            }
        }
    }
}
