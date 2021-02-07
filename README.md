# police-events
This repository consists of:  
* a CollectorService that fetches the latest event data from the Open Api of the Swedish Police every 10 minutes and stores it in a local MongoDB database  
* a .NET Core REST API that provides queries from the MongoDB database  
* an Angular front-end app that uses the REST API for searching and displaying nearby events (based on geolocation)  

The Angular application has a map view with OpenLayers/OpenStreeMap and fetches county and municipality geoJson polygons from public.opendatasoft.com  

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

```
cd police-events/App
yarn install
yarn startSSL  

https://localhost:4300/

```

# Screenshots

![Alt text](https://github.com/LarsBergqvist/police-events/blob/main/screenshot1.png?raw=true 'Police events list')
![Alt text](https://github.com/LarsBergqvist/police-events/blob/main/screenshot2.png?raw=true 'Map sidebar 1')
