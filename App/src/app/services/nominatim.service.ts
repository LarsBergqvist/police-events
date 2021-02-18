import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { GeoPosition } from '../models/geo-position';
import { LocationObjectViewModel } from '../models/location-object-viewmodel';
import { calcDistanceKm } from '../utils/distance-helper';
import { AppConfigService } from './app-config.service';
import { LoggingService } from './logging.service';
import { locationQueryFromTextAndAreaName } from './word-query-heuristics';

@Injectable({
    providedIn: 'root'
})
export class NominatimService {
    private readonly BaseUrl;
    private readonly MaxDistToRefPointKm = 100;

    constructor(
        private readonly http: HttpClient,
        private readonly logger: LoggingService,
        private readonly configService: AppConfigService
    ) {
        this.BaseUrl = configService.nominatimUrl;
    }

    async searchBestMatchingLocationObject(
        areaname: string,
        text: string,
        refPoint: GeoPosition
    ): Promise<LocationObjectViewModel> {
        const query = locationQueryFromTextAndAreaName(text, areaname);
        if (query === '') return;

        this.logger.logInfo(`nominatim query: ${query}`);
        const url = `${this.BaseUrl}?q=${query}&countrycodes=se&format=json`;
        const res = await this.http.get<any[]>(`${url}`).toPromise();
        let vm = this.convertToViewModel(res);

        if (vm) {
            // Discard results that are too far from the reference point
            const dist1 = calcDistanceKm(refPoint.lat, refPoint.lng, vm.boundingBox.lat1, vm.boundingBox.lng1);
            const dist2 = calcDistanceKm(refPoint.lat, refPoint.lng, vm.boundingBox.lat2, vm.boundingBox.lng2);
            if (dist1 > this.MaxDistToRefPointKm && dist2 > this.MaxDistToRefPointKm) {
                vm = null;
            }
        }
        return vm;
    }

    private convertToViewModel(data: any[]): LocationObjectViewModel {
        if (data.length === 0) return;
        let viewModel: LocationObjectViewModel;
        const e = data[0];
        if (e.boundingbox && e.display_name) {
            viewModel = {
                displayName: e.display_name,
                boundingBox: {
                    lat1: parseFloat(e.boundingbox[0]),
                    lat2: parseFloat(e.boundingbox[1]),
                    lng1: parseFloat(e.boundingbox[2]),
                    lng2: parseFloat(e.boundingbox[3])
                },
                lat: e.lat,
                lng: e.lon
            };
            return this.scaleUpSmallArea(viewModel);
        }
    }

    private scaleUpSmallArea(viewModel: LocationObjectViewModel): LocationObjectViewModel {
        //
        // Make very small areas slightly larger
        //
        let xdiff = viewModel.boundingBox.lng2 - viewModel.boundingBox.lng1;
        const scaleUp: number = 0.005;
        const minDiff: number = 0.005;
        if (xdiff < minDiff) {
            viewModel.boundingBox.lng1 -= scaleUp;
            viewModel.boundingBox.lng2 += scaleUp;
        }
        let ydiff = viewModel.boundingBox.lat2 - viewModel.boundingBox.lat1;
        if (ydiff < minDiff) {
            viewModel.boundingBox.lat1 -= scaleUp;
            viewModel.boundingBox.lat2 += scaleUp;
        }
        return viewModel;
    }
}
