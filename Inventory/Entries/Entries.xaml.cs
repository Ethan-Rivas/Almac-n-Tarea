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
    /// Lógica de interacción para Entries.xaml
    /// </summary>
    public partial class Entries : Page
    {
        public Entries()
        {
            InitializeComponent();

            // Asignar fechas default
            min_date.SelectedDate = DateTime.Now;
            max_date.SelectedDate = DateTime.Now;

            // Se crea la conexión con la base de datos/servidor
            string credentials = "server=127.0.0.1;user=rivas;password=test123;database=tarea;port=3306;";
            MySqlConnection connection = new MySqlConnection(credentials);

            try
            {
                // Abre la conexión
                connection.Open();

                // Consulta SQL con relaciones
                string entries = "SELECT " +
                        "entries.id AS ID, " +
                        "providers.name AS Proveedor, " +
                        "products.name AS Producto, " +
                        "entries.quantity AS Cantidad, " +
                        "entries.cost_price AS 'Costo Unitario', " +
                        "entries.date AS 'Fecha de Entrada' FROM entries " +
                        "JOIN providers ON entries.provider_id = providers.id " +
                        "JOIN products ON entries.product_id = products.id ";

                string providers = "SELECT * FROM providers";
                string products = "SELECT * FROM products";

                // Inserta como un DataSet lo devuelto en la consulta SQL
                MySqlDataAdapter da = new MySqlDataAdapter(entries, connection);
                DataSet ds = new DataSet();
                da.Fill(ds, "entries");
                DataTable dt = ds.Tables["entries"];

                // Llenar la DataGrid con la Tabla ingresada
                InsertTableData(dt);

                // Llenar el ComboBox con la Tabla de Marcas
                InsertComboBox(providers, connection, provider_filter);

                // Llenar el ComboBox con la Tabla de Modelos
                InsertComboBox(products, connection, product_filter);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception Handler: {ex}");

                // Limpiar la DataGrid
                ClearTableData();
            }

            connection.Close();
        }

        // Inserta valores en DataGrid
        private void InsertTableData(DataTable dt)
        {
            EntriesTable.ItemsSource = dt.DefaultView;
            EntriesTable.AutoGenerateColumns = true;
            EntriesTable.CanUserAddRows = false;
        }

        // Limpia la DataGrid
        private void ClearTableData()
        {
            EntriesTable.ItemsSource = "";
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

        // Filtra las Entradas
        private void Button_Search(object sender, RoutedEventArgs e)
        {
            // Consulta SQL con relaciones
            string search = "SELECT " +
                        "entries.id AS ID, " +
                        "providers.name AS Proveedor, " +
                        "products.name AS Producto, " +
                        "entries.quantity AS Cantidad, " +
                        "entries.date AS 'Fecha de Entrada' FROM entries " +
                        "JOIN providers ON entries.provider_id = providers.id " +
                        "JOIN products ON entries.product_id = products.id WHERE ";

            // Separado del string por fines didácticos.
            search += $"providers.name LIKE '%{provider_filter.Text}%' AND ";
            search += $"products.name LIKE '%{product_filter.Text}%' AND ";
            search += $"entries.date >= STR_TO_DATE('{min_date.Text}', '%d/%m/%Y') AND ";
            search += $"entries.date <= STR_TO_DATE('{max_date.Text}', '%d/%m/%Y')";

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
                da.Fill(ds, "entries");
                DataTable dt = ds.Tables["entries"];

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

        // Da formato a las DateTime en la DataGrid al generarla
        private void OnAutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            if (e.PropertyType == typeof(System.DateTime))
                (e.Column as DataGridTextColumn).Binding.StringFormat = "dd/MM/yyyy";
        }
    }
}
