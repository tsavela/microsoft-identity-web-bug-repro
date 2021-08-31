using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Identity.Web;
using Xunit;

namespace TestProject1
{
    public class UnitTest1
    {
        [Fact]
        public async Task GetAccessTokenForAppAsyncProblem()
        {
            // Original scenario: Having an implementation type requiring a ITokenAcquisition as dependency,
            //                    and we want to run integration tests for it.

            // Setting up a minimal IServiceProvider that can deliver an ITokenAcquisition instance
            var serviceCollection = new ServiceCollection();
            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string>
                {
                    { "AzureAd:Instance", "https://login.microsoftonline.com/" },
                    { "AzureAd:TenantId", "TODO" },
                    { "AzureAd:ClientId", "TODO" },
                    { "AzureAd:ClientSecret", "TODO" }
                })
                .Build();
            serviceCollection.AddMicrosoftIdentityWebApiAuthentication(configuration)
                .EnableTokenAcquisitionToCallDownstreamApi()
                .AddInMemoryTokenCaches();
            var services = serviceCollection.BuildServiceProvider();

            var tokenAcquisition = services.GetRequiredService<ITokenAcquisition>();

            // Below call throws an exception:
            // Object reference not set to an instance of an object.
            //    at Microsoft.Identity.Web.MergedOptions.PrepareAuthorityInstanceForMsal()
            var token = await tokenAcquisition.GetAccessTokenForAppAsync("api://TODO/.default");

            // Above code works in v1.10.0 of Microsoft.Identity.Web, but not later (haven't tried all of them though...)
            // Something missing in the IServiceProvider setup?
        }
    }
}