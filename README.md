# police-events

[![Build Status](https://larsbq.visualstudio.com/PoliceEvents/_apis/build/status/LarsBergqvist.police-events2?branchName=main)](https://larsbq.visualstudio.com/PoliceEvents/_build/latest?definitionId=16&branchName=main)

This repository consists of:  
* a <b>CollectorService</b> that fetches the latest event data from the Open Api of the Swedish Police every 10 minutes and stores it in a MongoDB database. When hosted in Azure, the service is an <b>Azure function with a timer trigger</b>    
* a <b>.NET 8 REST API</b> that dispatches queries to the to the MongoDB database   
* a mobile-friendly <b>Angular front-end app</b> that uses the REST API for searching and displaying nearby events (based on geolocation). The Angular application has a map view with overlays for Swedish counties and municipalities   
* a <b>Blazor web-assembly application</b> for searching among old police events. This app is hosted by the api-service   


# Architecture
![Alt text](https://github.com/LarsBergqvist/police-events/blob/main/overview_azure.png?raw=true 'Overview of the system in Azure')

![Alt text](https://github.com/LarsBergqvist/police-events/blob/main/dependencies.png?raw=true 'Dependencies')

The incoming requests to the Api triggers Queries that are dispatched to handlers in the Core assembly. The business logic in Core operates against the repository interfaces for fetching data. The CollectorService (and the Azure function) dispatches Commands to the Core assembly that in turn makes updates to the database via the repository interfaces.  

# Pre-reqs  
* Install a MongoDB instance and create a database called 'Police' with a collection called 'police_events'  
* Create a 2dsphere index on the Geo field for the collection  
* Modify the appsettings.json files in the CollectorService- and API-projects with connection strings for the database  

# Setup and start the CollectorService
```
cd police-events/CollectorService
dotnet run

```
You can also use the Azure functions-project if you want to trigger the data collection from an Azure function instead.  

# Setup and start the backend .NET Core API and the Blazor App
```
cd police-events/Api
dotnet run

```
## Access the swagger endpoint for the Api
https://localhost:5001/swagger
## Access the Blazor application
https://localhost:5001

# Setup and start the Angular application
https is required for using geolocation from browser  
A self-signed certificate is provided in the repo but you can replace it with your own  
```
cd police-events/App
yarn install
yarn startSSL  

https://localhost:4300/

```
# Screenshots

![Alt text](https://github.com/LarsBergqvist/police-events/blob/main/screenshot1.png?raw=true 'Police events list')
![Alt text](https://github.com/LarsBergqvist/police-events/blob/main/screenshot2.png?raw=true 'Map sidebar 1')
![Alt text](https://github.com/LarsBergqvist/police-events/blob/main/screenshot3.png?raw=true 'Map sidebar 2')
![Alt text](https://github.com/LarsBergqvist/police-events/blob/main/screenshot4.png?raw=true 'Blazor application')
