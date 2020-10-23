using System;
using System.Collections.Generic;
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

using Almacén.Services.Auth;
using MySql.Data.MySqlClient;

namespace Almacén
{
    /// <summary>
    /// Lógica de interacción para AddExit.xaml
    /// </summary>
    public partial class AddExit : Page
    {
        public AddExit()
        {
            InitializeComponent();

            date.SelectedDate = DateTime.Now;

            // Se crea la conexión con la base de datos/servidor
            string credentials = "server=127.0.0.1;user=rivas;password=test123;database=tarea;port=3306;";
            MySqlConnection connection = new MySqlConnection(credentials);

            string products = "SELECT * FROM products";

            try
            {
                // Abre la conexión
                connection.Open();

                // Combobox texto default con usuario loggeado
                user.Text = Auth.user.Name;

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
            // Se crea la conexión con la base de datos/servidor
            string credentials = "server=127.0.0.1;user=rivas;password=test123;database=tarea;port=3306;";
            MySqlConnection connection = new MySqlConnection(credentials);

            if (user.Text == null || product.SelectedValue == null || date.Text == null || quantity.Text == null)
                MessageBox.Show("Todos los campos son requeridos.", "Validación");
            else
            {
                string product_id = (product.SelectedItem as dynamic).Value;

                if (Validate_Stock(quantity.Text))
                {
                    // Consulta SQL para insertar una entrada
                    string sql = "INSERT INTO exits" +
                    "(date, quantity, user_id, product_id)" +
                    $"VALUES(STR_TO_DATE('{date.Text}', '%d/%m/%Y'), '{quantity.Text}', {Auth.user.Id}, {(product.SelectedItem as dynamic).Value});";

                    Console.WriteLine(sql);

                    try
                    {
                        // Abre la conexión
                        connection.Open();

                        // Ejecuta la insersión
                        MySqlCommand cmd = new MySqlCommand(sql, connection);
                        cmd.ExecuteNonQuery();

                        // Remover stock
                        RemoveFromStock(connection, product_id, quantity.Text);

                        var mainWin = Application.Current.Windows.Cast<Window>().FirstOrDefault(window => window is MainWindow) as MainWindow;
                        connection.Close();

                        MessageBox.Show("Salida cargada correctamente.", "Carga Exitosa");

                        // Redirección al index de salidas
                        mainWin.page_frame.Content = new Exits();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Exception Handler: {ex}");

                        MessageBox.Show("Error de carga.", "Carga Fallida");
                    }
                }

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

        // Obtiene el stock de un producto seleccionado
        private void GetStock(object sender, SelectionChangedEventArgs e)
        {
            // Se crea la conexión con la base de datos/servidor
            string credentials = "server=127.0.0.1;user=rivas;password=test123;database=tarea;port=3306;";
            MySqlConnection connection = new MySqlConnection(credentials);

            string product_id = (product.SelectedItem as dynamic).Value;

            // Consulta SQL para buscar el stock del producto seleccionado
            string sql = $"SELECT stock FROM products WHERE id = {product_id}";
            string stock = "";

            Console.WriteLine(sql);

            // Se solicita el stock del producto seleccionado
            try
            {
                // Abre la conexión
                connection.Open();

                MySqlCommand cmd = new MySqlCommand(sql, connection);
                MySqlDataReader sqlReader = cmd.ExecuteReader();

                while (sqlReader.Read())
                {
                    stock = sqlReader["stock"].ToString();
                }

                sqlReader.Close();
                connection.Open();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception Handler: {ex}");
                connection.Open();
            }

            stock_ind.Text = stock;
        }

        // Valida la cantidad de salida con stock disponible
        private bool Validate_Stock(string quantity)
        {
            if (Convert.ToInt32(quantity) > Convert.ToInt32(stock_ind.Text))
            {
                MessageBox.Show("Excede el stock.", "Mensaje");
                return false;
            }
            else if (Convert.ToInt32(quantity) == 0)
            {
                MessageBox.Show("Cantidad inválida.", "Mensaje");
                return false;
            }

            return true;
        }

        // Reducir la cantidad de la entrada al stock del producto
        public void RemoveFromStock(MySqlConnection connection, string product_id, string quantity)
        {
            string sql = $"UPDATE products SET stock = stock - {quantity} WHERE id = {product_id}";

            Console.WriteLine(sql);

            MySqlCommand cmd = new MySqlCommand(sql, connection);
            cmd.ExecuteNonQuery();
        }
    }
}
