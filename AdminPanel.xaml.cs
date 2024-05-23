using OxyPlot.Series;
using OxyPlot;
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
using static login_form_technova.MainWindow;
using Microsoft.Win32;
using System.ComponentModel;
using AddData;
using System.Collections.ObjectModel;

namespace login_form_technova
{
    public partial class AdminPanel : Window
    {
        public string csvPath = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "opendag_inschrijvingen\\inschijvingen.csv");
        private DataTable dataTable;


        public List<string> Admin_Dates { get; set; } = StaticData.Dates;
        public List<string> Admin_Opleindingen { get; set; } = StaticData.Opleidingen;

        public ObservableCollection<ChartData> ChartDataCollection { get; set; }

        public AdminPanel()
        {
            InitializeComponent();
            LoadCsvData(csvPath);

            //string[] headers;
            //string[,] dataArray = ReadCsvToArray(csvPath, out headers);

            //ProcessEachRow(dataArray);


            CreateDate();
            CreateOpleiding();


            ChartDataCollection = new ObservableCollection<ChartData>();

            var categoryA = new ChartData("Category A");
            categoryA.AddSeriesData("Series 1", 25);
            categoryA.AddSeriesData("Series 2", 137);
            categoryA.AddSeriesData("Series 3", 18);
            categoryA.AddSeriesData("Series 4", 40);

            var categoryB = new ChartData("Category B");
            categoryB.AddSeriesData("Series 1", 12);
            categoryB.AddSeriesData("Series 2", 14);
            categoryB.AddSeriesData("Series 3", 120);
            categoryB.AddSeriesData("Series 4", 26);

            ChartDataCollection.Add(categoryA);
            ChartDataCollection.Add(categoryB);
        }




        private void CreateDate()
        {
            if (cbDate == null)
            {
                throw new InvalidOperationException("ComboBox 'cbDate' is not initialized.");
            }

            if (Admin_Dates == null)
            {
                throw new ArgumentNullException(nameof(Admin_Dates), "Input dates array is null.");
            }

            foreach (string date in Admin_Dates)
            {

                cbDate.Items.Add(date);
            }
            cbDate.SelectedIndex = 0;

        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            if (dp.SelectedDate != null)
            {
                string selectedDate = dp.SelectedDate.Value.ToString("dd-MM-yyyy");
                if (!Admin_Dates.Contains(selectedDate))
                {
                    cbDate.Items.Add(selectedDate);
                    Admin_Dates.Add(selectedDate);
                    cbDate.SelectedIndex = cbDate.Items.Count - 1;
                }
            }

        }

        private void btnRemove_Click(object sender, RoutedEventArgs e)
        {
            if (cbDate.SelectedIndex != 0 && cbDate.SelectedIndex < Admin_Dates.Count)
            {
                Admin_Dates.RemoveAt(cbDate.SelectedIndex);
                cbDate.Items.RemoveAt(cbDate.SelectedIndex);
                cbDate.SelectedIndex = 0;
            }
        }

        public void CreateOpleiding()
        {
            if (cbDate == null)
            {
                throw new InvalidOperationException("ComboBox 'cbDate' is not initialized.");
            }

            if (Admin_Opleindingen == null)
            {
                throw new ArgumentNullException(nameof(Admin_Opleindingen), "Input dates array is null.");
            }

            foreach (string opleiding in Admin_Opleindingen)
            {

                cbOpleiding.Items.Add(opleiding);
            }
            cbOpleiding.SelectedIndex = 0;
        }

        private void btnAddOP_Click(object sender, RoutedEventArgs e)
        {
            if (inputOpleiding.Text.Trim() != "")
            {
                string Opleiding = inputOpleiding.Text.Trim();
                if (!Admin_Opleindingen.Contains(Opleiding))
                {
                    cbOpleiding.Items.Add(Opleiding);
                    Admin_Opleindingen.Add(Opleiding);
                    cbOpleiding.SelectedIndex = cbOpleiding.Items.Count - 1;

                }
            }
        }

        private void btnRemoveOP_Click(object sender, RoutedEventArgs e)
        {
            if (cbOpleiding.SelectedIndex != 0 && cbOpleiding.SelectedIndex < Admin_Opleindingen.Count)
            {
                Admin_Opleindingen.RemoveAt(cbOpleiding.SelectedIndex);
                cbOpleiding.Items.RemoveAt(cbOpleiding.SelectedIndex);
                cbOpleiding.SelectedIndex = 0;
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            StaticData.Dates = Admin_Dates;
            StaticData.Opleidingen = Admin_Opleindingen;    
        }

        /*private void ProcessEachRow(string[,] array)
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
        }*/







        private void LoadCsvData(string PathCsv)
        {
            
            try
            {
                string[] headers;
                string[,] dataArray = ReadCsvToArray(PathCsv, out headers);
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

            if (rowIndex >= 0 && rowIndex < dataTable.Rows.Count)
            {
                dataTable.Rows[rowIndex][colIndex] = newValue;

                SaveDataTableToCsv(csvPath, dataTable);
            }
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

        private void btnDownload_Click(object sender, RoutedEventArgs e)
        {

            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "All Files (*.*)|*.*";
            saveFileDialog.FileName = "inschijvingen.csv"; 

            bool? result = saveFileDialog.ShowDialog();

            if (result == true)
            {
                string destinationFilePath = saveFileDialog.FileName;

                try
                {
                    File.Copy(csvPath, destinationFilePath, true);

                    MessageBox.Show("File downloaded successfully!", "Download Complete", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error downloading file: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void btnImport_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();

            openFileDialog.Title = "Open File";
            openFileDialog.Filter = "CSV files (*.csv)|*.csv|All files (*.*)|*.*";

            if (openFileDialog.ShowDialog() == true)
            {
                string filePath = openFileDialog.FileName;
                try
                {
                    LoadCsvData(filePath);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"There was an error while trying to load your CSV file: {ex.Message}");
                }
            }
        }

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
