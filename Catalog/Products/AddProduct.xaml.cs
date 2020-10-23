using System;
using System.Collections.Generic;
using System.Data;
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
    /// Lógica de interacción para AddProduct.xaml
    /// </summary>
    public partial class AddProduct : Page
    {
        public AddProduct()
        {
            InitializeComponent();

            // Se crea la conexión con la base de datos/servidor
            string credentials = "server=127.0.0.1;user=rivas;password=test123;database=tarea;port=3306;";
            MySqlConnection connection = new MySqlConnection(credentials);

            string brands = "SELECT * FROM brands WHERE deleted_at IS NULL";
            string models = "SELECT * FROM models WHERE deleted_at IS NULL";

            try
            {
                // Abre la conexión
                connection.Open();

                // Llenar el ComboBox con la Tabla de Marcas
                InsertComboBox(brands, connection, brand_filter);

                // Llenar el ComboBox con la Tabla de Modelos
                InsertComboBox(models, connection, model_filter);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception Handler: {ex}");
            }

            connection.Close();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (decimal.TryParse(web_price.Text, out decimal price))
            {
                // Se crea la conexión con la base de datos/servidor
                string credentials = "server=127.0.0.1;user=rivas;password=test123;database=tarea;port=3306;";
                MySqlConnection connection = new MySqlConnection(credentials);

                if (brand_filter.SelectedValue == null || model_filter.SelectedValue == null || sku.Text == null || name.Text == null || description.Text == null || web_price == null)
                    MessageBox.Show("Todos los campos son requeridos.", "Validación");
                else
                {
                    // Consulta SQL para insertar un producto
                    string sql = "INSERT INTO products" +
                    "(sku, name, description, web_price, brand_id, model_id)" +
                    $"VALUES('{sku.Text}', '{name.Text}', '{description.Text}', '{price}', {(brand_filter.SelectedItem as dynamic).Value}, {(model_filter.SelectedItem as dynamic).Value});";

                    Console.WriteLine(sql);

                    try
                    {
                        // Abre la conexión
                        connection.Open();

                        // Ejecuta la insersión
                        MySqlCommand cmd = new MySqlCommand(sql, connection);
                        cmd.ExecuteNonQuery();

                        var mainWin = Application.Current.Windows.Cast<Window>().FirstOrDefault(window => window is MainWindow) as MainWindow;
                        connection.Close();

                        MessageBox.Show("Producto cargado correctamente.", "Carga Exitosa");

                        // Redirección al index de productos
                        mainWin.page_frame.Content = new Products();
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
                MessageBox.Show("Precio Invalido.", "Error");
            }
        }

        // Lee una consulta y lo que retorne se inserta en el combobox
        public void InsertComboBox(string sql, MySqlConnection connection, ComboBox comboBox)
        {
            MySqlCommand cmd = new MySqlCommand(sql, connection);
            MySqlDataReader sqlReader = cmd.ExecuteReader();

            comboBox.DisplayMemberPath= "Text";
            comboBox.SelectedValuePath = "Value";

            while (sqlReader.Read())
            {
                comboBox.Items.Add(new { Text = sqlReader["name"].ToString(), Value = sqlReader["id"].ToString() });
            }

            sqlReader.Close();
        }
    }
}
