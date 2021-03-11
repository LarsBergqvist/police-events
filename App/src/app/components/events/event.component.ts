import { Component, EventEmitter, Input, Output } from '@angular/core';
import { PoliceEventViewModel } from 'src/app/models/police-event-viewmodel';
import { PoliceEventService } from 'src/app/services/police-event.service';

@Component({
    selector: 'app-event',
    templateUrl: './event.component.html',
    styleUrls: ['./event.component.scss']
})
export class EventComponent {
    @Input('event') event: PoliceEventViewModel;
    @Input('isLoading') isLoading: boolean;
    @Output() onShowOnMap = new EventEmitter<PoliceEventViewModel>();

    constructor(private readonly service: PoliceEventService) {}

    async onShowDetails(ev: any) {
        if (!this.event.details || this.event.details === '') {
            this.service.fetchEventById(this.event.id.toString()).then((e) => {
                this.event.details = e.details;
            });
        }
    }

    async onShowMapClicked(event: PoliceEventViewModel) {
        this.onShowOnMap.emit(event);
    }
}
