import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { AppConfigService } from './app-config.service';
import { LoggingService } from './logging.service';

@Injectable({
    providedIn: 'root'
})
export class AreaService {
    constructor(
        private readonly http: HttpClient,
        private readonly logger: LoggingService,
        private readonly configService: AppConfigService
    ) {}

    async fetchGeoJsonForMunicipality(municipality: string): Promise<any> {
        const baseUrl = this.configService.municipalityUrl;
        let url = `${baseUrl}&q=${municipality}`;
        const res = await this.http.get<any>(`${url}`).toPromise();
        if (res.nhits !== 1) return null;

        return this.createGeoJsonObject(res);
    }

    async fetchGeoJsonForCounty(county: string): Promise<any> {
        const baseUrl = this.configService.countyUrl;
        let url = `${baseUrl}&q=${county}`;
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
