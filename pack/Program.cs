using System.CommandLine;


var bundleCommand = new Command("bundle", "Bundle code files to single file");

var outputBundleOption = new Option<FileInfo>(new[]{"--output","-o"}, "Output file path and name");
var noteBundleOption = new Option<bool>(new[]{"--note","-n"}, "Write source file path as note");
var sortBundleOption = new Option<string>(new[]{"--sort","-s"}, ()=>"name", "Sort by name or extension (ext)");
var removeBundleOption = new Option<bool>(new[]{"--remove-empty-lines","--rml","-r"}, "Remove empty lines");
var authorBundleOption = new Option<string>(new[]{"--author","-a"}, "Write author name in head");
var languageBundleOption = new Option<string[]>(new[]{"--language","--lang","-l"}, ()=>["all"], "List [] of programming languages to include. Use 'all' to include all files")
{
    AllowMultipleArgumentsPerToken = true,
};

bundleCommand.AddOption(outputBundleOption);
bundleCommand.AddOption(noteBundleOption);
bundleCommand.AddOption(sortBundleOption);
bundleCommand.AddOption(removeBundleOption);
bundleCommand.AddOption(authorBundleOption);
bundleCommand.AddOption(languageBundleOption);

bundleCommand.SetHandler((output,note,sort,removeEmptyLines,author,languages) =>
{
    string currentDirectory = Path.GetFullPath(".");
    string outputPath = Path.GetFullPath(output.FullName);
    if (outputPath.StartsWith(currentDirectory, StringComparison.OrdinalIgnoreCase))
    {
        Console.WriteLine("Error: The output file must be located outside of the current directory");
        return;
    }
    if (currentDirectory.ToLower().Contains("bin") || currentDirectory.ToLower().Contains("debug"))
    {
        Console.WriteLine("Error: Can't bundle files from bin/debug folders");
        return;
    }

    if (sort!="name" && sort!="ext" && sort!="extension")
    {
        Console.WriteLine("Error: sort can recieve name/extension/ext");
        return;
    }

    try
    {
        using (StreamWriter writer = new StreamWriter(output.FullName, false))
        {
            if (author!=null) writer.WriteLine("// Author: "+author);
            string[] files = Directory.GetFiles(".");
            files = sort switch
            {
                "name" => files.OrderBy(f => Path.GetFileName(f)).ToArray(),
                _ => files.OrderBy(f => Path.GetExtension(f)).ToArray()
            };
            if (!languages.Contains("all", StringComparer.OrdinalIgnoreCase))
            {
                var normalizedLanguages = languages.Select(lang => lang.Trim().ToLower()).ToArray();
                var extensions = normalizedLanguages.Select(lang => $".{lang}");
                files = files.Where(file => extensions.Contains(Path.GetExtension(file).ToLower())).ToArray();
            }
            foreach (string file in files)
            {
                if(note) writer.WriteLine("// "+file);
                string[] lines = File.ReadAllLines(file);
                if (removeEmptyLines) lines = lines.Where(line => !string.IsNullOrWhiteSpace(line)).ToArray();
                foreach (var line in lines)
                {
                    writer.WriteLine(line);
                }
            }
        }
        Console.WriteLine("Success: "+output);
    }
    catch (DirectoryNotFoundException)
    {
        Console.WriteLine("Error: file path is invalid");
    }
    catch (Exception ex)
    {
        Console.WriteLine("Error: "+ex.Message);
    }
},outputBundleOption, noteBundleOption, sortBundleOption, removeBundleOption, authorBundleOption, languageBundleOption);

var rootCommand = new RootCommand("Root Command for File Bundler CLI");
rootCommand.AddCommand(bundleCommand);

rootCommand.InvokeAsync(args);