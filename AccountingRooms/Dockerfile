FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build-env

ENV DOTNET_URLS=http://+:5000

WORKDIR /app
EXPOSE 80
EXPOSE 5000

COPY *.csproj ./
RUN dotnet restore

COPY . ./
RUN dotnet publish -c Release -o out

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS final-env
WORKDIR /app
COPY --from=build-env /app/out .
ENTRYPOINT [ "dotnet", "AccountingRooms.dll" ]