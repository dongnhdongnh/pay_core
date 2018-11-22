 FROM microsoft/aspnetcore-build:2.0.5-2.1.500 AS build-env
 WORKDIR /source

 COPY . ./
 
 RUN dotnet restore
 
 RUN dotnet publish -c Release -o out

 # Stage 2
 FROM microsoft/aspnetcore:2.0.5-2.1.500
 WORKDIR /app
 COPY --from=build-env /app/out .
 ENTRYPOINT ["dotnet", "Vakapay.ApiServer.dll"]