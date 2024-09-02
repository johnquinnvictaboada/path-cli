# path-cli

`path-cli` is a simple command-line tool to manage your system's `$PATH` variable by adding, removing, listing paths, and opening your shell configuration file in Visual Studio Code. It supports both `bash` and `zsh` shells.

## Features

- **Add Paths**: Add directories to your `$PATH` in your `.bashrc` or `.zshrc` file.
- **Remove Paths**: Remove directories from your `$PATH`.
- **List Paths**: List all directories currently in your `$PATH`.
- **Open Config**: Open your `.bashrc` or `.zshrc` file in Visual Studio Code.

## Installation

1. **Clone the Repository**:
    ```sh
    git clone https://github.com/yourusername/path-cli.git
    cd path-cli
    ```

2. **Build the Application**:
    ```sh
    dotnet build
    ```

3. **Add to PATH**:
    - Compile the application and add the output directory to your system's `$PATH` to use it globally:
    ```sh
    export PATH="$PATH:/path/to/your/compiled/path-cli"
    ```

## Usage

### Commands

- **Add a Path**
    ```sh
    path-cli add <path>
    ```
    Example:
    ```sh
    path-cli add /usr/local/bin
    ```

- **Remove a Path**
    ```sh
    path-cli remove <path>
    ```
    Example:
    ```sh
    path-cli remove /usr/local/bin
    ```

- **List Paths**
    ```sh
    path-cli list
    ```
    Lists all the paths in your shell configuration file.

- **Open Config File**
    ```sh
    path-cli code
    ```
    Opens the `.bashrc` or `.zshrc` file in Visual Studio Code.

### Help Command
Display the help information:
```sh
path-cli --help
```

## Compatibility

- **Shells**: `path-cli` automatically detects and supports both `bash` and `zsh` shells.
- **Operating System**: The application is designed for Unix-like operating systems (Linux, macOS).
