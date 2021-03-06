﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Formatting;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace MapCore
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddMvc();
            services.Configure<MvcOptions>(options =>
            {
                options.OutputFormatters.RemoveType<JsonOutputFormatter>();
                options.OutputFormatters.Add(new JsonpOutputFormatter());
                var formatter = options.OutputFormatters.First(f => f is JsonOutputFormatter) as JsonOutputFormatter;

                if (formatter == null) return;

                formatter.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                formatter.SerializerSettings.DefaultValueHandling = Newtonsoft.Json.DefaultValueHandling.Populate;
                formatter.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            app.UseMvc();
        }

        
    }

    public class JsonpOutputFormatter : JsonOutputFormatter
    {
        public override async Task WriteResponseBodyAsync(OutputFormatterWriteContext context, Encoding selectedEncoding)
        {
            var httpQuery = context.HttpContext.Request.Query;
            StringValues callback;
            if (httpQuery.TryGetValue("callback", out callback) && callback.Count == 1)
            {
                if (selectedEncoding == null)
                    throw new ArgumentNullException("selectedEncoding");
                string callbackFunc = callback[0];
                TextWriter writer = context.WriterFactory(context.HttpContext.Response.Body, selectedEncoding);
                try
                {
                    writer.Write(callbackFunc + "(");
                    WriteObject(writer, context.Object);
                    writer.Write(")");
                    await writer.FlushAsync();
                }
                finally
                {
                    writer?.Dispose();
                }
            }
            else
            {
                await base.WriteResponseBodyAsync(context, selectedEncoding);
            }
            
        }
    }

    
}
