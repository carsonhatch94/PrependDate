# File Renamer - Date Prefix Manager

Prepend a year and month to files in folder. Will be formatted "YYYY_MM_", ex. "2025_09_"

Created as a part of my "One Session Project" series. 

Created this to make it easier to name the files for my family website. Reduces typing since I like to name my files like:

YYYY_MM_{People-in-photo}_{what-they-are-doing} ------> 2025_09_kids_playground

## Features

- **Add Date Prefix**: Add YYYY_MM_ prefix to files that don't already have one
- **Edit Date Prefix**: Change existing date prefixes with options to:
  - Change ALL date prefixes to a new date
  - Change only files matching a specific date pattern
- **Remove Date Prefix**: Remove YYYY_MM_ prefixes from files
- **Change File Path**: Switch between different folders during operation

## Usage

1. Run the application
2. Enter the folder path containing the files you want to rename
3. Select from the menu options:
   - **A** - Add Date Prefix
   - **E** - Edit Date Prefix  
   - **R** - Remove Date Prefix
   - **C** - Change file path
   - **Q** - Quit

## Requirements

- .NET 9
- C# 13.0

## Safety Features

- Checks for existing files to prevent overwrites
- Skips files that already have the correct prefix
- Confirmation prompt before processing files
- Error handling for individual file operations
