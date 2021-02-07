import { Component, OnInit } from '@angular/core';
import { ErrorOccurredMessage } from 'src/app/messages/error-occurred.message';
import { PoliceEventViewModel } from 'src/app/models/police-event-viewmodel';
import { MessageBrokerService } from 'src/app/services/message-broker.service';
import { PoliceEventService } from 'src/app/services/police-event.service';
import { getDateTimeNDaysFromNow, getUTCDateStringFromLocalDateTime } from 'src/app/utils/date-helper';
import { GeoPosition } from 'src/app/models/geo-position';

@Component({
    selector: 'app-event-list',
    templateUrl: './event-list.component.html',
    styleUrls: ['event-list.component.scss']
})
export class EventListComponent implements OnInit {
    events: PoliceEventViewModel[];
    keyword: string = '';
    isLoading = false;

    constructor(private readonly service: PoliceEventService, private readonly broker: MessageBrokerService) {}

    async ngOnInit() {}

    onClear() {
        this.keyword = '';
    }

    matchesKeyword(vm: PoliceEventViewModel): boolean {
        if (!this.keyword || this.keyword.length < 2) return true;

        var re = new RegExp(this.keyword, 'gi');
        if (vm.summary.match(re) || vm.location.name.match(re) || vm.type.match(re)) {
            return true;
        } else {
            return false;
        }
    }

    onFetchPositionVeryClose() {
        this.fetchMessageClose(15);
    }

    onFetchPositionClose() {
        this.fetchMessageClose(35);
    }

    async onFetchAll() {
        const utcStrings = this.getUtcFromToStrings();
        this.events = await this.service.fetchEventsForDate(utcStrings[0], utcStrings[1]);
    }

    fetchMessageClose(distance: number) {
        navigator.geolocation.getCurrentPosition(
            async (position) => {
                const userPos: GeoPosition = {
                    lat: position.coords.latitude,
                    lng: position.coords.longitude
                };
                const utcStrings = this.getUtcFromToStrings();
                this.events = await this.service.fetchEventsForDateWithinRadius(
                    utcStrings[0],
                    utcStrings[1],
                    userPos,
                    distance
                );
            },
            (error) => {
                this.broker.sendMessage(new ErrorOccurredMessage(error.message));
            }
        );
    }

    private getUtcFromToStrings(): [string, string] {
        const toDate = new Date();
        const fromDate = getDateTimeNDaysFromNow(toDate, -3);
        const utcToDateString = getUTCDateStringFromLocalDateTime(toDate);
        const utcFromDateString = getUTCDateStringFromLocalDateTime(fromDate);
        return [utcFromDateString, utcToDateString];
    }
}
