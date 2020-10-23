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
using System.Windows.Shapes;

using Almacén.Services.Auth;

namespace Almacén.Login
{
    /// <summary>
    /// Lógica de interacción para Login.xaml
    /// </summary>
    public partial class Login : Window
    {
        public Login()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (Auth.Authenticate(email.Text, password.Password))
            {
                MessageBox.Show("Bienvenido: " + Auth.user.Email, "Bienvenido");

                this.Hide();

                var mainWin = Application.Current.Windows.Cast<Window>().FirstOrDefault(window => window is MainWindow) as MainWindow;
                mainWin.currentUser.Text = Auth.user.Name + " " + "(" + Auth.user.Email + ")";
                Application.Current.MainWindow.Show();
            }
            else
            {
                MessageBox.Show("Ingrese Datos Correctos", "Error");
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            App.Current.Shutdown(0);
        }
    }
}
