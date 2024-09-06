# Define the source and destination directories
$sourceDir = "Samples"
$destDir = "Schematica"

# Create the destination directory if it doesn't exist
if (-Not (Test-Path -Path $destDir)) {
    New-Item -ItemType Directory -Path $destDir
}

# Get all XML files in the source directory
$xmlFiles = Get-ChildItem -Path $sourceDir -Filter *.xml

# Loop through each XML file
foreach ($file in $xmlFiles) {
    # Load the XML content
    [xml]$xmlContent = Get-Content -Path $file.FullName

    # Generate the XML schema using XmlSchemaInference
    $xmlSchemaSet = New-Object System.Xml.Schema.XmlSchemaSet
    $xmlReaderSettings = New-Object System.Xml.XmlReaderSettings
    $xmlReaderSettings.DtdProcessing = "Ignore"  # Disable DTD processing
    $xmlReader = [System.Xml.XmlReader]::Create($file.FullName, $xmlReaderSettings)
    $schemaInference = New-Object System.Xml.Schema.XmlSchemaInference
    $xmlSchemaSet = $schemaInference.InferSchema($xmlReader)

    # Convert the schema to a string
    $stringWriter = New-Object System.IO.StringWriter
    $xmlWriterSettings = New-Object System.Xml.XmlWriterSettings
    $xmlWriterSettings.Indent = $true
    $xmlWriterSettings.IndentChars = "    "
    $xmlWriterSettings.NewLineOnAttributes = $false
    $xmlWriter = [System.Xml.XmlWriter]::Create($stringWriter, $xmlWriterSettings)
    foreach ($schema in $xmlSchemaSet.Schemas()) {
        $schema.Write($xmlWriter)
    }
    $xmlWriter.Flush()
    $xmlSchemaString = $stringWriter.ToString()

    # Define the destination file path
    $destFilePath = Join-Path -Path $destDir -ChildPath ($file.BaseName + ".xsd")

    # Write the XML schema to the destination file
    $xmlSchemaString | Out-File -FilePath $destFilePath -Encoding utf8
}