//-----------------------------------------------------------------------
//  <copyright file="XunitLogger.cs" company="Jorge Alberto Hernández Quirino">
//  Copyright (c) Jorge Alberto Hernández Quirino 2019-2020. All rights reserved.
//  </copyright>
//  <author>Jorge Alberto Hernández Quirino</author>
// -----------------------------------------------------------------------
using System;
using Microsoft.Extensions.Logging;
using Xunit.Abstractions;

namespace Xunit.Extensions.Logging
{
    /// <summary>
    /// A logger that writes messages to xunit (<see cref="ITestOutputHelper"/>).
    /// </summary>
    public sealed class XunitLogger : ILogger
    {
        #region Inner members

        /// <summary>
        /// The helper to provide xunit test output.
        /// </summary>
        private readonly ITestOutputHelper outputHelper;

        /// <summary>
        /// The function used to filter events based on the log level.
        /// </summary>
        private readonly Func<string, LogLevel, bool> filter;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the (category) name of the logger.
        /// </summary>
        public string Name { get; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="XunitLogger"/> class.
        /// </summary>
        /// <param name="outputHelper">The helper to provide xunit test output.</param>
        /// <param name="name">The category name for the logger.</param>
        /// <param name="filter">The function used to filter events based on the log level.</param>
        public XunitLogger(ITestOutputHelper outputHelper, string name, Func<string, LogLevel, bool> filter)
        {
            this.outputHelper = outputHelper ?? throw new ArgumentNullException(nameof(outputHelper));
            Name = string.IsNullOrWhiteSpace(name) ? nameof(XunitLogger) : name.Trim();
            this.filter = filter ?? ((categoryName, logLevel) => true);
        }

        #endregion

        #region ILogger implementation

        /// <summary>
        /// Writes a log entry.
        /// </summary>
        /// <typeparam name="TState"></typeparam>
        /// <param name="logLevel">Entry will be written on this level.</param>
        /// <param name="eventId">Id of the event.</param>
        /// <param name="state">The entry to be written. Can be also an object.</param>
        /// <param name="exception">The exception related to this entry.</param>
        /// <param name="formatter">Function to create a <c>string</c> message of the <paramref name="state" /> and <paramref name="exception" />.</param>
        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (!IsEnabled(logLevel))
                return;
            outputHelper.WriteLine($"{DateTime.Now:O} - {logLevel:G} - {eventId.Id} - {Name} - {formatter(state, null)}");
            if (exception != null)
                outputHelper.WriteLine($"{exception}");
        }

        /// <summary>
        /// Checks if the given <paramref name="logLevel" /> is enabled.
        /// </summary>
        /// <param name="logLevel">Level to be checked.</param>
        /// <returns><see langword="true"/> if enabled; otherwise, <see langword="false"/>.</returns>
        public bool IsEnabled(LogLevel logLevel)
        {
            return logLevel != LogLevel.None && filter(Name, logLevel);
        }

        /// <summary>
        /// Begins a logical operation scope.
        /// </summary>
        /// <typeparam name="TState"></typeparam>
        /// <param name="state">The identifier for the scope.</param>
        /// <returns>
        /// An IDisposable that ends the logical operation scope on dispose.
        /// </returns>
        public IDisposable BeginScope<TState>(TState state)
        {
            return null;
        }

        #endregion
    }
}
