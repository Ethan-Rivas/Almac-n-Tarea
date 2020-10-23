using MySql.Data.MySqlClient;
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

namespace Almacén
{
    /// <summary>
    /// Lógica de interacción para AddModel.xaml
    /// </summary>
    public partial class AddModel : Page
    {
        public AddModel()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            // Se crea la conexión con la base de datos/servidor
            string credentials = "server=127.0.0.1;user=rivas;password=test123;database=tarea;port=3306;";
            MySqlConnection connection = new MySqlConnection(credentials);

            if (name.Text == null || description.Text == null)
                MessageBox.Show("Todos los campos son requeridos.", "Validación");
            else
            {
                // Consulta SQL para insertar un modelo
                string sql = "INSERT INTO models" +
                "(name, description)" +
                $"VALUES('{name.Text}', '{description.Text}');";

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

                    MessageBox.Show("Modelo cargado correctamente.", "Carga Exitosa");

                    // Redirección al index de modelos
                    mainWin.page_frame.Content = new Models();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Exception Handler: {ex}");

                    MessageBox.Show("Error de carga.", "Carga Fallida");
                }
            }
        }
    }
}
