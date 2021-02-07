import { PoliceEventViewModel } from '../models/police-event-viewmodel';
import { GeoPosition } from '../view-models/geo-position';
import { Message } from './message';

export class ShowMapMessage extends Message {
    positions: GeoPosition[];
    userPos: GeoPosition;
    event: PoliceEventViewModel;
    geoJson: any;
    constructor(positions: GeoPosition[], userPos: GeoPosition, event: PoliceEventViewModel, geoJson?: any) {
        super();
        this.positions = positions;
        this.userPos = userPos;
        this.event = event;
        this.geoJson = geoJson;
    }

    get Type(): string {
        return 'ShowMapMessage';
    }
}
