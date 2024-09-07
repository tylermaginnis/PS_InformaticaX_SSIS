# Define the server URL and session ID
$serverUrl = "https://your-informatica-server.com"
$icSessionId = "your-session-id"

# Define the API endpoint
$apiEndpoint = "$serverUrl/api/v2/mapping"

# Create the headers
$headers = @{
    "Accept" = "application/json"
    "icSessionId" = $icSessionId
}

# Make the GET request
$response = Invoke-RestMethod -Uri $apiEndpoint -Method Get -Headers $headers

# Check if the response is successful
if ($response) {
    # Output the response
    $response | ConvertTo-Json -Depth 10
} else {
    Write-Host "Failed to retrieve mappings."
}