@echo off
setlocal enabledelayedexpansion

:: Navigate one folder level up
cd ..

:: Define project paths and variables
set TOKEN_FILE=token_data\sonar-token.txt
set SONAR_PROJECT_KEY=tc-cloudgames-local
set SONAR_HOST_URL=http://localhost:9000

:: Print variables for debugging
echo Current Directory = "%CD%"
echo TOKEN_FILE = "%CD%\%TOKEN_FILE%"
echo SONAR_PROJECT_KEY = "%SONAR_PROJECT_KEY%"
echo SONAR_HOST_URL = "%SONAR_HOST_URL%"
timeout /t 3 /nobreak >nul

:: Install required dotnet tools if missing
echo Checking required dotnet tools...
timeout /t 2 /nobreak >nul

dotnet tool list --global | findstr /C:"dotnet-sonarscanner" >nul
if %errorlevel% neq 0 (
    echo Installing dotnet-sonarscanner...
    dotnet tool install --global dotnet-sonarscanner
)

:: Debugging: Print the resolved path to check
echo Checking for token file at "%CD%\%TOKEN_FILE%"
timeout /t 2 /nobreak >nul

:: Read SonarQube token
if exist "%CD%\%TOKEN_FILE%" (
    for /f "delims=" %%A in ('type "%CD%\%TOKEN_FILE%"') do set SONAR_TOKEN=%%A
) else (
    echo ERROR: Sonar token file not found at %CD%\%TOKEN_FILE%
    pause
    exit /b 1
)

:: Debugging: Print extracted token
echo SONAR_TOKEN = "%SONAR_TOKEN%"
timeout /t 2 /nobreak >nul

:: Start SonarScanner analysis
echo Starting SonarScanner analysis...
timeout /t 2 /nobreak >nul

dotnet sonarscanner begin ^
  /k:"%SONAR_PROJECT_KEY%" ^
  /d:sonar.token="%SONAR_TOKEN%" ^
  /d:sonar.host.url="%SONAR_HOST_URL%"

:: Build the project
echo Building the project...
timeout /t 2 /nobreak >nul

set API_PROJECT=src\TC.CloudGames.Api\TC.CloudGames.Api.csproj
dotnet build "%CD%\%API_PROJECT%" --no-incremental

:: End SonarScanner analysis
echo Finishing SonarScanner analysis...
timeout /t 2 /nobreak >nul

dotnet sonarscanner end ^
  /d:sonar.token="%SONAR_TOKEN%"

pause
endlocal
