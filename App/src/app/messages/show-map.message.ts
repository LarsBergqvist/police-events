import { GeoJsonWrapper } from '../models/geojson-wrapper';
import { LocationObjectViewModel } from '../models/location-object-viewmodel';
import { PoliceEventViewModel } from '../models/police-event-viewmodel';
import { Message } from './message';

export class ShowMapMessage extends Message {
    event: PoliceEventViewModel;
    geoJsonWrapper: GeoJsonWrapper;
    locationObject: LocationObjectViewModel;
    constructor(
        event: PoliceEventViewModel,
        geoJsonWrapper?: GeoJsonWrapper,
        locationObject?: LocationObjectViewModel
    ) {
        super();
        this.event = event;
        this.geoJsonWrapper = geoJsonWrapper;
        this.locationObject = locationObject;
    }

    get Type(): string {
        return 'ShowMapMessage';
    }
}
