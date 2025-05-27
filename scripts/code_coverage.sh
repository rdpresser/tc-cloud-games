#!/bin/sh

set -e  # Exit immediately if any command fails

# Define project paths and variables
ROOT_DIR=".."
TOKEN_FILE="$ROOT_DIR/token_data/sonar-token.txt"
SONAR_PROJECT_KEY="tc-cloudgames-local"
SONAR_HOST_URL="http://tc-cloudgames-sonarqube:9000"
TEST_RESULTS_DIR="$ROOT_DIR/TestResults"

# Function to check and install dotnet tools if missing
check_and_install_tool() {
    TOOL_NAME=$1
    INSTALL_CMD=$2

    if ! dotnet tool list -g | grep -q "$TOOL_NAME"; then
        echo "Installing $TOOL_NAME..."
        eval "$INSTALL_CMD"
    else
        echo "$TOOL_NAME is already installed."
    fi
}

# Check for required dotnet tools and install if missing
echo "Checking required dotnet tools..."
sleep 2

check_and_install_tool "dotnet-sonarscanner" "dotnet tool install --global dotnet-sonarscanner"
check_and_install_tool "dotnet-reportgenerator-globaltool" "dotnet tool install --global dotnet-reportgenerator-globaltool"

# Debugging: Print the resolved path to check
echo "Checking for token file at $TOKEN_FILE..."
sleep 2

# Read SonarQube token from shared volume
if [ -f "$TOKEN_FILE" ]; then
    SONAR_TOKEN=$(cat "$TOKEN_FILE")
else
    echo "ERROR: Sonar token file not found at $TOKEN_FILE"
    exit 1
fi

# Debugging: Print extracted token
echo "SONAR_TOKEN = $SONAR_TOKEN"
sleep 2

# **Clean up old test results before analysis**
if [ -d "$TEST_RESULTS_DIR" ]; then
    echo "Deleting previous test results..."
    rm -rf "$TEST_RESULTS_DIR"
fi

# Start SonarScanner analysis
echo "Starting SonarScanner analysis"
sleep 2

dotnet sonarscanner begin \
  /k:"$SONAR_PROJECT_KEY" \
  /d:sonar.host.url="$SONAR_HOST_URL" \
  /d:sonar.login="$SONAR_TOKEN"

# Build the project
echo "Starting the building of the project"
sleep 2

API_PROJECT="$ROOT_DIR/src/TC.CloudGames.Api/TC.CloudGames.Api.csproj"
dotnet build "$API_PROJECT"

# Run tests sequentially with code coverage
echo "Starting to run tests sequentially with code coverage"
sleep 2

dotnet test "$ROOT_DIR/test/TC.CloudGames.Api.Tests/TC.CloudGames.Api.Tests.csproj" --collect:"XPlat Code Coverage" --results-directory "$TEST_RESULTS_DIR"
dotnet test "$ROOT_DIR/test/TC.CloudGames.Application.Tests/TC.CloudGames.Application.Tests.csproj" --collect:"XPlat Code Coverage" --results-directory "$TEST_RESULTS_DIR"
dotnet test "$ROOT_DIR/test/TC.CloudGames.Domain.Tests/TC.CloudGames.Domain.Tests.csproj" --collect:"XPlat Code Coverage" --results-directory "$TEST_RESULTS_DIR"
dotnet test "$ROOT_DIR/test/TC.CloudGames.Architecture.Tests/TC.CloudGames.Architecture.Tests.csproj" --collect:"XPlat Code Coverage" --results-directory "$TEST_RESULTS_DIR"
dotnet test "$ROOT_DIR/test/TC.CloudGames.Integration.Tests/TC.CloudGames.Integration.Tests.csproj" --collect:"XPlat Code Coverage" --results-directory "$TEST_RESULTS_DIR"

# Generate coverage report
echo "Starting to generate coverage report"
sleep 2

reportgenerator \
  -reports:"$TEST_RESULTS_DIR/**/coverage.cobertura.xml" \
  -targetdir:"$TEST_RESULTS_DIR/CoverageReport" \
  -reporttypes:Cobertura

# End SonarScanner analysis
echo "Finishing SonarScanner analysis"
sleep 2

dotnet sonarscanner end \
  /d:sonar.login="$SONAR_TOKEN"

echo "Process completed successfully!"
