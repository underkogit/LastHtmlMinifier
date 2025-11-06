namespace HtmlMinifier;

public static class ConsoleFileManager
{
    public static string GetFilePath(string storageFile)
    {
        var filePath = string.Empty;

        if (File.Exists(storageFile))
        {
            filePath = File.ReadAllText(storageFile);
            Console.WriteLine($"Найден сохранённый путь к файлу: {filePath}");
        }

        while (true)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                Console.WriteLine("Введите путь к файлу: ");
                filePath = Console.ReadLine().Replace("\"", string.Empty).Trim();
            }

            if (File.Exists(filePath))
            {
                Console.WriteLine("Файл существует. Сохраняем путь.");
                File.WriteAllText(storageFile, filePath);
                return filePath;
            }

            Console.WriteLine("Файл не найден. Пожалуйста, попробуйте снова.");
            filePath = string.Empty;
        }
    }
}