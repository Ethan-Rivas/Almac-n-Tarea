using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Almacén.Services.Auth
{
    class Auth
    {
        public static User user;

        public static bool Authenticate(string email, string pwd)
        {
            // Se crea la conexión con la base de datos/servidor
            string credentials = "server=127.0.0.1;user=rivas;password=test123;database=tarea;port=3306;";
            MySqlConnection connection = new MySqlConnection(credentials);

            try
            {
                string sql = $"SELECT * FROM users WHERE email = '{email}' AND password = '{pwd}' AND deleted_at IS NULL;";

                Console.WriteLine(sql);

                // Abre la conexión
                connection.Open();

                // Inserta como un DataSet lo devuelto en la consulta SQL
                MySqlDataAdapter da = new MySqlDataAdapter(sql, connection);

                DataTable dt = new DataTable();
                da.Fill(dt);

                Console.WriteLine(da);

                // Verifica si hay datos existentes
                if (dt.Rows.Count > 0)
                {
                    DataRow row = dt.Rows[0];

                    user = new User(Convert.ToInt32(row["id"]), Convert.ToString(row["name"]), Convert.ToString(row["email"]));

                    connection.Close();
                    return true;
                }
                else
                {
                    connection.Close();
                    return false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception Handler: {ex}");
                return false;
            }
        }
    }
}
