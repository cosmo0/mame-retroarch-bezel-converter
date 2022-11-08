dotnet publish src/BezelTools.sln -r win-x64 -p:PublishSingleFile=true --self-contained true -o out/win-x64
dotnet publish src/BezelTools/BezelTools.csproj -r win-arm -p:PublishSingleFile=true --self-contained true -o out/win-arm
dotnet publish src/BezelTools/BezelTools.csproj -r win-arm64 -p:PublishSingleFile=true --self-contained true -o out/win-arm64
dotnet publish src/BezelTools/BezelTools.csproj -r linux-x64 -p:PublishSingleFile=true --self-contained true -o out/linux-x64
dotnet publish src/BezelTools/BezelTools.csproj -r linux-arm -p:PublishSingleFile=true --self-contained true -o out/linux-arm
dotnet publish src/BezelTools/BezelTools.csproj -r linux-arm64 -p:PublishSingleFile=true --self-contained true -o out/linux-arm64
dotnet publish src/BezelTools/BezelTools.csproj -r osx-x64 -p:PublishSingleFile=true --self-contained true -o out/osx-x64
