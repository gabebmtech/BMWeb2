﻿using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(BMWeb2.Startup))]
namespace BMWeb2
{
    public partial class Startup {
        public void Configuration(IAppBuilder app) {
            ConfigureAuth(app);
        }
    }
}
