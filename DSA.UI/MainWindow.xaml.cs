using System;
using Microsoft.Win32;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using DSA.Algorithm;
using HashAlgorithms;

namespace DSA.UI
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void StartButtonOnClick(object sender, RoutedEventArgs e)
        {
            ClearFields();
            if (SignRadioButton.IsChecked == true)
            {
                Sign();
            }
            else
            {
                Check();
            }
        }

        private void Sign()
        {
            if (!IsFieldsFilled("sign"))
            {
                return;
            }

            if (!IsFieldsFilledCorrectly("sign"))
            {
                return;
            }

            var q = BigInteger.Parse(QTextBox.Text);
            var p = BigInteger.Parse(PTextBox.Text);
            var k = BigInteger.Parse(KTextBox.Text);
            var x = BigInteger.Parse(XTextBox.Text);
            var h = BigInteger.Parse(HTextBox.Text);
            string msg = File.ReadAllText(FilePathTextBox.Text);
            byte[] initialMsg = Encoding.ASCII.GetBytes(msg);
            
            if (Signer.Sign(initialMsg, q, p, k, x, h , out BigInteger r, out BigInteger s))
            {
                HashTextBox.Text = SHA1.GetHash(initialMsg).ToString();
                RTextBox.Text = r.ToString();
                STextBox.Text = s.ToString();
                string file = FilePathTextBox.Text;
                string extension = file.Substring(file.LastIndexOf('.'), file.Length - file.LastIndexOf('.'));
                string filePath = file.Substring(0, file.LastIndexOf('.'));
                filePath = filePath + "_signed" + extension;
                File.WriteAllBytes(filePath, initialMsg);
                File.AppendAllText(filePath, "," + r + "," + s);
            }
            else
            {
                MessageBox.Show("R or S value is 0. Please, rewrite k value.", "Error with input values", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Check()
        {
            if (!IsFieldsFilled("check"))
            {
                return;
            }

            if (!IsFieldsFilledCorrectly("check"))
            {
                return;
            }

            var q = BigInteger.Parse(QTextBox.Text);
            var p = BigInteger.Parse(PTextBox.Text);
            var x = BigInteger.Parse(XTextBox.Text);
            var h = BigInteger.Parse(HTextBox.Text);

            string msg = File.ReadAllText(FilePathTextBox.Text);

            BigInteger r;
            BigInteger s;
            try
            {
                msg = TrimSignature(msg, out r, out s);
            }
            catch (ArgumentException ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            byte[] initialMsg = Encoding.ASCII.GetBytes(msg);

            HashTextBox.Text = SHA1.GetHash(initialMsg).ToString();
            RTextBox.Text = r.ToString();
            STextBox.Text = s.ToString();

            string result;
            MessageBoxImage image;
            if (Signer.CheckSign(initialMsg, r, s, q, p, h, x, out BigInteger v))
            {
                result = "EQUAL";
                image = MessageBoxImage.Information;
            }
            else
            {
                result = "NOT EQUAL";
                image = MessageBoxImage.Error;
            }

            MessageBox.Show($"Checking result:\nr = {r}\nv = {v}\nResult is {result}", "Conclusion",
                            MessageBoxButton.OK, image);
        }

        private string TrimSignature(string msg, out BigInteger r, out BigInteger s)
        {
            r = 0;
            s = 0;
            string rStr = "";
            string sStr = "";

            int commaCount = 0;
            int index = msg.Length - 1;
            while(index > 0 && commaCount < 2)
            {
                if (commaCount == 0)
                {
                    sStr = msg[index] + sStr;
                    index--;
                    if (msg[index] == ',')
                    {
                        commaCount++;
                        index--;
                    }
                }

                if (commaCount == 1)
                {
                    rStr = msg[index] + rStr;
                    index--;
                    if (msg[index] == ',')
                    {
                        commaCount++;
                    }
                }
            }

            if (commaCount < 2 || !BigInteger.TryParse(rStr, out r) || !BigInteger.TryParse(sStr, out s))
            {
                throw new ArgumentException("Signature was not found.");
            }

            return msg.Substring(0, index);
        }

        private void OpenFileButtonOnClick(object sender, RoutedEventArgs e)
        {
            ClearFields();

            var openFileDialog = new OpenFileDialog
            {
                InitialDirectory = "D:\\Test"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                FilePathTextBox.Text = openFileDialog.FileName;
            }
        }

        private bool IsFieldsFilled(string action)
        {
            if (QTextBox.Text == "" || PTextBox.Text == "" || HTextBox.Text == "" ||
                XTextBox.Text == "" || FilePathTextBox.Text == "")
            {
                MessageBox.Show("Please, fill input fields!", "Error with input values",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            if (action == "sign" && KTextBox.Text == "")
            {
                MessageBox.Show("Please, fill all input fields!", "Error with input values",
                                MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            return true;
        }

        private bool IsFieldsFilledCorrectly(string action)
        {
            bool isValid = true;
            if (!Checker.FermaPrimeTest(BigInteger.Parse(QTextBox.Text)))
            {
                MessageBox.Show("Q value must be prime.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                isValid = false;
            }

            if (!Checker.CheckP(QTextBox.Text, PTextBox.Text))
            {
                MessageBox.Show("P value must be prime and (p - 1) mod q = 0", 
                                "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                isValid = false;
            }

            if (!Checker.CheckH(QTextBox.Text, PTextBox.Text, HTextBox.Text))
            {
                MessageBox.Show("H value must be in (1; p - 1) prime and h ^ ((p - 1) / q) mod p > 1",
                                "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                isValid = false;
            }

            if (!Checker.CheckIsInInterval("0", QTextBox.Text, XTextBox.Text))
            {
                MessageBox.Show("X value must be in (0; q)", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                isValid = false;
            }

            if (action == "sign" && !Checker.CheckIsInInterval("0", QTextBox.Text, KTextBox.Text))
            {
                MessageBox.Show("K value must be in (0; q)", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                isValid = false;
            }

            if (!File.Exists(FilePathTextBox.Text))
            {
                MessageBox.Show("File with current file path does not exist.", "Error", 
                                MessageBoxButton.OK, MessageBoxImage.Error);
                isValid = false;
            }

            return isValid;
        }

        private void TextBoxOnTextChanged(object sender, TextChangedEventArgs e)
        {
            if (sender is TextBox textBox)
            {
                textBox.Text = new string
                (
                    textBox.Text
                           .Where
                            (ch =>
                                ch == '0' || ch == '1' || ch == '2' || ch == '3' ||
                                ch == '4' || ch == '5' || ch == '6' || ch == '7' ||
                                ch == '8' || ch == '9'
                            )
                           .ToArray()
                );
                textBox.SelectionStart = textBox.Text.Length;
                textBox.SelectionLength = 0;
            }
        }

        private void SignRadioButton_OnClick(object sender, RoutedEventArgs e)
        {
            KTextBox.IsEnabled = true;
        }

        private void CheckRadioButton_OnClick(object sender, RoutedEventArgs e)
        {
            KTextBox.IsEnabled = false;
        }

        private void ClearFields()
        {
            RTextBox.Clear();
            STextBox.Clear();
            HashTextBox.Clear();
        }
    }
}
