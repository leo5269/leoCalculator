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
using System.Windows.Shapes;
using MySql.Data.MySqlClient;
namespace leoCalculator
{
    /// <summary>
    /// Window1.xaml 的互動邏輯
    /// </summary>
    public partial class Window1 : Window
    {
        private DataTable dataTable;
        private string connectionString = "server=127.0.0.1;database=calculator_data;user=root;";
        public Window1()
        {
            InitializeComponent();
        }

        private void Show_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();
                    string query = "SELECT Id, Infix, Prefix, Postfix, `Dec`, Bin FROM calculatordata";
                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            Result.Items.Clear();
                            while (reader.Read())
                            {
                                var item = $"Id: {reader["Id"]}, Infix: {reader["Infix"]}, Prefix: {reader["Prefix"]}, Postfix: {reader["Postfix"]}, Dec: {reader["Dec"]}, Bin: {reader["Bin"]}";
                                Result.Items.Add(item);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"找不到資料: {ex.Message}");
            }
        }

        private void Del_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var selectedItem = Result.SelectedItem as string;

                if (string.IsNullOrWhiteSpace(selectedItem)) // 如果沒選擇就會跑出要使用者選擇的訊息
                {
                    MessageBox.Show("Please select a record to delete.");
                    return;
                }

                int idToDelete = ExtractIdFromSelection(selectedItem); // 下面的function來找要del的data

                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();
                    string query = "DELETE FROM calculatordata WHERE Id = @id";
                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@id", idToDelete);
                        int rowsAffected = command.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("資料成功刪除!");
                            Show_Click(sender, e); // listbox的data 再次展示
                        }
                        else
                        {
                            MessageBox.Show("無資料.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"刪除資料失敗: {ex.Message}");
            }
        }
 
        private int ExtractIdFromSelection(string selection)
        {
            int id;
            int.TryParse(selection.Split(new[] { "Id: " }, StringSplitOptions.None)[1].Split(',')[0].Trim(), out id); //從selection中找到ID，並把ID轉成數字
            return id;
        }
    }
}
