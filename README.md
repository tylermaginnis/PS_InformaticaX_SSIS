# SSIS Package Management and Validation

This project provides tools for managing and validating SQL Server Integration Services (SSIS) packages. It includes scripts for converting PowerMart files to DTSX packages and validating DTSX files.

## Table of Contents

- [Overview](#overview)
- [Directory Structure](#directory-structure)
- [Scripts](#scripts)
  - [InformaticaX_SSIS.ps1](#informaticax_ssisps1)
  - [DTSXValidator.ps1](#dtsxvalidatorps1)
- [Usage](#usage)
  - [Converting PowerMart Files](#converting-powermart-files)
  - [Validating DTSX Files](#validating-dtsx-files)
- [Configuration](#configuration)
- [Contributing](#contributing)
- [License](#license)

## Overview

This project contains PowerShell scripts to assist with the conversion of PowerMart files to SSIS packages and the validation of these packages. The main components are:

- `InformaticaX_SSIS.ps1`: Converts PowerMart XML files to SSIS DTSX packages.
- `DTSXValidator.ps1`: Validates the generated DTSX packages using `dtexec`.

## Directory Structure

The directory structure of the project is as follows:

```
Ctrl+K to generate a command
Write a comprehensive readme.md

Sure, here's a comprehensive README.md for your project:
project-root/
├── scripts/
│ ├── InformaticaX_SSIS.ps1
│ ├── DTSXValidator.ps1
├── examples/
│ ├── example1.xml
│ ├── example2.dtsx
├── README.md
└── config/
├── settings.json
```

## Scripts

### InformaticaX_SSIS.ps1

This script converts PowerMart XML files to SSIS DTSX packages.

**Usage:**

```
powershell
.\InformaticaX_SSIS.ps1 -InputFile "path\to\input.xml" -OutputFile "path\to\output.dtsx"
```

**Parameters:**
- `-InputFile`: Path to the PowerMart XML file.
- `-OutputFile`: Path where the SSIS DTSX package will be saved.

### DTSXValidator.ps1

This script validates the generated DTSX packages using `dtexec`.

**Usage:**

```powershell
.\DTSXValidator.ps1 -DTSXFile "path\to\package.dtsx"
```

**Parameters:**
- `-DTSXFile`: Path to the DTSX package file.

## Usage

### Converting PowerMart Files

To convert a PowerMart file to a DTSX package, run the `InformaticaX_SSIS.ps1` script with the appropriate parameters. For example:

```powershell
.\scripts\InformaticaX_SSIS.ps1 -InputFile "examples\example1.xml" -OutputFile "output\example1.dtsx"
```

### Validating DTSX Files

To validate a DTSX package, run the `DTSXValidator.ps1` script with the appropriate parameters. For example:

```powershell
.\scripts\DTSXValidator.ps1 -DTSXFile "examples\example2.dtsx"
```

### SSISWrapper

The `SSISWrapper` is a C# executable used within the `InformaticaX_SSIS.ps1` script to facilitate the conversion of PowerMart XML files to SSIS DTSX packages. It takes various parameters such as package name, folder name, source and target connection strings, and generates a DTSX package based on these inputs.

**Usage within PowerShell script:**

The `SSISWrapper` is invoked by the `InformaticaX_SSIS.ps1` script with the following parameters:
- `--create`: Indicates the creation of a new DTSX package.
- `--name`: The name of the package.
- `--folder`: The folder name where the package will be stored.
- `--folder-description`: A description of the folder.
- `--source`: The name of the source.
- `--source-connection`: The connection string for the source.
- `--target`: The name of the target.
- `--target-connection`: The connection string for the target.
- `--output`: The path where the generated DTSX package will be saved.

The `SSISWrapper` executable processes these parameters and generates the corresponding DTSX package, which is then saved to the specified output path.

**Example:**

The `InformaticaX_SSIS.ps1` script uses the `SSISWrapper` as follows:


### Architecture of `InformaticaX_SSIS.ps1` and `SSISWrapper`

The `InformaticaX_SSIS.ps1` script and the `SSISWrapper` executable work together to convert PowerMart XML files into SSIS DTSX packages. Below is an explanation of their architecture and the libraries they use.

#### `InformaticaX_SSIS.ps1`

The `InformaticaX_SSIS.ps1` script is a PowerShell script that orchestrates the conversion process. It performs the following tasks:

1. **Parameter Definition**: The script accepts two parameters: `samplesDir` (the directory containing PowerMart XML files) and `outputDir` (the directory where the generated DTSX files will be saved).

2. **Convert-PowerMartToDtsx Function**: This function reads a PowerMart XML file, extracts necessary parameters, and invokes the `SSISWrapper` executable to perform the conversion. It uses the following PowerShell cmdlets and libraries:
   - `Get-Content`: Reads the content of the XML file.
   - `Where-Object` and `Select-Object`: Extract specific attributes from the XML.
   - `New-Object System.Diagnostics.ProcessStartInfo`: Configures the process to run the `SSISWrapper` executable.
   - `New-Object System.Diagnostics.Process`: Executes the `SSISWrapper` process and captures its output.

3. **Process-Directory Function**: This function ensures the output directory exists and processes each PowerMart XML file in the samples directory by calling the `Convert-PowerMartToDtsx` function.

4. **Execution**: The script executes the `Process-Directory` function with the provided parameters.

#### `SSISWrapper`

The `SSISWrapper` is a C# executable that performs the actual conversion of PowerMart XML files to SSIS DTSX packages. It uses the following libraries and components:

1. **System.Xml**: This library is used to parse and manipulate XML data. It reads the PowerMart XML file and extracts the necessary information to create the DTSX package.

2. **Microsoft.SqlServer.Dts.Runtime**: This library is part of the SQL Server Integration Services (SSIS) API. It provides classes and methods to create and manipulate SSIS packages programmatically.

3. **Command-Line Arguments**: The `SSISWrapper` accepts various command-line arguments to specify the package name, folder name, source and target connection strings, and the output path for the generated DTSX package.

4. **Package Creation**: The `SSISWrapper` uses the extracted information and the SSIS API to create a new DTSX package. It sets the package properties, adds connection managers, and configures the data flow components based on the input parameters.

5. **Output**: The generated DTSX package is saved to the specified output path.

## License

This project is licensed under the NON-FREE, NON-COMMERCIAL, OPENSOURCE LICENSE. 

### Terms and Conditions

1. **Non-Free**: This software is not free. You must obtain a valid license to use it.
2. **Non-Commercial**: This software is provided for non-commercial use only. You may not use it for any commercial purposes.
3. **Open Source**: The source code is available for viewing, modification, and distribution under the terms of this license.

### Permissions

- **Modification**: You are allowed to modify the source code.
- **Distribution**: You are allowed to distribute the modified or unmodified source code, provided that it remains under the same license.

### Restrictions

- **Commercial Use**: You may not use this software for commercial purposes.
- **Sublicensing**: You may not sublicense this software.
- **Liability**: The authors are not liable for any damages arising from the use of this software.

### Disclaimer

This software is provided "as is", without warranty of any kind, express or implied, including but not limited to the warranties of merchantability, fitness for a particular purpose, and noninfringement. In no event shall the authors be liable for any claim, damages, or other liability, whether in an action of contract, tort, or otherwise, arising from, out of, or in connection with the software or the use or other dealings in the software.
