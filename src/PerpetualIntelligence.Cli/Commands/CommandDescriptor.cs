﻿/*
    Copyright (c) Perpetual Intelligence L.L.C. All Rights Reserved.

    For license, terms, and data policies, go to:
    https://terms.perpetualintelligence.com
*/

using PerpetualIntelligence.Cli.Commands.Extractors;
using PerpetualIntelligence.Shared.Attributes;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("PerpetualIntelligence.Cli.Test")]

namespace PerpetualIntelligence.Cli.Commands
{
    /// <summary>
    /// An immutable <c>cli</c> command description.
    /// </summary>
    /// <remarks>
    /// The <see cref="CommandDescriptor"/> defines <see cref="Command"/> identity and its supported
    /// <see cref="Argument"/>. The <see cref="Command"/> is a runtime validated representation of an actual command and
    /// its argument values passed by a user or an application.
    /// </remarks>
    /// <seealso cref="Command"/>
    /// <seealso cref="ArgumentDescriptor"/>
    public sealed class CommandDescriptor
    {
        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        /// <param name="id">The command id.</param>
        /// <param name="name">The command name.</param>
        /// <param name="prefix">The command prefix to map the command string.</param>
        /// <param name="description">The command description.</param>
        /// <param name="argumentDescriptors">The command argument descriptors.</param>
        /// <param name="properties">The custom properties.</param>
        /// <param name="defaultArgument">The default argument.</param>
        public CommandDescriptor(string id, string name, string prefix, string description, ArgumentDescriptors? argumentDescriptors = null, Dictionary<string, object>? properties = null, string? defaultArgument = null)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                throw new ArgumentException($"'{nameof(id)}' cannot be null or whitespace.", nameof(id));
            }

            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException($"'{nameof(name)}' cannot be null or whitespace.", nameof(name));
            }

            if (string.IsNullOrEmpty(prefix))
            {
                throw new ArgumentException($"'{nameof(prefix)}' cannot be null or empty.", nameof(prefix));
            }

            Id = id;
            Name = name;
            Prefix = prefix;
            Description = description;
            ArgumentDescriptors = argumentDescriptors;
            Properties = properties;
            DefaultArgument = defaultArgument;
        }

        /// <summary>
        /// The command argument descriptors.
        /// </summary>
        public ArgumentDescriptors? ArgumentDescriptors { get; }

        /// <summary>
        /// The default argument. <c>null</c> means the command does not support a default argument.
        /// </summary>
        /// <remarks>
        /// <see cref="DefaultArgument"/> is not the default argument value (see
        /// <see cref="ArgumentDescriptor.DefaultValue"/>), it is the default argument identifier (see
        /// <see cref="ArgumentDescriptor.Id"/>) whose value is populated automatically based on the
        /// <see cref="CommandString"/>. If <see cref="DefaultArgument"/> is set to a non <c>null</c> value, then the
        /// <see cref="ICommandExtractor"/> will attempt to extract the value from the <see cref="CommandString"/> and
        /// put it in an <see cref="Argument"/> identified by <see cref="DefaultArgument"/>.
        /// </remarks>
        /// <seealso cref="ArgumentDescriptor.DefaultValue"/>
        public string? DefaultArgument { get; }

        /// <summary>
        /// The command description.
        /// </summary>
        public string Description { get; }

        /// <summary>
        /// The command id.
        /// </summary>
        /// <remarks>The command id is unique across all command group.</remarks>
        public string Id { get; }

        /// <summary>
        /// Determines if the descriptor represents a command group.
        /// </summary>
        public bool IsGroup { get; }

        /// <summary>
        /// The command name.
        /// </summary>
        /// <remarks>The command name is unique within a command group.</remarks>
        public string Name { get; }

        /// <summary>
        /// The prefix to match the command string.
        /// </summary>
        public string Prefix { get; }

        /// <summary>
        /// The custom properties.
        /// </summary>
        public Dictionary<string, object>? Properties { get; }

        /// <summary>
        /// Attempts to find an argument descriptor.
        /// </summary>
        /// <param name="argId">The argument descriptor identifier.</param>
        /// <param name="argumentDescriptor">The argument descriptor if found.</param>
        /// <returns><c>true</c> if an argument descriptor exist in the collection, otherwise <c>false</c>.</returns>
        public bool TryGetArgumentDescriptor(string argId, out ArgumentDescriptor argumentDescriptor)
        {
            if (ArgumentDescriptors == null)
            {
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
                argumentDescriptor = default;
                return false;
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
            }

#if NETSTANDARD2_1_OR_GREATER
            return ArgumentDescriptors.TryGetValue(argId, out argumentDescriptor);
#else
            if (ArgumentDescriptors.Contains(argId))
            {
                argumentDescriptor = ArgumentDescriptors[argId];
                return true;
            }
            else
            {
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
                argumentDescriptor = default;
                return false;
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
            }
#endif
        }

        /// <summary>
        /// The command checker.
        /// </summary>
        [InternalInfrastructure]
        internal Type? Checker { get; set; }

        /// <summary>
        /// The command runner.
        /// </summary>
        [InternalInfrastructure]
        internal Type? Runner { get; set; }
    }
}
