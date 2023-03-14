﻿/*
    Copyright (c) Perpetual Intelligence L.L.C. All Rights Reserved.

    For license, terms, and data policies, go to:
    https://terms.perpetualintelligence.com
*/

using Microsoft.VisualStudio.TestTools.UnitTesting;
using PerpetualIntelligence.Cli.Commands.Handlers;
using PerpetualIntelligence.Cli.Configuration.Options;
using PerpetualIntelligence.Cli.Mocks;

using PerpetualIntelligence.Test;
using PerpetualIntelligence.Test.Services;
using System;
using System.Threading.Tasks;

namespace PerpetualIntelligence.Cli.Commands.Extractors
{
    [TestClass]
    public class ArgumentExtractorTests : InitializerTests
    {
        public ArgumentExtractorTests() : base(TestLogger.Create<ArgumentExtractorTests>())
        {
        }

        [DataTestMethod]
        [DataRow("~")]
        [DataRow(":")]
        [DataRow("~")]
        [DataRow("#")]
        [DataRow("-")]
        [DataRow("öö")]
        [DataRow("मासे")]
        public async Task ArgumentAliasNotConfiguredButAliasPrefixUsedShouldErrorAsync(string aliasPrefix)
        {
            options.Extractor.ArgumentPrefix = "--";
            options.Extractor.ArgumentAliasPrefix = aliasPrefix;

            CommandDescriptor cmd = new("i1", "n1", "p1", "desc1", new OptionDescriptors(textHandler, new[] { new OptionDescriptor("key1", System.ComponentModel.DataAnnotations.DataType.Text, "desc1") }));
            OptionExtractorContext context = new(new OptionString($"{aliasPrefix}key1=value1", aliasPrefix: true, 0), cmd);
            await TestHelper.AssertThrowsErrorExceptionAsync(() => extractor.ExtractAsync(context), Errors.InvalidConfiguration, $"The option extraction by alias prefix is not configured. argument_string={aliasPrefix}key1=value1");
        }

        [TestMethod]
        public void ArgumentExtractor_RegexPatterns_ShouldBeValid()
        {
            Assert.AreEqual("^[ ]*(-)+(.+?)[ ]*$", extractor.ArgumentAliasNoValueRegexPattern);
            Assert.AreEqual("^[ ]*(-)+(.+?)=+(.*?)[ ]*$", extractor.ArgumentAliasValueRegexPattern);
            Assert.AreEqual("^[ ]*(-)+(.+?)[ ]*$", extractor.ArgumentIdNoValueRegexPattern);
            Assert.AreEqual("^[ ]*(-)+(.+?)=+(.*?)[ ]*$", extractor.ArgumentIdValueRegexPattern);
            Assert.AreEqual("^(.*)$", extractor.ArgumentValueWithinRegexPattern);
        }

        [DataTestMethod]
        [DataRow("ü")]
        [DataRow(":")]
        [DataRow("~")]
        [DataRow("#")]
        [DataRow("-")]
        [DataRow("--")]
        [DataRow("öö")]
        [DataRow("मासे")]
        [DataRow("女性")]
        public async Task ArgumentValueWithArgumentSeparatorShouldNotErrorAsync(string separator)
        {
            options.Extractor.ArgumentValueSeparator = separator;

            OptionExtractorContext context = new(new OptionString($"-key1{separator}value{separator}value2{separator}"), command.Item1);
            var result = await extractor.ExtractAsync(context);
            Assert.IsNotNull(result.Argument);
            Assert.AreEqual($"key1", result.Argument.Id);
            Assert.AreEqual($"value{separator}value2{separator}", result.Argument.Value);
        }

        [TestMethod]
        public async Task ArgumentWithoutPrefixShouldError()
        {
            // Argument extractor does not work with prefix
            CommandDescriptor cmd = new("i1", "n1", "p1", "desc1", new OptionDescriptors(textHandler, new[] { new OptionDescriptor("key1", System.ComponentModel.DataAnnotations.DataType.Text, "desc1") }));
            OptionExtractorContext context = new(new OptionString($"key1=value1"), cmd);
            await TestHelper.AssertThrowsErrorExceptionAsync(() => extractor.ExtractAsync(context), Errors.InvalidArgument, $"The option string is not valid. argument_string=key1=value1");
        }

        [TestMethod]
        public async Task ArgumentWithoutPrefixShouldErrorAsync()
        {
            OptionExtractorContext context = new(new OptionString($"key1=value"), command.Item1);
            await TestHelper.AssertThrowsErrorExceptionAsync(() => extractor.ExtractAsync(context), Errors.InvalidArgument, "The option string is not valid. argument_string=key1=value");
        }

        [TestMethod]
        public async Task AttrubuteShouldSetTheArgumentIdCorrectlyAsync()
        {
            OptionExtractorContext context = new(new OptionString("-key6"), command.Item1);
            var result = await extractor.ExtractAsync(context);
            Assert.IsNotNull(result.Argument);
            Assert.AreEqual("key6", result.Argument.Id);
        }

        [TestMethod]
        public async Task EmptyArgumentIdShouldErrorAsync()
        {
            OptionExtractorContext context = new(new OptionString($"-  =value"), command.Item1);
            await TestHelper.AssertThrowsErrorExceptionAsync(() => extractor.ExtractAsync(context), Errors.InvalidArgument, "The option identifier is null or empty. argument_string=-  =value");
        }

        [DataTestMethod]
        [DataRow("~", "`")]
        [DataRow("#", "öö")]
        [DataRow("öö", "मा")]
        [DataRow("öö", "-")]
        [DataRow("मासे", "#")]
        public async Task InvalidArgumentValueSepratorShouldErrorAsync(string valid, string invalid)
        {
            // Set the correct separator
            options.Extractor.ArgumentValueSeparator = valid;

            // Arg string has incorrect separator Without the valid value separator the extractor will interpret as a
            // key only option and that wil fail
            OptionExtractorContext context = new(new OptionString($"-key1{invalid}value1"), command.Item1);
            await TestHelper.AssertThrowsErrorExceptionAsync(() => extractor.ExtractAsync(context), Errors.UnsupportedArgument, $"The option is not supported. option=key1{invalid}value1");
        }

        [DataTestMethod]
        [DataRow("=")]
        [DataRow(" ")]
        [DataRow(":")]
        [DataRow("~")]
        [DataRow("#")]
        [DataRow("-")]
        [DataRow("--")]
        [DataRow("öö")]
        [DataRow("मासे")]
        [DataRow("女性")]
        [DataRow("माणू女性")]
        public async Task KeySeparatorValueShouldNotErrorAsync(string argSeparator)
        {
            options.Extractor.ArgumentValueSeparator = argSeparator;

            OptionExtractorContext context = new(new OptionString($"-key1{argSeparator}value1"), command.Item1);
            var result = await extractor.ExtractAsync(context);
            Assert.IsNotNull(result.Argument);
            Assert.AreEqual($"key1", result.Argument.Id);
            Assert.AreEqual($"value1", result.Argument.Value);
        }

        [TestMethod]
        public async Task KeyValueArgumentShouldSetTheArgumentIdAndValueCorrecltyAsync()
        {
            OptionExtractorContext context = new(new OptionString($"-key5=htts://google.com"), command.Item1);
            var result = await extractor.ExtractAsync(context);
            Assert.IsNotNull(result.Argument);
            Assert.AreEqual("key5", result.Argument.Id);
            Assert.AreEqual("htts://google.com", result.Argument.Value);
        }

        [DataTestMethod]
        [DataRow("ü")]
        [DataRow(":")]
        [DataRow("~")]
        [DataRow("#")]
        [DataRow("-")]
        [DataRow("--")]
        [DataRow("öö")]
        [DataRow("मासे")]
        [DataRow("女性")]
        public async Task MultiplePrefixBeforeArgumentIdShouldNotErrorAsync(string prefix)
        {
            options.Extractor.ArgumentPrefix = prefix;

            CommandDescriptor cmd = new("i1", "n1", "p1", "desc1", new OptionDescriptors(textHandler, new[] { new OptionDescriptor("key1", System.ComponentModel.DataAnnotations.DataType.Text, "desc1") }));
            OptionExtractorContext context = new(new OptionString($"{prefix}{prefix}{prefix}key1=value1"), cmd);
            var result = await extractor.ExtractAsync(context);
            Assert.IsNotNull(result.Argument);
            Assert.AreEqual($"key1", result.Argument.Id);
            Assert.AreEqual($"value1", result.Argument.Value);
        }

        [DataTestMethod]
        [DataRow("ü")]
        [DataRow(":")]
        [DataRow("~")]
        [DataRow("#")]
        [DataRow("-")]
        [DataRow("öö")]
        [DataRow("मासे")]
        [DataRow("女性")]
        public async Task MultiplePrefixNotMatchingArgumentIdShouldErrorAsync(string prefix)
        {
            options.Extractor.ArgumentPrefix = prefix;

            CommandDescriptor cmd = new("i1", "n1", "p1", "desc1", new OptionDescriptors(textHandler, new[] { new OptionDescriptor("key1", System.ComponentModel.DataAnnotations.DataType.Text, "desc1") }));
            OptionExtractorContext context = new(new OptionString($"{prefix}{prefix}{prefix}{prefix}key=value1"), cmd);
            await TestHelper.AssertThrowsErrorExceptionAsync(() => extractor.ExtractAsync(context), Errors.UnsupportedArgument, $"The option is not supported. option=key");
        }

        [TestMethod]
        public void NullCommandDescriptorShouldError()
        {
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
#pragma warning disable CA1806 // Do not ignore method results
            TestHelper.AssertThrowsWithMessage<ArgumentException>(() => new OptionExtractorContext(new OptionString("test_arg_string"), null), "Value cannot be null. (Parameter 'commandDescriptor')");
#pragma warning restore CA1806 // Do not ignore method results
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
        }

        [TestMethod]
        public void NullOrWhiteSpaceArgumentStringShouldError()
        {
#pragma warning disable CA1806 // Do not ignore method results
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
            TestHelper.AssertThrowsWithMessage<ArgumentException>(() => new OptionExtractorContext(new OptionString(null), command.Item1), "'raw' cannot be null or whitespace. (Parameter 'raw')");
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.

            TestHelper.AssertThrowsWithMessage<ArgumentException>(() => new OptionExtractorContext(new OptionString("   "), command.Item1), "'raw' cannot be null or whitespace. (Parameter 'raw')");
#pragma warning restore CA1806 // Do not ignore method results
        }

        [DataTestMethod]
        [DataRow(":")]
        [DataRow("+")]
        [DataRow("~")]
        [DataRow("#")]
        [DataRow("-")]
        [DataRow("öö")]
        [DataRow("मासे")]
        [DataRow("女性")]
        public async Task PrefixEmptyButArgumentIdWithPrefixShouldNotErrorAsync(string prefix)
        {
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
            options.Extractor.ArgumentPrefix = "";
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.

            CommandDescriptor cmd = new("i1", "n1", "p1", "desc1", new OptionDescriptors(textHandler, new[] { new OptionDescriptor($"{prefix}key", System.ComponentModel.DataAnnotations.DataType.Text, "desc1") }));
            OptionExtractorContext context = new OptionExtractorContext(new OptionString($"{prefix}key=value"), cmd);
            var result = await extractor.ExtractAsync(context);
            Assert.IsNotNull(result.Argument);
            Assert.AreEqual($"{prefix}key", result.Argument.Id);
            Assert.AreEqual($"value", result.Argument.Value);
        }

        [DataTestMethod]
        [DataRow("ü")]
        [DataRow(":")]
        [DataRow("~")]
        [DataRow("#")]
        [DataRow("-")]
        [DataRow("--")]
        [DataRow("öö")]
        [DataRow("मासे")]
        [DataRow("女性")]
        public async Task PrefixInArgumentIdShouldErrorAsync(string prefix)
        {
            options.Extractor.ArgumentPrefix = prefix;

            // Prefix are not allowed in option id
            CommandDescriptor cmd = new("i1", "n1", "p1", "desc1", new OptionDescriptors(textHandler, new[] { new OptionDescriptor($"{prefix}key", System.ComponentModel.DataAnnotations.DataType.Text, "desc1") }));
            OptionExtractorContext context = new(new OptionString($"{prefix}key1=value1"), cmd);

            await TestHelper.AssertThrowsErrorExceptionAsync(() => extractor.ExtractAsync(context), Errors.UnsupportedArgument, "The option is not supported. option=key1");
        }

        [DataTestMethod]
        [DataRow("ü")]
        [DataRow(":")]
        [DataRow("~")]
        [DataRow("#")]
        [DataRow("-")]
        [DataRow("--")]
        [DataRow("öö")]
        [DataRow("मासे")]
        [DataRow("女性")]
        public async Task PrefixInArgumentValueShouldNotErrorAsync(string prefix)
        {
            options.Extractor.ArgumentPrefix = prefix;

            // Prefix are not allowed in option id
            CommandDescriptor cmd = new("i1", "n1", "p1", "desc1", new OptionDescriptors(textHandler, new[] { new OptionDescriptor($"key1", System.ComponentModel.DataAnnotations.DataType.Text, "desc1") }));
            OptionExtractorContext context = new(new OptionString($"{prefix}key1={prefix}value1{prefix}"), cmd);
            var result = await extractor.ExtractAsync(context);

            Assert.AreEqual("key1", result.Argument.Id);
            Assert.AreEqual($"{prefix}value1{prefix}", result.Argument.Value);
        }

        [TestMethod]
        public async Task UnicodeArgStringDoesNotStartWithSeparatorShouldExtractCorrectly()
        {
            options.Extractor.Separator = "स";
            options.Extractor.ArgumentPrefix = "ई";
            options.Extractor.ArgumentValueSeparator = "र";

            CommandDescriptor cmd = new("i1", "n1", "p1", "desc1", new OptionDescriptors(textHandler, new[] { new OptionDescriptor("पक्षी", System.ComponentModel.DataAnnotations.DataType.Text, "desc1") }));
            OptionExtractorContext context = new(new OptionString($"ईपक्षीरप्राणीप्रेम"), cmd);
            var result = await extractor.ExtractAsync(context);
            Assert.IsNotNull(result.Argument);
            Assert.AreEqual("पक्षी", result.Argument.Id);
            Assert.AreEqual($"प्राणीप्रेम", result.Argument.Value);
        }

        [TestMethod]
        public async Task UnicodeArgStringNoArgSeparatorForBooleanShouldNotError()
        {
            options.Extractor.Separator = "स";
            options.Extractor.ArgumentPrefix = "ई";
            options.Extractor.ArgumentValueSeparator = "र";

            CommandDescriptor cmd = new("i1", "n1", "p1", "desc1", new OptionDescriptors(textHandler, new[] { new OptionDescriptor("पक्षी", nameof(Boolean), "पक्षी वर्णन") }));
            OptionExtractorContext context = new(new OptionString("ईपक्षी"), cmd);
            var result = await extractor.ExtractAsync(context);
            Assert.IsNotNull(result.Argument);
            Assert.AreEqual("पक्षी", result.Argument.Id);
            Assert.AreEqual("True", result.Argument.Value);
        }

        [TestMethod]
        public async Task UnicodeArgStringNoArgSeparatorShouldErrorAsync()
        {
            options.Extractor.Separator = "स";
            options.Extractor.ArgumentPrefix = "ई";
            options.Extractor.ArgumentValueSeparator = "र";

            CommandDescriptor cmd = new("i1", "n1", "p1", "desc1", new OptionDescriptors(textHandler, new[] { new OptionDescriptor("पक्षी", System.ComponentModel.DataAnnotations.DataType.Text, "पक्षी वर्णन") }));
            OptionExtractorContext context = new(new OptionString("ईपक्षी"), cmd);

            await TestHelper.AssertThrowsErrorExceptionAsync(() => extractor.ExtractAsync(context), Errors.InvalidArgument, "The option value is missing. argument_string=ईपक्षीर");
        }

        [TestMethod]
        public async Task UnicodeArgStringNoValueShouldNotErrorAsync()
        {
            options.Extractor.Separator = "स";
            options.Extractor.ArgumentPrefix = "ई";
            options.Extractor.ArgumentValueSeparator = "र";

            CommandDescriptor cmd = new("i1", "n1", "p1", "desc1", new OptionDescriptors(textHandler, new[] { new OptionDescriptor("पक्षी", System.ComponentModel.DataAnnotations.DataType.Text, "पक्षी वर्णन") }));
            OptionExtractorContext context = new(new OptionString("ईपक्षीर"), cmd);
            var result = await extractor.ExtractAsync(context);
            Assert.IsNotNull(result.Argument);
            Assert.AreEqual("पक्षी", result.Argument.Id);
            Assert.AreEqual("", result.Argument.Value);
        }

        [TestMethod]
        public async Task UnicodeArgStringStartsWithArgSeparatorShouldErrorAsync()
        {
            options.Extractor.Separator = "स";
            options.Extractor.ArgumentPrefix = "ई";
            options.Extractor.ArgumentValueSeparator = "र";

            CommandDescriptor cmd = new("i1", "n1", "p1", "desc1", new OptionDescriptors(textHandler, new[] { new OptionDescriptor("पक्षी", System.ComponentModel.DataAnnotations.DataType.Text, "पक्षी वर्णन") }));
            OptionExtractorContext context = new(new OptionString("रईपक्षीप्राणीप्रेम"), cmd);

            await TestHelper.AssertThrowsErrorExceptionAsync(() => extractor.ExtractAsync(context), Errors.InvalidArgument, "The option string is not valid. argument_string=रईपक्षीप्राणीप्रेम");
        }

        [TestMethod]
        public async Task UnicodeArgStringStartsWithSeparatorShouldExtractCorrectly()
        {
            options.Extractor.Separator = "स";
            options.Extractor.ArgumentPrefix = "ई";
            options.Extractor.ArgumentValueSeparator = "र";

            CommandDescriptor cmd = new("i1", "n1", "p1", "desc1", new OptionDescriptors(textHandler, new[] { new OptionDescriptor("पक्षी", System.ComponentModel.DataAnnotations.DataType.Text, "पक्षी वर्णन") }));
            OptionExtractorContext context = new(new OptionString("सईपक्षीरप्राणीप्रेम"), cmd);
            var result = await extractor.ExtractAsync(context);
            Assert.IsNotNull(result.Argument);
            Assert.AreEqual("पक्षी", result.Argument.Id);
            Assert.AreEqual($"प्राणीप्रेम", result.Argument.Value);
        }

        [TestMethod]
        public async Task UnicodeArgStringValueNotWithinShouldNotErrorAsync()
        {
            options.Extractor.Separator = "स";
            options.Extractor.ArgumentPrefix = "ई";
            options.Extractor.ArgumentValueSeparator = "र";
            options.Extractor.ArgumentValueWithIn = "बी";

            CommandDescriptor cmd = new("i1", "n1", "p1", "desc1", new OptionDescriptors(textHandler, new[] { new OptionDescriptor("पक्षी", System.ComponentModel.DataAnnotations.DataType.Text, "पक्षी वर्णन") }));
            OptionExtractorContext context = new(new OptionString("ईपक्षीरप्राणीप्रेमबी"), cmd);
            var result = await extractor.ExtractAsync(context);
            Assert.IsNotNull(result.Argument);
            Assert.AreEqual("पक्षी", result.Argument.Id);
            Assert.AreEqual($"प्राणीप्रेमबी", result.Argument.Value);
        }

        [TestMethod]
        public async Task UnicodeArgStringValueWithinShouldNotErrorAsync()
        {
            options.Extractor.Separator = "स";
            options.Extractor.ArgumentPrefix = "ई";
            options.Extractor.ArgumentValueSeparator = "र";
            options.Extractor.ArgumentValueWithIn = "बी";

            CommandDescriptor cmd = new("i1", "n1", "p1", "desc1", new OptionDescriptors(textHandler, new[] { new OptionDescriptor("पक्षी", System.ComponentModel.DataAnnotations.DataType.Text, "पक्षी वर्णन") }));
            OptionExtractorContext context = new(new OptionString("ईपक्षीरबीप्राणीप्रेमबी"), cmd);
            var result = await extractor.ExtractAsync(context);
            Assert.IsNotNull(result.Argument);
            Assert.AreEqual("पक्षी", result.Argument.Id);
            Assert.AreEqual($"प्राणीप्रेम", result.Argument.Value);
        }

        [TestMethod]
        public async Task UnicodeArgStringWithMultiplePrefixAndSeparatorShouldNotErrorAsync()
        {
            options.Extractor.Separator = "स";
            options.Extractor.ArgumentPrefix = "ई";
            options.Extractor.ArgumentValueSeparator = "र";

            CommandDescriptor cmd = new("i1", "n1", "p1", "desc1", new OptionDescriptors(textHandler, new[] { new OptionDescriptor("पक्षी", System.ComponentModel.DataAnnotations.DataType.Text, "पक्षी वर्णन") }));
            OptionExtractorContext context = new(new OptionString("ससससससईईईईईईईईपक्षीरररररररररप्राणीप्रेमसससससस"), cmd);
            var result = await extractor.ExtractAsync(context);
            Assert.IsNotNull(result.Argument);
            Assert.AreEqual("पक्षी", result.Argument.Id);
            Assert.AreEqual($"प्राणीप्रेम", result.Argument.Value);
        }

        [TestMethod]
        public async Task UnicodePrefixInValueShouldNotErrorAsync()
        {
            options.Extractor.ArgumentPrefix = "माणू";

            CommandDescriptor cmd = new("i1", "n1", "p1", "desc1", new OptionDescriptors(textHandler, new[] { new OptionDescriptor("स", System.ComponentModel.DataAnnotations.DataType.Text, "पक्षी वर्णन") }));
            OptionExtractorContext context = new(new OptionString("माणूमाणूस=माणूसमास"), cmd);
            var result = await extractor.ExtractAsync(context);
            Assert.IsNotNull(result.Argument);
            Assert.AreEqual($"स", result.Argument.Id);
            Assert.AreEqual($"माणूसमास", result.Argument.Value);
        }

        [TestMethod]
        public async Task UsupportedArgumentShouldErrorAsync()
        {
            OptionExtractorContext context = new OptionExtractorContext(new OptionString("-invalid=value"), command.Item1);
            await TestHelper.AssertThrowsErrorExceptionAsync(() => extractor.ExtractAsync(context), Errors.UnsupportedArgument, "The option is not supported. option=invalid");
        }

        [DataTestMethod]
        [DataRow("key1_alias")]
        [DataRow("öö")]
        [DataRow("मासे")]
        [DataRow("女性")]
        public async Task ValidArgAliasOnlyShouldNotErrorAsync(string keyAlias)
        {
            options.Extractor.ArgumentAlias = true;
            options.Extractor.ArgumentPrefix = "-";
            options.Extractor.ArgumentAliasPrefix = "--";

            CommandDescriptor cmd = new("i1", "n1", "p1", "desc1", new OptionDescriptors(textHandler, new[] { new OptionDescriptor("key1", nameof(Boolean), "test desc") { Alias = keyAlias } }));
            OptionExtractorContext context = new(new OptionString($"--{keyAlias}", aliasPrefix: true, 0), cmd);
            var result = await extractor.ExtractAsync(context);
            Assert.IsNotNull(result.Argument);
            Assert.AreEqual("key1", result.Argument.Id);
            Assert.AreEqual(System.ComponentModel.DataAnnotations.DataType.Custom, result.Argument.DataType);
            Assert.AreEqual(nameof(Boolean), result.Argument.CustomDataType);
            Assert.AreEqual("True", result.Argument.Value);
        }

        [DataTestMethod]
        [DataRow("key1")]
        [DataRow("öö")]
        [DataRow("मासे")]
        [DataRow("女性")]
        public async Task ValidArgIdOnlyShouldNotErrorAsync(string key)
        {
            CommandDescriptor cmd = new("i1", "n1", "p1", "desc1", new OptionDescriptors(textHandler, new[] { new OptionDescriptor(key, nameof(Boolean), "test desc") }));
            OptionExtractorContext context = new OptionExtractorContext(new OptionString($"-{key}"), cmd);
            var result = await extractor.ExtractAsync(context);
            Assert.IsNotNull(result.Argument);
            Assert.AreEqual(key, result.Argument.Id);
            Assert.AreEqual(System.ComponentModel.DataAnnotations.DataType.Custom, result.Argument.DataType);
            Assert.AreEqual(nameof(Boolean), result.Argument.CustomDataType);
            Assert.AreEqual("True", result.Argument.Value);
        }

        [TestMethod]
        [DataTestMethod]
        [DataRow("\"")]
        [DataRow("~")]
        [DataRow("#")]
        [DataRow("sp")]
        [DataRow("öö")]
        [DataRow("मासे")]
        [DataRow("女性")]
        public async Task WithInConfiguredButNotUsedShouldExtractCorrectlyAsync(string withIn)
        {
            options.Extractor.ArgumentValueWithIn = withIn;

            OptionExtractorContext context = new(new OptionString($"-key1=test string with {withIn} in between and end but not at start {withIn}"), command.Item1);
            var result = await extractor.ExtractAsync(context);
            Assert.IsNotNull(result.Argument);
            Assert.AreEqual($"key1", result.Argument.Id);
            Assert.AreEqual($"test string with {withIn} in between and end but not at start {withIn}", result.Argument.Value);
        }

        [TestMethod]
        [DataTestMethod]
        [DataRow("\"")]
        [DataRow("~")]
        [DataRow("#")]
        [DataRow("sp")]
        [DataRow("öö")]
        [DataRow("मासे")]
        [DataRow("女性")]
        public async Task WithInConfiguredShouldExtractCorrectlyAsync(string withIn)
        {
            options.Extractor.ArgumentValueWithIn = withIn;

            OptionExtractorContext context = new(new OptionString($"-key1={withIn}test string with {withIn} in between {withIn}"), command.Item1);
            var result = await extractor.ExtractAsync(context);
            Assert.IsNotNull(result.Argument);
            Assert.AreEqual($"key1", result.Argument.Id);
            Assert.AreEqual($"test string with {withIn} in between ", result.Argument.Value);
        }

        [TestMethod]
        [DataTestMethod]
        [DataRow("\"")]
        [DataRow("~")]
        [DataRow("#")]
        [DataRow("sp")]
        [DataRow("öö")]
        [DataRow("मासे")]
        [DataRow("女性")]
        public async Task WithInNotConfiguredShouldExtractCorrectlyAsync(string withIn)
        {
            options.Extractor.ArgumentValueWithIn = null;

            OptionExtractorContext context = new(new OptionString($"-key1={withIn}test string with {withIn} in between {withIn}"), command.Item1);
            var result = await extractor.ExtractAsync(context);
            Assert.IsNotNull(result.Argument);
            Assert.AreEqual($"key1", result.Argument.Id);
            Assert.AreEqual($"{withIn}test string with {withIn} in between {withIn}", result.Argument.Value);
        }

        protected override void OnTestInitialize()
        {
            textHandler = new UnicodeTextHandler();
            command = MockCommands.NewCommandDefinition("id1", "name1", "prefix1", "desc1", MockCommands.TestArgumentDescriptors, null, null);
            options = MockCliOptions.New();
            extractor = new OptionExtractor(textHandler, options, TestLogger.Create<OptionExtractor>());
        }

        private Tuple<CommandDescriptor, Command> command = null!;
        private OptionExtractor extractor = null!;
        private CliOptions options = null!;
        private ITextHandler textHandler = null!;
    }
}
