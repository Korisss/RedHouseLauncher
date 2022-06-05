using System;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using RedHouseLauncher.Core.Auth;
using RedHouseLauncher.Core.Auth.Models.Requests;
using RedHouseLauncher.Core.Auth.Models.Responses;
using RedHouseLauncher.Core.Settings;

namespace RedHouseLauncher.UI.Views
{
    /// <summary>
    /// Interaction logic for AuthWindow.xaml
    /// </summary>
    public partial class AuthWindow
    {
        public AuthWindow()
        {
            if (Settings.UserId == -1 || Settings.UserToken is "" or null)
            {
                InitializeComponent();
                return;
            }

            try
            {
                new SelectPathWindow().Show();
            }
            catch
            {
                // ignored
            }

            Close();
        }

        #region Вход в аккаунт

        private async void Login(object? sender, MouseButtonEventArgs? e)
        {
            try
            {
                string email = EmailTextBox.Text;
                string password = PasswordTextBoxHidden.Password;

                #region Проверка почты

                if (!IsEmailValid(email))
                {
                    _ = MessageBox.Show("Почта неправильного формата.");
                    return;
                }

                #endregion


                LoginButton.IsEnabled = false;
                RegisterButton.IsEnabled = false;

                LoginModelRequest loginModelRequest = new(email, password);

                try
                {
                    LoginModelResponse? loginModelResponse = await AccountWorker.Login(loginModelRequest);

                    if (loginModelResponse == null)
                    {
                        throw new Exception("Пустой ответ на логин");
                    }

                    Settings.UserId = loginModelResponse.Id;
                    Settings.UserToken = loginModelResponse.Token;

                    await Settings.Save();

                    try
                    {
                        new SelectPathWindow().Show();
                    }
                    catch
                    {
                        // ignored
                    }

                    Close();
                }
                catch (WebException ex)
                {
                    if (ex.Response != null)
                    {
                        if (((HttpWebResponse)ex.Response).StatusCode.ToString() != "Unauthorized")
                        {
                            throw;
                        }

                        _ = MessageBox.Show("Неверный пароль, либо аккаунт не зарегистрирован.");
                    }
                }
            }
            catch (Exception err)
            {
                _ = MessageBox.Show($"Произошла ошибка во время входа:\n\n{err}");
            }

            LoginButton.IsEnabled = true;
            RegisterButton.IsEnabled = true;
        }

        #endregion

        #region Регистрация

        private async void Register(object? sender, MouseButtonEventArgs? e)
        {
            try
            {
                #region Проверка ника

                if (NicknameTextBox.Text is "" or "Имя")
                {
                    _ = MessageBox.Show("Введите имя.");
                    return;
                }

                if (NicknameTextBox.Text.Length < 4)
                {
                    _ = MessageBox.Show("Имя должно быть длиной минимум 4 символа.");
                    return;
                }

                #endregion

                #region Проверка почты

                if (!IsEmailValid(RegisterEmailTextBox.Text))
                {
                    _ = MessageBox.Show("Почта неправильного формата.");
                    return;
                }

                #endregion

                #region Проверка паролей

                if (RegisterPasswordTextBoxHidden1.Password.Length < 8 ||
                    RegisterPasswordTextBoxHidden2.Password.Length < 8)
                {
                    _ = MessageBox.Show("Пароль должен быть длиннее 8 символов");
                    return;
                }

                if (RegisterPasswordTextBoxHidden1.Password != RegisterPasswordTextBoxHidden2.Password)
                {
                    _ = MessageBox.Show("Пароли не совпадают.");
                    return;
                }

                #endregion

                #region Отключение всех кнопок

                RegisterRegisterButton.IsEnabled = false;
                BackToLoginButton.IsEnabled = false;
                RegisterEmailTextBox.IsEnabled = false;
                RegisterPasswordTextBox1.IsEnabled = false;
                RegisterPasswordTextBox2.IsEnabled = false;
                RegisterPasswordTextBoxHidden1.IsEnabled = false;
                RegisterPasswordTextBoxHidden2.IsEnabled = false;
                NicknameTextBox.IsEnabled = false;

                #endregion

                #region Регистрация

                #region Запрос регистрации

                RegisterModelRequest registerModelRequest = new(NicknameTextBox.Text, RegisterEmailTextBox.Text, RegisterPasswordTextBoxHidden1.Password);

                try
                {
                    RegisterModelResponse? registerModelResponse = await AccountWorker.Register(registerModelRequest);

                    if (registerModelResponse == null)
                    {
                        throw new Exception("Пустой ответ на регистрацию");
                    }

                    Settings.UserId = registerModelResponse.Id;

                    await Settings.Save();

                    _ = MessageBox.Show("Вы были успешно зарегистрированы");
                }
                catch (HttpRequestException ex)
                {
                    if (ex.StatusCode == HttpStatusCode.BadRequest)
                    {
                        _ = MessageBox.Show("Скорее всего данный аккаунт уже зарегистрирован");
                    }
                    else if (ex.StatusCode == HttpStatusCode.NotFound)
                    {
                        _ = MessageBox.Show("Сервер авторизации недоступен. Возможно, проблемы на стороне провайдера, но лучше напишите в техподдержку.");
                    }
                    else
                    {
                        _ = MessageBox.Show(ex.Message);
                        _ = MessageBox.Show(ex.StatusCode.ToString());
                    }
                }

                #endregion

                //MessageBox.Show($"Вам нужно подтвердить e-mail {registerModelRequest.Email}, письмо отправлено.");


                BackToLogin(sender, e);

                #endregion
            }
            catch (Exception err)
            {
                _ = MessageBox.Show($"Произошла ошибка во время регистрации:\n\n{err.GetType()}");
            }

            #region Включение всех кнопок

            RegisterRegisterButton.IsEnabled = true;
            BackToLoginButton.IsEnabled = true;
            RegisterEmailTextBox.IsEnabled = true;
            RegisterPasswordTextBox1.IsEnabled = true;
            RegisterPasswordTextBox2.IsEnabled = true;
            RegisterPasswordTextBoxHidden1.IsEnabled = true;
            RegisterPasswordTextBoxHidden2.IsEnabled = true;
            NicknameTextBox.IsEnabled = true;

            #endregion
        }

        #endregion

        #region Чек почты

        private static bool IsEmailValid(string email)
        {
            try
            {
                _ = new MailAddress(email);
                return true;
            }
            catch
            {
                return false;
            }
        }

        #endregion

        #region

        private void EnterClicked(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Enter)
            {
                return;
            }

            if (LoginPage.Visibility == Visibility.Visible)
            {
                Login(null, null);
            }
            else
            {
                Register(null, null);
            }
        }

        #endregion

        #region Тулбар

        private void DragWindow(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (e.ChangedButton == MouseButton.Left)
                {
                    DragMove();
                }
            }
            catch
            {
                // ignored
            }
        }

        private void MinimizeWindow(object sender, MouseButtonEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        #endregion

        #region Ховеры

        private void RegisterButtonHoverEnable(object sender, MouseEventArgs e)
        {
            object? colorConverter = ColorConverter.ConvertFromString("#D6D6D6");

            if (colorConverter == null)
            {
                return;
            }

            RegisterButton.BorderBrush = new SolidColorBrush((Color)colorConverter);
        }

        private void RegisterButtonHoverDisable(object sender, MouseEventArgs e)
        {
            RegisterButton.BorderBrush = Brushes.White;
        }

        private void Register_RegisterButtonHoverEnable(object sender, MouseEventArgs e)
        {
            object? colorConverter = ColorConverter.ConvertFromString("#D6D6D6");

            if (colorConverter == null)
            {
                return;
            }

            RegisterRegisterButton.Background = new SolidColorBrush((Color)colorConverter);
        }

        private void Register_RegisterButtonHoverDisable(object sender, MouseEventArgs e)
        {
            RegisterRegisterButton.Background = Brushes.White;
        }

        #endregion

        #region Стили текстбоксов

        private void EmailTextBoxFill(object sender, RoutedEventArgs e)
        {
            if (EmailTextBox.Text != "")
            {
                return;
            }

            EmailTextBox.Text = "Почта";

            object? colorConverter = ColorConverter.ConvertFromString("#8C8C8C");

            if (colorConverter == null)
            {
                return;
            }

            EmailTextBox.Foreground = new SolidColorBrush((Color)colorConverter);
        }

        private void EmailTextBoxClear(object sender, RoutedEventArgs e)
        {
            if (EmailTextBox.Text != "Почта")
            {
                return;
            }

            EmailTextBox.Text = "";
            EmailTextBox.Foreground = Brushes.White;
        }

        private void PasswordTextBoxChange1(object sender, RoutedEventArgs e)
        {
            PasswordTextBox.Visibility = Visibility.Hidden;
            PasswordTextBoxHidden.Visibility = Visibility.Visible;

            _ = PasswordTextBoxHidden.Focus();
        }

        private void PasswordTextBoxChange2(object sender, RoutedEventArgs e)
        {
            if (PasswordTextBoxHidden.Password != "")
            {
                return;
            }

            PasswordTextBox.Visibility = Visibility.Visible;
            PasswordTextBoxHidden.Visibility = Visibility.Hidden;
        }

        private void NicknameTextBoxFill(object sender, RoutedEventArgs e)
        {
            if (NicknameTextBox.Text != "")
            {
                return;
            }

            NicknameTextBox.Text = "Имя";

            object? colorConverter = ColorConverter.ConvertFromString("#8C8C8C");

            if (colorConverter == null)
            {
                return;
            }

            NicknameTextBox.Foreground = new SolidColorBrush((Color)colorConverter);
        }

        private void NicknameTextBoxClear(object sender, RoutedEventArgs e)
        {
            if (NicknameTextBox.Text != "Имя")
            {
                return;
            }

            NicknameTextBox.Text = "";
            NicknameTextBox.Foreground = Brushes.White;
        }

        private void Register_EmailTextBoxFill(object sender, RoutedEventArgs e)
        {
            if (RegisterEmailTextBox.Text != "")
            {
                return;
            }

            RegisterEmailTextBox.Text = "Почта";

            object? colorConverter = ColorConverter.ConvertFromString("#8C8C8C");

            if (colorConverter == null)
            {
                return;
            }

            RegisterEmailTextBox.Foreground = new SolidColorBrush((Color)colorConverter);
        }

        private void Register_EmailTextBoxClear(object sender, RoutedEventArgs e)
        {
            if (RegisterEmailTextBox.Text != "Почта")
            {
                return;
            }

            RegisterEmailTextBox.Text = "";
            RegisterEmailTextBox.Foreground = Brushes.White;
        }

        private void Register_PasswordTextBoxChange11(object sender, RoutedEventArgs e)
        {
            RegisterPasswordTextBox1.Visibility = Visibility.Hidden;
            RegisterPasswordTextBoxHidden1.Visibility = Visibility.Visible;

            _ = RegisterPasswordTextBoxHidden1.Focus();
        }

        private void Register_PasswordTextBoxChange21(object sender, RoutedEventArgs e)
        {
            if (RegisterPasswordTextBoxHidden1.Password != "")
            {
                return;
            }

            RegisterPasswordTextBox1.Visibility = Visibility.Visible;
            RegisterPasswordTextBoxHidden1.Visibility = Visibility.Hidden;
        }

        private void Register_PasswordTextBoxChange12(object sender, RoutedEventArgs e)
        {
            RegisterPasswordTextBox2.Visibility = Visibility.Hidden;
            RegisterPasswordTextBoxHidden2.Visibility = Visibility.Visible;

            _ = RegisterPasswordTextBoxHidden2.Focus();
        }

        private void Register_PasswordTextBoxChange22(object sender, RoutedEventArgs e)
        {
            if (RegisterPasswordTextBoxHidden2.Password != "")
            {
                return;
            }

            RegisterPasswordTextBox2.Visibility = Visibility.Visible;
            RegisterPasswordTextBoxHidden2.Visibility = Visibility.Hidden;
        }

        #endregion

        #region Переключение страниц

        private void ShowRegisterPage(object sender, MouseButtonEventArgs e)
        {
            RegisterPage.Visibility = Visibility.Visible;
            LoginPage.Visibility = Visibility.Hidden;
        }

        private void BackToLogin(object? sender, MouseButtonEventArgs? e)
        {
            RegisterPage.Visibility = Visibility.Hidden;
            LoginPage.Visibility = Visibility.Visible;
        }

        #endregion
    }
}
