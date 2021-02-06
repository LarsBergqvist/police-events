import { Component, Input, OnInit } from '@angular/core';
import { ShowMapMessage } from 'src/app/messages/show-map.message';
import { PoliceEventViewModel } from 'src/app/models/police-event-viewmodel';
import { AreaService } from 'src/app/services/area-service';
import { MessageBrokerService } from 'src/app/services/message-broker.service';
import { GeoPosition } from 'src/app/view-models/geo-position';

@Component({
    selector: 'app-event',
    templateUrl: './event.component.html',
    styleUrls: ['./event.component.scss']
})
export class EventComponent implements OnInit {
    @Input('event') event: PoliceEventViewModel;

    constructor(private readonly broker: MessageBrokerService, private readonly areaService: AreaService) {}

    async onClickMessage(event: PoliceEventViewModel) {
        let areaResult: any;
        if (event.location.name.toLowerCase().includes('län')) {
            // The location is a county
            areaResult = await this.areaService.fetchGeoJsonForCounty(event.location.name);
        } else {
            // The location is a likely municipality
            areaResult = await this.areaService.fetchGeoJsonForMunicpality(event.location.name);
        }
        const positions: GeoPosition[] = [];
        positions.push(event.location.pos);
        const header = `${event.location.name}, ${event.type}`;
        this.broker.sendMessage(new ShowMapMessage(positions, null, `${header}`, event.summary, areaResult));
    }

    ngOnInit(): void {}
}
