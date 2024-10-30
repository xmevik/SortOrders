using CommandLine;

namespace SortOrders.Entities
{
    internal class Options
    {
        [Option(shortName: 'c', longName: "citydistrict", HelpText = "Район доставки по которому нужно отсортировать заказы.")]
        public string? CityDistrict { get; set; }

        [Option(shortName: 'f', longName: "firstdeliverydatetime", HelpText = "Время первой доставки по которому нужно отсортировать заказы.")]
        public string? FirstDeliveryDateTime { get; set; }

        [Option(longName: "orders", HelpText = "Путь к файлу с заказами.", Default = "/Files/orders.json")]
        public string? Orders { get; set; }

        [Option(longName: "deliveryorder", HelpText = "Путь к файлу с результатом выборки.", Default = "./Files/deliveryorder.json")]
        public string? DeliveryOrder { get; set; }

        [Option(longName: "deliverylog", HelpText = "Путь к файлу с логами.", Default = "./Files/logs.log")]
        public string? DeliveryLog { get; set; }
    }
}
