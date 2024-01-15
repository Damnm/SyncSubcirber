FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
ADD ./src /code
WORKDIR /code

RUN mkdir /artifacts

RUN dotnet restore
    
RUN dotnet publish -c Release -o /artifacts

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:6.0
# RUN apt-get update && apt-get -y install ca-certificates && update-ca-certificates
RUN apt-get update && apt-get install -y vim
RUN apt install -y grep mlocate
# Create a group and user
RUN adduser --disabled-password --gecos "" -u 2024 epay
WORKDIR /home/epay/app
RUN chown -R epay /home/epay/app

USER epay
COPY --chown=epay:epay --from=build /artifacts .
ENTRYPOINT ["dotnet", "EPAY.ETC.Core.Sync-Subcriber.dll"]