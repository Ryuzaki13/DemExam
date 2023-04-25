using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Variant10.Pages
{
    public partial class Authorization : Page
    {
        private DatabaseUserX database;
        private bool isCaptchaRequire = false;
        private string captchaCode;

        Random random = new Random();

        public Authorization(DatabaseUserX database)
        {
            InitializeComponent();
            this.database = database;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (isCaptchaRequire && !CaptchaTest())
            {
                return;
            }

            string login = tbUserLogin.Text.Trim();
            string password = tbUserPassword.Password.Trim();

            User user = database.User.Where(u => u.Login == login && u.Password == password).FirstOrDefault();
            if (user == null)
            {
                MessageBox.Show("неверный логин/пароль");
                CaptchaActivate();
                return;
            }

            switch (user.Role1.Name)
            {
                case "Администратор":
                    NavigationService.Navigate(new Administrator(database, user));
                    break;
                case "Менеджер":
                    NavigationService.Navigate(new Manager(database, user));
                    break;
                case "Клиент":
                    NavigationService.Navigate(new Client(database, user));
                    break;
                default:
                    MessageBox.Show("некорректная роль пользователя");
                    break;
            }

            isCaptchaRequire = false;
        }

        private void CaptchaActivate()
        {
            isCaptchaRequire = true;
            CaptchaCanvas.Visibility = Visibility.Visible;
            tbCaptcha.Visibility = Visibility.Visible;

            GenerateCaptchaCode();
            GenerateCaptchaImage();
        }

        private void CaptchaDeactivate()
        {
            isCaptchaRequire = false;
            CaptchaCanvas.Visibility = Visibility.Collapsed;
            tbCaptcha.Visibility = Visibility.Collapsed;
        }

        private void BlockActivate()
        {
            stackPanel.IsEnabled = false;

            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(10);
            timer.Tick += new EventHandler((s, e) =>
            {
                timer.Stop();
                stackPanel.IsEnabled = true;
            });
            timer.Start();
        }

        private bool CaptchaTest()
        {
            if (captchaCode != tbCaptcha.Text)
            {
                MessageBox.Show("неверный код проверки");
                BlockActivate();
            }
            else
            {
                CaptchaDeactivate();
            }

            return !isCaptchaRequire;
        }

        /************************
         *      CAPTCHA         *
         ************************/

        private void GenerateCaptchaCode()
        {
            captchaCode = "";
            for (int i = 0; i < 2; i++)
            {
                captchaCode += (char)random.Next(65, 91);
                captchaCode += (char)random.Next(48, 58);
            }
        }
        private void GenerateCaptchaImage()
        {
            CaptchaCanvas.Children.Clear();
            for (int i = 0; i < captchaCode.Length; i++)
            {
                CreateCaptchaLabel(i);
            }
            CreateCaptchaLine();
            CreateCaptchaLine();
        }
        private void CreateCaptchaLabel(int index)
        {
            Label label = new Label();
            label.Content = captchaCode[index];
            label.Height = 64;
            label.FontSize = random.Next(28, 36);
            label.FontStyle = FontStyles.Normal;
            label.FontWeight = FontWeights.Black;
            label.RenderTransformOrigin = new Point(0.5, 0.5);
            label.RenderTransform = new RotateTransform(random.Next(-30, 20));
            CaptchaCanvas.Children.Add(label);

            Canvas.SetLeft(label, 120 + (index * 30));
            Canvas.SetTop(label, random.Next(0, 10));
        }
        private void CreateCaptchaLine()
        {
            Line line = new Line();
            line.X1 = random.Next(100, 120);
            line.Y1 = random.Next(10, 54);
            line.X2 = random.Next(260, 280);
            line.Y2 = random.Next(10, 54);
            line.Stroke = new SolidColorBrush(Colors.Black);
            line.StrokeThickness = random.Next(2, 4);
            CaptchaCanvas.Children.Add(line);
        }

    }
}
