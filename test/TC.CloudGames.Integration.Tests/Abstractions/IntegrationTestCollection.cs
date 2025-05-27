namespace TC.CloudGames.Integration.Tests.Abstractions
{
    [CollectionDefinition(nameof(IntegrationTestCollection))]
    public sealed class IntegrationTestCollection : ICollectionFixture<IntegrationTestWebAppFactory>;
}
