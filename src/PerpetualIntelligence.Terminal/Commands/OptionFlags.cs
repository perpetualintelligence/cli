﻿/*
    Copyright (c) 2023 Perpetual Intelligence L.L.C. All Rights Reserved.

    For license, terms, and data policies, go to:
    https://terms.perpetualintelligence.com/articles/intro.html
*/

using System;

namespace PerpetualIntelligence.Terminal.Commands
{
    /// <summary>
    /// Defines special option flags.
    /// </summary>
    [Flags]
    public enum OptionFlags
    {
        /// <summary>
        /// No special flags.
        /// </summary>
        None = 0,

        /// <summary>
        /// The option is required.
        /// </summary>
        Required = 2,

        /// <summary>
        /// The command is obsolete.
        /// </summary>
        Obsolete = 4,

        /// <summary>
        /// The command is disabled.
        /// </summary>
        Disabled = 8,
    }
}