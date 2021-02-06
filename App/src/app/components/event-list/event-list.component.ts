import { Component, OnInit } from '@angular/core';
import { ErrorOccurredMessage } from 'src/app/messages/error-occurred.message';
import { PoliceEventViewModel } from 'src/app/models/police-event-viewmodel';
import { MessageBrokerService } from 'src/app/services/message-broker.service';
import { PoliceEventService } from 'src/app/services/police-event.service';
import { getDateTimeNDaysFromNow, getUTCDateStringFromLocalDateTime } from 'src/app/utils/date-helper';
import { GeoPosition } from 'src/app/view-models/geo-position';

@Component({
    selector: 'app-event-list',
    templateUrl: './event-list.component.html'
})
export class EventListComponent implements OnInit {
    events: PoliceEventViewModel[];
    constructor(private readonly service: PoliceEventService, private readonly broker: MessageBrokerService) {}

    async ngOnInit() {}

    onFetchPositionVeryClose() {
        this.fetchMessageClose(15);
    }
    onFetchPositionClose() {
        this.fetchMessageClose(35);
    }
    fetchMessageClose(distance: number) {
        navigator.geolocation.getCurrentPosition(
            async (position) => {
                const userPos: GeoPosition = {
                    lat: position.coords.latitude,
                    lng: position.coords.longitude
                };
                const toDate = new Date();
                const fromDate = getDateTimeNDaysFromNow(toDate, -3);
                const utcToDateString = getUTCDateStringFromLocalDateTime(toDate);
                const utcFromDateString = getUTCDateStringFromLocalDateTime(fromDate);
                this.events = await this.service.fetchEventsForDateWithinRadius(
                    utcFromDateString,
                    utcToDateString,
                    userPos,
                    distance
                );
            },
            (error) => {
                this.broker.sendMessage(new ErrorOccurredMessage(error.message));
            }
        );
    }
}
