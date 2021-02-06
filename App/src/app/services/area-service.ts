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

        return this.createGeoJsonObject(res);
    }

    async fetchGeoJsonForCounty(county: string): Promise<any> {
        const BaseUrl =
            'https://public.opendatasoft.com/api/records/1.0/search/?dataset=sverige-lan-counties-of-sweden';
        let url = `${BaseUrl}&q=${county}`;
        const res = await this.http.get<any>(`${url}`).toPromise();
        if (res.nhits !== 1) return null;

        return this.createGeoJsonObject(res);
    }

    private createGeoJsonObject(data: any): any {
        const geoJson = {
            type: 'FeatureCollection',
            features: [
                {
                    type: 'Feature',
                    geometry: data.records[0].fields['geo_shape']
                }
            ]
        };
        return geoJson;
    }
}
