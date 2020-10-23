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
    /// Lógica de interacción para ManageProduct.xaml
    /// </summary>
    public partial class ManageProduct : Page
    {
        public ManageProduct()
        {
            InitializeComponent();

            // Se crea la conexión con la base de datos/servidor
            string credentials = "server=127.0.0.1;user=rivas;password=test123;database=tarea;port=3306;";
            MySqlConnection connection = new MySqlConnection(credentials);

            string products = "SELECT * FROM products WHERE deleted_at IS NULL";
            string brands = "SELECT * FROM brands WHERE deleted_at IS NULL";
            string models = "SELECT * FROM models WHERE deleted_at IS NULL";

            try
            {
                // Abre la conexión
                connection.Open();

                // Llenar el ComboBox con la Tabla de Productos
                InsertComboBox(products, connection, product_filter);

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

        // Obtiene un producto seleccionado
        private void GetProduct(object sender, SelectionChangedEventArgs e)
        {
            // Se crea la conexión con la base de datos/servidor
            string credentials = "server=127.0.0.1;user=rivas;password=test123;database=tarea;port=3306;";
            MySqlConnection connection = new MySqlConnection(credentials);

            if (product_filter.SelectedValue != null && product_filter.SelectedValue.ToString().Length > 0)
            {
                // Consulta SQL para buscar el producto seleccionado
                string sql = $"SELECT products.*, " +
                             "brands.id AS marca_id, " +
                             "brands.name AS marca, " +
                             "models.id AS modelo_id, " +
                             "models.name AS modelo " +
                             $"FROM products " +
                             "JOIN brands ON products.brand_id = brands.id " +
                             "JOIN models ON products.model_id = models.id " +
                             $"WHERE products.id = '{(product_filter.SelectedItem as dynamic).Value}' " +
                             "AND products.deleted_at IS NULL;";

                Console.WriteLine(sql);

                // Se solicita el producto seleccionado
                try
                {
                    // Abre la conexión
                    connection.Open();

                    MySqlCommand cmd = new MySqlCommand(sql, connection);
                    MySqlDataReader sqlReader = cmd.ExecuteReader();


                    while (sqlReader.Read())
                    {
                        Object brand = new { Text = sqlReader["marca"].ToString(), Value = sqlReader["marca_id"].ToString() };
                        Object model = new { Text = sqlReader["modelo"].ToString(), Value = sqlReader["modelo_id"].ToString() };

                        sku.Text = sqlReader["name"].ToString();
                        name.Text = sqlReader["name"].ToString();
                        description.Text = sqlReader["description"].ToString();
                        web_price.Text = sqlReader["web_price"].ToString();

                        brand_filter.SelectedIndex = brand_filter.Items.IndexOf(brand);
                        model_filter.SelectedIndex = model_filter.Items.IndexOf(model);
                    }

                    sqlReader.Close();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Exception Handler: {ex}");
                }

                connection.Close();
            }

        }

        private void Button_Update(object sender, RoutedEventArgs e)
        {
            if (decimal.TryParse(web_price.Text, out decimal price))
            {
                // Se crea la conexión con la base de datos/servidor
                string credentials = "server=127.0.0.1;user=rivas;password=test123;database=tarea;port=3306;";
                MySqlConnection connection = new MySqlConnection(credentials);

                if (product_filter.SelectedValue == null)
                    MessageBox.Show("No seleccionó un producto a actualizar.", "Validación");
                else if (sku.Text == null || name.Text == null || description.Text == null || brand_filter.SelectedValue == null || model_filter.SelectedValue == null)
                    MessageBox.Show("Todos los campos son requeridos.", "Validación");
                else
                {
                    // Consulta SQL para actualizar un producto
                    string sql = $"UPDATE products " +
                                 $"SET sku = '{sku.Text}', " +
                                 $"name = '{name.Text}', " +
                                 $"description = '{description.Text}', " +
                                 $"web_price = '{price}', " +
                                 $"brand_id = {(brand_filter.SelectedItem as dynamic).Value}, " +
                                 $"model_id = {(model_filter.SelectedItem as dynamic).Value} " +
                                 $"WHERE id = '{(product_filter.SelectedItem as dynamic).Value}'";

                    Console.WriteLine(sql);

                    try
                    {
                        // Abre la conexión
                        connection.Open();

                        // Ejecuta la actualización
                        MySqlCommand cmd = new MySqlCommand(sql, connection);
                        cmd.ExecuteNonQuery();

                        var mainWin = Application.Current.Windows.Cast<Window>().FirstOrDefault(window => window is MainWindow) as MainWindow;
                        connection.Close();

                        MessageBox.Show("Producto actualizado correctamente.", "Actualización Exitosa");

                        // Redirección al index de productos
                        mainWin.page_frame.Content = new Products();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Exception Handler: {ex}");
                        MessageBox.Show("Error de Actualización.", "Actualización Fallida");

                        connection.Close();
                    }
                }
            }
            else
            {
                MessageBox.Show("Rango de Precio Invalido.", "Error");
            }
        }

        private void Button_Delete(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show($"Está seguro de eliminar el producto: {(product_filter.SelectedItem as dynamic).Text}?", "Confirmación", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                // Se crea la conexión con la base de datos/servidor
                string credentials = "server=127.0.0.1;user=rivas;password=test123;database=tarea;port=3306;";
                MySqlConnection connection = new MySqlConnection(credentials);

                if (model_filter.SelectedValue == null)
                    MessageBox.Show("No seleccionó un producto a eliminar.", "Validación");
                else
                {
                    // Consulta SQL para elimiar un producto
                    //string sql = $"DELETE FROM products WHERE id = '{(product_filter.SelectedItem as dynamic).Value}';";

                    // Consulta SQL para soft delete
                    DateTime today = DateTime.Now;
                    string FormattedDate = today.ToString("yyyy-MM-dd HH:mm:ss.fff");

                    string sql = $"UPDATE products SET deleted_at = '{FormattedDate}' WHERE id = '{(product_filter.SelectedItem as dynamic).Value}';";

                    Console.WriteLine(sql);

                    try
                    {
                        // Abre la conexión
                        connection.Open();

                        // Ejecuta el delete
                        MySqlCommand cmd = new MySqlCommand(sql, connection);
                        cmd.ExecuteNonQuery();

                        var mainWin = Application.Current.Windows.Cast<Window>().FirstOrDefault(window => window is MainWindow) as MainWindow;
                        connection.Close();

                        MessageBox.Show("Producto eliminado correctamente.", "Acción Exitosa");

                        // Redirección al index de productos
                        mainWin.page_frame.Content = new Products();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Exception Handler: {ex}");
                        MessageBox.Show("Error de eliminación.", "Acción Fallida");

                        connection.Close();
                    }
                }
            }
        }
    }
}
