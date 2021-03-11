import { Component } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { PoliceEventViewModel } from 'src/app/models/police-event-viewmodel';
import { PoliceEventService } from 'src/app/services/police-event.service';
import { MapDataHelper } from './map-data-helper';

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
        private readonly mapDataHelper: MapDataHelper
    ) {}

    async ngOnInit() {
        this.id = this.route.snapshot.paramMap.get('id');
        this.event = await this.service.fetchEventById(this.id);
    }

    async onShowOnMap(event: PoliceEventViewModel) {
        this.isLoading = true;
        await this.mapDataHelper.openMapWithGeoData(event);
        this.isLoading = false;
    }
}
