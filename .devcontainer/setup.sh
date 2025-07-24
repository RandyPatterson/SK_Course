#!/bin/bash

# Semantic Kernel Course Setup Script for GitHub Codespaces
echo "🚀 Setting up Semantic Kernel Course Development Environment..."

# Update package lists
sudo apt-get update

# Install additional tools that might be useful
sudo apt-get install -y curl wget unzip

# Restore .NET packages for all projects
echo "📦 Restoring NuGet packages..."

# Restore packages for each module
for module in module2 module3/OpenAI module4 module5 module7 module8; do
    if [ -d "$module" ]; then
        echo "Restoring packages for $module..."
        cd "$module"
        
        # Find solution files and restore
        for sln in *.sln; do
            if [ -f "$sln" ]; then
                echo "Restoring solution: $sln"
                dotnet restore "$sln"
            fi
        done
        
        # Find individual project files if no solution
        if ! ls *.sln >/dev/null 2>&1; then
            find . -name "*.csproj" -exec dotnet restore {} \;
        fi
        
        cd ..
    fi
done

# Build the solutions to ensure everything works
echo "🔨 Building solutions..."
for module in module2 module4 module5 module7 module8; do
    if [ -d "$module" ]; then
        cd "$module"
        for sln in *.sln; do
            if [ -f "$sln" ]; then
                echo "Building solution: $sln"
                dotnet build "$sln" --no-restore
            fi
        done
        cd ..
    fi
done

# Install .NET Aspire workload (needed for modules 5, 7, 8)
echo "🌟 Installing .NET Aspire workload..."
dotnet workload install aspire

# Create a sample appsettings file template if needed
echo "⚙️ Creating configuration templates..."
cat > appsettings.template.json << 'EOF'
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*",
  "AzureOpenAI": {
    "Endpoint": "YOUR_AZURE_OPENAI_ENDPOINT",
    "ApiKey": "YOUR_AZURE_OPENAI_API_KEY",
    "DeploymentName": "YOUR_DEPLOYMENT_NAME"
  },
  "OpenAI": {
    "ApiKey": "YOUR_OPENAI_API_KEY",
    "ModelId": "gpt-4"
  }
}
EOF

# Set up user secrets for each project (they'll need to configure the actual values)
echo "🔐 Initializing user secrets for projects..."
for module in module2 module4 module5 module7 module8; do
    if [ -d "$module" ]; then
        find "$module" -name "*.csproj" -exec bash -c '
            if grep -q "UserSecretsId" "$1"; then
                echo "Initializing user secrets for $1"
                dotnet user-secrets init --project "$1" 2>/dev/null || true
            fi
        ' _ {} \;
    fi
done

# Create a helpful README for Codespaces users
cat > CODESPACES_README.md << 'EOF'
# Semantic Kernel Course in GitHub Codespaces

Welcome to the Semantic Kernel course development environment! 🎉

## Getting Started

1. **Configure API Keys**: You'll need to set up your OpenAI or Azure OpenAI API keys using user secrets:
   ```bash
   # For Azure OpenAI
   dotnet user-secrets set "AzureOpenAI:Endpoint" "YOUR_ENDPOINT" --project module2/SK_Console
   dotnet user-secrets set "AzureOpenAI:ApiKey" "YOUR_API_KEY" --project module2/SK_Console
   dotnet user-secrets set "AzureOpenAI:DeploymentName" "YOUR_DEPLOYMENT" --project module2/SK_Console
   
   # For OpenAI
   dotnet user-secrets set "OpenAI:ApiKey" "YOUR_API_KEY" --project module2/SK_Console
   ```

2. **Run Console Applications**:
   ```bash
   cd module2/SK_Console
   dotnet run
   ```

3. **Run Web Applications** (modules 5, 7, 8):
   ```bash
   cd module5
   dotnet run --project SKDevChat.AppHost
   ```

## Module Structure

- **Module 2**: Foundation concepts with console application
- **Module 3**: Different model providers (OpenAI, Azure AI, Ollama, etc.)
- **Module 4**: Multimodal AI capabilities
- **Module 5**: ASP.NET Core chat application with .NET Aspire
- **Module 7**: Advanced features and plugins
- **Module 8**: Production-ready applications

## Ports

The following ports are forwarded for development:
- 5000/5001: Standard ASP.NET Core development server
- 7000/7001: Chat application
- 8080/8081: Customer API
- 15000/15001: .NET Aspire Dashboard

## Useful Commands

```bash
# Restore all packages
dotnet restore

# Build specific solution
dotnet build module5/SKDevChat.sln

# Run with hot reload
dotnet watch run --project module5/Chat

# View Aspire dashboard
# Navigate to forwarded port 15000 or 15001
```

## Configuration Notes

- User secrets are initialized for projects that use them
- Remember to configure your AI service API keys
- The Aspire dashboard will be available when running Aspire projects

Happy coding! 🚀
EOF

echo "✅ Setup complete! Check CODESPACES_README.md for next steps."
echo "🔑 Don't forget to configure your API keys using dotnet user-secrets!"
