﻿/*
    Copyright (c) Perpetual Intelligence L.L.C. All Rights Reserved.

    For license, terms, and data policies, go to:
    https://terms.perpetualintelligence.com
*/

using Microsoft.Extensions.Logging;
using PerpetualIntelligence.Cli.Configuration.Options;
using PerpetualIntelligence.Cli.Runtime;
using System.Threading.Tasks;

namespace PerpetualIntelligence.Cli.Commands.Runners
{
    /// <summary>
    /// Runs an OS command. This runner will be available in a future release. It does not perform any action right now.
    /// For more information on road-map, <see href="https://docs.perpetualintelligence.com/articles/pi-cli/roadmap.html"/>
    /// </summary>
    public class RunRunner : CommandRunner<CommandRunnerResult>
    {
        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        /// <param name="options">The configuration options.</param>
        /// <param name="terminalLogger">The terminal logger.</param>
        /// <param name="logger">The logger.</param>
        public RunRunner(CliOptions options, ITerminalLogger terminalLogger, ILogger<ExitRunner> logger)
        {
        }

        /// <inheritdoc/>
        public override Task<CommandRunnerResult> RunAsync(CommandRunnerContext context)
        {
            // TODO;
            return Task.FromResult(new CommandRunnerResult());
        }
    }
}