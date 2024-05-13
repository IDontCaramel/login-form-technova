using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static System.Net.Mime.MediaTypeNames;


namespace login_form_technova
{
  
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            txtName = InputName;
            txtPhone = InputPhone;
            txtEmail = InputMail;


            userDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            newDirectoryPath = System.IO.Path.Combine(userDirectory, "opendag_inschrijvingen");
            

        }


        private TextBox txtName;
        private TextBox txtPhone;
        private TextBox txtEmail;   
        private string Chosendate;
        private string ChosenOpleiding;
        private string userDirectory;
        private string newDirectoryPath;
        private string pwHash = "240be518fabd2724ddb6f04eeb1da5967448d7e831c08c8fa822809f74c720a9";
        private int validInputsCount = 0;

        private void InputName_TextChanged(object sender, TextChangedEventArgs e)
        {
            txtName = (TextBox)sender;  
        }

        private void InputMail_TextChanged(object sender, TextChangedEventArgs e)
        {
            txtEmail = (TextBox)sender;
        }

        private void InputPhone_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            foreach (char c in e.Text)
            {
                if (!char.IsDigit(c))
                {
                    e.Handled = true; 
                    return;
                }
            }
        }

        private void InputPhone_lostfocus(object sender, RoutedEventArgs e)
        {
            txtPhone = (TextBox)sender;

            string phoneNumber = txtPhone.Text;

            txtPhone.Text = FormatPhoneNumber(phoneNumber);
        }

        private string FormatPhoneNumber(string phoneNumber)
        {
            if (phoneNumber == null)
                return string.Empty;

            switch (phoneNumber.Length)
            {
                case 10:
                    return Regex.Replace(phoneNumber, @"(\d{2})(\d{4})(\d{4})", "($1)-$2-$3");
                case 9:
                    return Regex.Replace(phoneNumber, @"(\d{3})(\d{3})(\d{3})", "($1)-$2-$3");
                case 11:
                    return Regex.Replace(phoneNumber, @"(\d{3})(\d{4})(\d{4})", "($1)-$2-$3");
                default:
                    return phoneNumber;
            }
        }

        private int ExtraPeople()
        {
            if (_1extra.IsChecked == true)
            {
                return 1;
            }
            else if (_2extra.IsChecked == true)
            {
                return 2;
            }
            
            else 
            { 
                return 0;
            }
        }

        private void cbDate_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox comboBox = sender as ComboBox;
            if (comboBox != null && comboBox.SelectedItem != null)
            {
                ComboBoxItem selectedItem = comboBox.SelectedItem as ComboBoxItem;
                if (selectedItem != null)
                {
                    Chosendate = selectedItem.Content.ToString();
                }
            }
        }


        private void cbOpleiding_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox comboBox = sender as ComboBox;
            if (comboBox != null && comboBox.SelectedItem != null)
            {
                ComboBoxItem selectedItem = comboBox.SelectedItem as ComboBoxItem;
                if (selectedItem != null)
                {
                    ChosenOpleiding = selectedItem.Content.ToString();
                }
            }
        }


        private void btnSend_Click(object sender, RoutedEventArgs e)
        {
            if (CheckInputs() == true)
            {
                SaveResults();
                MessageBox.Show("succes");

            }
            else
            {
                MessageBox.Show("ful al de velden in");
            }

        }



        private bool CheckInputs()
        {
            validInputsCount = 0;
            bool isValidEmail = IsValidEmail(txtEmail.Text);

            SetLabelColor(txtName.Text, lblName);
            SetLabelColor(txtEmail.Text, lblMail, isValidEmail);
            SetLabelColor(txtPhone.Text, lblPhone);
            SetLabelColor(cbDate.SelectedIndex, lblDate);
            SetLabelColor(cbOpleiding.SelectedIndex, lblOpleiding);

            if (validInputsCount >= 5)
            {
                validInputsCount = 0;
                return true;
            }
            else
            {
                validInputsCount = 0;
                return false;
            }
        }

        private void SetLabelColor(string input, Label label, bool isValidEmail = true)
        {
            if (string.IsNullOrWhiteSpace(input) || (label != null && !isValidEmail))
            {
                label.Foreground = new SolidColorBrush(Colors.Red);
            }
            else
            {
                label.Foreground = new SolidColorBrush(Colors.White);
                validInputsCount++;
            }
        }

        private void SetLabelColor(int selectedIndex, Label label)
        {
            if (selectedIndex == 0 && label != null)
            {
                label.Foreground = new SolidColorBrush(Colors.Red);
            }
            else
            {
                label.Foreground = new SolidColorBrush(Colors.White);
                validInputsCount++;
            }
        }

        private bool IsValidEmail(string email)
        {
            var trimmedEmail = email.Trim();
            if (trimmedEmail.EndsWith("."))
            {
                return false;
            }
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == trimmedEmail;
            }
            catch
            {
                return false;
            }
        }



        private bool SaveResults()
        {
            string csv = System.IO.Path.Combine(newDirectoryPath, "inschijvingen.csv");

            try
            {
                if (!Directory.Exists(newDirectoryPath))
                {
                    Directory.CreateDirectory(newDirectoryPath);
                }

                if (!File.Exists(csv))
                {
                    using (StreamWriter headerWriter = new StreamWriter(csv))
                    {
                        headerWriter.WriteLine("Name,Email,Phone,Date,Extra,Opleiding");
                    }
                }

                using (StreamWriter sw = File.AppendText(csv))
                {
                    string name = txtName.Text;
                    string email = txtEmail.Text;
                    string phone = txtPhone.Text;
                    string date = Chosendate;
                    string extra = ExtraPeople().ToString();
                    string Opleiding = ChosenOpleiding;

                    string newLine = $"{name},{email},{phone},{date},{extra},{Opleiding}";
                    sw.WriteLine(newLine);
                }


                return true; 
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
                return false;
            }
        }

        private void btnAdmin_Click(object sender, RoutedEventArgs e)
        {
            InputBox inputBox = new InputBox();
            bool? result = inputBox.ShowDialog();

            if (result == true)
            {
                string userInput = inputBox.UserInput;
                if (Hash(userInput) == true) 
                {
                    AdminPanel adminPanel = new AdminPanel();
                    adminPanel.Show();
                }
                else
                {
                    MessageBox.Show("wrong password");
                }
            }
        }

        private bool Hash(string hash)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(hash));

                string hashString = BitConverter.ToString(hashBytes).Replace("-", "").ToLower();

                if (hashString == pwHash)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        private void InputName_TextChanged_1(object sender, TextChangedEventArgs e)
        {

        }
    }
}
