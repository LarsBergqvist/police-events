import { Component, EventEmitter, Input, Output } from '@angular/core';
import { PoliceEventViewModel } from 'src/app/models/police-event-viewmodel';

@Component({
    selector: 'app-event',
    templateUrl: './event.component.html',
    styleUrls: ['./event.component.scss']
})
export class EventComponent {
    @Input('event') event: PoliceEventViewModel;
    @Input('isLoading') isLoading: boolean;
    @Output() onShowOnMap = new EventEmitter<PoliceEventViewModel>();

    async onShowMapClicked(event: PoliceEventViewModel) {
        this.onShowOnMap.emit(event);
    }
}
