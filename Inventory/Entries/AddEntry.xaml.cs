using System;
using System.Collections.Generic;
using System.Linq;
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

using MySql.Data.MySqlClient;

namespace Almacén
{
    /// <summary>
    /// Lógica de interacción para AddEntry.xaml
    /// </summary>
    public partial class AddEntry : Page
    {
        public AddEntry()
        {
            InitializeComponent();

            date.SelectedDate = DateTime.Now;

            // Se crea la conexión con la base de datos/servidor
            string credentials = "server=127.0.0.1;user=rivas;password=test123;database=tarea;port=3306;";
            MySqlConnection connection = new MySqlConnection(credentials);

            string providers = "SELECT * FROM providers";
            string products = "SELECT * FROM products";

            try
            {
                // Abre la conexión
                connection.Open();

                // Llenar el ComboBox con la Tabla de Proveedores
                InsertComboBox(providers, connection, provider);

                // Llenar el ComboBox con la Tabla de Productos
                InsertComboBox(products, connection, product);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception Handler: {ex}");
            }

            connection.Close();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (decimal.TryParse(cost_price.Text, out decimal price))
            {
                // Se crea la conexión con la base de datos/servidor
                string credentials = "server=127.0.0.1;user=rivas;password=test123;database=tarea;port=3306;";
                MySqlConnection connection = new MySqlConnection(credentials);

                if (provider.SelectedValue == null || product.SelectedValue == null || date.Text == null || quantity.Text == null)
                    MessageBox.Show("Todos los campos son requeridos.", "Validación");
                else if (quantity.Text == "0")
                    MessageBox.Show("Cantidad inválida.", "Validación");
                else
                {
                    // Consulta SQL para insertar una entrada
                    string sql = "INSERT INTO entries" +
                    "(date, quantity, cost_price, provider_id, product_id)" +
                    $"VALUES(STR_TO_DATE('{date.Text}', '%d/%m/%Y'), '{quantity.Text}', '{price}', {(provider.SelectedItem as dynamic).Value}, {(product.SelectedItem as dynamic).Value});";

                    Console.WriteLine(sql);

                    try
                    {
                        // Abre la conexión
                        connection.Open();

                        // Ejecuta la insersión
                        MySqlCommand cmd = new MySqlCommand(sql, connection);
                        cmd.ExecuteNonQuery();

                        // Añadir stock
                        string product_id = (product.SelectedItem as dynamic).Value;
                        AddToStock(connection, product_id, quantity.Text);

                        var mainWin = Application.Current.Windows.Cast<Window>().FirstOrDefault(window => window is MainWindow) as MainWindow;
                        connection.Close();

                        MessageBox.Show("Entrada cargada correctamente.", "Carga Exitosa");

                        // Redirección al index de entradas
                        mainWin.page_frame.Content = new Entries();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Exception Handler: {ex}");

                        MessageBox.Show("Error de carga.", "Carga Fallida");
                    }
                }
            }
            else
            {
                MessageBox.Show("Rango de Precio Invalido.", "Error");
            }
        }

        // Lee una consulta y lo que retorne se inserta en el combobox
        public void InsertComboBox(string sql, MySqlConnection connection, ComboBox comboBox)
        {
            MySqlCommand cmd = new MySqlCommand(sql, connection);
            MySqlDataReader sqlReader = cmd.ExecuteReader();

            comboBox.DisplayMemberPath = "Text";
            comboBox.SelectedValuePath = "Value";

            while (sqlReader.Read())
            {
                comboBox.Items.Add(new { Text = sqlReader["name"].ToString(), Value = sqlReader["id"].ToString() });
            }

            sqlReader.Close();
        }

        // Añade la cantidad de la entrada al stock del producto
        public void AddToStock(MySqlConnection connection, string product_id, string quantity)
        {
            string sql = $"UPDATE products SET stock = stock + {quantity} WHERE id = {product_id}";

            Console.WriteLine(sql);

            MySqlCommand cmd = new MySqlCommand(sql, connection);
            cmd.ExecuteNonQuery();
        }
    }
}
