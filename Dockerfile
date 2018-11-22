 FROM microsoft/aspnetcore-build:2.0 AS build-env
 WORKDIR /source

 COPY ./Vakapay.ApiServer/*.csproj ./
 
 RUN dotnet restore
 
 COPY ./Vakapay.ApiServer ./
 
 RUN dotnet publish -c Release -o out

 # Stage 2
 FROM microsoft/aspnetcore:2.0
 WORKDIR /app
 COPY --from=build-env /app/out .
 ENTRYPOINT ["dotnet", "DeedStore.dll"]