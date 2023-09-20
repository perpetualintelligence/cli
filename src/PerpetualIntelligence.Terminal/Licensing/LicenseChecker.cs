﻿/*
    Copyright (c) 2023 Perpetual Intelligence L.L.C. All Rights Reserved.

    For license, terms, and data policies, go to:
    https://terms.perpetualintelligence.com/articles/intro.html
*/

using Microsoft.Extensions.Logging;
using PerpetualIntelligence.Shared.Exceptions;
using PerpetualIntelligence.Terminal.Configuration.Options;
using PerpetualIntelligence.Terminal.Stores;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PerpetualIntelligence.Terminal.Licensing
{
    /// <summary>
    /// The default <see cref="ILicenseChecker"/> for all <c>pi-cli</c> features.
    /// </summary>
    public sealed class LicenseChecker : ILicenseChecker
    {
        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        public LicenseChecker(ICommandStoreHandler commandStoreHandler, TerminalOptions terminalOptions, ILogger<LicenseChecker> logger)
        {
            this.commandStoreHandler = commandStoreHandler;
            this.terminalOptions = terminalOptions;
            this.logger = logger;
        }

        /// <summary>
        /// Determines whether the checker is initialized.
        /// </summary>
        public bool Initialized { get => initialized; }

        /// <summary>
        /// Checks the licensing context.
        /// </summary>
        /// <param name="context">The licensing context.</param>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public async Task<LicenseCheckerResult> CheckAsync(LicenseCheckerContext context)
        {
            // Initialize if needed
            await InitializeLockAsync(context.License);

            // Check Limits
            await CheckLimitsAsync(context);

            // Check Options
            await CheckOptionsAsync(context);

            return new LicenseCheckerResult(context.License)
            {
                TerminalCount = terminalCount,
                RootCommandCount = rootCommandCount,
                CommandGroupCount = commandGroupCount,
                SubCommandCount = subCommandCount,
                OptionCount = optionCount,
            };
        }

        private Task<License> CheckLimitsAsync(LicenseCheckerContext context)
        {
            // Terminal limit
            if (terminalCount > context.License.Limits.TerminalLimit)
            {
                throw new ErrorException(TerminalErrors.InvalidLicense, "The terminal limit exceeded. max_limit={0} current={1}", context.License.Limits.TerminalLimit, terminalCount);
            }

            // Redistribution limit TODO, how do we check redistribution in a native bounded context

            // Root command limit
            if (rootCommandCount > context.License.Limits.RootCommandLimit)
            {
                throw new ErrorException(TerminalErrors.InvalidLicense, "The root command limit exceeded. max_limit={0} current={1}", context.License.Limits.RootCommandLimit, rootCommandCount);
            }

            // grouped command limit
            if (commandGroupCount > context.License.Limits.GroupedCommandLimit)
            {
                throw new ErrorException(TerminalErrors.InvalidLicense, "The grouped command limit exceeded. max_limit={0} current={1}", context.License.Limits.GroupedCommandLimit, commandGroupCount);
            }

            // grouped command limit
            if (subCommandCount > context.License.Limits.SubCommandLimit)
            {
                throw new ErrorException(TerminalErrors.InvalidLicense, "The sub command limit exceeded. max_limit={0} current={1}", context.License.Limits.SubCommandLimit, subCommandCount);
            }

            // Option limit
            if (optionCount > context.License.Limits.OptionLimit)
            {
                throw new ErrorException(TerminalErrors.InvalidLicense, "The option limit exceeded. max_limit={0} current={1}", context.License.Limits.OptionLimit, optionCount);
            }

            // We have found a valid license within limit so reset the previous failed and return.
            return Task.FromResult(context.License);
        }

        private Task CheckOptionsAsync(LicenseCheckerContext context)
        {
            // Follow the pricing http://localhost:8080/articles/pi-cli/pricing.html We drive all customization through
            // options and the License sets the allowed options. So here we don't need to check the license plan, just
            // check the options value with license value.
            LicenseLimits limits = context.License.Limits;

            // Strict Data Type
            if (!OptionsValid(limits.StrictDataType, terminalOptions.Checker.StrictOptionValueType))
            {
                throw new ErrorException(TerminalErrors.InvalidLicense, "The configured strict option value type is not allowed for your license edition.");
            }

            // Date Type handler, null allowed for data type handler.
            if (!OptionsValid(limits.DataTypeHandlers, terminalOptions.Handler.DataTypeHandler, allowNullActual: true))
            {
                throw new ErrorException(TerminalErrors.InvalidLicense, "The configured data-type handler is not allowed for your license edition. datatype_handler={0}", terminalOptions.Handler.DataTypeHandler);
            }

            // Unicode handler
            if (!OptionsValid(limits.TextHandlers, terminalOptions.Handler.TextHandler, allowNullActual: false))
            {
                throw new ErrorException(TerminalErrors.InvalidLicense, "The configured text handler is not allowed for your license edition. text_handler={0}", terminalOptions.Handler.TextHandler);
            }

            // Error handler
            if (!OptionsValid(limits.ErrorHandlers, terminalOptions.Handler.ErrorHandler, allowNullActual: false))
            {
                throw new ErrorException(TerminalErrors.InvalidLicense, "The configured error handler is not allowed for your license edition. error_handler={0}", terminalOptions.Handler.ErrorHandler);
            }

            // Store handler
            if (!OptionsValid(limits.StoreHandlers, terminalOptions.Handler.StoreHandler, allowNullActual: false))
            {
                throw new ErrorException(TerminalErrors.InvalidLicense, "The configured store handler is not allowed for your license edition. store_handler={0}", terminalOptions.Handler.StoreHandler);
            }

            // Service handler
            if (!OptionsValid(limits.ServiceHandlers, terminalOptions.Handler.ServiceHandler, allowNullActual: false))
            {
                throw new ErrorException(TerminalErrors.InvalidLicense, "The configured service handler is not allowed for your license edition. service_handler={0}", terminalOptions.Handler.ServiceHandler);
            }

            // License handler
            if (!OptionsValid(limits.LicenseHandlers, terminalOptions.Handler.LicenseHandler, allowNullActual: false))
            {
                throw new ErrorException(TerminalErrors.InvalidLicense, "The configured license handler is not allowed for your license edition. license_handler={0}", terminalOptions.Handler.LicenseHandler);
            }

            return Task.CompletedTask;
        }

        private async Task InitializeLockAsync(License license)
        {
            // Make sure the initialization is thread safe
            await semaphoreSlim.WaitAsync();
            try
            {
                // Make sure we don't double down on limits.
                if (initialized)
                {
                    return;
                }

                // TODO, how do we sync terminal count across apps
                terminalCount = 1;

                var commandDescriptors = await commandStoreHandler.AllAsync();
                foreach (var kvpCmd in commandDescriptors)
                {
                    // Register the commands
                    if (kvpCmd.Value.Type == Commands.CommandType.Root)
                    {
                        rootCommandCount += 1;
                    }
                    else if (kvpCmd.Value.Type == Commands.CommandType.Group)
                    {
                        commandGroupCount += 1;
                    }
                    else if (kvpCmd.Value.Type == Commands.CommandType.SubCommand)
                    {
                        subCommandCount += 1;
                    }
                    else
                    {
                        throw new ErrorException(TerminalErrors.InvalidCommand, "The command type is not supported. type={0}", kvpCmd.Value.Type);
                    }

                    // For now we only care about option count.
                    if (kvpCmd.Value.OptionDescriptors != null)
                    {
                        optionCount += kvpCmd.Value.OptionDescriptors.Count;
                    }
                }

                initialized = true;
            }
            finally
            {
                semaphoreSlim.Release();
            }
        }

        private bool OptionsValid(string[]? allowed, string? actual, bool? allowNullActual = true)
        {
            // The actual value can be null, it represents that the user has not configured the feature.
            if (actual == null)
            {
                if (allowNullActual.GetValueOrDefault())
                {
                    return true;
                }
                else
                {
                    // Some options have non null default value.
                    return false;
                }
            }

            // If expected is null then actual cannot be non null
            if (allowed == null)
            {
                return false;
            }

            return allowed.Contains(actual);
        }

        private bool OptionsValid(bool? allowed, bool? actual)
        {
            // An expected value can be true or false, actual value cannot be true if expected value is false.
            if (allowed.GetValueOrDefault() == false && actual.GetValueOrDefault() == true)
            {
                return false;
            }

            return true;
        }

        private readonly TerminalOptions terminalOptions;
        private readonly ICommandStoreHandler commandStoreHandler;
        private readonly ILogger<LicenseChecker> logger;
        private long optionCount;
        private long commandGroupCount;
        private bool initialized;
        private SemaphoreSlim semaphoreSlim = new(1, 1);
        private long rootCommandCount;
        private long subCommandCount;
        private int terminalCount;
    }
}