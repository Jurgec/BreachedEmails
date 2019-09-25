using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Configuration;
/*using System;
using System.Web;
using System.Net.Http;
using System;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Filters;*/


namespace BreachedEmails
{
    public class Startup
    {
        private IClusterClient CreateOrleansClient()
        {
//            try
 //           {
                var clientBuilder = new ClientBuilder()
                .UseLocalhostClustering()
                .Configure<ClusterOptions>(options =>
                {
                    options.ClusterId = "dev";
                    options.ServiceId = "BreachedEmails";
                })
                .ConfigureLogging(logging => logging.AddConsole());

                var client = clientBuilder.Build();
                client.Connect().Wait(); //TU PROBAJ TRY CACTCH

                return client;
//           }
//            catch 
//            {
//                throw new HttpResponseException();
//                return  HttpResponseMessage(System.Net.HttpStatusCode.InternalServerError);
             //   throw new HttpRequestException();// Exception("Silo is not running!");
                //return
                //throw new NotImplementedException("Silo is not running!");
 //           }
        }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var orleansClient = CreateOrleansClient();
            services.AddSingleton<IClusterClient>(orleansClient);

            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            //app.UseBotwin();
            app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}
