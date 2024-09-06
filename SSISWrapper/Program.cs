using System;
using System.IO;
using Microsoft.SqlServer.Dts.Runtime;
using Microsoft.SqlServer.Dts.Pipeline.Wrapper;

namespace SSISWrapper
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length > 0)
            {
                string command = args[0];
                switch (command)
                {
                    case "--create":
                        HandleCreateCommand(args);
                        break;
                    default:
                        PrintUsage();
                        break;
                }
            }
            else
            {
                PrintUsage();
            }
        }

        static void HandleCreateCommand(string[] args)
        {
            string packageName = null;
            string folderName = null;
            string folderDescription = null;
            string sourceName = null;
            string sourceConnectionString = null;
            string targetName = null;
            string targetConnectionString = null;
            string outputPath = "C:\\dev\\PS_InformaticaX_SSIS\\SSIS\\MyPackage.dtsx"; // Default output path

            for (int i = 1; i < args.Length; i++)
            {
                switch (args[i])
                {
                    case "--name":
                        if (i + 1 < args.Length)
                        {
                            packageName = args[++i];
                        }
                        break;
                    case "--folder":
                        if (i + 1 < args.Length)
                        {
                            folderName = args[++i];
                        }
                        break;
                    case "--folder-description":
                        if (i + 1 < args.Length)
                        {
                            folderDescription = args[++i];
                        }
                        break;
                    case "--source":
                        if (i + 1 < args.Length)
                        {
                            sourceName = args[++i];
                        }
                        break;
                    case "--source-connection":
                        if (i + 1 < args.Length)
                        {
                            sourceConnectionString = args[++i];
                        }
                        break;
                    case "--target":
                        if (i + 1 < args.Length)
                        {
                            targetName = args[++i];
                        }
                        break;
                    case "--target-connection":
                        if (i + 1 < args.Length)
                        {
                            targetConnectionString = args[++i];
                        }
                        break;
                    case "--output":
                        if (i + 1 < args.Length)
                        {
                            outputPath = args[++i];
                        }
                        break;
                }
            }

            if (!string.IsNullOrEmpty(packageName))
            {
                CreatePackage(outputPath, packageName, folderName, folderDescription, sourceName, sourceConnectionString, targetName, targetConnectionString);
            }
        }

        static void PrintUsage()
        {
            Console.WriteLine("Usage:");
            Console.WriteLine("--create --name <packageName> [--folder <folderName> --folder-description <folderDescription>] [--source <sourceName> --source-connection <connectionString>] [--target <targetName> --target-connection <connectionString>] [--output <outputPath>]");
        }

        static void CreatePackage(string outputPath, string packageName, string folderName, string folderDescription, string sourceName, string sourceConnectionString, string targetName, string targetConnectionString)
        {
            Package package = new Package
            {
                Name = packageName,
                Description = "Sample package created by SSISWrapper"
            };

            // Create a Data Flow Task
            Executable exec = package.Executables.Add("STOCK:PipelineTask");
            TaskHost taskHost = exec as TaskHost;
            MainPipe dataFlowTask = taskHost.InnerObject as MainPipe;

            if (!string.IsNullOrEmpty(sourceName) && !string.IsNullOrEmpty(sourceConnectionString))
            {
                CreateSource(package, dataFlowTask, sourceName, sourceConnectionString);
            }

            if (!string.IsNullOrEmpty(targetName) && !string.IsNullOrEmpty(targetConnectionString))
            {
                CreateTarget(package, dataFlowTask, targetName, targetConnectionString);
            }

            try
            {
                // Ensure the directory exists
                Directory.CreateDirectory(Path.GetDirectoryName(outputPath));
                Application app = new Application();
                app.SaveToXml(outputPath, package, null);
                Console.WriteLine($"Package created and saved to {outputPath} with name {packageName}.");
            }
            catch (DtsRuntimeException ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        static void CreateSource(Package package, MainPipe dataFlowTask, string sourceName, string connectionString)
        {
            IDTSComponentMetaData100 sourceComponent = dataFlowTask.ComponentMetaDataCollection.New();
            sourceComponent.ComponentClassID = "DTSAdapter.OleDbSource";
            CManagedComponentWrapper instance = sourceComponent.Instantiate();
            instance.ProvideComponentProperties();

            sourceComponent.Name = sourceName;
            instance.SetComponentProperty("AccessMode", 0); // 0 = SQL Command
            instance.SetComponentProperty("OpenRowset", "YourSourceTable");

            // Set the source connection
            ConnectionManager sourceConnection = package.Connections.Add("OLEDB");
            sourceConnection.Name = sourceName;
            sourceConnection.ConnectionString = connectionString;

            // Additional configuration for the source component here

            Console.WriteLine($"Source {sourceName} created with connection string: {connectionString}");
        }

        static void CreateTarget(Package package, MainPipe dataFlowTask, string targetName, string connectionString)
        {
            IDTSComponentMetaData100 targetComponent = dataFlowTask.ComponentMetaDataCollection.New();
            targetComponent.ComponentClassID = "DTSAdapter.OleDbDestination";
            CManagedComponentWrapper instance = targetComponent.Instantiate();
            instance.ProvideComponentProperties();

            targetComponent.Name = targetName;
            instance.SetComponentProperty("AccessMode", 3); // 3 = Table or view
            instance.SetComponentProperty("OpenRowset", "YourTargetTable");

            // Set the target connection
            ConnectionManager targetConnection = package.Connections.Add("OLEDB");
            targetConnection.Name = targetName;
            targetConnection.ConnectionString = connectionString;

            // Additional configuration for the target component here

            Console.WriteLine($"Target {targetName} created with connection string: {connectionString}");
        }
    }
}
