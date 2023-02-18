FROM mcr.microsoft.com/dotnet/runtime:7.0
WORKDIR /app
COPY published/ ./
ENTRYPOINT ["dotnet", "Intech.Invoice.dll"]