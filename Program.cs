using System;
using System.Collections.Generic;
using CommandLine;
using CoreHambusCommonLibrary.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

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

      [Option('n', "name", Required = true, HelpText = "Name of the instance.")]
      public string Name { get; set; }

      //[Option('c', "commport", Required = false, HelpText = "Comm Port to connect to.")]
      //public string CommPort { get; set; }

      //[Option('P', "parity", Required = false, HelpText = "Comm Port parity: (odd, even, none, mark).")]
      //public string Parity { get; set; }

      //[Option('P', "parity", Required = false, HelpText = "Comm Port parity: (odd, even, none, mark).")]
      //public string Parity { get; set; }
    }

    public static void Main(string[] args)
    {
      try
      {

        CommandLine.Parser.Default.ParseArguments<Options>(args)
          .WithParsed(RunOptions)
          .WithNotParsed(HandleParseError);
        BuildWebHost(args).Build().Run();
      }
      catch (Exception e)
      {
        Console.WriteLine(e.Message);
      }
    }

    public static IHostBuilder BuildWebHost(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(webBuilder =>
            {
              webBuilder.UseStartup<Startup>();
              webBuilder.UseUrls("http://*:7300");

            });
    static void RunOptions(Options opts)
    {


      var gConfig = GlobalDataServiceSqlite.Instance;
      if (string.IsNullOrWhiteSpace(opts.Name) == false)
        gConfig.Name = opts.Name;

      if (string.IsNullOrWhiteSpace(opts.Host))
        gConfig.Host = "*";

      gConfig.Name = opts.Name;
      if (opts.Port != null)
        gConfig.Port = opts.Port;
      gConfig.InitDB();

    }
    static void HandleParseError(IEnumerable<Error> errs)
    {
      throw new Exception("Invalid Args");
    }
  }
}
