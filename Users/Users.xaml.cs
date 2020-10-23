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
using System.Data;

namespace Almacén
{
    /// <summary>
    /// Lógica de interacción para Users.xaml
    /// </summary>
    public partial class Users : Page
    {
        public Users()
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
                             "FROM users WHERE deleted_at IS NULL " +
                             "ORDER BY id DESC";

                // Inserta como un DataSet lo devuelto en la consulta SQL
                MySqlDataAdapter da = new MySqlDataAdapter(sql, connection);
                DataSet ds = new DataSet();
                da.Fill(ds, "users");
                DataTable dt = ds.Tables["users"];

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
            UsersTable.ItemsSource = dt.DefaultView;
            UsersTable.AutoGenerateColumns = true;
            UsersTable.CanUserAddRows = false;
        }

        private void ClearTableData()
        {
            UsersTable.ItemsSource = "";
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
                             "FROM users WHERE ";

            if (deleted_check.IsChecked == false)
            {
                search += $"deleted_at IS NULL AND ";
            }

            // Separado del string por fines didácticos.
            search += $"email LIKE '%{email.Text}%' ";
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
