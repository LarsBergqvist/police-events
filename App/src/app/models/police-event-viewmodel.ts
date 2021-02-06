import { GeoPosition } from '../view-models/geo-position';

export interface PoliceEventViewModel {
    id: number;
    datetime: Date;
    summary: string;
    url: string;
    type: string;
    location: {
        name: string;
        pos: GeoPosition;
        distance: number;
    };
}
