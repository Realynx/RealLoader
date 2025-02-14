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

LATEST_VERSION=""
NET_VERSION=""

# Loop through directories and extract numeric versions
for dir in "$DOTNET_PACK_DIR"/*/; do
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

        if [[ -z "$LATEST_VERSION" || "$SORTABLE_VERSION" > "$LATEST_VERSION" ]]; then
            LATEST_VERSION="$SORTABLE_VERSION"
            NET_VERSION="$dir"
        fi
    fi
done

# Ensure a version was found
if [[ -z "$NET_VERSION" ]]; then
    echo "No valid .NET Core host pack found!" >&2
    exit 1
fi

# Find the runtime directory dynamically
RUNTIME_DIR=$(find "$DOTNET_PACK_DIR/$NET_VERSION/runtimes/" -maxdepth 1 -type d | grep -v "/runtimes/$" | head -n 1)
if [[ -z "$RUNTIME_DIR" ]]; then
    echo "No valid runtime directory found!" >&2
    exit 1
fi

SOURCE_FILE="$RUNTIME_DIR/native/libnethost.a"

# Ensure source file exists
if [[ ! -f "$SOURCE_FILE" ]]; then
    echo "libnethost.a not found in the expected location!" >&2
    exit 1
fi

echo "$SOURCE_FILE"
exit 0
