import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { PoliceEvent } from '../models/police-event';
import { PoliceEventViewModel } from '../models/police-event-viewmodel';
import { GeoPosition } from '../models/geo-position';
import { AppConfigService } from './app-config.service';
import { PoliceEventsResult } from '../models/police-event-results';

@Injectable({
    providedIn: 'root'
})
export class PoliceEventService {
    private readonly BaseUrl;

    constructor(private readonly http: HttpClient, configService: AppConfigService) {
        this.BaseUrl = `${configService.apiUrl}/events`;
    }

    async fetchEventById(id: string): Promise<PoliceEventViewModel> {
        let url = `${this.BaseUrl}/${id}`;
        const res = await this.http.get<PoliceEvent>(`${url}`).toPromise();
        return this.convertToViewModel(res);
    }

    async fetchEventsForDateWithinRadius(
        fromUtcDate: string,
        toUtcDate: string,
        userPos: GeoPosition,
        radiusKm: number
    ): Promise<PoliceEventViewModel[]> {
        let url = `${this.BaseUrl}?fromDate=${fromUtcDate}&toDate=${toUtcDate}&userLat=${userPos.lat}&userLng=${userPos.lng}&maxKm=${radiusKm}&page=1&pageSize=100`;
        const res = await this.http.get<PoliceEventsResult>(`${url}`).toPromise();
        return this.convertCollectionToViewModel(res.events);
    }

    async fetchEventsForDate(fromUtcDate: string, toUtcDate: string): Promise<PoliceEventViewModel[]> {
        let url = `${this.BaseUrl}?fromDate=${fromUtcDate}&toDate=${toUtcDate}&page=1&pageSize=100`;
        const res = await this.http.get<PoliceEventsResult>(`${url}`).toPromise();
        return this.convertCollectionToViewModel(res.events);
    }

    private convertToViewModel(e: PoliceEvent): PoliceEventViewModel {
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
            details: e.details,
            description: e.description,
            location: {
                name: e.location.name,
                pos: geoPos
            }
        };

        return viewModel;
    }

    private convertCollectionToViewModel(policeEvents: PoliceEvent[]): PoliceEventViewModel[] {
        let viewModels: PoliceEventViewModel[] = [];
        policeEvents.forEach((e) => {
            let viewModel = this.convertToViewModel(e);
            viewModels.push(viewModel);
        });

        viewModels.sort((a, b) => b.datetime.getTime() - a.datetime.getTime());
        return viewModels;
    }
}
