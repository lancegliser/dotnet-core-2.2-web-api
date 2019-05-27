using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Service.UnitTests.Utilities
{
    public class TestingUtility
    {

        // Summary:
        //     Creates a new ILogger instance using the full name of the given type.
        //
        // Parameters:
        //   factory:
        //     The factory.
        //
        // Type parameters:
        //   T:
        //     The type.
        public static ILogger<T> GetNullLogger<T>()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddSingleton<ILoggerFactory, NullLoggerFactory>();
            var serviceProvider = serviceCollection.BuildServiceProvider();
            var factory = serviceProvider.GetService<ILoggerFactory>();
            return factory.CreateLogger<T>();
        }

    }
}
