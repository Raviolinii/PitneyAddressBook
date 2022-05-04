#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

ENV ASPNETCORE_ENVIRONMENT=Development

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["PitneyAddressBook/PitneyAddressBook.csproj", "PitneyAddressBook/"]
COPY ["Tests/Tests.csproj", "Tests/"]

RUN dotnet restore "PitneyAddressBook/PitneyAddressBook.csproj"
RUN dotnet restore "Tests/Tests.csproj"
COPY . .

WORKDIR "/src/."
RUN dotnet build "PitneyAddressBook/PitneyAddressBook.csproj" -c Release -o /app/build
RUN dotnet build "Tests/Tests.csproj"

FROM build AS tests
CMD dotnet test "Tests/Tests.csproj"

FROM build AS publish
RUN dotnet publish "PitneyAddressBook/PitneyAddressBook.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PitneyAddressBook.dll"]