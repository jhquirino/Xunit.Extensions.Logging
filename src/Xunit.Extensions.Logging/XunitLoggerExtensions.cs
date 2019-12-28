// -----------------------------------------------------------------------
//  <copyright file="XunitLoggerExtensions.cs" company="Jorge Alberto Hernández Quirino">
//  Copyright (c) Jorge Alberto Hernández Quirino 2019-2020. All rights reserved.
//  </copyright>
//  <author>Jorge Alberto Hernández Quirino</author>
// -----------------------------------------------------------------------
// ReSharper disable CheckNamespace
using System;
using Microsoft.Extensions.Configuration;
using Xunit.Abstractions;
using Xunit.Extensions.Logging;

namespace Microsoft.Extensions.Logging
{
    /// <summary>
    /// Extension methods for the <see cref="ILoggerFactory"/> class.
    /// </summary>
    public static class XunitLoggerExtensions
    {
        #region ILoggingBuilder extensions

        /// <summary>Adds a xunit logger to the factory.</summary>
        /// <param name="builder">The extension method argument.</param>
        /// <param name="outputHelper">The helper to provide xunit test output.</param>
        public static ILoggingBuilder AddXunit(this ILoggingBuilder builder, ITestOutputHelper outputHelper)
        {
            builder.AddXunit(outputHelper, LogLevel.Information);
            return builder;
        }

        /// <summary>Adds a xunit logger to the factory.</summary>
        /// <param name="builder">The extension method argument.</param>
        /// <param name="outputHelper">The helper to provide xunit test output.</param>
        /// <param name="minLevel">The minimum <see cref="LogLevel"/> to be logged.</param>
        public static ILoggingBuilder AddXunit(this ILoggingBuilder builder, ITestOutputHelper outputHelper, LogLevel minLevel)
        {
            builder.AddXunit(outputHelper, (category, logLevel) => logLevel >= minLevel);
            return builder;
        }

        /// <summary>Adds a xunit logger to the factory.</summary>
        /// <param name="builder">The extension method argument.</param>
        /// <param name="outputHelper">The helper to provide xunit test output.</param>
        /// <param name="filter">The function used to filter events based on the category name and log level.</param>
        public static ILoggingBuilder AddXunit(this ILoggingBuilder builder, ITestOutputHelper outputHelper, Func<string, LogLevel, bool> filter)
        {
            builder.AddProvider(new XunitLoggerProvider(outputHelper, filter));
            return builder;
        }

        /// <summary>Adds a xunit logger to the factory.</summary>
        /// <param name="builder">The extension method argument.</param>
        /// <param name="outputHelper">The helper to provide xunit test output.</param>
        /// <param name="configuration">The <see cref="IConfiguration"/> to use.</param>
        /// <returns></returns>
        public static ILoggingBuilder AddXunit(this ILoggingBuilder builder, ITestOutputHelper outputHelper, IConfiguration configuration)
        {
            builder.AddProvider(new XunitLoggerProvider(outputHelper, configuration));
            return builder;
        }

        #endregion

        #region ILoggerFactory extensions

        /// <summary>
        /// Adds xunit logger that is enabled for <see cref="LogLevel.Information"/> or higher.
        /// </summary>
        /// <param name="factory">The extension method argument.</param>
        /// <param name="outputHelper">The helper to provide xunit test output.</param>
        public static ILoggerFactory AddXunit(this ILoggerFactory factory, ITestOutputHelper outputHelper)
        {
            factory.AddXunit(outputHelper, LogLevel.Information);
            return factory;
        }

        /// <summary>
        /// Adds a xunit logger that is enabled for <see cref="LogLevel"/>s of <paramref name="minLevel"/> or higher.
        /// </summary>
        /// <param name="factory">The extension method argument.</param>
        /// <param name="outputHelper">The helper to provide xunit test output.</param>
        /// <param name="minLevel">The minimum <see cref="LogLevel"/> to be logged.</param>
        public static ILoggerFactory AddXunit(this ILoggerFactory factory, ITestOutputHelper outputHelper, LogLevel minLevel)
        {
            factory.AddXunit(outputHelper, (category, logLevel) => logLevel >= minLevel);
            return factory;
        }

        /// <summary>
        /// Adds a xunit logger that is enabled as defined by the filter function.
        /// </summary>
        /// <param name="factory">The extension method argument.</param>
        /// <param name="outputHelper">The helper to provide xunit test output.</param>
        /// <param name="filter">The function used to filter events based on the category name and log level.</param>
        public static ILoggerFactory AddXunit(this ILoggerFactory factory, ITestOutputHelper outputHelper, Func<string, LogLevel, bool> filter)
        {
            factory.AddProvider(new XunitLoggerProvider(outputHelper, filter));
            return factory;
        }

        /// <summary>
        /// Adds a xunit logger that uses <paramref name="configuration"/> to filter by category name and log level.
        /// </summary>
        /// <param name="factory">The extension method argument.</param>
        /// <param name="outputHelper">The helper to provide xunit test output.</param>
        /// <param name="configuration">The <see cref="IConfiguration"/> to use.</param>
        /// <returns></returns>
        public static ILoggerFactory AddXunit(this ILoggerFactory factory, ITestOutputHelper outputHelper, IConfiguration configuration)
        {
            factory.AddProvider(new XunitLoggerProvider(outputHelper, configuration));
            return factory;
        }

        #endregion
    }
}