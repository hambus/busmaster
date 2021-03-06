using BusMaster.Hubs;
using CoreHambusCommonLibrary.Services;
using HamBusCommonStd;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace BusMaster
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
      services.AddSingleton(GlobalDataServiceSqlite.Instance);
      services.AddCors(o => o.AddPolicy("CorsPolicy", builder =>
      {
        builder
        .AllowAnyMethod()
        .AllowAnyHeader()
        .AllowAnyOrigin();
      }));

      services.AddSignalR();
      services.AddSingleton<IGlobalDataService, GlobalDataServiceSqlite>();
      services.AddSingleton<ActiveBusesService>();
      services.AddSingleton<BusConfigurationDB>();
      //services.AddSingleton<IAuth, DatabaseAuth>();
      //services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_3_0);
      services.AddMvc();


    }


    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
      if (env.IsDevelopment())
      {
        app.UseDeveloperExceptionPage();
      }

      app.UseRouting();
      app.UseDefaultFiles();
      app.UseStaticFiles();
      app.UseAuthorization();

      app.UseCors("CorsPolicy");
      app.UseEndpoints(endpoints =>
      {
        endpoints.MapHub<MasterHub>("/masterbus");
        endpoints.MapDefaultControllerRoute();
      });
    }
  }
}
