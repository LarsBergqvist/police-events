import { Component, OnInit } from '@angular/core';
import { ErrorOccurredMessage } from 'src/app/messages/error-occurred.message';
import { PoliceEventViewModel } from 'src/app/models/police-event-viewmodel';
import { MessageBrokerService } from 'src/app/services/message-broker.service';
import { PoliceEventService } from 'src/app/services/police-event.service';
import { getDateTimeNDaysFromNow, getUTCDateStringFromLocalDateTime } from 'src/app/utils/date-helper';
import { GeoPosition } from 'src/app/models/geo-position';
import { GeoJsonWrapper } from 'src/app/models/geojson-wrapper';
import { LocationObjectViewModel } from 'src/app/models/location-object-viewmodel';
import { AreaService } from 'src/app/services/area-service';
import { NominatimService } from 'src/app/services/nominatim.service';
import { LoggingService } from 'src/app/services/logging.service';
import { ShowMapMessage } from 'src/app/messages/show-map.message';

@Component({
    selector: 'app-event-list',
    templateUrl: './event-list.component.html',
    styleUrls: ['event-list.component.scss']
})
export class EventListComponent implements OnInit {
    events: PoliceEventViewModel[];
    keyword: string = '';
    isLoading = false;

    constructor(
        private readonly service: PoliceEventService,
        private readonly broker: MessageBrokerService,
        private readonly areaService: AreaService,
        private readonly nominatimService: NominatimService,
        private readonly logger: LoggingService
    ) {}

    async ngOnInit() {}

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
        this.fetchMessageClose(15);
    }

    onFetchPositionClose() {
        this.fetchMessageClose(35);
    }

    async onFetchAll() {
        try {
            this.isLoading = true;
            const utcStrings = this.getUtcFromToStrings();
            this.events = await this.service.fetchEventsForDate(utcStrings[0], utcStrings[1]);
        } catch (error) {
        } finally {
            this.isLoading = false;
        }
    }

    fetchMessageClose(distance: number) {
        this.isLoading = true;
        navigator.geolocation.getCurrentPosition(async (position) => {
            const userPos: GeoPosition = {
                lat: position.coords.latitude,
                lng: position.coords.longitude
            };
            try {
                const utcStrings = this.getUtcFromToStrings();
                this.events = await this.service.fetchEventsForDateWithinRadius(
                    utcStrings[0],
                    utcStrings[1],
                    userPos,
                    distance
                );
            } catch (error) {
                this.broker.sendMessage(new ErrorOccurredMessage(error.message));
            } finally {
                this.isLoading = false;
            }
        });
    }

    async onShowOnMap(event: PoliceEventViewModel) {
        let areaResult: GeoJsonWrapper;
        let locationObject: LocationObjectViewModel;
        try {
            this.isLoading = true;
            if (event.location.name.toLowerCase().includes('län')) {
                // The location is a county
                areaResult = await this.areaService.fetchGeoJsonForCounty(event.location.name);
            } else {
                // The location is a likely municipality
                areaResult = await this.areaService.fetchGeoJsonForMunicipality(event.location.name);
                // Make a text search for a more detailed location
                locationObject = await this.nominatimService.searchBestMatchingLocationObject(
                    event.location.name,
                    event.summary,
                    areaResult.boundingBox
                );
            }
        } catch (error) {
            this.logger.logError(error?.message);
            const message = `Could not get map information for '${event.location.name}'`;
            this.broker.sendMessage(new ErrorOccurredMessage(message));
            return;
        } finally {
            this.isLoading = false;
        }
        const positions: GeoPosition[] = [];
        positions.push(event.location.pos);
        this.broker.sendMessage(new ShowMapMessage(event, areaResult, locationObject));
    }

    private getUtcFromToStrings(): [string, string] {
        const toDate = new Date();
        const fromDate = getDateTimeNDaysFromNow(toDate, -3);
        const utcToDateString = getUTCDateStringFromLocalDateTime(toDate);
        const utcFromDateString = getUTCDateStringFromLocalDateTime(fromDate);
        return [utcFromDateString, utcToDateString];
    }
}
