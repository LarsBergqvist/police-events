import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { LoggingService } from './logging.service';

@Injectable({
    providedIn: 'root'
})
export class AreaService {
    constructor(private readonly http: HttpClient, private readonly logger: LoggingService) {}

    async fetchGeoJsonForMunicpality(municipality: string): Promise<any> {
        const BaseUrl =
            'https://public.opendatasoft.com/api/records/1.0/search/?dataset=sverige-kommuner-municipalities-of-sweden';
        let url = `${BaseUrl}&q=${municipality}`;
        const res = await this.http.get<any>(`${url}`).toPromise();
        if (res.nhits !== 1) return null;

        const geoJson = {
            type: 'FeatureCollection',
            features: [
                {
                    type: 'Feature',
                    geometry: res.records[0].fields['geo_shape']
                }
            ]
        };
        return geoJson;
    }
}

/*

{
    "nhits": 1,
    "parameters": {
        "dataset": "sverige-kommuner-municipalities-of-sweden",
        "timezone": "UTC",
        "q": "Upplands-Bro",
        "rows": 10,
        "start": 0,
        "format": "json"
    },
    "records": [
        {
            "datasetid": "sverige-kommuner-municipalities-of-sweden",
            "recordid": "bb7183e79012042ad370f67cc702c155e22ca156",
            "fields": {
                "geo_shape": {
                    "type": "Polygon",

*/
