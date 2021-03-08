import { Component, EventEmitter, Input, Output } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { ErrorOccurredMessage } from 'src/app/messages/error-occurred.message';
import { ShowMapMessage } from 'src/app/messages/show-map.message';
import { GeoPosition } from 'src/app/models/geo-position';
import { GeoJsonWrapper } from 'src/app/models/geojson-wrapper';
import { LocationObjectViewModel } from 'src/app/models/location-object-viewmodel';
import { PoliceEventViewModel } from 'src/app/models/police-event-viewmodel';
import { AreaService } from 'src/app/services/area-service';
import { LoggingService } from 'src/app/services/logging.service';
import { MessageBrokerService } from 'src/app/services/message-broker.service';
import { NominatimService } from 'src/app/services/nominatim.service';
import { PoliceEventService } from 'src/app/services/police-event.service';

@Component({
    selector: 'app-event-view',
    templateUrl: './event-view.component.html'
})
export class EventViewComponent {
    id: string;
    event: PoliceEventViewModel;
    isLoading: boolean;

    constructor(
        private readonly route: ActivatedRoute,
        private readonly service: PoliceEventService,
        private readonly broker: MessageBrokerService,
        private readonly areaService: AreaService,
        private readonly nominatimService: NominatimService,
        private readonly logger: LoggingService
    ) {}

    async ngOnInit() {
        this.id = this.route.snapshot.paramMap.get('id');
        this.event = await this.service.fetchEventById(this.id);
    }

    async onShowOnMap(event: PoliceEventViewModel) {
        let areaResult: GeoJsonWrapper;
        let locationObject: LocationObjectViewModel;
        try {
            this.isLoading = true;
            if (event.location.name.toLowerCase().endsWith('län')) {
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
}
