﻿/*
    Copyright (c) 2023 Perpetual Intelligence L.L.C. All Rights Reserved.

    For license, terms, and data policies, go to:
    https://terms.perpetualintelligence.com/articles/intro.html
*/

using PerpetualIntelligence.Terminal.Runtime;
using System.Threading.Tasks;

namespace PerpetualIntelligence.Terminal.Mocks
{
    internal class MockRouting : ITerminalRouting<MockRoutingContext>
    {
        public bool Called
        {
            get; private set;
        }

        public Task RunAsync(MockRoutingContext context)
        {
            Called = true;
            return Task.CompletedTask;
        }
    }
}