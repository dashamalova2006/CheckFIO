using System;
using System.Linq;
using System.Net.Http; // (Подключаем библиотеку) Нужен для работы с API и интернет-запросами
using System.Windows;

namespace CheckFIO
{
    public partial class MainWindow : Window
    {
        // Создаем переменную fullName - в ней будет храниться ФИО, которое придет с API
        private string fullName = "";

        public MainWindow()
        {
            InitializeComponent();
        }

        // Метод вызывается при нажатии кнопки "Получить данные"
        // Перед void пишем async - это необходимо потому что запрос к API выполняется не мгновенно и с ожиданием ответам сервера (API)
        private async void GetDataButton_Click(object sender, RoutedEventArgs e)
        {
            // Пробуем получить данные от API
            try
            {
                // Создаем объект HttpClient - Он нужен для отправки запросов в интернет/API
                using (HttpClient client = new HttpClient())
                {
                    // Ссылка на API
                    string url = "http://localhost:4444/TransferSimulator/fullName";

                    // Отправляем GET-запрос на API
                    // await - ждем пока сервер ответит
                    // Ответ сохраняем в переменную jsonAnswer
                    string jsonAnswer = await client.GetStringAsync(url);


                    // Пример того, что приходит с API:
                    //
                    // {
                    //   "value":"Иванов Иван Иванович"
                    // }
                    //
                    // Нам нужно вытащить только:
                    // Иванов Иван Иванович


                    // Поэтому удаляем лишний текст
                    jsonAnswer = jsonAnswer.Replace("{", "");
                    jsonAnswer = jsonAnswer.Replace("}", "");
                    jsonAnswer = jsonAnswer.Replace("\"", "");
                    jsonAnswer = jsonAnswer.Replace("value :", "");

                    // Убираем пробелы по краям
                    fullName = jsonAnswer.Trim();

                    // Показываем ФИО в TextBox
                    txtBoxFullNameText.Text = fullName;

                    // Очищаем старый результат
                    txtBoxResultText.Text = "";
                }
            }
            // Если произошла ошибка: например API выключен или сервер недоступен
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка: " + ex.Message, "Ошибка API", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Метод вызывается при нажатии кнопки "Отправить результат теста"
        private void CheckButton_Click(object sender, RoutedEventArgs e)
        {
            // Проверяем есть ли вообще ФИО
            if (string.IsNullOrWhiteSpace(fullName))
            {
                txtBoxResultText.Text = "Сначала получите данные";

                // return завершает выполнение метода, то есть код ниже уже не выполнится
                return;
            }


            // Проверка на запрещенные символы

            // Строка с запрещенными символами
            string forbiddenSymbols = "0123456789!@#$%^&*():;_-+=[]{}<>?/|\\&";


            // Intersect сравнивает две строки и ищет одинаковые символы
            // Если количество совпадений больше 0 - значит запрещенные символы найдены
            if (fullName.Intersect(forbiddenSymbols).Count() > 0)
            {
                txtBoxResultText.Text = "ФИО содержит запрещенные символы";
                return;
            }

            // Если все проверки пройдены
            txtBoxResultText.Text = "ФИО корректно";
        }
    }
}