import { Component, OnDestroy, OnInit } from '@angular/core';
import { Subject } from 'rxjs';
import { filter, takeUntil } from 'rxjs/operators';
import { ShowMapMessage } from 'src/app/messages/show-map.message';
import { PoliceEventViewModel } from 'src/app/models/police-event-viewmodel';
import { MessageBrokerService } from 'src/app/services/message-broker.service';
import { MapInput } from './map.component';

@Component({
    selector: 'app-map-sidebar',
    templateUrl: './map-sidebar.component.html'
})
export class MapSidebarComponent implements OnInit, OnDestroy {
    private unsubscribe$ = new Subject();
    event: PoliceEventViewModel;
    isVisible = false;
    mapInput: MapInput = null;

    constructor(private readonly broker: MessageBrokerService) {}

    ngOnInit() {
        this.broker
            .getMessage()
            .pipe(
                takeUntil(this.unsubscribe$),
                filter((message) => message instanceof ShowMapMessage)
            )
            .subscribe((message: ShowMapMessage) => {
                const input = new MapInput();
                input.centerPos = message.event.location.pos;
                input.geoJson = message.geoJson;
                input.locationObject = message.locationObject;
                this.mapInput = input;
                this.event = message.event;
                if (this.event.location.name.includes(' län')) {
                    input.showCounty = true;
                }
                this.isVisible = true;
            });
    }

    ngOnDestroy(): void {
        this.unsubscribe$.next();
        this.unsubscribe$.complete();
    }

    close() {
        this.isVisible = false;
    }
}
