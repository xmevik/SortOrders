# SortOrders

## 1. Сборка и запуск проекта

Для сборки можно использовать либо Visual Studio, либо через командную строку

```bash
dotnet build
```

```bash
dotnet run -- <args>
```

## 2. Передача аргументов программы через командную строку

Программа поддерживает следующие команды:

 1. --help Выводит аргументы которые поддерживаются командной строкой.
 2. -c, --citydistrict Указывает район по которому надо отсортировать заказы, стоит учитывать, при работе из командной строки это обязательный параметр.
 3. -f, --firstdeliverydatetime Указывает время первой доставки по которому нужно отсортировать заказы, стоит учитывать, при работе из командной строки это обязательный параметр.
 4. --orders Путь к файлу с заказами. Стандартный путь: ./Files/orders.json
 5. --deliveryorder Путь к файлу с результатом выборки. Стандартный путь:  ./Files/deliveryorder.json
 6. --deliverylog Путь к файлу с логами. Стандартный путь: ./Files/logs.log

## 3. Передача аргументов через файл конфигурации

В папке с программой есть файл с размеченными полями, поля CityDistrict и FirstDeliveryDateTime являются обязательными при заполнении.

Стандартный вид файла:

```json
{
  "CityDistrict": "",
  "FirstDeliveryDateTime": "",
  "Orders": "",
  "DeliveryOrder": "",
  "DeliveryLog": ""
}
```

## 4. Запуск тестирования

Для тестирования можно воспользоваться стандартными средствами Visual Studio, либо через командную строку

```bash
dotnet test
```