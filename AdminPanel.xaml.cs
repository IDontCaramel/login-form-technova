using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using LiveCharts;
using LiveCharts.Definitions.Charts;
using LiveCharts.Wpf;
using LiveChartsCore.SkiaSharpView.Painting;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore;
using SkiaSharp;

namespace login_form_technova
{
    public partial class AdminPanel : Window
    {
        private string csvPath = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "opendag_inschrijvingen\\inschijvingen.csv");
        private DataTable dataTable;
        private SeriesCollection seriesCollection = new SeriesCollection();


        public AdminPanel()
        {
            InitializeComponent();
            LoadCsvData();

            string[] headers;
            string[,] dataArray = ReadCsvToArray(csvPath, out headers);

            ProcessEachRow(dataArray);

            


        }

        private void ProcessEachRow(string[,] array)
        {
            int rowCount = array.GetLength(0);
            int colCount = array.GetLength(1);

            Dictionary<string, Dictionary<string, int>> dateToChoiceCounts = new Dictionary<string, Dictionary<string, int>>();

            for (int i = 1; i < rowCount; i++)
            {
                string date = array[i, 3]; 
                string choice = array[i, 5]; 

                if (!string.IsNullOrEmpty(date) && !string.IsNullOrEmpty(choice))
                {
                    if (!dateToChoiceCounts.ContainsKey(date))
                    {
                        dateToChoiceCounts[date] = new Dictionary<string, int>();
                    }

                    if (dateToChoiceCounts[date].ContainsKey(choice))
                    {
                        dateToChoiceCounts[date][choice]++;
                    }
                    else
                    {
                        dateToChoiceCounts[date][choice] = 1;
                    }
                }
            }

            TextBox.Clear();

            foreach (var kvp in dateToChoiceCounts)
            {
                

                TextBox.AppendText($"Date: {kvp.Key}\n");
                foreach (var choiceCount in kvp.Value)
                {
                    TextBox.AppendText($"Choice: {choiceCount.Key}, Count: {choiceCount.Value}\n");
                }
                TextBox.AppendText("\n"); 
            }
        }







        private void LoadCsvData()
        {
            try
            {
                string[] headers;
                string[,] dataArray = ReadCsvToArray(csvPath, out headers);
                dataTable = ConvertArrayToDataTable(dataArray, headers);
                csvData.ItemsSource = dataTable.DefaultView;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading CSV file: " + ex.Message);
            }
        }

        private string[,] ReadCsvToArray(string filePath, out string[] headers)
        {
            string[] lines = File.ReadAllLines(filePath);

            headers = lines[0].Split(',');
            int rowCount = lines.Length;
            int colCount = headers.Length;

            string[,] dataArray = new string[rowCount - 1, colCount];

            for (int i = 1; i < rowCount; i++)
            {
                string[] fields = lines[i].Split(',');
                for (int j = 0; j < colCount; j++)
                {
                    if (j < fields.Length)
                        dataArray[i - 1, j] = fields[j];
                    else
                        dataArray[i - 1, j] = "";
                }
            }
            return dataArray;
        }

        private DataTable ConvertArrayToDataTable(string[,] array, string[] headers)
        {
            DataTable dataTable = new DataTable();
            int rowCount = array.GetLength(0);
            int colCount = array.GetLength(1);

            foreach (string header in headers)
            {
                dataTable.Columns.Add(header);
            }

            for (int i = 0; i < rowCount; i++)
            {
                DataRow newRow = dataTable.NewRow();
                for (int j = 0; j < colCount; j++)
                {
                    newRow[j] = array[i, j];
                }
                dataTable.Rows.Add(newRow);
            }

            return dataTable;
        }

        private void csvData_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            var editedTextBox = e.EditingElement as TextBox;
            var newValue = editedTextBox.Text;

            int rowIndex = e.Row.GetIndex();
            int colIndex = e.Column.DisplayIndex;

            dataTable.Rows[rowIndex][colIndex] = newValue;

            SaveDataTableToCsv(csvPath, dataTable);
        }

        private void SaveDataTableToCsv(string filePath, DataTable dataTable)
        {
            using (StreamWriter writer = new StreamWriter(filePath))
            {
                writer.WriteLine(string.Join(",", dataTable.Columns));

                foreach (DataRow row in dataTable.Rows)
                {
                    var fields = row.ItemArray;
                    writer.WriteLine(string.Join(",", fields));
                }
            }
        }
    }
}
