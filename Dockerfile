FROM ://microsoft.com AS base
WORKDIR /app
EXPOSE 8080

FROM ://microsoft.com AS build
WORKDIR /src
COPY ["RaceDay.Web.csproj", "./"]
RUN dotnet restore "RaceDay.Web.csproj"
COPY . .
RUN dotnet build "RaceDay.Web.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "RaceDay.Web.csproj" -c Release /p:UseAppHost=false -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "RaceDay.Web.dll"]
