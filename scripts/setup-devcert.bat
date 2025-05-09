@echo off
setlocal

:: Define certificate variables
set CERT_PASSWORD=SecurePassword123!
set CERT_DIR=%USERPROFILE%\.aspnet\https
set CERT_FILE=%CERT_DIR%\aspnetapp.pfx

echo Setting up ASP.NET Core development certificate for HTTPS...

:: Create the directory if it doesn't exist
if not exist "%CERT_DIR%" (
  echo Creating certificate directory: %CERT_DIR%
  mkdir "%CERT_DIR%"
)

:: Check if certificate already exists
if exist "%CERT_FILE%" (
  echo Certificate already exists at %CERT_FILE%
  echo To regenerate, first run 'dotnet dev-certs https --clean' and then run this script again.
) else (
  :: Clean any existing certificates
  echo Cleaning existing development certificates...
  dotnet dev-certs https --clean
  
  :: Create and trust a new certificate
  echo Creating and trusting a new development certificate...
  dotnet dev-certs https --trust
  
  :: Export the certificate
  echo Exporting certificate to %CERT_FILE% with password...
  dotnet dev-certs https -ep "%CERT_FILE%" -p "%CERT_PASSWORD%"
  
  echo Certificate created and exported successfully.
)

echo Dev certificate setup complete!
echo HTTPS should now work in Docker with the ASP.NET Core application.
echo Use 'docker compose up' to start your application.

endlocal

