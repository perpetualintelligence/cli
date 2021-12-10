﻿/*
    Copyright (c) 2019-2022. All Rights Reserved. Perpetual Intelligence L.L.C.
    https://perpetualintelligence.com
    https://api.perpetualintelligence.com
*/

using Microsoft.Extensions.DependencyInjection;
using System;

namespace PerpetualIntelligence.Cli.Integration
{
    /// <summary>
    /// The default <see cref="ICliBuilder"/>.
    /// </summary>
    public class CliBuilder : ICliBuilder
    {
        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        /// <param name="services">The services.</param>
        /// <exception cref="ArgumentNullException">services</exception>
        public CliBuilder(IServiceCollection services)
        {
            Services = services ?? throw new ArgumentNullException(nameof(services));
        }

        /// <summary>
        /// The service collection.
        /// </summary>
        public IServiceCollection Services { get; }
    }
}
