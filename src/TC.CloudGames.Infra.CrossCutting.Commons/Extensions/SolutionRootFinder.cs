namespace TC.CloudGames.Infra.CrossCutting.Commons.Extensions
{
    public static class SolutionRootFinder
    {
        public static string FindRoot(string? startPath = null, string markerFile = ".solution-root")
        {
            var dir = new DirectoryInfo(startPath ?? Directory.GetCurrentDirectory());

            while (dir != null)
            {
                if (dir.GetFiles(markerFile).Any())
                    return dir.FullName;

                dir = dir.Parent;
            }

            throw new DirectoryNotFoundException($"Marker file '{markerFile}' not found in directory tree.");
        }
    }
}