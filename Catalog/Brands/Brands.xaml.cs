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
    /// Lógica de interacción para Brands.xaml
    /// </summary>
    public partial class Brands : Page
    {
        public Brands()
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
                             "name AS Nombre, " +
                             "description AS Descripción " +
                             "FROM brands " +
                             "ORDER BY id DESC";

                // Inserta como un DataSet lo devuelto en la consulta SQL
                MySqlDataAdapter da = new MySqlDataAdapter(sql, connection);
                DataSet ds = new DataSet();
                da.Fill(ds, "brands");
                DataTable dt = ds.Tables["brands"];

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
            BrandsTable.ItemsSource = dt.DefaultView;
            BrandsTable.AutoGenerateColumns = true;
            BrandsTable.CanUserAddRows = false;
        }

        private void ClearTableData()
        {
            BrandsTable.ItemsSource = "";
        }
    }
}
