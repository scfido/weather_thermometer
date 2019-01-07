FROM microsoft/dotnet:2.2-aspnetcore-runtime AS base
ENV TZ=Asia/Shanghai
RUN ln -snf /usr/share/zoneinfo/$TZ /etc/localtime && echo $TZ > /etc/timezone
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM microsoft/dotnet:2.2-sdk AS build
WORKDIR /src
COPY ["*.csproj", "./"]
RUN dotnet restore "./Server.csproj"
COPY . ./
WORKDIR "/src"
RUN dotnet build "Server.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "Server.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Server.dll"]