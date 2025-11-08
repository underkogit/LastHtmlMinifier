using System.Text.RegularExpressions;
using NUglify;

namespace HtmlMinifierOneCs;

public class HtmlScriptProcessor
{
    private readonly string _pathHtmlFile;
    private readonly string _pathOutputHtmlFile;

    public HtmlScriptProcessor(string htmlFilePath, string outputFilePath)
    {
        _pathHtmlFile = htmlFilePath;
        _pathOutputHtmlFile = outputFilePath;
    }

    private string GenerateCppHeader(string code)
    {
        return @"
#ifndef WEBUI_H
#define WEBUI_H
const char* htmlContent = R""rawliteral(
{{WEBUI}}
)rawliteral"";
#endif
".Replace("{{WEBUI}}", code);
    }

    private string ExtractAllJsScripts(string directoryFile, string content)
    {
        var scripts = Regex.Matches(content, "((<script>)|(<script))[\\w\\W]+?<\\/script>").Select(a => a.Value)
            .ToList();

        var scriptsCode = new List<string>();

        foreach (var script1 in scripts)
            if (Regex.IsMatch(script1, @"((<script>)|(<script))[\w\W]+?><\/script>"))
            {
                var srcPath = Path.GetFullPath(
                    Path.Combine(directoryFile,
                        Regex.Match(script1, @"src=?"".+?""").Value.Replace("src=\"", string.Empty)
                            .Replace("\"", string.Empty))
                );
                if (File.Exists(srcPath))
                {
                    scriptsCode.Add(File.ReadAllText(srcPath));
                }
                else
                {
                    srcPath = script1.Replace("<script src=", string.Empty).Replace("defer></script>", string.Empty);
                    srcPath = Path.GetFullPath(
                        Path.Combine(directoryFile, srcPath));
                    if (File.Exists(srcPath))
                        scriptsCode.Add(File.ReadAllText(srcPath));
                }
            }
            else
            {
                scriptsCode.Add(script1.Replace("<script>", string.Empty).Replace("</script>", string.Empty));
            }

        return string.Join("\n\n", scriptsCode);
    }

    private string RmoveAllScripts(string content)
    {
        return Regex.Replace(content, "((<script>)|(<script))[\\w\\W]+?<\\/script>", string.Empty);
    }


    public void ProcessHtml()
    {
        var compactHtmlCode = Uglify.Html(File.ReadAllText(_pathHtmlFile)).Code;

        string jsContentAll = ExtractAllJsScripts(Path.GetFullPath(Path.GetDirectoryName(_pathHtmlFile)), compactHtmlCode);


        compactHtmlCode = $"{RmoveAllScripts(compactHtmlCode)}<script>{jsContentAll}</script>";
        compactHtmlCode = Uglify.Html(compactHtmlCode).Code;

        var result = GenerateCppHeader(compactHtmlCode);


        File.WriteAllText(_pathOutputHtmlFile, result);
    }
}