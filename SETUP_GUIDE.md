# Setting Up FreshNoodle Locally

This guide will help you clone the **FreshNoodle** repository to your local machine and get it running. This application uses .NET 9 and includes a Blazor WebAssembly frontend with an ASP.NET Core Web API backend.

## Prerequisites

Before starting, ensure you have the following installed on your computer:

1.  **Git**: [Download and Install Git](https://git-scm.com/downloads)
2.  **.NET 9.0 SDK**: [Download .NET 9.0](https://dotnet.microsoft.com/download/dotnet/9.0)
3.  **Visual Studio 2022** (Recommended) or **Visual Studio Code**.

---

## Step 1: Clone the Repository

1.  Open your terminal (Command Prompt, PowerShell, or Git Bash).
2.  Navigate to the folder where you want to keep the project (e.g., `Documents` or `Projects`).
    ```powershell
    cd Documents
    ```
3.  Run the clone command:
    ```powershell
    git clone https://github.com/ytanya/FreshNoodle.git
    ```
4.  Navigate into the newly created folder:
    ```powershell
    cd FreshNoodle
    ```

---

## Step 2: Open the Project

### Using Visual Studio 2022 (Easiest)
1.  Open **Visual Studio 2022**.
2.  Click **"Open a project or solution"**.
3.  Navigate to the `FreshNoodle` folder you just cloned.
4.  Select the `FreshNoodle.sln` file and click **Open**.

### Using Visual Studio Code
1.  Open **Visual Studio Code**.
2.  Go to **File > Open Folder...**.
3.  Select the `FreshNoodle` folder.
4.  It is recommended to install the "C# Dev Kit" extension for the best experience.

---

## Step 3: Setup and Run

1.  **Restore dependencies**:
    Open the terminal in your IDE or use your command line (inside the `FreshNoodle` folder) and run:
    ```powershell
    dotnet restore
    ```

2.  **Create the Database**:
    The application uses a local SQL Server database. It will try to create it automatically when you start the API.
    *   Ensure you have **SQL Server LocalDB** installed (included with Visual Studio).

3.  **Run the Application**:
    You need to run both the **API** and the **UI**.

    **In Visual Studio:**
    1.  Right-click on the Solution (`FreshNoodle`) -> **Configure Startup Projects**.
    2.  Select **Multiple startup projects**.
    3.  Set `FreshNoodle.API.Authentication` to **Start**.
    4.  Set `FreshNoodle.UI` to **Start**.
    5.  Press **F5** or click the green "Start" button.

    **Using CLI (Command Line):**
    Open two separate terminals in the `FreshNoodle` folder.

    *Terminal 1 (API):*
    ```powershell
    cd FreshNoodle.API.Authentication
    dotnet run
    ```

    *Terminal 2 (UI):*
    ```powershell
    cd FreshNoodle.UI
    dotnet run
    ```

---

## Common Issues

*   **"dotnet not found"**: Ensure you installed the .NET 9.0 SDK and restarted your terminal.
*   **Database errors**: Make sure SQL Server LocalDB is running. You can reset it by running `sqllocaldb stop && sqllocaldb start` in your terminal.
