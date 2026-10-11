namespace BoardingHouse.API.Configuration;

internal static class EnvironmentFile
{
    public static void Load()
    {
        var workingDirectory = Directory.GetCurrentDirectory();
        var candidates = new List<string> { Path.Combine(workingDirectory, ".env") };
        var parentDirectory = Directory.GetParent(workingDirectory);

        // dotnet run / Visual Studio may use the API project directory as the working directory.
        if (parentDirectory is not null && File.Exists(Path.Combine(parentDirectory.FullName, "BoardingHouse.sln")))
        {
            candidates.Add(Path.Combine(parentDirectory.FullName, ".env"));
        }

        candidates.Add(Path.Combine(AppContext.BaseDirectory, ".env"));
        var envFile = candidates.Distinct(StringComparer.OrdinalIgnoreCase).FirstOrDefault(File.Exists);
        if (envFile is not null)
        {
            DotNetEnv.Env.NoClobber().Load(envFile);
        }
    }
}
