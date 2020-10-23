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

using MySql.Data.MySqlClient;

namespace Almacén
{
    /// <summary>
    /// Lógica de interacción para UpdateBrand.xaml
    /// </summary>
    public partial class ManageBrand : Page
    {
        public ManageBrand()
        {
            InitializeComponent();

            // Se crea la conexión con la base de datos/servidor
            string credentials = "server=127.0.0.1;user=rivas;password=test123;database=tarea;port=3306;";
            MySqlConnection connection = new MySqlConnection(credentials);

            string brands = "SELECT * FROM brands";

            try
            {
                // Abre la conexión
                connection.Open();

                // Llenar el ComboBox con la Tabla de Marcas
                InsertComboBox(brands, connection, brand_filter);
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

            while (sqlReader.Read())
            {
                comboBox.Items.Add(sqlReader["name"].ToString());
            }

            sqlReader.Close();
        }

        // Obtiene el stock de un producto seleccionado
        private void GetBrand(object sender, SelectionChangedEventArgs e)
        {
            // Se crea la conexión con la base de datos/servidor
            string credentials = "server=127.0.0.1;user=rivas;password=test123;database=tarea;port=3306;";
            MySqlConnection connection = new MySqlConnection(credentials);

            if (brand_filter.SelectedValue != null && brand_filter.SelectedValue.ToString().Length > 0)
            {
                // Consulta SQL para buscar la marca seleccionada
                string sql = $"SELECT * FROM brands WHERE name = '{brand_filter.SelectedValue}'";

                Console.WriteLine(sql);

                // Se solicita el la marca seleccionada
                try
                {
                    // Abre la conexión
                    connection.Open();

                    MySqlCommand cmd = new MySqlCommand(sql, connection);
                    MySqlDataReader sqlReader = cmd.ExecuteReader();

                    while (sqlReader.Read())
                    {
                        name.Text = sqlReader["name"].ToString();
                        description.Text = sqlReader["description"].ToString();
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
            // Se crea la conexión con la base de datos/servidor
            string credentials = "server=127.0.0.1;user=rivas;password=test123;database=tarea;port=3306;";
            MySqlConnection connection = new MySqlConnection(credentials);

            if (brand_filter.SelectedValue == null)
                MessageBox.Show("No seleccionó un producto a actualizar.", "Validación");
            else if (name.Text == null || description.Text == null)
                MessageBox.Show("Todos los campos son requeridos.", "Validación");
            else
            {
                // Consulta SQL para actualizar una marca
                string sql = $"UPDATE brands SET name = '{name.Text}', description = '{description.Text}' WHERE name = '{brand_filter.SelectedValue}'";

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

                    MessageBox.Show("Marca actualizada correctamente.", "Carga Exitosa");

                    // Redirección al index de modelos
                    mainWin.page_frame.Content = new Brands();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Exception Handler: {ex}");
                    MessageBox.Show("Error de carga.", "Carga Fallida");

                    connection.Close();
                }
            }
        }

        private void Button_Delete(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show($"Está seguro de eliminar la marca: {brand_filter.SelectedValue}?", "Confirmación", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                // Se crea la conexión con la base de datos/servidor
                string credentials = "server=127.0.0.1;user=rivas;password=test123;database=tarea;port=3306;";
                MySqlConnection connection = new MySqlConnection(credentials);

                if (brand_filter.SelectedValue == null)
                    MessageBox.Show("No seleccionó un producto a actualizar.", "Validación");
                else if (name.Text == null || description.Text == null)
                    MessageBox.Show("Todos los campos son requeridos.", "Validación");
                else
                {
                    // Consulta SQL para elimiar una marca
                    string sql = $"DELETE FROM brands WHERE name = '{brand_filter.SelectedValue}';";

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

                        MessageBox.Show("Marca eliminada correctamente.", "Acción Exitosa");

                        // Redirección al index de modelos
                        mainWin.page_frame.Content = new Brands();
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
