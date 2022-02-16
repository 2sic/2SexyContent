﻿using Microsoft.Extensions.DependencyInjection;
using ToSic.Testing.Shared;

namespace ToSic.Sxc.Tests
{
    public class TestBaseSxcDb: TestBaseDiEavFullAndDb
    {

        protected override IServiceCollection SetupServices(IServiceCollection services = null)
        {
            return base.SetupServices(services)
                .AddSxcCore();
        }

    }
}
