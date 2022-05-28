import { Component, OnDestroy, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { Subject } from 'rxjs';
import { filter, takeUntil } from 'rxjs/operators';
import { ShowMapMessage } from 'src/app/messages/show-map.message';
import { MapInput } from 'src/app/models/map-input';
import { PoliceEventViewModel } from 'src/app/models/police-event-viewmodel';
import { MapDataService } from 'src/app/services/map-data.service';
import { MessageBrokerService } from 'src/app/services/message-broker.service';
import { PoliceEventService } from 'src/app/services/police-event.service';

@Component({
    selector: 'app-map-sidebar',
    templateUrl: './map-sidebar.component.html'
})
export class MapSidebarComponent implements OnInit, OnDestroy {
    private unsubscribe$ = new Subject();
    event: PoliceEventViewModel;
    isVisible = false;
    details: string = null;
    description: string = null;

    constructor(
        private readonly broker: MessageBrokerService,
        private readonly mapDataService: MapDataService,
        private readonly service: PoliceEventService,
        private readonly router: Router
    ) {}

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
                this.details = null;
                this.description = null;
                this.service.fetchEventById(input.centerPos.id.toString()).then((e) => {
                    this.details = e.details;
                    this.description = e.description;
                });
                this.isVisible = true;
                this.mapDataService.addNewMapInput(input);
            });
    }

    ngOnDestroy(): void {
        this.unsubscribe$.next(undefined);
        this.unsubscribe$.complete();
    }

    close() {
        this.isVisible = false;
    }

    get canShare(): boolean {
        return !!navigator.share;
    }

    share() {
        if (navigator.share) {
            let url = `${window.location.href}/event/${this.event.id}`;
            navigator.share({
                title: this.event.summary,
                text: this.event.summary,
                url: url
            });
        }
    }
}
