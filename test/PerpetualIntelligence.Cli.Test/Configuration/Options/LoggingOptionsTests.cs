﻿/*
    Copyright (c) Perpetual Intelligence L.L.C. All Rights Reserved
    https://perpetualintelligence.com
    https://api.perpetualintelligence.com
*/

using Microsoft.VisualStudio.TestTools.UnitTesting;
using PerpetualIntelligence.Test;
using PerpetualIntelligence.Test.Services;

namespace PerpetualIntelligence.Cli.Configuration.Options
{
    [TestClass]
    public class LoggingOptionsTests : OneImlxLogTest
    {
        public LoggingOptionsTests() : base(TestLogger.Create<LoggingOptionsTests>())
        {
        }

        [TestMethod]
        public void LoggingOptionsShouldHaveCorrectDefaultValues()
        {
            LoggingOptions options = new LoggingOptions();

            Assert.AreEqual(null, options.ErrorArguments);
        }
    }
}
