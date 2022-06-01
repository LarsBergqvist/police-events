import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { lastValueFrom } from 'rxjs';
import { BoundingBox } from '../models/boundingbox';
import { GeoJsonWrapper } from '../models/geojson-wrapper';
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

    async fetchGeoJsonForMunicipality(municipality: string): Promise<GeoJsonWrapper> {
        const baseUrl = this.configService.municipalityUrl;
        let url = `${baseUrl}&q=${municipality}`;
        const res = await lastValueFrom(this.http.get<any>(`${url}`));
        if (res.nhits < 1) throw new Error(`No GeoJson data found for '${municipality}'`);

        return this.createGeoJsonWrapperObject(res);
    }

    async fetchGeoJsonForCounty(county: string): Promise<GeoJsonWrapper> {
        const baseUrl = this.configService.countyUrl;
        let url = `${baseUrl}&q=${county}`;
        const res = await lastValueFrom(this.http.get<any>(`${url}`));
        if (res.nhits < 1) throw new Error(`No GeoJson data found for '${county}'`);

        return this.createGeoJsonWrapperObject(res);
    }

    private createGeoJsonWrapperObject(data: any): GeoJsonWrapper {
        const geoJson = {
            type: 'FeatureCollection',
            features: [
                {
                    type: 'Feature',
                    geometry: data?.records[0]?.fields['geo_shape']
                }
            ]
        };

        let geometryType = geoJson?.features[0]?.geometry?.type;

        const wrapper: GeoJsonWrapper = {
            geoJson,
            boundingBox: this.calculateBoundingBox(geometryType, geoJson.features[0]?.geometry?.coordinates)
        };

        return wrapper;
    }

    private calculateBoundingBox(geometryType: string, coordinates: any): BoundingBox {
        let boundingBox: BoundingBox = {
            lngMax: -180.0,
            lngMin: 180.0,
            latMax: -90.0,
            latMin: 90.0
        };

        if (coordinates) {
            if (geometryType === 'MultiPolygon') {
                const numPolygons = coordinates.length;
                for (let i = 0; i < numPolygons; i++) {
                    const coordCollection = coordinates[i];
                    this.adjustBoundingBoxMinMax(coordCollection, boundingBox);
                }
            } else if (geometryType === 'Polygon') {
                const coordCollection = coordinates;
                this.adjustBoundingBoxMinMax(coordCollection, boundingBox);
            } else {
                // unsupported type
                this.logger.logError(`Unsupported geometry type: ${geometryType}`);
                return boundingBox;
            }
        }

        return boundingBox;
    }

    private adjustBoundingBoxMinMax(coordCollection: any, bb: BoundingBox) {
        let coords = coordCollection[0];
        const numCoords = coords.length;
        for (let i = 0; i < numCoords; i++) {
            bb.lngMax = Math.max(bb.lngMax, coords[i][0]);
            bb.lngMin = Math.min(bb.lngMin, coords[i][0]);
            bb.latMax = Math.max(bb.latMax, coords[i][1]);
            bb.latMin = Math.min(bb.latMin, coords[i][1]);
        }
    }
}
