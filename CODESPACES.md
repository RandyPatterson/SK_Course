# Semantic Kernel Course - GitHub Codespaces

This repository is configured to run in GitHub Codespaces with all necessary tools and dependencies pre-installed.

## 🚀 Quick Start

1. **Open in Codespaces**: Click the green "Code" button and select "Create codespace on main"
2. **Wait for setup**: The environment will automatically install .NET 8.0, .NET 9.0, and all required packages
3. **Configure API Keys**: Set up your OpenAI or Azure OpenAI credentials (see below)
4. **Start coding**: All modules are ready to run!

## 🔑 API Configuration

You'll need to configure API keys for the AI services. Use .NET user secrets to store them securely:

### For Azure OpenAI:
```bash
# Replace with your actual values
dotnet user-secrets set "AzureOpenAI:Endpoint" "https://your-resource.openai.azure.com/" --project module2/SK_Console
dotnet user-secrets set "AzureOpenAI:ApiKey" "your-api-key" --project module2/SK_Console
dotnet user-secrets set "AzureOpenAI:DeploymentName" "your-deployment-name" --project module2/SK_Console
```

### For OpenAI:
```bash
dotnet user-secrets set "OpenAI:ApiKey" "your-openai-api-key" --project module2/SK_Console
```

## 📁 Module Overview

- **Module 2**: Foundation - Basic Semantic Kernel concepts
- **Module 3**: Model Providers - OpenAI, Azure AI, Ollama, Hugging Face, etc.
- **Module 4**: Multimodal AI - Working with images and text
- **Module 5**: Web Applications - ASP.NET Core with Semantic Kernel
- **Module 7**: Advanced Features - Plugins and complex scenarios
- **Module 8**: Production Applications - Real-world implementations

## 🏃‍♂️ Running the Projects

### Console Applications (Module 2, 3, 4):
```bash
cd module2/SK_Console
dotnet run
```

### Web Applications with .NET Aspire (Module 5, 7, 8):
```bash
cd module5
dotnet run --project SKDevChat.AppHost
```

The Aspire dashboard will be available at the forwarded port (usually port 15000).

## 🌐 Forwarded Ports

The Codespace automatically forwards these ports:
- **5000/5001**: Standard .NET development server
- **7000/7001**: Chat application
- **8080/8081**: Customer API  
- **15000/15001**: .NET Aspire Dashboard
- **18888**: Aspire OTLP endpoint

## 🔧 Available Tools

The Codespace includes:
- ✅ .NET 8.0 & 9.0 SDK
- ✅ .NET Aspire workload
- ✅ Azure CLI
- ✅ Node.js (for any frontend dependencies)
- ✅ Git & GitHub CLI
- ✅ VS Code extensions for C# development

## 💡 Tips

1. **Hot Reload**: Use `dotnet watch run` for automatic reloading during development
2. **Multiple Projects**: Each module can be run independently
3. **Secrets Management**: Always use `dotnet user-secrets` for API keys, never commit them to code
4. **Aspire Dashboard**: Great for monitoring distributed applications in modules 5, 7, and 8

## 🐛 Troubleshooting

- **Build Errors**: Run `dotnet restore` and `dotnet build` in the specific module directory
- **Missing Packages**: The setup script should install everything, but you can manually restore with `dotnet restore`
- **Port Issues**: Check the "Ports" tab in VS Code to see forwarded ports

## 📚 Course Resources

- [Course on Udemy](https://www.udemy.com/course/building-smarter-ai-apps-with-semantic-kernel)
- [Semantic Kernel Documentation](https://learn.microsoft.com/en-us/semantic-kernel/)
- [.NET Aspire Documentation](https://learn.microsoft.com/en-us/dotnet/aspire/)

Happy learning! 🎓
