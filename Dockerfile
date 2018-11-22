 FROM microsoft/dotnet:2.1-sdk AS build-env
 WORKDIR /source

 COPY . ./
 
 RUN dotnet restore
 
 RUN npm install
 
 RUN dotnet publish -c Release -o out

 # Stage 2
 FROM microsoft/dotnet:2.1-aspnetcore-runtime
 WORKDIR /app
 COPY --from=build-env /app/out .
 ENTRYPOINT ["dotnet", "Vakapay.ApiServer.dll"]