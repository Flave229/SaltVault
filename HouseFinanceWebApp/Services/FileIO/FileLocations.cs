using System;

namespace Services.FileIO
{
    public enum FilePath
    {
        Bills,
        Payments,
        People,
        Shopping
    }

    public static class FilePathToString
    {
        public static string ToString(FilePath filePath)
        {
            var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;

            switch (filePath)
            {
                case FilePath.Bills:
                    return baseDirectory + @"Data\Bills\bills.txt";
                case FilePath.Payments:
                    return baseDirectory + @"Data\Payments\payments.txt";
                case FilePath.People:
                    return baseDirectory + @"Data\People\people.txt";
                case FilePath.Shopping:
                    return baseDirectory + @"Data\Shopping\ShoppingItems.txt";
                default:
                    return "";
            }
        }
    }
}
