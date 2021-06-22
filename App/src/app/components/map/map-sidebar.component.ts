import { Component, OnDestroy, OnInit } from '@angular/core';
import { Subject } from 'rxjs';
import { filter, takeUntil } from 'rxjs/operators';
import { ShowMapMessage } from 'src/app/messages/show-map.message';
import { MapInput } from 'src/app/models/map-input';
import { PoliceEventViewModel } from 'src/app/models/police-event-viewmodel';
import { MapDataService } from 'src/app/services/map-data.service';
import { MessageBrokerService } from 'src/app/services/message-broker.service';

@Component({
    selector: 'app-map-sidebar',
    templateUrl: './map-sidebar.component.html'
})
export class MapSidebarComponent implements OnInit, OnDestroy {
    private unsubscribe$ = new Subject();
    event: PoliceEventViewModel;
    isVisible = false;

    constructor(private readonly broker: MessageBrokerService, private readonly mapDataService: MapDataService) {}

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
                input.geoJsonWrapper = message.geoJsonWrapper;
                input.locationObject = message.locationObject;
                this.event = message.event;
                this.isVisible = true;
                this.mapDataService.addNewMapInput(input);
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
