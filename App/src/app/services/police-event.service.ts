import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { PoliceEvent } from '../models/police-event';
import { PoliceEventViewModel } from '../models/police-event-viewmodel';
import { calcPosDistanceKm } from '../utils/distance-helper';
import { GeoPosition } from '../models/geo-position';
import { LoggingService } from './logging.service';
import { AppConfigService } from './app-config.service';
@Injectable({
    providedIn: 'root'
})
export class PoliceEventService {
    private readonly BaseUrl;

    constructor(
        private readonly http: HttpClient,
        private readonly logger: LoggingService,
        private readonly configService: AppConfigService
    ) {
        this.BaseUrl = `${configService.apiUrl}/events`;
    }

    async fetchEventsForDateWithinRadius(
        fromUtcDate: string,
        toUtcDate: string,
        userPos: GeoPosition,
        radiusKm: number
    ): Promise<PoliceEventViewModel[]> {
        const vm = await this.fetchEventsForDate(fromUtcDate, toUtcDate);
        vm.forEach((event) => {
            event.location.distance = calcPosDistanceKm(userPos, event.location.pos);
        });

        const withinRadius = vm.filter((event) => {
            if (calcPosDistanceKm(userPos, event.location.pos) < radiusKm) {
                return true;
            }
        });
        return withinRadius;
    }

    async fetchEventsForDate(fromUtcDate: string, toUtcDate: string): Promise<PoliceEventViewModel[]> {
        let url = `${this.BaseUrl}?fromDate=${fromUtcDate}&toDate=${toUtcDate}`;
        const res = await this.http.get<PoliceEvent[]>(`${url}`).toPromise();
        return this.convertToViewModel(res);
    }

    private convertToViewModel(policeEvents: PoliceEvent[]): PoliceEventViewModel[] {
        let viewModels: PoliceEventViewModel[] = [];
        policeEvents.forEach((e) => {
            var geoPos: GeoPosition = {
                lat: e.location.lat,
                lng: e.location.lng,
                id: e.id,
                info: e.summary
            };

            let viewModel: PoliceEventViewModel = {
                id: e.id,
                datetime: new Date(e.utcDateTime),
                summary: e.summary,
                url: e.url,
                type: e.type,
                location: {
                    name: e.location.name,
                    pos: geoPos,
                    distance: undefined
                }
            };
            viewModels.push(viewModel);
        });

        viewModels.sort((a, b) => b.datetime.getTime() - a.datetime.getTime());
        return viewModels;
    }
}
