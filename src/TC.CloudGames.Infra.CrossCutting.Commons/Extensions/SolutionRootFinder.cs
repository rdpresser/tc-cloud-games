namespace TC.CloudGames.Infra.CrossCutting.Commons.Extensions
{
    public static class SolutionRootFinder
    {
        public static string FindRoot(string markerFile = ".solution-root", params string[] startPaths)
        {
            // If no startPaths are provided, use the current directory
            if (startPaths == null || startPaths.Length == 0)
            {
                startPaths = new[] { Directory.GetCurrentDirectory() };
            }

            foreach (var startPath in startPaths)
            {
                var dir = new DirectoryInfo(startPath);

                while (dir != null)
                {
                    try
                    {
                        if (dir.GetFiles(markerFile).Any())
                            return dir.FullName;

                        dir = dir.Parent;
                    }
                    catch
                    {
                        // If any exception occurs (e.g., access denied), skip to next startPath
                        break;
                    }
                }
            }

            throw new DirectoryNotFoundException($"Marker file '{markerFile}' not found in any provided directory tree.");
        }
    }
}