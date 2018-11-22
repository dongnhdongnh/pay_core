FROM microsoft/dotnet:2.1-sdk AS build-env
WORKDIR /source

RUN curl -sL https://deb.nodesource.com/setup_10.x |  bash -
RUN apt-get install -y nodejs
 
COPY . ./
 
RUN dotnet restore
 
RUN npm install
 
RUN dotnet publish "./Vakapay.ApiServer/Vakapay.ApiServer.csproj" -c Release --output "./dist"

# Stage 2
FROM microsoft/dotnet:2.1-aspnetcore-runtime
WORKDIR /app
COPY --from=build-env /app/dist .
ENTRYPOINT ["dotnet", "Vakapay.ApiServer.dll"]