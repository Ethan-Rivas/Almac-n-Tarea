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
    /// Lógica de interacción para Manage.xaml
    /// </summary>
    public partial class ManageModel : Page
    {
        public ManageModel()
        {
            InitializeComponent();

            // Se crea la conexión con la base de datos/servidor
            string credentials = "server=127.0.0.1;user=rivas;password=test123;database=tarea;port=3306;";
            MySqlConnection connection = new MySqlConnection(credentials);

            string models = "SELECT * FROM models";

            try
            {
                // Abre la conexión
                connection.Open();

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

        // Obtiene el stock de un producto seleccionado
        private void GetModel(object sender, SelectionChangedEventArgs e)
        {
            // Se crea la conexión con la base de datos/servidor
            string credentials = "server=127.0.0.1;user=rivas;password=test123;database=tarea;port=3306;";
            MySqlConnection connection = new MySqlConnection(credentials);

            if (model_filter.SelectedValue != null && model_filter.SelectedValue.ToString().Length > 0)
            {
                // Consulta SQL para buscar el modelo seleccionado
                string sql = $"SELECT * FROM models WHERE id = '{(model_filter.SelectedItem as dynamic).Value}'";

                Console.WriteLine(sql);

                // Se solicita el modelo seleccionado
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

            if (model_filter.SelectedValue == null)
                MessageBox.Show("No seleccionó un producto a actualizar.", "Validación");
            else if (name.Text == null || description.Text == null)
                MessageBox.Show("Todos los campos son requeridos.", "Validación");
            else
            {
                // Consulta SQL para actualizar una marca
                string sql = $"UPDATE brands SET name = '{name.Text}', description = '{description.Text}' WHERE id = '{(model_filter.SelectedItem as dynamic).Value}'";

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

                    MessageBox.Show("Modelo actualizado correctamente.", "Carga Exitosa");

                    // Redirección al index de modelos
                    mainWin.page_frame.Content = new Models();
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
            if (MessageBox.Show($"Está seguro de eliminar el modelo: {(model_filter.SelectedItem as dynamic).Text}?", "Confirmación", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                // Se crea la conexión con la base de datos/servidor
                string credentials = "server=127.0.0.1;user=rivas;password=test123;database=tarea;port=3306;";
                MySqlConnection connection = new MySqlConnection(credentials);

                if (model_filter.SelectedValue == null)
                    MessageBox.Show("No seleccionó un producto a actualizar.", "Validación");
                else
                {
                    // Consulta SQL para elimiar un modelo
                    //string sql = $"DELETE FROM models WHERE id = '{(model_filter.SelectedItem as dynamic).Value}';";

                    // Consulta SQL para soft delete
                    DateTime today = DateTime.Now;
                    string FormattedDate = today.ToString("yyyy-MM-dd HH:mm:ss.fff");

                    string sql = $"UPDATE models SET deleted_at '{FormattedDate}' WHERE id = '{(model_filter.SelectedItem as dynamic).Value}';";

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

                        MessageBox.Show("Modelo eliminado correctamente.", "Acción Exitosa");

                        // Redirección al index de modelos
                        mainWin.page_frame.Content = new Models();
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
