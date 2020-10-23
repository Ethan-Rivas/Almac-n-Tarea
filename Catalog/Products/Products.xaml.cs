using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
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

namespace Almacén
{
    /// <summary>
    /// Lógica de interacción para Products.xaml
    /// </summary>
    public partial class Products : Page
    {

        public Products()
        {
            InitializeComponent();

            // Se crea la conexión con la base de datos/servidor
            string credentials = "server=127.0.0.1;user=rivas;password=test123;database=tarea;port=3306;";
            MySqlConnection connection = new MySqlConnection(credentials);

            // Consulta SQL con relaciones
            string products = "SELECT " +
                         "products.id AS ID, " +
                         "products.sku AS SKU, " +
                         "products.name AS Nombre, " +
                         "products.description AS Descripción, " +
                         "products.web_price AS 'Precio Venta', " +
                         "products.stock AS Stock, " +
                         "brands.name AS Marca, " +
                         "models.name AS Modelo FROM products " +
                         "JOIN brands ON products.brand_id = brands.id " +
                         "JOIN models ON products.model_id = models.id " +
                         "WHERE products.deleted_at IS NULL";

            string brands = "SELECT * FROM brands";
            string models = "SELECT * FROM models";

            string min_web_price = "SELECT MIN(web_price) AS min_price FROM products WHERE products.deleted_at IS NULL";
            string max_web_price = "SELECT MAX(web_price) AS max_price FROM products WHERE products.deleted_at IS NULL";

            try
            {
                // Abre la conexión
                connection.Open();

                // Inserta como un DataSet lo devuelto en la consulta SQL
                MySqlDataAdapter products_da = new MySqlDataAdapter(products, connection);
                DataSet ds = new DataSet();
                products_da.Fill(ds, "products");
                DataTable dt = ds.Tables["products"];

                // Insertar Rango de Precios
                InsertData(min_web_price, connection, "min_price", min_price);
                InsertData(max_web_price, connection, "max_price", max_price);

                // Llenar la DataGrid con la Tabla ingresada
                InsertTableData(dt);

                // Llenar el ComboBox con la Tabla de Marcas
                InsertComboBox(brands, connection, brand_filter);

                // Llenar el ComboBox con la Tabla de Modelos
                InsertComboBox(models, connection, model_filter);
            } 
            catch (Exception ex)
            {
                Console.WriteLine($"Exception Handler: {ex}");

                // Limpiar la DataGrid
                ClearTableData();
            }

            connection.Close();
        }

        // Inserta datos en la DataGrid
        private void InsertTableData(DataTable dt)
        {
            ProductsTable.ItemsSource = dt.DefaultView;
            ProductsTable.AutoGenerateColumns = true;
            ProductsTable.CanUserAddRows = false;
        }

        // Limpia la DataGrid
        private void ClearTableData()
        {
            ProductsTable.ItemsSource = "";
        }

        // Lee una consulta y lo que retorne se inserta en el combobox
        public void InsertComboBox(string sql, MySqlConnection connection, ComboBox comboBox)
        {
            MySqlCommand cmd = new MySqlCommand(sql, connection);
            MySqlDataReader sqlReader = cmd.ExecuteReader();

            while (sqlReader.Read())
            {
                comboBox.Items.Add(sqlReader["name"].ToString());
            }

            sqlReader.Close();
        }

        // Lee una consulta y lo que retorne se inserta en el textbox
        public void InsertData(string sql, MySqlConnection connection, string column, TextBox textBox)
        {
            MySqlCommand cmd = new MySqlCommand(sql, connection);
            MySqlDataReader sqlReader = cmd.ExecuteReader();

            while (sqlReader.Read())
            {
                textBox.Text = sqlReader[column].ToString();
            }

            sqlReader.Close();
        }

        private void Button_Search(object sender, RoutedEventArgs e)
        {
            if (decimal.TryParse(min_price.Text, out decimal min) && decimal.TryParse(max_price.Text, out decimal max))
            {
                min_price.Text = min.ToString();
                max_price.Text = max.ToString();

                // Se crea la conexión con la base de datos/servidor
                string credentials = "server=127.0.0.1;user=rivas;password=test123;database=tarea;port=3306;";
                MySqlConnection connection = new MySqlConnection(credentials);

                try
                {
                    // Abre la conexión
                    connection.Open();

                    // Consulta SQL con relaciones
                    string search = "SELECT " +
                                    "products.id AS ID, " +
                                    "products.sku AS SKU, " +
                                    "products.name AS Nombre, " +
                                    "products.description AS Descripción, " +
                                    "products.web_price AS 'Precio Venta', " +
                                    "products.stock AS Stock, " +
                                    "brands.name AS Marca, " +
                                    "models.name AS Modelo FROM products " +
                                    "JOIN brands ON products.brand_id = brands.id " +
                                    "JOIN models ON products.model_id = models.id WHERE ";

                    string min_web_price, max_web_price;

                    if (deleted_check.IsChecked == false)
                    {
                        search += $"products.deleted_at IS NULL AND ";

                        min_web_price = "SELECT MIN(web_price) AS min_price FROM products WHERE products.deleted_at IS NULL";
                        max_web_price = "SELECT MAX(web_price) AS max_price FROM products WHERE products.deleted_at IS NULL";
                    }
                    else
                    {
                        min_web_price = "SELECT MIN(web_price) AS min_price FROM products";
                        max_web_price = "SELECT MAX(web_price) AS max_price FROM products";
                    }

                    // Insertar Rango de Precios
                    InsertData(min_web_price, connection, "min_price", min_price);
                    InsertData(max_web_price, connection, "max_price", max_price);

                    // Separado del string por fines didácticos.
                    search += $"products.sku LIKE '%{sku_filter.Text}%' AND ";
                    search += $"products.name LIKE '%{name_filter.Text}%' AND ";
                    search += $"products.web_price >= {min_price.Text} AND ";
                    search += $"products.web_price <= {max_price.Text} AND ";
                    search += $"brands.name LIKE '%{brand_filter.Text}%' AND ";
                    search += $"models.name LIKE '%{model_filter.Text}%'";

                    Console.WriteLine(search);

                    // Inserta como un DataSet lo devuelto en la consulta SQL
                    MySqlDataAdapter da = new MySqlDataAdapter(search, connection);
                    DataSet ds = new DataSet();
                    da.Fill(ds, "products");
                    DataTable dt = ds.Tables["products"];

                    // Debug para la Tabla ingresada
                    //DebugTable(dt);

                    // Limpiar la DataGrid antes de Ingresar
                    ClearTableData();

                    // Llenar la DataGrid con la Tabla ingresada
                    InsertTableData(dt);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Exception Handler: {ex}");

                    // Limpiar la DataGrid
                    ClearTableData();
                }

                connection.Close();
            }
            else
            {
                MessageBox.Show("Rango de Precio Invalido.", "Error");
            }
        }
    }
}
