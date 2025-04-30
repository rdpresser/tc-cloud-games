#!/bin/bash
set -e

# Define certificate variables
CERT_PASSWORD="SecurePassword123!"
CERT_DIR="$HOME/.aspnet/https"
CERT_FILE="$CERT_DIR/aspnetapp.pfx"

echo "Setting up ASP.NET Core development certificate for HTTPS..."

# Create the directory if it doesn't exist
if [ ! -d "$CERT_DIR" ]; then
  echo "Creating certificate directory: $CERT_DIR"
  mkdir -p "$CERT_DIR"
fi

# Check if certificate already exists
if [ -f "$CERT_FILE" ]; then
  echo "Certificate already exists at $CERT_FILE"
  echo "To regenerate, first run 'dotnet dev-certs https --clean' and then run this script again."
else
  # Clean any existing certificates
  echo "Cleaning existing development certificates..."
  dotnet dev-certs https --clean
  
  # Create and trust a new certificate
  echo "Creating and trusting a new development certificate..."
  dotnet dev-certs https --trust
  
  # Export the certificate
  echo "Exporting certificate to $CERT_FILE with password..."
  dotnet dev-certs https -ep "$CERT_FILE" -p "$CERT_PASSWORD"
  
  # Set appropriate permissions for the certificate file
  chmod 600 "$CERT_FILE"
  echo "Certificate permissions set."
fi

echo "Dev certificate setup complete!"
echo "HTTPS should now work in Docker with the ASP.NET Core application."
echo "Use 'docker compose up' to start your application."

