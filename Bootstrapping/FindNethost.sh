#!/bin/bash

# Find the latest version of Microsoft.NETCore.App.Host in /usr/lib/dotnet/packs/
DOTNET_PACK_DIR="/usr/lib/dotnet/packs/"
DOTNET_PACK_NAME="Microsoft.NETCore.App.Host"

# Function to find the directory matching DOTNET_PACK_NAME
find_dotnet_pack() {
    local pack_path
    pack_path=$(find "$DOTNET_PACK_DIR" -maxdepth 1 -type d -name "${DOTNET_PACK_NAME}*" | sort -V | tail -n 1)
    echo "$pack_path"
}

DOTNET_PACK_DIR=$(find_dotnet_pack)

# Loop through directories and extract numeric versions
for dir in "$DOTNET_PACKS_DIR"/*/; do
    dir=$(basename "$dir")
    
    # Ensure valid version format (only digits and dots)
    if [[ "$dir" =~ ^[0-9][0-9.]*$ ]]; then
        # Parse version into sortable format
        IFS='.' read -r -a versionParts <<< "$dir"

        MAJOR=$(printf "%04d" "${versionParts[0]}")
        MINOR=$(printf "%04d" "${versionParts[1]}")
        PATCH=$(printf "%04d" "${versionParts[2]}")
        BUILD=$(printf "%04d" "${versionParts[3]:-0}")

        SORTABLE_VERSION="$MAJOR.$MINOR.$PATCH.$BUILD"

        if [[ "$SORTABLE_VERSION" > "$LATEST_VERSION" ]]; then
            LATEST_VERSION="$SORTABLE_VERSION"
            NET_VERSION="$dir"
        fi
    fi
done

if [[ -z "$NET_VERSION" ]]; then
    exit 1
fi

SOURCE_FILE="$DOTNET_PACKS_DIR/$NET_VERSION/runtimes/linux-x64/native/libnethost.a"
if [[ ! -f "$SOURCE_FILE" ]]; then
    exit 1
fi

echo "$SOURCE_FILE"
exit 0