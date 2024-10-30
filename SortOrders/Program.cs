using CommandLine;
using Microsoft.Extensions.Configuration;
using NLog;
using NLog.Config;
using NLog.Targets;
using SortOrders.Entities;
using System.Globalization;
using System.Reflection;

namespace SortOrders
{
    internal class Program
    {
        static string OrdersPath = "./Files/orders.json";
        static readonly string LogPath = "./Files/logs.log";
        static string DeliveryOrderPath = "./Files/deliveryorder.json";

        static string FirstDeliveryDateTime = null!;
        static string CityDistrict = null!;

        static Logger _logger = null!;
        static readonly OrdersManager manager = new();

        static void Main(string[] args)
        {
            var resultArgs = Parser.Default.ParseArguments<Options>(args);
            IConfiguration conf = new ConfigurationBuilder().AddJsonFile("env.json").Build();
            BuildLoggers(resultArgs.Value, conf);

            _logger.Debug("Путь записи логов: {logpath}", (_logger.Factory.Configuration.LoggingRules.FirstOrDefault().Targets.FirstOrDefault() as FileTarget).FileName);

            if (resultArgs.Value.FirstDeliveryDateTime != null && resultArgs.Value.CityDistrict != null)
            {
                if (resultArgs.Errors.Any() == false)
                {
                    _logger.Debug("Парсинг данных из аргументов командной строки");

                    Options options = resultArgs.Value;

                    OrdersPath = options.Orders?? OrdersPath;
                    DeliveryOrderPath = options.DeliveryOrder ?? DeliveryOrderPath;
                    FirstDeliveryDateTime = options.FirstDeliveryDateTime;
                    CityDistrict = options.CityDistrict;
                }
            }
            else if (resultArgs.Value.FirstDeliveryDateTime != null ^ resultArgs.Value.CityDistrict != null) // XOR ^
            {
                _logger.Fatal("Введен один из параметров, но пропущен второй. Нужно использовать оба или использовать файл конфигурации");
                Environment.Exit(1);
            }
            else
            {
                _logger.Debug("Парсинг данных из файла конфигурации");

                OrdersPath = conf[nameof(OrdersPath)] != "" ? conf[nameof(OrdersPath)] : OrdersPath;
                DeliveryOrderPath = conf[nameof(DeliveryOrderPath)] != "" ? conf[nameof(DeliveryOrderPath)] : DeliveryOrderPath;

                if (conf[nameof(FirstDeliveryDateTime)] != "" && conf[nameof(CityDistrict)] != "")
                {
                    FirstDeliveryDateTime = conf[nameof(FirstDeliveryDateTime)];
                    CityDistrict = conf[nameof(CityDistrict)];
                }
                else
                {
                    _logger.Fatal($"Ошибка в аргументах файла конифгурации, проверьте наличие полей {nameof(FirstDeliveryDateTime)}, {nameof(CityDistrict)}. Или используйте другой метод работы программы");
                    Environment.Exit(1);
                }

            }

            ProcessOrders();
        }

        private static void BuildLoggers(Options options, IConfiguration conf)
        {
            if(options.DeliveryLog != null)
            {
                AddNLogRules(options.DeliveryLog);
            }
            else if (conf[nameof(LogPath)] != null)
            {
                AddNLogRules(conf[nameof(LogPath)]);
            }
            else
            {
                AddNLogRules(LogPath);
            }

            _logger = LogManager.GetCurrentClassLogger();
        }

        private static void AddNLogRules(string? path)
        {
            var config = new LoggingConfiguration();
            string layout = "${longdate} [${level}] ${message} | ${all-event-properties} ${exception:format=tostring}";

            var logfile = new FileTarget("logfile")
            {
                FileName = path,
                Layout = layout,
            };

            var logconsole = new ConsoleTarget("logconsole")
            {
                Layout = layout,
            };

            config.AddRule(NLog.LogLevel.Info, NLog.LogLevel.Fatal, logfile);
            config.AddRule(NLog.LogLevel.Debug, NLog.LogLevel.Fatal, logconsole);

            LogManager.Configuration = config;
        }

        private static void ProcessOrders()
        {
            manager.LoadOrdersFromFile(OrdersPath);
            DateTime firstDeliveryDateTime = default;

            try
            {
                firstDeliveryDateTime = DateTime.ParseExact(FirstDeliveryDateTime, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
            }
            catch (FormatException ex)
            {
                _logger.Fatal("Ошибка парсинга формата времени доставки, проверьте пожалуйста правильность написания: {FirstDeliveryDateTime} - yyyy-MM-dd HH:mm:ss\r\n{exception}", FirstDeliveryDateTime, ex.Message);
                Environment.Exit(1);
            }

            manager.FilterBy(district: CityDistrict, firstDeliveryTime: firstDeliveryDateTime);

            manager.SaveOrdersToFile(DeliveryOrderPath);
        }
    }
}

