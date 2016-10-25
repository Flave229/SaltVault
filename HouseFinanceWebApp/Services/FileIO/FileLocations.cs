using System;

namespace Services.FileIO
{
    public static class FilePath
    {
        public static string BaseDirectory = AppDomain.CurrentDomain.BaseDirectory;
        public static string Bills = BaseDirectory + @"Data\Bills\bills.txt";
        public static string Payments = BaseDirectory + @"Data\Payments\payments.txt";
        public static string People = BaseDirectory + @"Data\People\people.txt";
        public static string Shopping = BaseDirectory + @"Data\Shopping\ShoppingItems.txt";
    }
}
