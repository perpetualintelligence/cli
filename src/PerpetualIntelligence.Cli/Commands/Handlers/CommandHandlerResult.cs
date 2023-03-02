﻿/*
    Copyright (c) Perpetual Intelligence L.L.C. All Rights Reserved.

    For license, terms, and data policies, go to:
    https://terms.perpetualintelligence.com
*/

using PerpetualIntelligence.Cli.Commands.Checkers;
using PerpetualIntelligence.Cli.Commands.Runners;

namespace PerpetualIntelligence.Cli.Commands.Handlers
{
    /// <summary>
    /// The command handler result.
    /// </summary>
    public class CommandHandlerResult
    {
        /// <summary>
        /// Initialize a new instance.
        /// </summary>
        /// <param name="runnerResult">The command runner result.</param>
        /// <param name="checkerResult">The command checker result.</param>
        public CommandHandlerResult(CommandRunnerResult runnerResult, CommandCheckerResult checkerResult)
        {
            RunnerResult = runnerResult;
            CheckerResult = checkerResult;
        }

        /// <summary>
        /// The command runner result.
        /// </summary>
        public CommandRunnerResult RunnerResult { get; }

        /// <summary>
        /// The command checker result.
        /// </summary>
        public CommandCheckerResult CheckerResult { get; }
    }
}