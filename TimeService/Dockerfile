FROM stockdesk-dotnet-sdk-base:1.0 AS build-env
WORKDIR /app

# Copy necessary files and restore as distinct layer
COPY *.csproj ./
RUN --mount=type=cache,id=nuget,target=/root/.nuget/packages \
	dotnet restore

# Copy everything else and build
COPY . ./
RUN --mount=type=cache,id=nuget,target=/root/.nuget/packages \
	dotnet publish -c Release -o out

# Build runtime image
FROM stockdesk-dotnet-runtime-base:1.0
COPY --from=build-env /app/out .

# Start
ENTRYPOINT ["dotnet", "StockDesk.TimeService.dll"]
