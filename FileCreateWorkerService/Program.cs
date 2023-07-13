using FileCreateWorkerService;
using FileCreateWorkerService.Models;
using Microsoft.EntityFrameworkCore;
using RabbitMQ.Client;
using RabbitMQ.ExcelCreate.Services;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext,services) =>
    {
        IConfiguration Configuration = hostContext.Configuration;
        services.AddDbContext<AdventureWorks2022Context>(options =>
        {
            options.UseSqlServer(Configuration.GetConnectionString("SqlServer"));
        });
        services.AddSingleton<RabbitMQClientService>();
        services.AddSingleton(sp => new ConnectionFactory()
        {
            Uri = new Uri(Configuration.GetConnectionString("RabbitMQ")),
            DispatchConsumersAsync = true

        });
        services.AddHostedService<Worker>();
    })
    .Build();

host.Run();
