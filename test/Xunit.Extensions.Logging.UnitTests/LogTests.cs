// -----------------------------------------------------------------------
//  <copyright file="LogTests.cs" company="Jorge Alberto Hernández Quirino">
//  Copyright (c) Jorge Alberto Hernández Quirino 2019-2020. All rights reserved.
//  </copyright>
//  <author>Jorge Alberto Hernández Quirino</author>
// -----------------------------------------------------------------------
using System;
using Microsoft.Extensions.Logging;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace Xunit.Extensions.Logging.UnitTests
{
    [Trait("Xunit.Extensions.Logging", "UnitTests")]
    public class LogTests
    {
        private readonly TestOutputHelper output;

        private readonly ILoggerFactory loggerFactory;

        public LogTests(ITestOutputHelper output)
        {
            this.output = output as TestOutputHelper;
            loggerFactory = LoggerFactory.Create(builder =>
            {
                builder
                    .AddDebug()
                    .AddXunit(this.output);
            });
        }

        [Fact]
        public void When_log_message_Then_message_is_in_output()
        {
            const string message = "Hello world!";
            var logger = loggerFactory.CreateLogger<LogTests>();
            Assert.Empty(output.Output);
            logger.LogInformation(message);
            Assert.Contains(message, output.Output);
        }

        [Fact]
        public void When_log_exception_Then_exception_is_in_output()
        {
            const string message = "An exception has occurred.";
            const string exceptionMessage = "This is the exception!";
            var logger = loggerFactory.CreateLogger<LogTests>();
            var exception = new InvalidOperationException(exceptionMessage);
            Assert.Empty(output.Output);
            logger.LogError(exception, message);
            Assert.Contains(message, output.Output);
            Assert.Contains(exception.GetType().FullName, output.Output);
            Assert.Contains(exceptionMessage, output.Output);
        }

        [Fact]
        public void When_log_formatted_message_Then_message_is_in_output()
        {
            const string message = "The value is:";
            var value = DateTime.UtcNow.ToFileTime();
            var logger = loggerFactory.CreateLogger<LogTests>();
            Assert.Empty(output.Output);
            logger.LogInformation("{message} {value}", message, value);
            Assert.Contains(message, output.Output);
            Assert.Contains(value.ToString(), output.Output);
        }
    }
}
