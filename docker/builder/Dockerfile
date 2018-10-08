FROM harbor.vakaxalab.com/sysad/docker/dotnet-node-alpine
WORKDIR /app

# Copy csproj and restore as distinct layers
COPY . ./
RUN dotnet restore && \
    dotnet publish -c Release -o out
