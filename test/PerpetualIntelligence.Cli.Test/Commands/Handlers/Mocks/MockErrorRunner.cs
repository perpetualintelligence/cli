﻿/*
    Copyright (c) Perpetual Intelligence L.L.C. All Rights Reserved
    https://perpetualintelligence.com
    https://api.perpetualintelligence.com
*/

using PerpetualIntelligence.Cli.Commands.Runners;
using PerpetualIntelligence.Shared.Infrastructure;
using System.Threading.Tasks;

namespace PerpetualIntelligence.Cli.Commands.Handlers.Mocks
{
    internal class MockErrorRunner : ICommandRunner
    {
        public Task<CommandRunnerResult> RunAsync(CommandRunnerContext context)
        {
            return Task.FromResult(OneImlxResult.NewError<CommandRunnerResult>("test_runner_error", "test_runner_error_desc"));
        }
    }
}
