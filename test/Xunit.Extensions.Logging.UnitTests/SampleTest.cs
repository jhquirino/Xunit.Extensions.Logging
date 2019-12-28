// -----------------------------------------------------------------------
//  <copyright file="SampleTest.cs" company="Jorge Alberto Hernández Quirino">
//  Copyright (c) Jorge Alberto Hernández Quirino 2019-2020. All rights reserved.
//  </copyright>
//  <author>Jorge Alberto Hernández Quirino</author>
// -----------------------------------------------------------------------
using Microsoft.Extensions.Logging;
using Xunit.Abstractions;

namespace Xunit.Extensions.Logging.UnitTests
{
    public class SampleTest
    {
        private readonly ILogger logger;

        public SampleTest(ITestOutputHelper outputHelper)
        {
            logger = LoggerFactory
                        .Create(builder =>
                        {
                            builder
                                .AddXunit(outputHelper);
                                // Add other loggers, e.g.: AddConsole, AddDebug, etc.
                        })
                        .CreateLogger<SampleTest>();
        }

        [Fact]
        public void DoSomeTest()
        {
            // Arrange
            // Act
            // Assert
            logger.LogInformation("Hello world!");
        }
    }
}
