var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.Chat>("chat");

builder.Build().Run();
