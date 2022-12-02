using System;
using System.IO;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System.Web.Http.Cors;
using Microsoft.AspNetCore.Cors;

namespace DataServerApi
{
    public class Program
    {
        //const string IP = "http://10.136.227.168:4244"; //UF
        const string IP = "http://192.168.5.231:4244"; //Server
        //const string IP = "http://192.168.1.5:4244"; //Home

        public static void Main(string[] args)
        {
            WebHost.CreateDefaultBuilder(args)
              .UseUrls(IP)
              .UseWebRoot("public")
              .UseStartup<Startup>()
              .Build()
              .Run();
        }
    }
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().AddNewtonsoftJson();

            services.AddCors(options =>
            {
                options.AddDefaultPolicy(
                    policy =>
                    {
                        policy.AllowAnyOrigin();
                        policy.AllowAnyMethod();
                        policy.AllowAnyHeader();
                    });
            });



        }
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {

            app.UseCors();

            if (env.IsDevelopment()) app.UseDeveloperExceptionPage();
            app.UseRouting();
            app.UseEndpoints(endpoints => endpoints.MapControllers());
        }
    }
    [System.Web.Http.Cors.EnableCors("*", "*", "*")]
    [Route("api")]
    [ApiController]
    public class ApiController : Controller
    {
        [HttpPost]
        public async Task<IActionResult> Index()
        {
            Console.WriteLine("recieved");
            //Get http request
            var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();

            //Debug
            Console.WriteLine("\nJson raw" + json); //{"firstParam":"yourValue","secondParam":"yourOtherValue"}

            //In case of error or incorrect json format
            try
            {
                //Parse JSON
                Dictionary<string, string> jsonMap = new Dictionary<string, string>();
                Utils.parseJson(ref json, ref jsonMap, true);

                //Verify appID existence
                if (!jsonMap.ContainsKey("appID"))
                {
                    Console.WriteLine("\nDid not contain AppID");
                    return BadRequest();
                }
                string app = jsonMap.GetValueOrDefault("appID");
                if (app == "zoomSniperWeb")
                {
                    //Create object with json
                    ZoomWeb webObject = new ZoomWeb(ref jsonMap);
                    if (webObject.valid)
                    {
                        webObject.insert();
                    }
                }
                else if (app == "dominikSiteData")
                {
                    Console.WriteLine("new");

                    //Create object with json
                    DominikSite siteObject = new DominikSite(ref jsonMap);
                    if (siteObject.valid)
                    {
                        siteObject.insert();
                    }
                }
                else if (app == "mapEvent")
                {

                }
                else
                {
                    Console.WriteLine("\nInvalid AppID");
                    return BadRequest();
                }

                return Ok("test");
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: {0}", e.Message);
                return BadRequest();
            }
        }


    }

}