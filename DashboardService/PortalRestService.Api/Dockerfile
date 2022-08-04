#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 6010
EXPOSE 443



FROM base AS final
WORKDIR /app
COPY /app/publish /app/publish .
ENV OCPPAPI=http://10.0.0.5:6003/api/
ENV ASSETAPI=http://10.0.0.5:6009/api/
ENTRYPOINT ["dotnet", "PortalRestService.Api.dll"]