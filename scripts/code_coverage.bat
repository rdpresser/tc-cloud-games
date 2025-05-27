@echo off

dotnet sonarscanner begin ^
  /k:"tc-cloudgames-local" ^
  /d:sonar.host.url="http://localhost:9000" ^
  /d:sonar.login="sqp_c3c48b658840cb4ba8ae948682f254550baf4845"

dotnet build ".\src\TC.CloudGames.Api\TC.CloudGames.Api.csproj"

dotnet test .\test\TC.CloudGames.Api.Tests\TC.CloudGames.Api.Tests.csproj --collect:"XPlat Code Coverage" --results-directory ".\TestResults"
dotnet test .\test\TC.CloudGames.Application.Tests\TC.CloudGames.Application.Tests.csproj --collect:"XPlat Code Coverage" --results-directory ".\TestResults"
dotnet test .\test\TC.CloudGames.Domain.Tests\TC.CloudGames.Domain.Tests.csproj --collect:"XPlat Code Coverage" --results-directory ".\TestResults"
dotnet test .\test\TC.CloudGames.Architecture.Tests\TC.CloudGames.Architecture.Tests.csproj --collect:"XPlat Code Coverage" --results-directory ".\TestResults"
dotnet test .\test\TC.CloudGames.Integration.Tests\TC.CloudGames.Integration.Tests.csproj --collect:"XPlat Code Coverage" --results-directory ".\TestResults"

reportgenerator ^
  -reports:".\TestResults\**\coverage.cobertura.xml" ^
  -targetdir:".\TestResults\CoverageReport" ^
  -reporttypes:Cobertura

dotnet sonarscanner end ^
  /d:sonar.login="sqp_c3c48b658840cb4ba8ae948682f254550baf4845"

  pause