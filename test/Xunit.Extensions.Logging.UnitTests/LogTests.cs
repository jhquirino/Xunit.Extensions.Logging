// -----------------------------------------------------------------------
//  <copyright file="LogTests.cs" company="Jorge Alberto Hernández Quirino">
//  Copyright (c) Jorge Alberto Hernández Quirino 2019-2020. All rights reserved.
//  </copyright>
//  <author>Jorge Alberto Hernández Quirino</author>
// -----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
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

        [Fact]
        public void When_create_using_null_output_Then_throw_exception()
        {
            Assert.Throws<ArgumentNullException>(() =>
                LoggerFactory.Create(builder => builder.AddXunit(null))
            );
        }

        [Fact]
        public void When_create_using_configuration_Then_logger_is_initialized()
        {
            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(
                    new Dictionary<string, string>
                    {
                        ["Logging:LogLevel:Default"] = "Trace"
                    }
                )
                .Build();
            var loggerByConfig = LoggerFactory.Create(builder =>
                builder.AddXunit(output, configuration)
            )
            .CreateLogger<LogTests>();
            Assert.NotNull(loggerByConfig);
        }

        [Fact]
        public void When_create_using_configuration_and_null_output_Then_throw_exception()
        {
            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(
                    new Dictionary<string, string>
                    {
                        ["Logging:LogLevel:Default"] = "Trace"
                    }
                )
                .Build();
            Assert.Throws<ArgumentNullException>(() =>
                LoggerFactory.Create(builder => builder.AddXunit(null, configuration))
            );
        }

        [Fact]
        public void When_init_from_factory_Then_logger_is_initialized()
        {
            var logger = new LoggerFactory().AddXunit(output).CreateLogger<LogTests>();
            Assert.NotNull(logger);
        }

        [Fact]
        public void When_init_from_factory_and_config_Then_logger_is_initialized()
        {
            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(
                    new Dictionary<string, string>
                    {
                        ["Logging:LogLevel:Default"] = "Trace"
                    }
                )
                .Build();
            var logger = new LoggerFactory().AddXunit(output, configuration).CreateLogger<LogTests>();
            Assert.NotNull(logger);
        }

        [Fact]
        public void When_init_from_service_provider_Then_logger_is_initialized()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddLogging(c => c.AddXunit(output));
            using (var serviceProvider = serviceCollection.BuildServiceProvider())
            {
                var logger = serviceProvider.GetService<ILogger<LogTests>>();
                Assert.NotNull(logger);
            }
        }

        [Fact]
        public void When_init_from_host_builder_Then_logger_is_initialized()
        {
            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(
                    new Dictionary<string, string>
                    {
                        ["Logging:LogLevel:Default"] = "Trace"
                    }
                )
                .Build();
            using (var host = Host.CreateDefaultBuilder().ConfigureServices((context, services) => {
                    services.AddSingleton<ITestOutputHelper>(output);
                    services.AddSingleton<IConfiguration>(configuration);
                    services.AddSingleton<ILoggerProvider, XunitLoggerProvider>();
                })
                .Build()
            ) {
                var logger = host.Services.GetService<ILogger<LogTests>>();
                Assert.NotNull(logger);
            }
        }

        [Fact]
        public void When_create_using_null_configuration_Then_empty_output()
        {
            IConfiguration configuration = null;
            var logger = LoggerFactory.Create(builder =>
                builder.AddXunit(output, configuration)
            )
            .CreateLogger<LogTests>();
            const string message = "Null configuration";
            Assert.NotNull(logger);
            Assert.Empty(output.Output);
            logger.LogInformation(message);
            Assert.DoesNotContain(message, output.Output);
        }
    }
}
