﻿/*
    Copyright (c) Perpetual Intelligence L.L.C. All Rights Reserved.

    For license, terms, and data policies, go to:
    https://terms.perpetualintelligence.com
*/

using Microsoft.VisualStudio.TestTools.UnitTesting;
using PerpetualIntelligence.Cli.Commands.Handlers;
using PerpetualIntelligence.Cli.Commands.Providers;
using PerpetualIntelligence.Cli.Configuration.Options;
using PerpetualIntelligence.Cli.Mocks;
using PerpetualIntelligence.Cli.Stores;
using PerpetualIntelligence.Cli.Stores.InMemory;
using PerpetualIntelligence.Test;
using PerpetualIntelligence.Test.Services;
using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace PerpetualIntelligence.Cli.Commands.Extractors
{
    [TestClass]
    public class UnicodeCommandExtractorTests : InitializerTests
    {
        public UnicodeCommandExtractorTests() : base(TestLogger.Create<CommandExtractorTests>())
        {
        }

        [TestMethod]
        public async Task UnicodeGroupedCommand_Should_Extract_Correctly()
        {
            CommandExtractorContext context = new(new CommandString("यूनिकोड परीक्षण"));
            var result = await extractor.ExtractAsync(context);

            Assert.IsNotNull(result.CommandDescriptor);
            Assert.AreEqual("यूनिकोड परीक्षण", result.CommandDescriptor.Prefix);
            Assert.IsFalse(result.CommandDescriptor.IsRoot);
            Assert.IsTrue(result.CommandDescriptor.IsGroup);

            Assert.IsNotNull(result.Command);
            Assert.AreEqual("uc2", result.Command.Id);
            Assert.AreEqual("परीक्षण", result.Command.Name);
            Assert.AreEqual("यूनिकोड समूहीकृत कमांड", result.Command.Description);
            Assert.IsNull(result.Command.Arguments);
        }

        [TestMethod]
        public async Task UnicodeGroupedCommand_With_Imcomplete_Prefix_ShouldError()
        {
            CommandExtractorContext context = new(new CommandString("परीक्षण"));
            await TestHelper.AssertThrowsErrorExceptionAsync(() => extractor.ExtractAsync(context), Errors.UnsupportedCommand, "The command prefix is not valid. prefix=परीक्षण");
        }

        [TestMethod]
        public async Task UnicodeRootCommand_Should_Extract_Correctly()
        {
            CommandExtractorContext context = new(new CommandString("यूनिकोड"));
            var result = await extractor.ExtractAsync(context);

            Assert.IsNotNull(result.CommandDescriptor);
            Assert.AreEqual("यूनिकोड", result.CommandDescriptor.Prefix);
            Assert.IsTrue(result.CommandDescriptor.IsRoot);

            Assert.IsNotNull(result.Command);
            Assert.AreEqual("uc1", result.Command.Id);
            Assert.AreEqual("यूनिकोड", result.Command.Name);
            Assert.AreEqual("यूनिकोड रूट कमांड", result.Command.Description);
            Assert.IsNull(result.Command.Arguments);
        }

        [TestMethod]
        public async Task UnicodeSubCommand_Should_Extract_Correctly()
        {
            CommandExtractorContext context = new(new CommandString("यूनिकोड परीक्षण प्रिंट --एक पहला मूल्य --दो --तीन तीसरा मूल्य --चार 253.36"));
            var result = await extractor.ExtractAsync(context);

            Assert.IsNotNull(result.CommandDescriptor);
            Assert.AreEqual("यूनिकोड परीक्षण प्रिंट", result.CommandDescriptor.Prefix);

            Assert.IsNotNull(result.Command);
            Assert.AreEqual("uc3", result.Command.Id);
            Assert.AreEqual("प्रिंट", result.Command.Name);
            Assert.AreEqual("प्रिंट कमांड", result.Command.Description);
            Assert.IsNotNull(result.Command.Arguments);
            Assert.AreEqual(4, result.Command.Arguments.Count);

            AssertArgument(result.Command.Arguments[0], "एक", DataType.Text, "पहला तर्क", "पहला मूल्य");
            AssertArgument(result.Command.Arguments[1], "दो", nameof(Boolean), "दूसरा तर्क", true.ToString());
            AssertArgument(result.Command.Arguments[2], "तीन", DataType.PhoneNumber, "तीसरा तर्क", "तीसरा मूल्य");
            AssertArgument(result.Command.Arguments[3], "चार", nameof(Double), "चौथा तर्क", "253.36");
        }

        [TestMethod]
        public async Task UnicodeSubCommand_Alias_Should_Extract_Correctly()
        {
            // एकहै and चारहै are alias
            CommandExtractorContext context = new(new CommandString("यूनिकोड परीक्षण प्रिंट -एकहै पहला मूल्य --दो --तीन तीसरा मूल्य -चारहै 253.36"));
            var result = await extractor.ExtractAsync(context);

            Assert.IsNotNull(result.CommandDescriptor);
            Assert.AreEqual("यूनिकोड परीक्षण प्रिंट", result.CommandDescriptor.Prefix);

            Assert.IsNotNull(result.Command);
            Assert.AreEqual("uc3", result.Command.Id);
            Assert.AreEqual("प्रिंट", result.Command.Name);
            Assert.AreEqual("प्रिंट कमांड", result.Command.Description);
            Assert.IsNotNull(result.Command.Arguments);
            Assert.AreEqual(4, result.Command.Arguments.Count);

            AssertArgument(result.Command.Arguments[0], "एक", DataType.Text, "पहला तर्क", "पहला मूल्य");
            AssertArgument(result.Command.Arguments[1], "दो", nameof(Boolean), "दूसरा तर्क", true.ToString());
            AssertArgument(result.Command.Arguments[2], "तीन", DataType.PhoneNumber, "तीसरा तर्क", "तीसरा मूल्य");
            AssertArgument(result.Command.Arguments[3], "चार", nameof(Double), "चौथा तर्क", "253.36");
        }

        [TestMethod]
        public async Task UnicodeSubCommand_Default_Should_Extract_Correctly()
        {
            options.Extractor.DefaultArgumentValue = true;
            options.Extractor.DefaultArgument = true;

            // एक is required and has default value
            CommandExtractorContext context = new(new CommandString("यूनिकोड परीक्षण दूसरा --दो --तीन तीसरा मूल्य -चारहै 253.36"));
            var result = await extractor.ExtractAsync(context);

            Assert.IsNotNull(result.CommandDescriptor);
            Assert.AreEqual("यूनिकोड परीक्षण दूसरा", result.CommandDescriptor.Prefix);

            Assert.IsNotNull(result.Command);
            Assert.AreEqual("uc4", result.Command.Id);
            Assert.AreEqual("दूसरा", result.Command.Name);
            Assert.AreEqual("दूसरा आदेश", result.Command.Description);
            Assert.IsNotNull(result.Command.Arguments);
            Assert.AreEqual(4, result.Command.Arguments.Count);

            
            AssertArgument(result.Command.Arguments[0], "दो", nameof(Boolean), "दूसरा तर्क", true.ToString());
            AssertArgument(result.Command.Arguments[1], "तीन", DataType.PhoneNumber, "तीसरा तर्क", "तीसरा मूल्य");
            AssertArgument(result.Command.Arguments[2], "चार", nameof(Double), "चौथा तर्क", "253.36");

            // Default added at the end
            AssertArgument(result.Command.Arguments[3], "एक", DataType.Text, "पहला तर्क", "डिफ़ॉल्ट मान");
        }

        protected override void OnTestInitialize()
        {
            options = MockCliOptions.NewOptions();
            textHandler = new UnicodeTextHandler();
            argExtractor = new ArgumentExtractor(textHandler, options, TestLogger.Create<ArgumentExtractor>());
            commands = new InMemoryCommandStore(textHandler, MockCommands.UnicodeCommands, options, TestLogger.Create<InMemoryCommandStore>());
            argExtractor = new ArgumentExtractor(textHandler, options, TestLogger.Create<ArgumentExtractor>());
            defaultArgValueProvider = new DefaultArgumentValueProvider(textHandler);
            defaultArgProvider = new DefaultArgumentProvider(options, TestLogger.Create<DefaultArgumentProvider>());
            extractor = new CommandExtractor(commands, argExtractor, textHandler, options, TestLogger.Create<CommandExtractor>(), defaultArgProvider, defaultArgValueProvider);
        }

        private void AssertArgument(Argument arg, string name, DataType dataType, string description, object value)
        {
            Assert.AreEqual(arg.Id, name);
            Assert.AreEqual(arg.DataType, dataType);
            Assert.IsNull(arg.CustomDataType);
            Assert.AreEqual(arg.Description, description);
            Assert.AreEqual(arg.Value, value);
        }

        private void AssertArgument(Argument arg, string name, string customDataType, string description, object value)
        {
            Assert.AreEqual(arg.Id, name);
            Assert.AreEqual(arg.DataType, DataType.Custom);
            Assert.AreEqual(arg.CustomDataType, customDataType);
            Assert.AreEqual(arg.Description, description);
            Assert.AreEqual(arg.Value, value);
        }

        private ArgumentExtractor argExtractor = null!;
        private ICommandStoreHandler commands = null!;
        private IDefaultArgumentProvider defaultArgProvider = null!;
        private IDefaultArgumentValueProvider defaultArgValueProvider = null!;
        private CommandExtractor extractor = null!;
        private CliOptions options = null!;
        private ITextHandler textHandler = null!;
    }
}
