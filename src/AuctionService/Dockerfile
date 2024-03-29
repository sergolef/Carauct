FROM mcr.microsoft.com/dotnet/sdk:8.0 as build
WORKDIR /app
EXPOSE 80

# copy all .csproj files and restore a distinct layer. Use of the same COPY command
#from every dockerfile in the project to take advantage of docker caching
COPY Carauct.sln Carauct.sln
COPY src/AuctionService/AuctionService.csproj src/AuctionService/AuctionService.csproj
COPY src/SearchService/SearchService.csproj src/SearchService/SearchService.csproj
COPY src/GatewayService/GatewayService.csproj src/GatewayService/GatewayService.csproj
COPY src/Contracts/Contracts.csproj src/Contracts/Contracts.csproj
COPY src/IdentityServer/IdentityServer.csproj src/IdentityServer/IdentityServer.csproj

#restore package deps
RUN dotnet restore Carauct.sln

#Copy the app folders over
COPY src/AuctionService src/AuctionService
COPY src/Contracts src/Contracts

WORKDIR /app/src/AuctionService 

RUN dotnet publish -c Release -o /app/src/out

#Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/src/out .
ENTRYPOINT [ "dotnet", "AuctionService.dll" ]
