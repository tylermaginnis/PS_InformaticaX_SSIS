param (
    [string]$schemaPath = "DTSX_Schema/DTSX_Schema.xml",
    [string]$ssisDirectory = "SSIS"
)

function Validate-DTSXFile {
    param (
        [string]$filePath,
        [string]$schemaPath
    )

    try {
        $xmlSchemaSet = New-Object System.Xml.Schema.XmlSchemaSet
        $xmlSchemaSet.Add("", $schemaPath) | Out-Null

        $xmlReaderSettings = New-Object System.Xml.XmlReaderSettings
        $xmlReaderSettings.Schemas.Add($xmlSchemaSet)
        $xmlReaderSettings.ValidationType = [System.Xml.ValidationType]::Schema

        $xmlReader = [System.Xml.XmlReader]::Create($filePath, $xmlReaderSettings)
        while ($xmlReader.Read()) { }

        Write-Host "Validation succeeded for file: $filePath"
    } catch {
        Write-Host "Validation failed for file: $filePath"
        Write-Host $_.Exception.Message
    }
}

$files = Get-ChildItem -Path $ssisDirectory -Filter *.dtsx -Recurse
foreach ($file in $files) {
    Validate-DTSXFile -filePath $file.FullName -schemaPath $schemaPath
}
