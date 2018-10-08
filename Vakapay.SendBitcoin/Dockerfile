FROM harbor.vakaxalab.com/vakagroup/vakapay/core:builder as builder

FROM microsoft/dotnet:2.1-aspnetcore-runtime-alpine
WORKDIR /app
ENV NETCORE_ENVIRONMENT Production

COPY --from=builder /app/Vakapay.SendBitcoin/out .
ENTRYPOINT ["dotnet", "Vakapay.SendBitcoin.dll"]