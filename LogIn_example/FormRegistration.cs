using LogIn_example.Entities;
using LogIn_example.Providers;
using System;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace LogIn_example
{
    public partial class FormRegistration : Form
    {
        private UserProvider _provider;

        public FormRegistration(UserProvider userProvider)
        {
            InitializeComponent();
            _provider = userProvider;
            _provider.InitConnection();
            _provider.OpenConnection();
        }

        private void buttonRegistration_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrWhiteSpace(textBoxLogin.Text))
            {
                MessageBox.Show("Укажите логин!");
                return;
            }
            if (String.IsNullOrWhiteSpace(textBoxPassword.Text)) {
                MessageBox.Show("Укажите пароль!");
                return;
            }
            if (!IsValidPassword(textBoxPassword.Text)) {
                MessageBox.Show("Пароль должен быть:\nНе менее 8 символов\nСодержать символы верхнего и нижнего регистра\nСпецсимволы: @, ! или .");
                return;
            }

            try
            {
                _provider.SaveUser(new User(login: textBoxLogin.Text, password: textBoxPassword.Text));
                MessageBox.Show("Регистрация прошла успешно!");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private bool IsValidPassword(string password)
        {
            Regex hasNumber = new Regex(@"[0-9]+");
            Regex hasUpperChar = new Regex(@"[A-Z]+");
            Regex hasMinimum8Chars = new Regex(@".{8,}");
            Regex hasSpecialChars = new Regex(@"[@!.]+");

            return hasNumber.IsMatch(password) && hasUpperChar.IsMatch(password) && hasMinimum8Chars.IsMatch(password) && hasSpecialChars.IsMatch(password);
        }

        private void FormRegistration_FormClosed(object sender, FormClosedEventArgs e)
        {
            _provider.CloseConnection();
        }
    }
}
