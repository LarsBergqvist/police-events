# police-events
This repository consists of:  
a CollectorService that fetches the latest event data from the Open Api of the Swedish Police every 10 minutes and stores it in a local MongoDB database  
a REST API that returns the data from the MongoDB database  
an Angular front-end app that uses the REST API for searching for and displaying nearby events (based on geolocation)  
