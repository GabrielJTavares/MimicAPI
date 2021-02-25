using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MimicWebApi.DataBase;
using MimicWebApi.V1.Repositories;
using MimicWebApi.V1.Repositories.Contracts;
using AutoMapper;
using MimicWebApi.Helpers;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.OpenApi.Models;
using MimicWebApi.Helpers.Swagger;
using Microsoft.Extensions.PlatformAbstractions;
using System.IO;

namespace MimicWebApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Auto Mapper
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new DTOMapperProfile());
            });
            IMapper mapper = config.CreateMapper();
            services.AddSingleton(mapper);

            //---------------------------


            services.AddControllers();
            services.AddDbContext<ApiContext>(opt =>
            {
                opt.UseSqlite("data source=DataBase\\ApiMimic.db");
            });
            services.AddScoped<IWordRepository, WordRepository>();
            services.AddApiVersioning(cfg =>
            {
                cfg.ReportApiVersions = true;
                // cfg.ApiVersionReader = new HeaderApiVersionReader("");
                cfg.AssumeDefaultVersionWhenUnspecified = true;
                cfg.DefaultApiVersion = new ApiVersion(1, 0);
            });

            // Config Swagger
            services.AddSwaggerGen(cfg => {
                cfg.ResolveConflictingActions(apiDescription => apiDescription.First());
                cfg.SwaggerDoc("v2.0", new OpenApiInfo
                {
                    Title = "MimicApi - v2.0",
                    Version = "v2.0"

                });
                cfg.SwaggerDoc("v1.0", new OpenApiInfo
                {
                    Title="MimicApi - v1.0",Version="v1.0"
                    
                });
                var caminhoProjeto = PlatformServices.Default.Application.ApplicationBasePath;
                var NomeProjeto = $"{PlatformServices.Default.Application.ApplicationName}.xml";
                var arquivoXml = Path.Combine(caminhoProjeto, NomeProjeto);
                cfg.IncludeXmlComments(arquivoXml);


                cfg.DocInclusionPredicate((docName, apiDesc) =>
                {
                    var actionApiVersionModel = apiDesc.ActionDescriptor?.GetApiVersion();
                    // would mean this action is unversioned and should be included everywhere
                    if (actionApiVersionModel == null)
                    {
                        return true;
                    }
                    if (actionApiVersionModel.DeclaredApiVersions.Any())
                    {
                        return actionApiVersionModel.DeclaredApiVersions.Any(v => $"v{v.ToString()}" == docName);
                    }
                    return actionApiVersionModel.ImplementedApiVersions.Any(v => $"v{v.ToString()}" == docName);
                });

                cfg.OperationFilter<ApiVersionOperationFilter>();


            });
            
                
           
       
        }


    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        app.UseHttpsRedirection();

        app.UseRouting();

        app.UseAuthorization();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });
        app.UseStatusCodePages();

            // Config Swagger
            app.UseSwagger();
            app.UseSwaggerUI(cfg =>
            {
                
                cfg.SwaggerEndpoint("/swagger/v1.0/swagger.json", "v1.0");
                cfg.SwaggerEndpoint("/swagger/v2.0/swagger.json", "v2.0");
                cfg.RoutePrefix = string.Empty;
            });
    }
}

}