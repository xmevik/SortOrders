using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using NLog;
using SortOrders.Entities;
using System.Text;
using System.Text.Json;

namespace SortOrders
{
    public class OrdersManager
    {
        public List<DeliveryOrder> Orders { get; private set; }

        private readonly Logger _logger;

        public OrdersManager() : this([]) { }

        public OrdersManager(List<DeliveryOrder> orders)
        {
            _logger = LogManager.GetCurrentClassLogger();
            Orders = orders;
        }

        public void LoadOrdersFromFile(string filePath)
        {
            StringBuilder json = new();
            try
            {
                _logger.Debug("Производится попытка найти заказы");
                json.Append(File.ReadAllText(filePath));
            }
            catch(PathTooLongException ex)
            {
                _logger.Fatal("Слишком длинный путь до файла: {exception}", ex.Message);
                Environment.Exit(1);
            }
            catch(DirectoryNotFoundException ex)
            {
                _logger.Fatal("Путь до файла не найден: {exception}", ex.Message);
                Environment.Exit(1);
            }
            catch(FileNotFoundException ex)
            {
                _logger.Fatal("Файл не найден: {exception}", ex.Message);
                Environment.Exit(1);
            }
            catch(UnauthorizedAccessException ex)
            {
                _logger.Fatal("Ошибка несанкционированного доступа: {exception}", ex.Message);
                Environment.Exit(1);
            }
            catch(Exception ex)
            {
                _logger.Fatal("Ошибка загрузки данных, приложение будет закрыто: {exception}", ex.Message);
                Environment.Exit(1);
            }

            Orders = this.DeserializeOrders(json.ToString())?? [];
        }

        private List<DeliveryOrder>? DeserializeOrders(string json)
        {
            try
            {
                _logger.Debug("Производится попытка десериализовать заказы");
                return JsonSerializer.Deserialize<List<DeliveryOrder>>(json);
            }
            catch(ArgumentNullException ex)
            {
                _logger.Fatal("Ошибка загрузки заказов, вероятно в файле нет заказов: {exception}", ex.Message);
                Environment.Exit(1);
            }
            catch(JsonException ex)
            {
                _logger.Fatal("Неправильный формат файла, файл должен быть в формате JSON: {exception}", ex.Message);
                Environment.Exit(1);
            }
            catch (Exception ex)
            {
                _logger.Fatal("Ошибка загрузки данных: {exception}", ex.Message);

                try
                {
                    _logger.Info("Производится повторная попытка получить заказы...");
                    return JsonSerializer.Deserialize<List<DeliveryOrder>>(json);
                }
                catch(Exception exception)
                {
                    _logger.Fatal("Ошибка загрузки данных, приложение будет закрыто: {exception}", exception.Message);
                    Environment.Exit(1);
                }
            }

            return default;
        }

        public void FilterBy(string district, DateTime firstDeliveryTime)
        {
            _logger.Debug("Сортируем заказы");
            if(Orders.Count != 0)
            {
                DateTime timeLimit = firstDeliveryTime.AddMinutes(30);
                Orders = Orders.Where(s => s.District == district && s.DeliveryTime >= firstDeliveryTime && s.DeliveryTime <= timeLimit).ToList();
            }
        }

        public void FilterBy(string district)
        {
            _logger.Debug("Сортируем заказы");
            if (Orders.Count != 0)
            {
                Orders = Orders.Where(s => s.District == district).ToList();
            }
        }

        public void FilterBy(DateTime firstDeliveryTime)
        {
            _logger.Debug("Сортируем заказы");
            if (Orders.Count != 0)
            {
                DateTime timeLimit = firstDeliveryTime.AddMinutes(30);
                Orders = Orders.Where(s => s.DeliveryTime >= firstDeliveryTime && s.DeliveryTime <= timeLimit).ToList();
            }
        }

        public void SaveOrdersToFile(string filePath)
        {
            try
            {
                var json = this.SerializeOrders();
                File.WriteAllText(filePath, json);
            }
            catch (PathTooLongException ex)
            {
                _logger.Fatal("Слишком длинный путь до файла: {exception}", ex.Message);
                Environment.Exit(1);
            }
            catch (DirectoryNotFoundException ex)
            {
                _logger.Fatal("Путь до файла не найден: {exception}", ex.Message);
                Environment.Exit(1);
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.Fatal("Ошибка несанкционированного доступа: {exception}", ex.Message);
                Environment.Exit(1);
            }
            catch (Exception ex)
            {
                _logger.Fatal("Ошибка сохранения данных, приложение будет закрыто: {exception}", ex.Message);
                Environment.Exit(1);
            }
        }

        private string SerializeOrders()
        {
            return JsonSerializer.Serialize(Orders);
        }
    }
}
