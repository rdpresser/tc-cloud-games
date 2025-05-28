global using Ardalis.Result;
global using FastEndpoints;
global using FluentValidation;
global using FluentValidation.Results;
global using Microsoft.EntityFrameworkCore;
global using Microsoft.Extensions.Logging;
global using TC.CloudGames.Application.Abstractions;
global using TC.CloudGames.Application.Abstractions.Messaging;
global using TC.CloudGames.Domain.Abstractions;

using System.Runtime.CompilerServices;
[assembly: InternalsVisibleTo("TC.CloudGames.Application.Tests")]
[assembly: InternalsVisibleTo("TC.CloudGames.Api.Tests")]
[assembly: InternalsVisibleTo("TC.CloudGames.Integration.Tests")]
[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")]
[assembly: InternalsVisibleTo("TC.CloudGames.BDD.Tests")]