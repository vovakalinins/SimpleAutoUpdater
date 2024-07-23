# AutoUpdater

AutoUpdater is a C# utility class designed to automatically update a Windows Forms application by checking for the latest version online, downloading it, and applying the update seamlessly.

## Features

- Checks for the latest version from a specified URL.
- Downloads the latest version if an update is available.
- Extracts and applies the update.
- Restarts the application after updating.

## Getting Started

### Prerequisites

- .NET Framework with Windows Forms.
- Internet connection for checking and downloading updates.

### Installation

**Add `AutoUpdater` to your project:**

    Copy the `AutoUpdater` class file into your project directory.

### Usage

1. **Instantiate the `AutoUpdater` class:**

    ```csharp
    using System;
    using System.Windows.Forms;

    class Program
    {
        static void Main()
        {
            string currentVersion = "1.0.0"; // Replace with your current version
            AutoUpdater updater = new AutoUpdater(currentVersion);
            updater.CheckForUpdates();

            Application.Run(new Form1()); // Replace Form1 with your main form
        }
    }
    ```

2. **Configure the update URLs:**

    Update the following URLs in the `AutoUpdater` class to point to your version file and update zip file:

    ```csharp
    private string latestVersionUrl = "https://www.yourwebsite.com/version"; // GET LATEST VERSION OF PROGRAM
    private string updateFileUrl = "https://www.yourwebsite.com/latest.zip"; // LOCATION OF LATEST PROGRAM ZIP
    ```
