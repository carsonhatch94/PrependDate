Console.WriteLine("File Renamer - Prepend Year and Month");
Console.WriteLine("=====================================");

// Get folder path from user
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

// Get year from user
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

// Get month from user
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

// Format the prefix
string prefix = $"{year:D4}_{month:D2}_";

try
{
    // Get all files in the directory
    string[] files = Directory.GetFiles(folderPath);

    if (files.Length == 0)
    {
        Console.WriteLine("No files found in the specified folder.");
        return;
    }

    Console.WriteLine($"\nFound {files.Length} file(s). Renaming with prefix: {prefix}");
    Console.WriteLine("Press 'Y' to continue or any other key to cancel:");

    if (Console.ReadKey().Key != ConsoleKey.Y)
    {
        Console.WriteLine("\nOperation cancelled.");
        return;
    }

    Console.WriteLine("\n");

    int renamedCount = 0;
    foreach (string filePath in files)
    {
        try
        {
            string fileName = Path.GetFileName(filePath);
            string directory = Path.GetDirectoryName(filePath) ?? folderPath;

            // Skip if file already has the prefix to avoid duplicate prefixes
            if (fileName.StartsWith(prefix))
            {
                Console.WriteLine($"Skipped (already has prefix): {fileName}");
                continue;
            }

            string newFileName = prefix + fileName;
            string newFilePath = Path.Combine(directory, newFileName);

            // Check if target file already exists
            if (File.Exists(newFilePath))
            {
                Console.WriteLine($"Skipped (target exists): {fileName} -> {newFileName}");
                continue;
            }

            File.Move(filePath, newFilePath);
            Console.WriteLine($"Renamed: {fileName} -> {newFileName}");
            renamedCount++;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error renaming {Path.GetFileName(filePath)}: {ex.Message}");
        }
    }

    Console.WriteLine($"\nOperation completed. {renamedCount} file(s) renamed successfully.");
}
catch (Exception ex)
{
    Console.WriteLine($"Error accessing folder: {ex.Message}");
}

Console.WriteLine("\nPress any key to exit...");
Console.ReadKey();