﻿/*
    Copyright (c) 2023 Perpetual Intelligence L.L.C. All Rights Reserved.

    For license, terms, and data policies, go to:
    https://terms.perpetualintelligence.com/articles/intro.html
*/

using PerpetualIntelligence.Terminal.Commands.Routers;
using PerpetualIntelligence.Terminal.Licensing;
using System;

namespace PerpetualIntelligence.Terminal.Commands.Handlers
{
    /// <summary>
    /// The command handler context.
    /// </summary>
    public sealed class CommandHandlerContext
    {
        /// <summary>
        /// Initialize a new instance.
        /// </summary>
        /// <param name="routerContext">The command router context.</param>
        /// <param name="extractedCommand">The extracted command handle.</param>
        /// <param name="license">The extracted license.</param>
        /// <exception cref="ArgumentNullException"></exception>
        public CommandHandlerContext(CommandRouterContext routerContext, ExtractedCommand extractedCommand, License license)
        {
            RouterContext = routerContext ?? throw new ArgumentNullException(nameof(routerContext));
            ExtractedCommand = extractedCommand ?? throw new ArgumentNullException(nameof(extractedCommand));
            License = license ?? throw new ArgumentNullException(nameof(license));
        }

        /// <summary>
        /// The command router context.
        /// </summary>
        public CommandRouterContext RouterContext { get; }

        /// <summary>
        /// The extracted command to handle.
        /// </summary>
        public ExtractedCommand ExtractedCommand { get; }

        /// <summary>
        /// The extracted licenses.
        /// </summary>
        public License License { get; }
    }
}