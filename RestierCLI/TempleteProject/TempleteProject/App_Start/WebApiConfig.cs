using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using TempleteProject.Models;
using Microsoft.Restier.Providers.EntityFramework;
using Microsoft.Restier.Publishers.OData;
using Microsoft.Restier.Publishers.OData.Batch;
using Microsoft.Restier.Publishers.OData.Routing;
using Microsoft.OData.Core;
using Microsoft.OData.Edm;
using Microsoft.OData.Core.UriBuilder;
using Microsoft.OData.Core.UriParser;
using Microsoft.OData.Core.Atom;
using System.Web.OData.Extensions;

namespace TempleteProject
{
    public static class WebApiConfig
    {
        public async static void Register(HttpConfiguration config)
        {
            // Web API configuration and services
            config.SetUrlConventions(ODataUrlConventions.ODataSimplified);
            await config.MapRestierRoute<EntityFrameworkApi<TempleteProject>>(
                "TempleteProject",
                "api/TempleteProject",
                new RestierBatchHandler(GlobalConfiguration.DefaultServer));

        }
    }
}
