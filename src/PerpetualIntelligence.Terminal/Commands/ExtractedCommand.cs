﻿/*
    Copyright (c) 2023 Perpetual Intelligence L.L.C. All Rights Reserved.

    For license, terms, and data policies, go to:
    https://terms.perpetualintelligence.com/articles/intro.html
*/

namespace PerpetualIntelligence.Terminal.Commands
{
    /// <summary>
    /// Represents an extracted command.
    /// </summary>
    public sealed class ExtractedCommand
    {
        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        /// <param name="commandRoute">The command route.</param>
        /// <param name="command">The command.</param>
        /// <param name="hierarchy">The command hierarchy.</param>
        public ExtractedCommand(CommandRoute commandRoute, Command command, Root hierarchy)
        {
            CommandRoute = commandRoute ?? throw new System.ArgumentNullException(nameof(commandRoute));
            Command = command ?? throw new System.ArgumentNullException(nameof(command));
            Hierarchy = hierarchy ?? throw new System.ArgumentNullException(nameof(hierarchy));
        }

        /// <summary>
        /// The command route.
        /// </summary>
        public CommandRoute CommandRoute { get; }

        /// <summary>
        /// The command.
        /// </summary>
        public Command Command { get; }

        /// <summary>
        /// The command hierarchy.
        /// </summary>
        public Root Hierarchy { get; }
    }
}