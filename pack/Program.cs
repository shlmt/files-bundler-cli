using System.CommandLine;
using System.Text;


var bundleCommand = new Command("bundle", "Bundle code files to single file");

var outputBundleOption = new Option<FileInfo>(new[]{"--output","-o"}, "File path and name for the bundled output");
var noteBundleOption = new Option<bool>(new[]{"--note","-n"}, "Include the source file path as a comment");
var sortBundleOption = new Option<string>(new[]{"--sort","-s"}, ()=>"name", "Sort files by name (default) or extension");
var removeBundleOption = new Option<bool>(new[]{"--remove-empty-lines","--rml","-r"}, "Remove empty lines");
var authorBundleOption = new Option<string>(new[]{"--author","-a"}, "Add the author's name as a header comment");
var languageBundleOption = new Option<string[]>(new[]{"--language","--lang","-l"}, ()=>["all"], "Specify programming languages to include. Use 'all' to include all files (default)")
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

    if (sort!="name" && sort!="ext" && sort!="extension")
    {
        Console.WriteLine("Error: sort can recieve name/extension/ext");
        return;
    }

    try
    {
        using (StreamWriter writer = new StreamWriter(output.FullName, false))
        {
            string[] files = Directory.GetFiles(".", "*", SearchOption.AllDirectories);

            string[] excludedDirs = { "bin", "debug", "object", "node_modules" };
            files = files.Where(file =>
            {
                var directory = Path.GetDirectoryName(file);
                return directory != null && !excludedDirs.Any(dir => directory.Contains(dir, StringComparison.OrdinalIgnoreCase));
            }).ToArray();

            if (!languages.Contains("all", StringComparer.OrdinalIgnoreCase))
            {
                var normalizedLanguages = languages.Select(lang => lang.Trim().ToLower()).ToArray();
                var extensions = normalizedLanguages.Select(lang => $".{lang}");
                files = files.Where(file => extensions.Contains(Path.GetExtension(file).ToLower())).ToArray();
            }

            files = sort switch
            {
                "name" => files.OrderBy(f => Path.GetFileName(f)).ToArray(),
                _ => files.OrderBy(f => Path.GetExtension(f)).ToArray()
            };

            if (author != null) writer.WriteLine("// Author: " + author);

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



var createRspCommand = new Command("create-rsp","Create response file for bundle command");

createRspCommand.SetHandler(() =>
{
    string output = "";
    while (string.IsNullOrEmpty(output))
    {
        Console.WriteLine("Enter output file path:");
        output = Console.ReadLine();
    }

    string note = "";
    while (note!="n" && note!="y")
    {
        Console.WriteLine("Write files names? press y, else n or enter to skip");
        note = Console.ReadLine();
        if (string.IsNullOrEmpty(note))
        {
            note = "n";
            break;
        }
    }

    string sort = "";
    while (sort!="ext" && sort!="extension" && sort!="name")
    {
        Console.WriteLine("Sort by: extension/ext or name. press enter to default(name)");
        sort = Console.ReadLine();
        if (string.IsNullOrEmpty(sort))
        {
            sort = "name";
            break;
        }
    }

    string author = "";
    Console.WriteLine("Enter author name if you want. else skip");
    author = Console.ReadLine();


    string remove = "";
    while (remove!="n" && remove!="y")
    {
        Console.WriteLine("Remove empty lines? press y, else n or skip");
        remove = Console.ReadLine();
        if (string.IsNullOrEmpty(remove))
        {
            remove = "n";
            break;
        }
    }

    Console.WriteLine("Write list of programming languages to include, separate by spaces.\n Write 'all' or skip to include all files");
    string languages = Console.ReadLine();
    if (string.IsNullOrEmpty(languages))
        languages = "all";

    string command = $"bundle --output \"{output}\"" +
                     (note == "y" ? " --note" : "") +
                     $" --sort {sort}" +
                     (!string.IsNullOrEmpty(author) ? $" --author \"{author}\"" : "") +
                     (remove == "y" ? " --remove-empty-lines" : "") +
                     $" --language {languages}";

    File.WriteAllText("bundle.rsp", command, Encoding.UTF8);
    Console.WriteLine($"Response file created successfully");
    Console.WriteLine($"Run the command using:  @bundle.rsp");
});



var rootCommand = new RootCommand("Root Command for File Bundler CLI");
rootCommand.AddCommand(bundleCommand);
rootCommand.AddCommand(createRspCommand);

rootCommand.InvokeAsync(args);