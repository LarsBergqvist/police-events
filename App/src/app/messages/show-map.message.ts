import { PoliceEventViewModel } from '../models/police-event-viewmodel';
import { Message } from './message';

export class ShowMapMessage extends Message {
    event: PoliceEventViewModel;
    geoJson: any;
    constructor(event: PoliceEventViewModel, geoJson?: any) {
        super();
        this.event = event;
        this.geoJson = geoJson;
    }

    get Type(): string {
        return 'ShowMapMessage';
    }
}
