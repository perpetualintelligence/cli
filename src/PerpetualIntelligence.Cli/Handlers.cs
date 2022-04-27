﻿/*
    Copyright (c) Perpetual Intelligence L.L.C. All Rights Reserved.

    For license, terms, and data policies, go to:
    https://terms.perpetualintelligence.com
*/

namespace PerpetualIntelligence.Cli
{
    /// <summary>
    /// These well-known <c>pi-cli</c> handlers allow app authors to configure the framework and provide custom implementations.
    /// </summary>
    public class Handlers
    {
        /// <summary>
        /// The <c>boyl</c> handler.
        /// </summary>
        public const string BoylHandler = "boyl";

        /// <summary>
        /// The <c>custom</c> handler.
        /// </summary>
        public const string CustomHandler = "custom";

        /// <summary>
        /// The <c>default</c> handler.
        /// </summary>
        public const string DefaultHandler = "default";

        /// <summary>
        /// The <c>json</c> handler.
        /// </summary>
        public const string InMemoryHandler = "in-memory";

        /// <summary>
        /// The <c>json</c> handler.
        /// </summary>
        public const string JsonHandler = "json";

        /// <summary>
        /// The <c>offline</c> handler.
        /// </summary>
        public const string OfflineHandler = "offline";

        /// <summary>
        /// The <c>boyl</c> handler.
        /// </summary>
        public const string OnlineHandler = "online";
    }
}
