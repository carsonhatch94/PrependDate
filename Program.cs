using System.Text.RegularExpressions;

Console.WriteLine("File Renamer - Date Prefix Manager");
Console.WriteLine("==================================");

string folderPath = GetFolderPath();

while (true)
{
    Console.WriteLine($"\nCurrent folder: {folderPath}");
    Console.WriteLine("\nSelect an operation:");
    Console.WriteLine("A - Add Date Prefix");
    Console.WriteLine("E - Edit Date Prefix");
    Console.WriteLine("R - Remove Date Prefix");
    Console.WriteLine("C - Change file path");
    Console.WriteLine("Q - Quit");
    Console.Write("\nEnter your choice: ");

    var choice = Console.ReadKey().KeyChar;
    Console.WriteLine("\n");

    switch (char.ToUpper(choice))
    {
        case 'A':
            AddDatePrefix(folderPath);
            break;
        case 'E':
            EditDatePrefix(folderPath);
            break;
        case 'R':
            RemoveDatePrefix(folderPath);
            break;
        case 'C':
            folderPath = ChangeFolderPath();
            break;
        case 'Q':
            Console.WriteLine("Goodbye!");
            return;
        default:
            Console.WriteLine("Invalid choice. Please try again.");
            break;
    }
}

static string GetFolderPath()
{
    string folderPath;
    do
    {
        Console.Write("Enter the folder path: ");
        folderPath = Console.ReadLine()?.Trim() ?? string.Empty;

        if (string.IsNullOrEmpty(folderPath))
        {
            Console.WriteLine("Please enter a valid folder path.");
            continue;
        }

        if (!Directory.Exists(folderPath))
        {
            Console.WriteLine("The specified folder does not exist. Please try again.");
            folderPath = string.Empty;
        }
    } while (string.IsNullOrEmpty(folderPath));

    return folderPath;
}

static string ChangeFolderPath()
{
    Console.WriteLine("=== Change File Path ===");
    return GetFolderPath();
}

static (int year, int month) GetDateInput()
{
    int year;
    do
    {
        Console.Write("Enter the year (4 digits): ");
        string yearInput = Console.ReadLine()?.Trim() ?? string.Empty;

        if (!int.TryParse(yearInput, out year) || year < 1000 || year > 9999)
        {
            Console.WriteLine("Please enter a valid 4-digit year (1000-9999).");
            year = 0;
        }
    } while (year == 0);

    int month;
    do
    {
        Console.Write("Enter the month (1-12): ");
        string monthInput = Console.ReadLine()?.Trim() ?? string.Empty;

        if (!int.TryParse(monthInput, out month) || month < 1 || month > 12)
        {
            Console.WriteLine("Please enter a valid month (1-12).");
            month = 0;
        }
    } while (month == 0);

    return (year, month);
}

static void AddDatePrefix(string folderPath)
{
    Console.WriteLine("=== Add Date Prefix ===");
    var (year, month) = GetDateInput();
    string prefix = $"{year:D4}_{month:D2}_";

    ProcessFiles(folderPath, (fileName, filePath) =>
    {
        string directory = Path.GetDirectoryName(filePath) ?? folderPath;

        // Skip if file already has any date prefix
        if (Regex.IsMatch(fileName, @"^\d{4}_\d{2}_"))
        {
            Console.WriteLine($"Skipped (already has date prefix): {fileName}");
            return false;
        }

        string newFileName = prefix + fileName;
        string newFilePath = Path.Combine(directory, newFileName);

        if (File.Exists(newFilePath))
        {
            Console.WriteLine($"Skipped (target exists): {fileName} -> {newFileName}");
            return false;
        }

        File.Move(filePath, newFilePath);
        Console.WriteLine($"Renamed: {fileName} -> {newFileName}");
        return true;
    });
}

static void EditDatePrefix(string folderPath)
{
    Console.WriteLine("=== Edit Date Prefix ===");
    var (newYear, newMonth) = GetDateInput();
    string newPrefix = $"{newYear:D4}_{newMonth:D2}_";

    Console.WriteLine("\nEdit options:");
    Console.WriteLine("1 - Change ALL date prefixes to the new date");
    Console.WriteLine("2 - Change only files matching a specific date pattern");
    Console.Write("Enter your choice (1 or 2): ");

    var editChoice = Console.ReadKey().KeyChar;
    Console.WriteLine("\n");

    string? targetPrefix = null;
    if (editChoice == '2')
    {
        Console.WriteLine("Enter the date pattern to match:");
        var (targetYear, targetMonth) = GetDateInput();
        targetPrefix = $"{targetYear:D4}_{targetMonth:D2}_";
    }

    ProcessFiles(folderPath, (fileName, filePath) =>
    {
        string directory = Path.GetDirectoryName(filePath) ?? folderPath;

        // Check if file has a date prefix
        var match = Regex.Match(fileName, @"^(\d{4}_\d{2}_)(.+)$");
        if (!match.Success)
        {
            Console.WriteLine($"Skipped (no date prefix): {fileName}");
            return false;
        }

        string currentPrefix = match.Groups[1].Value;
        string baseFileName = match.Groups[2].Value;

        // If targeting specific pattern, check if it matches
        if (targetPrefix != null && currentPrefix != targetPrefix)
        {
            Console.WriteLine($"Skipped (doesn't match target pattern): {fileName}");
            return false;
        }

        // Skip if already has the new prefix
        if (currentPrefix == newPrefix)
        {
            Console.WriteLine($"Skipped (already has new prefix): {fileName}");
            return false;
        }

        string newFileName = newPrefix + baseFileName;
        string newFilePath = Path.Combine(directory, newFileName);

        if (File.Exists(newFilePath))
        {
            Console.WriteLine($"Skipped (target exists): {fileName} -> {newFileName}");
            return false;
        }

        File.Move(filePath, newFilePath);
        Console.WriteLine($"Renamed: {fileName} -> {newFileName}");
        return true;
    });
}

static void RemoveDatePrefix(string folderPath)
{
    Console.WriteLine("=== Remove Date Prefix ===");

    ProcessFiles(folderPath, (fileName, filePath) =>
    {
        string directory = Path.GetDirectoryName(filePath) ?? folderPath;

        // Check if file has a date prefix matching YYYY_MM_ pattern
        var match = Regex.Match(fileName, @"^(\d{4}_\d{2}_)(.+)$");
        if (!match.Success)
        {
            Console.WriteLine($"Skipped (no date prefix): {fileName}");
            return false;
        }

        string baseFileName = match.Groups[2].Value;
        string newFilePath = Path.Combine(directory, baseFileName);

        if (File.Exists(newFilePath))
        {
            Console.WriteLine($"Skipped (target exists): {fileName} -> {baseFileName}");
            return false;
        }

        File.Move(filePath, newFilePath);
        Console.WriteLine($"Renamed: {fileName} -> {baseFileName}");
        return true;
    });
}

static void ProcessFiles(string folderPath, Func<string, string, bool> fileProcessor)
{
    try
    {
        string[] files = Directory.GetFiles(folderPath);

        if (files.Length == 0)
        {
            Console.WriteLine("No files found in the specified folder.");
            return;
        }

        Console.WriteLine($"\nFound {files.Length} file(s).");
        Console.WriteLine("Press 'Y' to continue or any other key to cancel:");

        if (Console.ReadKey().Key != ConsoleKey.Y)
        {
            Console.WriteLine("\nOperation cancelled.");
            return;
        }

        Console.WriteLine("\n");

        int processedCount = 0;
        foreach (string filePath in files)
        {
            try
            {
                string fileName = Path.GetFileName(filePath);
                if (fileProcessor(fileName, filePath))
                {
                    processedCount++;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error processing {Path.GetFileName(filePath)}: {ex.Message}");
            }
        }

        Console.WriteLine($"\nOperation completed. {processedCount} file(s) processed successfully.");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error accessing folder: {ex.Message}");
    }

    Console.WriteLine("\nPress any key to continue...");
    Console.ReadKey();
}