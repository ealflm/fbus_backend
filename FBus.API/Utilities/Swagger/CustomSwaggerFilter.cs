﻿using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FBus.API.Utilities.Swagger
{
    public class CustomSwaggerFilter : IDocumentFilter
    {
        public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
        {
            if (swaggerDoc.Info.Version.Equals("admin"))
            {
                var nonMobileRoutes = swaggerDoc.Paths
                .Where(x => !x.Key.ToLower().Contains("/admin/"))
                .ToList();
                nonMobileRoutes.ForEach(x => { swaggerDoc.Paths.Remove(x.Key); });
            }
            if (swaggerDoc.Info.Version.Equals("driver"))
            {
                var nonMobileRoutes = swaggerDoc.Paths
                .Where(x => !x.Key.ToLower().Contains("/driver/") || x.Key.ToLower().Contains("/admin/"))
                .ToList();
                nonMobileRoutes.ForEach(x => { swaggerDoc.Paths.Remove(x.Key); });
            }
            if (swaggerDoc.Info.Version.Equals("student"))
            {
                var nonMobileRoutes = swaggerDoc.Paths
                .Where(x => !x.Key.ToLower().Contains("/student/") || x.Key.ToLower().Contains("/admin/"))
                .ToList();
                nonMobileRoutes.ForEach(x => { swaggerDoc.Paths.Remove(x.Key); });
            }
        }
    }
}
