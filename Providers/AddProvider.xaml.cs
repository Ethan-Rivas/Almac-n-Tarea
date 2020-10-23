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
    /// Lógica de interacción para AddProvider.xaml
    /// </summary>
    public partial class AddProvider : Page
    {
        public AddProvider()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            // Se crea la conexión con la base de datos/servidor
            string credentials = "server=127.0.0.1;user=rivas;password=test123;database=tarea;port=3306;";
            MySqlConnection connection = new MySqlConnection(credentials);

            if (code.Text == null || name.Text == null || address.Text == null || email.Text == null || phone.Text == null)
                MessageBox.Show("Todos los campos son requeridos.", "Validación");
            else
            {
                // Consulta SQL para insertar un proveedor
                string sql = "INSERT INTO providers" +
                "(code, name, address, email, phone)" +
                $"VALUES('{code.Text}', '{name.Text}', '{address.Text}', '{email.Text}', '{phone.Text}');";

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

                    MessageBox.Show("Proveedor cargado correctamente.", "Carga Exitosa");

                    // Redirección al index de proveedores
                    mainWin.page_frame.Content = new Providers();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Exception Handler: {ex}");

                    MessageBox.Show("Error de carga.", "Carga Fallida");
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
