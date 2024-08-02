using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
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
using MySql.Data.MySqlClient;
namespace leoCalculator
{
    /// <summary>
    /// MainWindow.xaml 的互動邏輯
    /// </summary>
    public partial class MainWindow : Window
    {
        double op1, op2;
        bool flag1 = false, flag2 = false, flag3 = false, flag4 = false;
        private string connectionString = "server=127.0.0.1;database=calculator_data;user=root;"; // 原本卡關的:看建立的database的權限的user名字是誰 user指派對才能成功insert
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)sender;
            中序顯示.Text += btn.Content;
            if (Convert.ToChar(btn.Content) == '+') flag1 = true;
            else if (Convert.ToChar(btn.Content) == '-') flag2 = true;
            else if (Convert.ToChar(btn.Content) == '*') flag3 = true;
            else if (Convert.ToChar(btn.Content) == '/') flag4 = true;
        }
        private void insert(object sender, RoutedEventArgs e)
        {
            string infix = 中序顯示.Text;
            if(Preorder.Content == null || Postorder.Content == null || Decimal.Content == null || Binary.Content == null)
            {
                MessageBox.Show($"錯誤的中序顯示");
                return;
            }
            string prefix = Preorder.Content.ToString();
            string postfix = Postorder.Content.ToString();
            string dec = Decimal.Content.ToString();
            string bin = Binary.Content.ToString();

            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();
                    string checkQuery = "SELECT COUNT(*) FROM calculatordata WHERE Infix = @infix AND Prefix = @prefix AND Postfix = @postfix AND `Dec` = @dec AND Bin = @bin";
                    using (MySqlCommand checkCmd = new MySqlCommand(checkQuery, connection))
                    {
                        checkCmd.Parameters.AddWithValue("@infix", infix);
                        checkCmd.Parameters.AddWithValue("@prefix", prefix);
                        checkCmd.Parameters.AddWithValue("@postfix", postfix);
                        checkCmd.Parameters.AddWithValue("@dec", dec);
                        checkCmd.Parameters.AddWithValue("@bin", bin);

                        int count = Convert.ToInt32(checkCmd.ExecuteScalar());
                        if (count > 0)
                        {
                            MessageBox.Show("資料已存在!");
                            return;
                        }
                    }

                    string query = "INSERT INTO calculatordata (Infix, Prefix, Postfix, `Dec`, Bin) VALUES (@infix, @prefix, @postfix, @dec, @bin)";
                    MySqlCommand cmd = new MySqlCommand(query, connection);
                    cmd.Parameters.AddWithValue("@Infix", infix);
                    cmd.Parameters.AddWithValue("@Prefix", prefix);
                    cmd.Parameters.AddWithValue("@Postfix", postfix);
                    cmd.Parameters.AddWithValue("@Dec", dec);
                    cmd.Parameters.AddWithValue("@Bin", bin);
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error inserting data: {ex.Message}");
            }
        }
        private void query(object sender, RoutedEventArgs e)
        {
            Window1 window1 = new Window1();
            window1.ShowDialog();
        }

        private void equal(object sender, RoutedEventArgs e)
        {
            string a = 中序顯示.Text;
            if (flag1)
            {
                char target1 = '+';
                Preorder.Content += Convert.ToString(target1);
                foreach (char c in a)
                {
                    if (c != target1) Preorder.Content += Convert.ToString(c);
                }

                foreach (char c in a)
                {
                    if (c != target1) Postorder.Content += Convert.ToString(c);
                }
                Postorder.Content += Convert.ToString(target1);

                int index1 = a.IndexOf(target1);
                op1 = Convert.ToDouble(a.Substring(0, index1));
                op2 = Convert.ToDouble(a.Substring(index1 + 1));
                double ans = op1 + op2;
                Decimal.Content = ans;
                long integerPart = (long)Math.Truncate(ans);
                string binary = Convert.ToString(integerPart, 2);
                Binary.Content = binary;
            }

            else if (flag2)
            {
                char target2 = '-';
                Preorder.Content += Convert.ToString(target2);
                foreach (char c in a)
                {
                    if (c != target2) Preorder.Content += Convert.ToString(c);
                }

                foreach (char c in a)
                {
                    if (c != target2) Postorder.Content += Convert.ToString(c);
                }
                Postorder.Content += Convert.ToString(target2);

                int index2 = a.IndexOf(target2);
                op1 = Convert.ToDouble(a.Substring(0, index2));
                op2 = Convert.ToDouble(a.Substring(index2 + 1));
                double ans = op1 - op2;
                Decimal.Content = ans;
                long integerPart = (long)Math.Truncate(ans);
                string binary = Convert.ToString(integerPart, 2);
                Binary.Content = binary;
            }

            else if (flag3)
            {
                char target3 = '*';
                Preorder.Content += Convert.ToString(target3);
                foreach (char c in a)
                {
                    if (c != target3) Preorder.Content += Convert.ToString(c);
                }

                foreach (char c in a)
                {
                    if (c != target3) Postorder.Content += Convert.ToString(c);
                }
                Postorder.Content += Convert.ToString(target3);

                int index3 = a.IndexOf(target3);
                op1 = Convert.ToDouble(a.Substring(0, index3));
                op2 = Convert.ToDouble(a.Substring(index3 + 1));
                double ans = op1 * op2;
                Decimal.Content = ans;
                long integerPart = (long)Math.Truncate(ans);
                string binary = Convert.ToString(integerPart, 2);
                Binary.Content = binary;
            }

            else if (flag4)
            {
                char target4 = '/';
                int index4 = a.IndexOf(target4);
                op1 = Convert.ToDouble(a.Substring(0, index4));
                op2 = Convert.ToDouble(a.Substring(index4 + 1));
                if (op2 == 0)
                {
                    中序顯示.Clear();
                    ResetFlags();
                    return;
                }
                Preorder.Content += Convert.ToString(target4);
                foreach (char c in a)
                {
                    if (c != target4) Preorder.Content += Convert.ToString(c);
                }

                foreach (char c in a)
                {
                    if (c != target4) Postorder.Content += Convert.ToString(c);
                }
                Postorder.Content += Convert.ToString(target4);

                double ans = op1 / op2;
                Decimal.Content = ans;
                long integerPart = (long)Math.Truncate(ans);
                string binary = Convert.ToString(integerPart, 2);
                Binary.Content = binary;
            }
            ResetFlags();
        }

        private void Clear(object sender, RoutedEventArgs e)
        {
            中序顯示.Clear();
            Preorder.Content = "";
            Postorder.Content = "";
            Decimal.Content = "";
            Binary.Content = "";
            ResetFlags();
        }
        private void ResetFlags()
        {
            flag1 = flag2 = flag3 = flag4 = false;
        }

    }
}
