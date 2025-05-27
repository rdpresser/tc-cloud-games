@echo off
setlocal

:: Configura variáveis
set CERT_PASSWORD=SecurePassword123!
set CERT_DIR=%USERPROFILE%\.aspnet\https
set CERT_FILE=%CERT_DIR%\aspnetapp.pfx

echo ===========================================
echo == ASP.NET Core HTTPS Certificate Script ==
echo ===========================================

:: Cria diretório se não existir
if not exist "%CERT_DIR%" (
    echo Creating certificate directory at: %CERT_DIR%
    mkdir "%CERT_DIR%"
)

:: Se existir um arquivo com o nome, remove pasta que foi criada erroneamente
if exist "%CERT_FILE%\" (
    echo Warning: A folder named aspnetapp.pfx exists. Removing...
    rmdir /S /Q "%CERT_FILE%"
)

:: Verifica se o certificado já existe
if exist "%CERT_FILE%" (
    echo Certificate already exists at %CERT_FILE%
    echo If you want to regenerate it, run 'dotnet dev-certs https --clean' and then this script again.
) else (
    echo Cleaning existing development certificates...
    dotnet dev-certs https --clean

    echo Creating and trusting a new development certificate...
    dotnet dev-certs https --trust

    echo Exporting certificate to %CERT_FILE% with password...
    dotnet dev-certs https -ep "%CERT_FILE%" -p "%CERT_PASSWORD%"

    if exist "%CERT_FILE%" (
        echo Certificate created successfully at %CERT_FILE%
    ) else (
        echo ERROR: Failed to create the certificate.
    )
)

echo ===========================================
echo Dev certificate setup complete!
echo ===========================================

endlocal
pause
