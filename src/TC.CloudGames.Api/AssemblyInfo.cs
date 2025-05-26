global using Ardalis.Result;
global using FastEndpoints;
global using FastEndpoints.Security;
global using FastEndpoints.Swagger;
global using FluentValidation.Results;
global using Microsoft.AspNetCore.Diagnostics.HealthChecks;
global using Serilog;
global using System.Net;
global using TC.CloudGames.Api.Abstractions;
global using TC.CloudGames.Api.Extensions;
global using TC.CloudGames.Application.Abstractions;
global using TC.CloudGames.Application.Middleware;
global using TC.CloudGames.Infra.CrossCutting.Commons.Middleware;
global using TC.CloudGames.Infra.Data;
using System.Runtime.CompilerServices;
[assembly: InternalsVisibleTo("TC.CloudGames.Api.Tests")]
[assembly: InternalsVisibleTo("TC.CloudGames.Integration.Tests")]
[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")]

// REMARK: Required for functional and integration tests to work.
namespace TC.CloudGames.Api
{
    public partial class Program;
}