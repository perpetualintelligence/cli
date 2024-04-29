﻿/*
    Copyright 2024 (c) Perpetual Intelligence L.L.C. All Rights Reserved.

    For license, terms, and data policies, go to:
    https://terms.perpetualintelligence.com/articles/intro.html
*/

using System.Reflection;
using System.Runtime.CompilerServices;
using FluentAssertions;
using OneImlx.Test.FluentAssertions;
using Xunit;

namespace OneImlx.Terminal
{
    public class TerminalIdentifiersTests
    {
        [Fact]
        public void TerminalIdentifiers_Defines_Assembly_InternalVisibleTo_ForUnitTests()
        {
            InternalsVisibleToAttribute? internalsVisibleToAttribute = typeof(TerminalIdentifiers).Assembly.GetCustomAttribute<InternalsVisibleToAttribute>();
            internalsVisibleToAttribute.Should().NotBeNull();
            internalsVisibleToAttribute!.AssemblyName.Should().Be("OneImlx.Terminal.Tests");
        }

        [Fact]
        public void TerminalIdentifiers_Defines_Identifiers()
        {
            typeof(TerminalIdentifiers).Should().HaveConstantCount(8);

            TerminalIdentifiers.OnlineLicenseMode.Should().Be("online");
            TerminalIdentifiers.OfflineLicenseMode.Should().Be("offline");
            TerminalIdentifiers.OnPremiseDeployment.Should().Be("onpremise");
            TerminalIdentifiers.CustomHandler.Should().Be("custom");
            TerminalIdentifiers.DefaultHandler.Should().Be("default");
            TerminalIdentifiers.TestApplicationId.Should().Be("08c6925f-a734-4e24-8d84-e06737420766");
            TerminalIdentifiers.RemoteCommandDelimiter.Should().Be("$c$");
            TerminalIdentifiers.RemoteMessageDelimiter.Should().Be("$m$");
        }
    }
}
