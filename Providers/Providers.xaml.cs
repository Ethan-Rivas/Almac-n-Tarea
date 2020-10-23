using System;
using System.Collections.Generic;
using System.Data;
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
    /// Lógica de interacción para Providers.xaml
    /// </summary>
    public partial class Providers : Page
    {
        public Providers()
        {
            InitializeComponent();

            // Se crea la conexión con la base de datos/servidor
            string credentials = "server=127.0.0.1;user=rivas;password=test123;database=tarea;port=3306;";
            MySqlConnection connection = new MySqlConnection(credentials);

            try
            {
                // Abre la conexión
                connection.Open();

                // Consulta SQL con relaciones
                string sql = "SELECT " +
                             "id AS ID, " +
                             "code AS Código, " +
                             "name AS Nombre, " +
                             "address AS Dirección, " +
                             "email AS Correo, " +
                             "phone AS Teléfono " +
                             "FROM providers WHERE deleted_at IS NULL " +
                             "ORDER BY id DESC";

                // Inserta como un DataSet lo devuelto en la consulta SQL
                MySqlDataAdapter da = new MySqlDataAdapter(sql, connection);
                DataSet ds = new DataSet();
                da.Fill(ds, "providers");
                DataTable dt = ds.Tables["providers"];

                // Debug para la Tabla ingresada
                //DebugTable(dt);

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

        private void DebugTable(DataTable dt)
        {
            foreach (DataRow row in dt.Rows)
            {
                foreach (DataColumn col in dt.Columns)
                {
                    Console.Write(row[col] + "\t");
                }
                Console.Write("\n");
            }
        }

        private void InsertTableData(DataTable dt)
        {
            ProvidersTable.ItemsSource = dt.DefaultView;
            ProvidersTable.AutoGenerateColumns = true;
            ProvidersTable.CanUserAddRows = false;
        }

        private void ClearTableData()
        {
            ProvidersTable.ItemsSource = "";
        }

        private void Button_Search(object sender, RoutedEventArgs e)
        {
            // Consulta SQL con relaciones
            string search = "SELECT " +
                             "id AS ID, " +
                             "code AS Código, " +
                             "name AS Nombre, " +
                             "address AS Dirección, " +
                             "email AS Correo, " +
                             "phone AS Teléfono " +
                             "FROM providers WHERE ";

            if (deleted_check.IsChecked == false)
            {
                search += $"deleted_at IS NULL AND ";
            }

            // Separado del string por fines didácticos.
            search += $"email LIKE '%{email.Text}%'";
            search += "ORDER BY id DESC";

            Console.WriteLine(search);

            // Se crea la conexión con la base de datos/servidor
            string credentials = "server=127.0.0.1;user=rivas;password=test123;database=tarea;port=3306;";
            MySqlConnection connection = new MySqlConnection(credentials);

            try
            {
                // Abre la conexión
                connection.Open();

                // Inserta como un DataSet lo devuelto en la consulta SQL
                MySqlDataAdapter da = new MySqlDataAdapter(search, connection);
                DataSet ds = new DataSet();
                da.Fill(ds, "products");
                DataTable dt = ds.Tables["products"];

                // Debug para la Tabla ingresada
                //DebugTable(dt);

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
    }
}
