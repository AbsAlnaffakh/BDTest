using ModularPipelines.Context;
using ModularPipelines.DotNet;
using ModularPipelines.DotNet.Extensions;
using ModularPipelines.DotNet.Options;
using ModularPipelines.Git.Extensions;
using ModularPipelines.Enums;
using ModularPipelines.Modules;

namespace BDTest.Pipeline.Modules;

public class RunUnitTestsModule : Module<List<DotNetTestResult>>
{
    protected override async Task<List<DotNetTestResult>?> ExecuteAsync(IPipelineContext context, CancellationToken cancellationToken)
    {
        var results = new List<DotNetTestResult>();

        foreach (var unitTestProjectFile in context
                     .Git().RootDirectory!
                     .GetFiles(file => file.Path.EndsWith(".csproj", StringComparison.OrdinalIgnoreCase)
                                       && file.Path.Contains("Tests", StringComparison.OrdinalIgnoreCase)))
        {
            results.Add(await context.DotNet().Test(new DotNetTestOptions
            {
                ProjectSolutionDirectoryDllExe = unitTestProjectFile.Path,
                CommandLogging = CommandLogging.Input | CommandLogging.Error,
            }, cancellationToken));
        }

        return results;
    }
}
