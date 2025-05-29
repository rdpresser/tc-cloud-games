#!/bin/bash
set -e

# Configure variables
CERT_PASSWORD="SecurePassword123!"
CERT_DIR="$HOME/.aspnet/https"
CERT_FILE="$CERT_DIR/aspnetapp.pfx"

echo "==========================================="
echo "== ASP.NET Core HTTPS Certificate Script =="
echo "==========================================="

# Create directory if it does not exist
if [ ! -d "$CERT_DIR" ]; then
  echo "Creating certificate directory at: $CERT_DIR"
  mkdir -p "$CERT_DIR"
fi

# If a folder named aspnetapp.pfx exists, remove it
if [ -d "$CERT_FILE" ]; then
  echo "Warning: A folder named aspnetapp.pfx exists. Removing..."
  rm -rf "$CERT_FILE"
fi

# Check if the certificate file already exists
if [ -f "$CERT_FILE" ]; then
  echo "Certificate already exists at $CERT_FILE"
  echo "Cleaning existing development certificates..."
  dotnet dev-certs https --clean

  echo "Removing existing certificate file..."
  rm -f "$CERT_FILE"
fi

# Check again if the file still exists
if [ -f "$CERT_FILE" ]; then
  echo "ERROR: Failed to delete existing certificate file."
  echo "==========================================="
  echo "Dev certificate setup complete!"
  echo "==========================================="
  exit 1
fi

echo "Creating and trusting a new development certificate..."
dotnet dev-certs https --trust

echo "Exporting certificate to $CERT_FILE with password..."
dotnet dev-certs https -ep "$CERT_FILE" -p "$CERT_PASSWORD"

if [ -f "$CERT_FILE" ]; then
  chmod 600 "$CERT_FILE"
  echo "Certificate created successfully at $CERT_FILE"
else
  echo "ERROR: Failed to create the certificate."
fi

echo "==========================================="
echo "Dev certificate setup complete!"
echo "==========================================="
read -n 1 -s -r -p "Press any key to continue..."
echo