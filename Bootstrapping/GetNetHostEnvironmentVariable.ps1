$DOTNET_PACKS_DIR = "C:\Program Files\dotnet\packs\Microsoft.NETCore.App.Host.win-x64"
$LATEST_VERSION = "00000000.00000000.00000000.00000000"
$NET_VERSION = $null
$DEST_FILE = "$PSScriptRoot\nethost.lib"

# Ensure the directory exists
if (-Not (Test-Path $DOTNET_PACKS_DIR)) {
    Write-Host "ERROR: .NET Core host pack directory not found!" -ForegroundColor Red
    exit 1
}

# Loop through directories and extract numeric versions
$directories = Get-ChildItem -Path $DOTNET_PACKS_DIR -Directory

foreach ($dir in $directories) {
    $VERSION = $dir.Name

    # Ensure valid version format (only digits and dots)
    if ($VERSION -match '^[0-9][0-9.]*$') {
        # Parse version into sortable format
        $versionParts = $VERSION -split '\.'

        $MAJOR = $versionParts[0].PadLeft(4, '0')
        $MINOR = $versionParts[1].PadLeft(4, '0')
        $PATCH = $versionParts[2].PadLeft(4, '0')
        $BUILD = if ($versionParts.Count -ge 4) { $versionParts[3].PadLeft(4, '0') } else { "0000" }

        $SORTABLE_VERSION = "$MAJOR.$MINOR.$PATCH.$BUILD"

        if ($SORTABLE_VERSION -gt $LATEST_VERSION) {
            $LATEST_VERSION = $SORTABLE_VERSION
            $NET_VERSION = $VERSION
        }
    }
}

# Ensure a version was found
if (-Not $NET_VERSION) {
    Write-Host "ERROR: No valid .NET Core host pack found!" -ForegroundColor Red
    exit 1
}

$SOURCE_FILE = "$DOTNET_PACKS_DIR\$NET_VERSION\runtimes\win-x64\native\nethost.lib"

# Ensure source file exists
if (-Not (Test-Path $SOURCE_FILE)) {
    Write-Host "ERROR: nethost.lib not found in the expected location!" -ForegroundColor Red
    exit 1
}

# Ensure destination directory exists
if (-Not (Test-Path $DEST_DIR)) {
    New-Item -ItemType Directory -Path $DEST_DIR -Force | Out-Null
}

# Copy the file, overwriting if necessary
Copy-Item -Path $SOURCE_FILE -Destination $DEST_FILE -Force

Write-Host "nethost.lib copied successfully to $DEST_FILE"
exit 0
