import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { BoundingBox } from '../models/boundingbox';
import { LocationObjectViewModel } from '../models/location-object-viewmodel';
import { AppConfigService } from './app-config.service';
import { LoggingService } from './logging.service';
import { locationQueryFromTextAndAreaName } from './word-query-heuristics';

@Injectable({
    providedIn: 'root'
})
export class NominatimService {
    private readonly BaseUrl;

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
        bb: BoundingBox
    ): Promise<LocationObjectViewModel> {
        const query = locationQueryFromTextAndAreaName(text, areaname);
        if (query === '') return;

        const viewbox = `${bb.lngMin},${bb.latMin},${bb.lngMax},${bb.latMax}`;
        this.logger.logInfo(`nominatim query: ${query}`);
        const url = `${this.BaseUrl}?q=${query}&countrycodes=se&limit=1&format=json&viewbox=${viewbox}`;
        const res = await this.http.get<any[]>(`${url}`).toPromise();
        let vm = this.convertToViewModel(res);
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
                    latMin: parseFloat(e.boundingbox[0]),
                    latMax: parseFloat(e.boundingbox[1]),
                    lngMin: parseFloat(e.boundingbox[2]),
                    lngMax: parseFloat(e.boundingbox[3])
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
        let xdiff = viewModel.boundingBox.lngMax - viewModel.boundingBox.lngMin;
        const scaleUp: number = 0.005;
        const minDiff: number = 0.005;
        if (xdiff < minDiff) {
            viewModel.boundingBox.lngMin -= scaleUp;
            viewModel.boundingBox.lngMax += scaleUp;
        }
        let ydiff = viewModel.boundingBox.latMax - viewModel.boundingBox.latMin;
        if (ydiff < minDiff) {
            viewModel.boundingBox.latMin -= scaleUp;
            viewModel.boundingBox.latMax += scaleUp;
        }
        return viewModel;
    }
}
