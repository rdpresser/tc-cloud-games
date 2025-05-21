using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace TC.CloudGames.Api.Tests.Shared
{
    [ExcludeFromCodeCoverage]
    internal static class Mapper
    {
        public static TTarget MapProperties<TSource, TTarget>(TSource source, Func<TTarget>? targetFactory = null)
            where TTarget : class
        {
            var target = targetFactory != null ? targetFactory() : Activator.CreateInstance<TTarget>();
            var sourceProps = typeof(TSource).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            var targetProps = typeof(TTarget).GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (var targetProp in targetProps)
            {
                var sourceProp = sourceProps.FirstOrDefault(p => p.Name == targetProp.Name && p.PropertyType == targetProp.PropertyType);
                if (sourceProp != null && targetProp.CanWrite)
                {
                    targetProp.SetValue(target, sourceProp.GetValue(source));
                }
            }
            return target;
        }

        /*example of usage
         * public static CreateGameResponse BuildResponse(CreateGameCommand command)
    {
        var createGameRes = Mapper.MapProperties<CreateGameCommand, CreateGameResponse>(
            command,
            () => new CreateGameResponse(
                Id: Guid.NewGuid(),
                Name: string.Empty,
                ReleaseDate: default,
                AgeRating: string.Empty,
                Description: null,
                DeveloperInfo: new(string.Empty, string.Empty),
                DiskSize: 0,
                Price: 0,
                Playtime: new(0, 0),
                GameDetails: new(null, [], null, string.Empty, string.Empty, null, false),
                SystemRequirements: new(string.Empty, string.Empty),
                Rating: null,
                OfficialLink: null,
                GameStatus: null
            )
        );

        return createGameRes;
    }
         */
    }
}
