import { Component, OnInit } from '@angular/core';
import { ErrorOccurredMessage } from 'src/app/messages/error-occurred.message';
import { PoliceEventViewModel } from 'src/app/models/police-event-viewmodel';
import { MessageBrokerService } from 'src/app/services/message-broker.service';
import { PoliceEventService } from 'src/app/services/police-event.service';
import { getDateTimeNDaysFromNow, getUTCDateStringFromLocalDateTime } from 'src/app/utils/date-helper';
import { GeoPosition } from 'src/app/models/geo-position';
import { LoggingService } from 'src/app/services/logging.service';
import { MapDataHelper } from './map-data-helper';

@Component({
    selector: 'app-search-events',
    templateUrl: './search-events.component.html',
    styleUrls: ['search-events.component.scss']
})
export class SearchEventsComponent implements OnInit {
    events: PoliceEventViewModel[];
    keyword: string = '';
    isLoading = false;
    geolocationAvailable = false;

    constructor(
        private readonly service: PoliceEventService,
        private readonly broker: MessageBrokerService,
        private readonly logger: LoggingService,
        private readonly mapDataHelper: MapDataHelper
    ) {}

    async ngOnInit() {
        this.geolocationAvailable = 'geolocation' in navigator;
    }

    onClear() {
        this.keyword = '';
    }

    matchesKeywords(vm: PoliceEventViewModel): boolean {
        if (!this.keyword || this.keyword.length < 2) return true;

        const keywords = this.keyword.split(' ');
        let hasMatch = false;
        for (let i = 0; i < keywords.length; i++) {
            let kw = keywords[i].trim();
            if (kw === '') continue;
            const re = new RegExp(kw, 'gi');
            if (vm.summary.match(re) || vm.location.name.match(re) || vm.type.match(re)) {
                hasMatch = true;
            } else {
                return false;
            }
        }
        return hasMatch;
    }

    onFetchPositionVeryClose() {
        this.fetchMessageClose(15, 8);
    }

    onFetchPositionClose() {
        this.fetchMessageClose(55, 4);
    }

    async onFetchAll() {
        try {
            this.isLoading = true;
            const utcStrings = this.getUtcFromToStrings(4);
            this.events = await this.service.fetchEventsForDate(utcStrings[0], utcStrings[1]);
        } catch (error) {
        } finally {
            this.isLoading = false;
        }
    }

    async fetchMessageClose(radiusKm: number, numDays: number) {
        this.isLoading = true;
        navigator.geolocation.getCurrentPosition(
            async (position) => {
                const userPos: GeoPosition = {
                    lat: position.coords.latitude,
                    lng: position.coords.longitude
                };
                try {
                    const utcStrings = this.getUtcFromToStrings(numDays);
                    this.events = await this.service.fetchEventsForDateWithinRadius(
                        utcStrings[0],
                        utcStrings[1],
                        userPos,
                        radiusKm
                    );

                    if (this.events && this.events.length === 0) {
                        const newRadiusKm = radiusKm * 2;
                        this.logger.logInfo(
                            `No events found within radius ${radiusKm}km. Trying radius ${newRadiusKm}km.`
                        );
                        this.events = await this.service.fetchEventsForDateWithinRadius(
                            utcStrings[0],
                            utcStrings[1],
                            userPos,
                            newRadiusKm
                        );
                    }
                } catch (error) {
                    this.broker.sendMessage(new ErrorOccurredMessage(error.message));
                } finally {
                    this.isLoading = false;
                }
            },
            (error) => {
                this.isLoading = false;
                this.logger.logError(error.message);
                this.broker.sendMessage(new ErrorOccurredMessage(error.message));
            }
        );
    }

    async onShowOnMap(event: PoliceEventViewModel) {
        this.isLoading = true;
        await this.mapDataHelper.openMapWithGeoData(event);
        this.isLoading = false;
    }

    private getUtcFromToStrings(numDays: number): [string, string] {
        const toDate = new Date();
        const fromDate = getDateTimeNDaysFromNow(toDate, -numDays);
        const utcToDateString = getUTCDateStringFromLocalDateTime(toDate);
        const utcFromDateString = getUTCDateStringFromLocalDateTime(fromDate);
        return [utcFromDateString, utcToDateString];
    }
}
