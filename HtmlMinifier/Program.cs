using System.Diagnostics;
using HtmlMinifier;

internal class Program
{
    private static void Main(string[] args)
    {
        
        
        
        var nodeRunner = new NodeRunner();
        var sotorageDirectoryScript = Path.GetFullPath("Scripts");
        var directoryCache = Path.GetFullPath("cache");

        
       // NodeRunner.InstallPackages("html-minifier terser" , sotorageDirectoryScript);
        
        string outputCppHFile = "C:\\Users\\UnderKo\\Documents\\PlatformIO\\Projects\\ESP32WebPanel\\include\\webui.h";
        string outputHtmlViewTest = Path.GetFullPath("test.html");
        if (!Directory.Exists(sotorageDirectoryScript))
        {
            Directory.CreateDirectory(sotorageDirectoryScript);
        }
        if (!Directory.Exists(directoryCache))
        {
            Directory.CreateDirectory(directoryCache);
        }

        var sotorageNodeScriptHtml = Path.Combine(sotorageDirectoryScript, @"MinifierHTML.js");
        var sotorageNodeScriptJs = Path.Combine(sotorageDirectoryScript, @"MinifierJS.js");
        var storageFile = "filePath.txt";
        var inputPath = ConsoleFileManager.GetFilePath(storageFile);

        void Build()
        {
            var pathMiniJs = nodeRunner.CreateScriptsJs(inputPath, directoryCache, sotorageNodeScriptJs);
            nodeRunner.RunNodeScript(sotorageNodeScriptJs);
            var pathMiniHtml = nodeRunner.CreateScriptsHtml(inputPath, directoryCache, sotorageNodeScriptHtml);
            nodeRunner.RunNodeScript(sotorageNodeScriptHtml);
            nodeRunner.CopressCodeToFile(pathMiniJs, pathMiniHtml, outputHtmlViewTest);
            
            string code = nodeRunner.CopressCode(pathMiniJs, pathMiniHtml);
            File.WriteAllText(outputCppHFile, nodeRunner.GetCppH(code));

        }

        if (!string.IsNullOrEmpty(inputPath))
        {
            var directoryPath = Path.GetDirectoryName(inputPath);
            Console.WriteLine($"Отслеживание изменений в директории: {directoryPath}");
            Build();

            using (var watcher = new FileSystemWatcher(directoryPath))
            {
                watcher.NotifyFilter = NotifyFilters.FileName | NotifyFilters.LastWrite | NotifyFilters.DirectoryName;
                watcher.Filter = "*.*";
                watcher.Changed += OnChanged;
                watcher.Created += OnChanged;
                watcher.Deleted += OnChanged;
                watcher.EnableRaisingEvents = true;
                Console.WriteLine("Нажмите 'q' для выхода.");
                while (Console.Read() != 'q') ;
            }
        }

        void OnChanged(object sender, FileSystemEventArgs e)
        {
            Console.WriteLine($"Файл: {e.FullPath} был {e.ChangeType}");
            Build();
        }
    }
}
