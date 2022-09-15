﻿using Microsoft.Extensions.DependencyInjection;
using ToSic.Eav;
using ToSic.Eav.StartUp;
using ToSic.Sxc.Startup;
using ToSic.Testing.Shared;

namespace ToSic.Sxc.Tests
{
    public class TestBaseSxc: TestBaseDiEmpty
    {

        protected override IServiceCollection SetupServices(IServiceCollection services)
        {
            return base.SetupServices(services)
                .AddSxcCore()
                .AddEavCore()
                .AddEavCorePlumbing()
                .AddEavCoreFallbackServices();
        }

    }
}