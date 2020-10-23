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
    /// Lógica de interacción para ManageProvider.xaml
    /// </summary>
    public partial class ManageProvider : Page
    {
        public ManageProvider()
        {
            InitializeComponent();

            // Se crea la conexión con la base de datos/servidor
            string credentials = "server=127.0.0.1;user=rivas;password=test123;database=tarea;port=3306;";
            MySqlConnection connection = new MySqlConnection(credentials);

            string providers = "SELECT * FROM providers";

            try
            {
                // Abre la conexión
                connection.Open();

                // Llenar el ComboBox con la Tabla de Proveedores
                InsertComboBox(providers, connection, provider_filter);
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

        // Obtiene un proveedor seleccionado
        private void GetProvider(object sender, SelectionChangedEventArgs e)
        {
            // Se crea la conexión con la base de datos/servidor
            string credentials = "server=127.0.0.1;user=rivas;password=test123;database=tarea;port=3306;";
            MySqlConnection connection = new MySqlConnection(credentials);

            if (provider_filter.SelectedValue != null && provider_filter.SelectedValue.ToString().Length > 0)
            {
                // Consulta SQL para buscar el proveedor seleccionado
                string sql = $"SELECT * FROM providers WHERE name = '{(provider_filter.SelectedItem as dynamic).Text}'";

                Console.WriteLine(sql);

                // Se solicita el proveedor seleccionado
                try
                {
                    // Abre la conexión
                    connection.Open();

                    MySqlCommand cmd = new MySqlCommand(sql, connection);
                    MySqlDataReader sqlReader = cmd.ExecuteReader();


                    while (sqlReader.Read())
                    {
                        code.Text = sqlReader["code"].ToString();
                        name.Text = sqlReader["name"].ToString();
                        address.Text = sqlReader["address"].ToString();
                        email.Text = sqlReader["email"].ToString();
                        phone.Text = sqlReader["phone"].ToString();
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

            if (provider_filter.SelectedValue == null)
                MessageBox.Show("No seleccionó un proveedor a actualizar.", "Validación");
            else if (code.Text == null || name.Text == null || address.Text == null || email.Text == null || phone.Text == null)
                MessageBox.Show("Todos los campos son requeridos.", "Validación");
            else
            {
                // Consulta SQL para actualizar un proveedor
                string sql = $"UPDATE providers " +
                             $"SET code = '{code.Text}', " +
                             $"name = '{name.Text}', " +
                             $"address = '{address.Text}', " +
                             $"email = '{email.Text}', " +
                             $"phone = {phone.Text} " +
                             $"WHERE name = '{(provider_filter.SelectedItem as dynamic).Text}'";

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

                    MessageBox.Show("Proveedor actualizado correctamente.", "Actualización Exitosa");

                    // Redirección al index de proveedores
                    mainWin.page_frame.Content = new Providers();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Exception Handler: {ex}");
                    MessageBox.Show("Error de Actualización.", "Actualización Fallida");

                    connection.Close();
                }
            }
        }

        private void Button_Delete(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show($"Está seguro de eliminar el proveedor: {(provider_filter.SelectedItem as dynamic).Text}?", "Confirmación", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                // Se crea la conexión con la base de datos/servidor
                string credentials = "server=127.0.0.1;user=rivas;password=test123;database=tarea;port=3306;";
                MySqlConnection connection = new MySqlConnection(credentials);

                if (provider_filter.SelectedValue == null)
                    MessageBox.Show("No seleccionó un proveedor a eliminar.", "Validación");
                else if (code.Text == null || name.Text == null || address.Text == null || email.Text == null || phone.Text == null)
                    MessageBox.Show("Todos los campos son requeridos.", "Validación");
                else
                {
                    // Consulta SQL para elimiar un usuario
                    string sql = $"DELETE FROM providers WHERE name = '{(provider_filter.SelectedItem as dynamic).Text}';";

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

                        MessageBox.Show("Proveedor eliminado correctamente.", "Acción Exitosa");

                        // Redirección al index de proveedores
                        mainWin.page_frame.Content = new Providers();
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

        // Validar textbox a solo números
        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }
    }
}
