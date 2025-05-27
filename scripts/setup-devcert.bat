@echo off
setlocal

:: Configure variables
set CERT_PASSWORD=SecurePassword123!
set CERT_DIR=%USERPROFILE%\.aspnet\https
set CERT_FILE=%CERT_DIR%\aspnetapp.pfx

echo ===========================================
echo == ASP.NET Core HTTPS Certificate Script ==
echo ===========================================

:: Create directory if it does not exist
if not exist "%CERT_DIR%" (
    echo Creating certificate directory at: %CERT_DIR%
    mkdir "%CERT_DIR%"
)

:: If a folder named aspnetapp.pfx exists, remove it
if exist "%CERT_FILE%\" (
    echo Warning: A folder named aspnetapp.pfx exists. Removing...
    rmdir /S /Q "%CERT_FILE%"
)

:: Check if the certificate file already exists
if exist "%CERT_FILE%" (
    echo Certificate already exists at %CERT_FILE%
    echo Cleaning existing development certificates...
    dotnet dev-certs https --clean

    echo Removing existing certificate file...
    del /F /Q "%CERT_FILE%"
)

:: Check again if the file still exists
if exist "%CERT_FILE%" (
    echo ERROR: Failed to delete existing certificate file.
    goto end
)

echo Creating and trusting a new development certificate...
dotnet dev-certs https --trust

echo Exporting certificate to %CERT_FILE% with password...
dotnet dev-certs https -ep "%CERT_FILE%" -p "%CERT_PASSWORD%"

if exist "%CERT_FILE%" (
    echo Certificate created successfully at %CERT_FILE%
) else (
    echo ERROR: Failed to create the certificate.
)

:end
echo ===========================================
echo Dev certificate setup complete!
echo ===========================================

endlocal
pause
