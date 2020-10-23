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
    public partial class Exits : Page
    {
        public Exits()
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
                string exits = "SELECT " +
                        "exits.id AS ID, " +
                        "users.name AS Usuario, " +
                        "products.name AS Producto, " +
                        "exits.quantity AS Cantidad, " +
                        "exits.date AS 'Fecha de Salida' FROM exits " +
                        "JOIN users ON exits.user_id = users.id " +
                        "JOIN products ON exits.product_id = products.id ";

                string users = "SELECT * FROM users";
                string products = "SELECT * FROM products";

                // Inserta como un DataSet lo devuelto en la consulta SQL
                MySqlDataAdapter da = new MySqlDataAdapter(exits, connection);
                DataSet ds = new DataSet();
                da.Fill(ds, "exits");
                DataTable dt = ds.Tables["exits"];

                // Llenar la DataGrid con la Tabla ingresada
                InsertTableData(dt);

                // Llenar el ComboBox con la Tabla de Usuarios
                InsertComboBox(users, connection, user_filter);

                // Llenar el ComboBox con la Tabla de Productos
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
            ExitsTable.ItemsSource = dt.DefaultView;
            ExitsTable.AutoGenerateColumns = true;
            ExitsTable.CanUserAddRows = false;
        }

        // Limpia la DataGrid
        private void ClearTableData()
        {
            ExitsTable.ItemsSource = "";
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
                        "exits.id AS ID, " +
                        "users.name AS Usuario, " +
                        "products.name AS Producto, " +
                        "exits.quantity AS Cantidad, " +
                        "exits.date AS 'Fecha de Salida' FROM exits " +
                        "JOIN users ON exits.user_id = users.id " +
                        "JOIN products ON exits.product_id = products.id WHERE ";

            // Separado del string por fines didácticos.
            search += $"users.name LIKE '%{user_filter.Text}%' AND ";
            search += $"products.name LIKE '%{product_filter.Text}%' AND ";
            search += $"exits.date >= STR_TO_DATE('{min_date.Text}', '%d/%m/%Y') AND ";
            search += $"exits.date <= STR_TO_DATE('{max_date.Text}', '%d/%m/%Y')";

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
                da.Fill(ds, "exits");
                DataTable dt = ds.Tables["exits"];

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
