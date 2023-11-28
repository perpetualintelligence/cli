﻿/*
    Copyright (c) 2023 Perpetual Intelligence L.L.C. All Rights Reserved.

    For license, terms, and data policies, go to:
    https://terms.perpetualintelligence.com/articles/intro.html
*/
using Microsoft.Extensions.Logging;
using Microsoft.Kiota.Abstractions.Authentication;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace PerpetualIntelligence.Terminal.Authentication.Msal
{
    public class TestMsalAccessTokenProviderDelegatingHandler : MsalAccessTokenProviderDelegatingHandler
    {
        public bool PreflightAsyncCalled { get; private set; } = false;

        public TestMsalAccessTokenProviderDelegatingHandler(IAccessTokenProvider accessTokenProvider, ILogger<MsalAccessTokenProviderDelegatingHandler> logger)
            : base(accessTokenProvider, logger)
        {
        }

        protected override async Task PreflightAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            PreflightAsyncCalled = true;
            await base.PreflightAsync(request, cancellationToken);
        }
    }
}