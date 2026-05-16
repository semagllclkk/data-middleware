# 1. Aşama: Derleme (Build)
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Bütün projeyi tek seferde içeri alıyoruz
COPY . .

# Consumer klasörünün içine girip derleme işlemini yapıyoruz
WORKDIR "/src/Consumer"
RUN dotnet restore "Consumer.csproj"
RUN dotnet publish "Consumer.csproj" -c Release -o /app/publish

# 2. Aşama: Çalıştırma (Runtime)
FROM mcr.microsoft.com/dotnet/runtime:9.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "Consumer.dll"]