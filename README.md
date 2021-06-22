# police-events

[![Build Status](https://larsbq.visualstudio.com/PoliceEvents/_apis/build/status/LarsBergqvist.police-events?branchName=main)](https://larsbq.visualstudio.com/PoliceEvents/_build/latest?definitionId=4&branchName=main)

This repository consists of:  
* a CollectorService that fetches the latest event data from the Open Api of the Swedish Police every 10 minutes and stores it in a local MongoDB database  
* a .NET Core REST API that does queries to the MongoDB database  
* an Angular front-end app that uses the REST API for searching and displaying nearby events (based on geolocation)   

The Angular application has a map view with OpenLayers/OpenStreeMap and fetches county and municipality geoJson polygons from public.opendatasoft.com  

![Alt text](https://github.com/LarsBergqvist/police-events/blob/main/overview.jpg?raw=true 'Overview of system')

# Pre-reqs  
Install a MongoDB instance and create a database called 'Police'  
Modify the appsettings*.json files in the CollectionService- and API-projects with connection strings for the database  

# Setup and start the CollectorService
```
cd police-events/CollectorService
dotnet run

```

# Setup and start the backend .NET Core API
```
cd police-events/Api
dotnet run

```

# Setup and start the Angular application
https is required for using geolocation from browser  
A self-signed certificate is provided in the repo but you can replace it with your own  
```
cd police-events/App
yarn install
yarn startSSL  

https://localhost:4300/

```

# Docker
There are Docker- and Docker-compose files in the repo if you want to host the services with Docker  
You will need to provide an https certificate for the api  

# Screenshots

![Alt text](https://github.com/LarsBergqvist/police-events/blob/main/screenshot1.png?raw=true 'Police events list')
![Alt text](https://github.com/LarsBergqvist/police-events/blob/main/screenshot2.png?raw=true 'Map sidebar 1')
![Alt text](https://github.com/LarsBergqvist/police-events/blob/main/screenshot3.png?raw=true 'Map sidebar 2')
