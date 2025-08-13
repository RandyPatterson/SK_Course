var builder = DistributedApplication.CreateBuilder(args);

// Add the chat application
builder.AddProject<Projects.Chat>("chat");


builder.Build().Run();
