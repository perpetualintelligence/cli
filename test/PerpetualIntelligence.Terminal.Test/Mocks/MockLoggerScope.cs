﻿/*
    Copyright (c) 2021 Perpetual Intelligence L.L.C. All Rights Reserved.

    For license, terms, and data policies, go to:
    https://terms.perpetualintelligence.com/articles/intro.html
*/

using System;

namespace PerpetualIntelligence.Terminal.Mocks
{
    public class MockLoggerScope : IDisposable
    {
        public void Dispose()
        {
            Called = true;
        }

        private bool Called { get; set; }
    }
}
