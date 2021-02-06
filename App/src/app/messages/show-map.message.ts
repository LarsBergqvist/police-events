import { GeoPosition } from '../view-models/geo-position';
import { Message } from './message';

export class ShowMapMessage extends Message {
    positions: GeoPosition[];
    userPos: GeoPosition;
    title: string;
    details: string;
    geoJson: any;
    constructor(positions: GeoPosition[], userPos: GeoPosition, title: string, details?: string, geoJson?: any) {
        super();
        this.positions = positions;
        this.userPos = userPos;
        this.title = title;
        this.details = details;
        this.geoJson = geoJson;
    }

    get Type(): string {
        return 'ShowMapMessage';
    }
}
