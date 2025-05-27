@echo off
setlocal enabledelayedexpansion

:: Navigate one folder level up
cd ..

:: Define project paths and variables
set TOKEN_FILE=token_data\sonar-token.txt
set SONAR_PROJECT_KEY=tc-cloudgames-local
set SONAR_HOST_URL=http://localhost:9000
set TEST_RESULTS_DIR=TestResults
set COVERAGE_REPORT_PATH=TestResults\CoverageReport\Cobertura.xml

:: Print variables with a delay for debugging
echo Current Directory = "%CD%"
echo TOKEN_FILE = "%CD%\%TOKEN_FILE%"
echo SONAR_PROJECT_KEY = "%SONAR_PROJECT_KEY%"
echo SONAR_HOST_URL = "%SONAR_HOST_URL%"
echo TEST_RESULTS_DIR = "%CD%\%TEST_RESULTS_DIR%"
echo COVERAGE_REPORT_PATH = "%CD%\%COVERAGE_REPORT_PATH%"
timeout /t 3 /nobreak >nul

:: Check for required dotnet tools and install if missing
echo Checking required dotnet tools...
timeout /t 2 /nobreak >nul

dotnet tool list --global | findstr /C:"dotnet-sonarscanner" >nul
if %errorlevel% neq 0 (
    echo Installing dotnet-sonarscanner...
    dotnet tool install --global dotnet-sonarscanner
)

dotnet tool list --global | findstr /C:"dotnet-reportgenerator-globaltool" >nul
if %errorlevel% neq 0 (
    echo Installing dotnet-reportgenerator-globaltool...
    dotnet tool install --global dotnet-reportgenerator-globaltool
)

:: Debugging: Print the resolved path to check
echo Checking for token file at "%CD%\%TOKEN_FILE%"
timeout /t 2 /nobreak >nul

:: Read SonarQube token from shared volume
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

:: **Clean up old test results before analysis**
if exist "%CD%\%TEST_RESULTS_DIR%" (
    echo Deleting previous test results...
    rmdir /s /q "%CD%\%TEST_RESULTS_DIR%"
)

:: Start SonarScanner analysis
echo Starting SonarScanner analysis
timeout /t 2 /nobreak >nul

dotnet sonarscanner begin ^
  /k:"%SONAR_PROJECT_KEY%" ^
  /d:sonar.host.url="%SONAR_HOST_URL%" ^
  /d:sonar.login="%SONAR_TOKEN%" ^
  /d:sonar.coverageReportPaths="%CD%\%COVERAGE_REPORT_PATH%"

:: Build the project
echo Starting the building of the project
timeout /t 2 /nobreak >nul

set API_PROJECT=src\TC.CloudGames.Api\TC.CloudGames.Api.csproj
dotnet build "%CD%\%API_PROJECT%"

:: Run tests sequentially with code coverage
echo Starting to run tests sequentially with code coverage
timeout /t 2 /nobreak >nul

dotnet test "%CD%\test\TC.CloudGames.Api.Tests\TC.CloudGames.Api.Tests.csproj" --collect:"XPlat Code Coverage" --results-directory "%CD%\%TEST_RESULTS_DIR%"
dotnet test "%CD%\test\TC.CloudGames.Application.Tests\TC.CloudGames.Application.Tests.csproj" --collect:"XPlat Code Coverage" --results-directory "%CD%\%TEST_RESULTS_DIR%"
dotnet test "%CD%\test\TC.CloudGames.Domain.Tests\TC.CloudGames.Domain.Tests.csproj" --collect:"XPlat Code Coverage" --results-directory "%CD%\%TEST_RESULTS_DIR%"
dotnet test "%CD%\test\TC.CloudGames.Architecture.Tests\TC.CloudGames.Architecture.Tests.csproj" --collect:"XPlat Code Coverage" --results-directory "%CD%\%TEST_RESULTS_DIR%"
dotnet test "%CD%\test\TC.CloudGames.Integration.Tests\TC.CloudGames.Integration.Tests.csproj" --collect:"XPlat Code Coverage" --results-directory "%CD%\%TEST_RESULTS_DIR%"

:: Generate coverage report
echo Starting to generate coverage report
timeout /t 2 /nobreak >nul

reportgenerator ^
  -reports:"%CD%\%TEST_RESULTS_DIR%\**\coverage.cobertura.xml" ^
  -targetdir:"%CD%\%TEST_RESULTS_DIR%\CoverageReport" ^
  -reporttypes:Cobertura

:: End SonarScanner analysis
echo Finishing SonarScanner analysis
timeout /t 2 /nobreak >nul

dotnet sonarscanner end ^
  /d:sonar.login="%SONAR_TOKEN%"

pause
endlocal
