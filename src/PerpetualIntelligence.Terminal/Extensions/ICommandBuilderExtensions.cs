﻿/*
    Copyright (c) 2023 Perpetual Intelligence L.L.C. All Rights Reserved.

    For license, terms, and data policies, go to:
    https://terms.perpetualintelligence.com/articles/intro.html
*/

using Microsoft.Extensions.DependencyInjection;
using PerpetualIntelligence.Terminal.Commands;
using PerpetualIntelligence.Terminal.Hosting;
using System;
using System.Linq;

namespace PerpetualIntelligence.Terminal.Extensions
{
    /// <summary>
    /// The <see cref="ICommandBuilder"/> extension methods.
    /// </summary>
    public static class ICommandBuilderExtensions
    {
        /// <summary>
        /// Adds a command owners to the <see cref="ICommandBuilder"/>.
        /// </summary>
        /// <param name="builder">The <see cref="ICommandBuilder"/>.</param>
        /// <param name="owners">The owner identifiers.</param>
        /// <returns>The configured <see cref="ICommandBuilder"/>.</returns>
        public static ICommandBuilder Owners(this ICommandBuilder builder, OwnerCollection owners)
        {
            if (!owners.Any())
            {
                throw new InvalidOperationException("The owners cannot be null or empty.");
            }

            builder.Services.AddSingleton(owners);
            return builder;
        }

        /// <summary>
        /// Adds a command custom property to the <see cref="ICommandBuilder"/>.
        /// </summary>
        /// <param name="builder">The <see cref="ICommandBuilder"/>.</param>
        /// <param name="key">The custom property key.</param>
        /// <param name="value">The custom property value.</param>
        /// <returns>The configured <see cref="ICommandBuilder"/>.</returns>
        public static ICommandBuilder CustomProperty(this ICommandBuilder builder, string key, object value)
        {
            builder.Services.AddSingleton(new Tuple<string, object>(key, value));
            return builder;
        }

        /// <summary>
        /// Starts a new <see cref="IOptionBuilder"/> definition.
        /// </summary>
        /// <param name="builder">The <see cref="ICommandBuilder"/>.</param>
        /// <param name="id">The option id.</param>
        /// <param name="dataType">The option data type.</param>
        /// <param name="description">The option description.</param>
        /// <param name="flags">The option flags.</param>
        /// <param name="alias">The option alias.</param>
        /// <returns>The configured <see cref="IOptionBuilder"/>.</returns>
        public static IOptionBuilder DefineOption(this ICommandBuilder builder, string id, string dataType, string description, OptionFlags flags, string? alias = null)
        {
            OptionDescriptor option = new(id, dataType, description, flags, alias);
            OptionBuilder argumentBuilder = new(builder);
            argumentBuilder.Services.AddSingleton(option);
            return argumentBuilder;
        }

        /// <summary>
        /// Starts a new <see cref="IArgumentBuilder"/> definition.
        /// </summary>
        /// <param name="builder">The <see cref="ICommandBuilder"/>.</param>
        /// <param name="order">The argument order.</param>
        /// <param name="id">The argument id.</param>
        /// <param name="dataType">The argument data type.</param>
        /// <param name="description">The argument description.</param>
        /// <param name="flags">The argument flags.</param>
        /// <returns>The configured <see cref="IArgumentBuilder"/>.</returns>
        public static IArgumentBuilder DefineArgument(this ICommandBuilder builder, int order, string id, string dataType, string description, ArgumentFlags flags)
        {
            ArgumentDescriptor argument = new(order, id, dataType, description, flags);
            ArgumentBuilder argumentBuilder = new(builder);
            argumentBuilder.Services.AddSingleton(argument);
            return argumentBuilder;
        }

        /// <summary>
        /// Adds command tags to the <see cref="ICommandBuilder"/>.
        /// </summary>
        /// <param name="builder">The <see cref="ICommandBuilder"/>.</param>
        /// <param name="tags">The tags.</param>
        /// <returns>The configured <see cref="ICommandBuilder"/>.</returns>
        public static ICommandBuilder Tags(this ICommandBuilder builder, TagCollection tags)
        {
            if (!tags.Any())
            {
                throw new InvalidOperationException("The tags cannot be null or empty.");
            }

            builder.Services.AddSingleton(tags);
            return builder;
        }
    }
}