namespace TC.CloudGames.Infra.CrossCutting.Commons.Extensions
{
    public static class SolutionLocator
    {
        /// <summary>
        /// Finds the root directory of the solution by locating a `.sln` file starting from a given path or the current directory.
        /// </summary>
        /// <param name="startPath">Optional starting path; if null, uses current directory.</param>
        /// <param name="solutionName">Optional: filter for a specific solution file name (without extension).</param>
        /// <returns>Full path to the solution directory.</returns>
        public static string FindSolutionRoot(string? startPath = null, string? solutionName = null)
        {
            var current = new DirectoryInfo(startPath ?? Directory.GetCurrentDirectory());

            while (current != null)
            {
                var slnFiles = current.GetFiles("*.sln");

                if (!string.IsNullOrEmpty(solutionName))
                {
                    var match = slnFiles.FirstOrDefault(f => Path.GetFileNameWithoutExtension(f.Name).Equals(solutionName, StringComparison.OrdinalIgnoreCase));
                    if (match != null)
                        return current.FullName;
                }
                else if (slnFiles.Any())
                {
                    return current.FullName;
                }

                current = current.Parent;
            }

            throw new DirectoryNotFoundException("Solution root not found. Make sure you're within a valid solution folder.");
        }
    }

}