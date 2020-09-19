using System;
using System.Collections.Generic;
using CommandLine;
using CoreHambusCommonLibrary.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace BusMaster
{

  public class Program
  {
    class Options
    {
      [Option('p', "port", Required = false, HelpText = "Port that Master Bus will listen")]
      public int? Port { get; set; }

      [Option('h', "host", Required = false, HelpText = "Name of host that Master Bus will listen")]
      public string? Host { get; set; }

      //[Option('n', "name", Required = true, HelpText = "Name of the instance.")]
      //public string? Name { get; set; } = "";

      //[Option('c', "commport", Required = false, HelpText = "Comm Port to connect to.")]
      //public string CommPort { get; set; }

      //[Option('P', "parity", Required = false, HelpText = "Comm Port parity: (odd, even, none, mark).")]
      //public string Parity { get; set; }

      //[Option('P', "parity", Required = false, HelpText = "Comm Port parity: (odd, even, none, mark).")]
      //public string Parity { get; set; }
    }
    static GlobalDataServiceSqlite? conf;
    public static void Main(string[] args)
    {
      //Read Configuration from appSettings
      var config = new ConfigurationBuilder()
          .AddJsonFile("appsettings.json")
          .Build();

      Log.Logger = new LoggerConfiguration()
          .ReadFrom.Configuration(config)
          .CreateLogger();
      conf = GlobalDataServiceSqlite.Instance;
      try
      {

        CommandLine.Parser.Default.ParseArguments<Options>(args)
          .WithParsed(RunOptions)
          .WithNotParsed(HandleParseError);


        BuildWebHost(args).Build().Run();
      }
      catch (Exception e)
      {
        Log.Error("Program: Main threw exception: {@e}", e);
      }
    }

    public static IHostBuilder BuildWebHost(string[] args) =>

        Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(webBuilder =>
            {
              webBuilder.UseStartup<Startup>();
              var conf = GlobalDataServiceSqlite.Instance;
              var url = $"http://*:{conf.Host}:{conf.Port}";
              _ = webBuilder.UseUrls($"http://*:{conf.Host}:{conf.Port}");
            }).UseSerilog();
    static void RunOptions(Options opts)
    {

      conf!.Port = 7300;

      if (string.IsNullOrWhiteSpace(opts.Host))
        conf.Host = "*";

      if (opts.Port != null)
        conf.Port = opts.Port;
 
      conf.InitDB();

    }
    static void HandleParseError(IEnumerable<Error> errs)
    {
      throw new Exception("Invalid Args");
    }
  }
}
