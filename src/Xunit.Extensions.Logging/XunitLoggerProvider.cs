// -----------------------------------------------------------------------
//  <copyright file="XunitLoggerProvider.cs" company="Jorge Alberto Hernández Quirino">
//  Copyright (c) Jorge Alberto Hernández Quirino 2019-2020. All rights reserved.
//  </copyright>
//  <author>Jorge Alberto Hernández Quirino</author>
// -----------------------------------------------------------------------
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Xunit.Abstractions;

namespace Xunit.Extensions.Logging
{
    /// <summary>
    /// The provider for the <see cref="XunitLogger"/>.
    /// </summary>
    public class XunitLoggerProvider : ILoggerProvider
    {
        #region Inner members

        /// <summary>
        /// The helper to provide xunit test output.
        /// </summary>
        private readonly ITestOutputHelper outputHelper;

        /// <summary>
        /// The registered loggers.
        /// </summary>
        private readonly ConcurrentDictionary<string, XunitLogger> loggers = new ConcurrentDictionary<string, XunitLogger>();

        /// <summary>
        /// The function used to filter events based on the category name and log level.
        /// </summary>
        private readonly Func<string, LogLevel, bool> filter;

        /// <summary>
        /// The configuration used to filter events based on the category name and log level.
        /// </summary>
        private readonly IConfiguration configuration;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="XunitLoggerProvider"/> class.
        /// </summary>
        /// <param name="outputHelper">The helper to provide xunit test output.</param>
        /// <param name="filter">The function used to filter events based on the category name and log level.</param>
        public XunitLoggerProvider(ITestOutputHelper outputHelper, Func<string, LogLevel, bool> filter)
        {
            this.outputHelper = outputHelper ?? throw new ArgumentNullException(nameof(outputHelper));
            this.filter = filter;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="XunitLoggerProvider"/> class.
        /// </summary>
        /// <param name="outputHelper">The helper to provide xunit test output.</param>
        /// <param name="configuration">The configuration used to filter events based on the category name and log level.</param>
        public XunitLoggerProvider(ITestOutputHelper outputHelper, IConfiguration configuration)
        {
            this.outputHelper = outputHelper ?? throw new ArgumentNullException(nameof(outputHelper));
            this.configuration = configuration;
        }

        #endregion

        #region ILoggerProvider/IDisposable implementation

        /// <summary>
        /// Creates a new <see cref="XunitLogger"/> instance.
        /// </summary>
        /// <param name="categoryName">The category name for messages produced by the logger.</param>
        /// <returns>An instance of <see cref="XunitLogger"/>.</returns>
        public ILogger CreateLogger(string categoryName)
        {
            return loggers.GetOrAdd(categoryName, CreateLoggerImplementation);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            loggers.Clear();
        }

        #endregion

        #region Auxiliar methods

        /// <summary>
        /// implementation for logger instance creation.
        /// </summary>
        /// <param name="categoryName">The category name for messages produced by the logger.</param>
        /// <returns>An instance of <see cref="XunitLogger"/>.</returns>
        private XunitLogger CreateLoggerImplementation(string categoryName)
        {
            return new XunitLogger(outputHelper, categoryName, GetFilter(categoryName));
        }

        /// <summary>
        /// Gets the filter applicable to category.
        /// </summary>
        /// <param name="categoryName">The category name for messages produced by the logger.</param>
        /// <returns>function used to filter events based on the category name and log level.</returns>
        private Func<string, LogLevel, bool> GetFilter(string categoryName)
        {
            if (filter != null)
                return filter;

            if (configuration == null)
                return (n, l) => false;

            foreach (var prefix in GetKeyPrefixes(categoryName))
            {
                if (TryGetSwitch(prefix, out var level))
                {
                    return (n, l) => l >= level;
                }
            }

            return (n, l) => false;
        }

        /// <summary>
        /// Gets the key prefixes.
        /// </summary>
        /// <param name="categoryName">The category name for messages produced by the logger.</param>
        /// <returns>The key prefixes for category name.</returns>
        private IEnumerable<string> GetKeyPrefixes(string categoryName)
        {
            while (!string.IsNullOrEmpty(categoryName))
            {
                yield return categoryName;
                var lastIndexOfDot = categoryName.LastIndexOf('.');
                if (lastIndexOfDot == -1)
                {
                    yield return "Default";
                    break;
                }
                categoryName = categoryName.Substring(0, lastIndexOfDot);
            }
        }

        /// <summary>
        /// Tries the get switch (log level) to filter by.
        /// </summary>
        /// <param name="categoryName">Name of the category.</param>
        /// <param name="level">The log level to filter by.</param>
        /// <returns><see langword="true"/> if a log level is applicable to category; otherwise, <see langword="false"/>.</returns>
        /// <exception cref="InvalidOperationException">If the log level string cannot be parsed to a valid <see cref="Microsoft.Extensions.Logging.LogLevel"/> value.</exception>
        private bool TryGetSwitch(string categoryName, out LogLevel level)
        {
            var isSuccessful = false;
            if (configuration != null)
            {
                var switches = configuration.GetSection("LogLevel");
                if (switches == null)
                {
                    level = LogLevel.None;
                }
                else
                {
                    var value = switches[categoryName];
                    if (string.IsNullOrEmpty(value?.Trim()))
                    {
                        level = LogLevel.None;
                    }
                    else if (Enum.TryParse(value, true, out level))
                    {
                        isSuccessful = true;
                    }
                    else
                    {
                        var message = $"Configuration value '{value}' for category '{categoryName}' is not supported.";
                        throw new InvalidOperationException(message);
                    }
                }
            }
            else
            {
                level = LogLevel.None;
            }
            return isSuccessful;
        }

        #endregion
    }
}