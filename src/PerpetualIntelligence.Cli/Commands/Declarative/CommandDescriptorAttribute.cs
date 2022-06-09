﻿/*
    Copyright (c) Perpetual Intelligence L.L.C. All Rights Reserved.

    For license, terms, and data policies, go to:
    https://terms.perpetualintelligence.com
*/

using PerpetualIntelligence.Cli.Commands.Extractors;
using System;

namespace PerpetualIntelligence.Cli.Commands.Declarative
{
    /// <summary>
    /// Declares the command descriptor for a command runner.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public class CommandDescriptorAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        /// <param name="id">The command id.</param>
        /// <param name="name">The command name.</param>
        /// <param name="prefix">The command prefix to map the command string.</param>
        /// <param name="description">The command description.</param>
        /// <param name="defaultArgument">The default argument.</param>
        public CommandDescriptorAttribute(string id, string name, string prefix, string description, string? defaultArgument = null)
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

            if (string.IsNullOrEmpty(description))
            {
                throw new ArgumentException($"'{nameof(description)}' cannot be null or empty.", nameof(prefix));
            }

            Id = id;
            Name = name;
            Prefix = prefix;
            Description = description;
            DefaultArgument = defaultArgument;
        }

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
        /// <remarks>The command id is unique across all commands.</remarks>
        public string Id { get; }

        /// <summary>
        /// <c>true</c> if this descriptor represents a grouped command; otherwise, <c>false</c>.
        /// </summary>
        public bool IsGroup { get; }

        /// <summary>
        /// Returns <c>true</c> if this descriptor represents a protected command; otherwise, <c>false</c>.
        /// </summary>
        public bool IsProtected { get; }

        /// <summary>
        /// Returns <c>true</c> if this descriptor represents a root command; otherwise, <c>false</c>.
        /// </summary>
        public bool IsRoot { get; }

        /// <summary>
        /// The command name.
        /// </summary>
        /// <remarks>The command name is unique within a grouped command.</remarks>
        public string Name { get; }

        /// <summary>
        /// The prefix to match the command string.
        /// </summary>
        public string Prefix { get; }
    }
}
