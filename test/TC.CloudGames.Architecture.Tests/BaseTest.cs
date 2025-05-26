using System.Reflection;
using TC.CloudGames.Api;
using TC.CloudGames.Application.Abstractions.Messaging;
using TC.CloudGames.Domain.User;
using TC.CloudGames.Infra.Data;

namespace TC.CloudGames.Architecture.Tests
{
    public abstract class BaseTest
    {
        protected static readonly Assembly DomainAssembly = typeof(User).Assembly;
        protected static readonly Assembly ApplicationAssembly = typeof(ICommand).Assembly;
        protected static readonly Assembly InfrastructureAssembly = typeof(ApplicationDbContext).Assembly;
        protected static readonly Assembly PresentationAssembly = typeof(Program).Assembly;
    }
}
