import { GeoPosition } from './geo-position';

export interface PoliceEventViewModel {
    id: number;
    datetime: Date;
    summary: string;
    url: string;
    type: string;
    details: string;
    location: {
        name: string;
        pos: GeoPosition;
        distance: number;
    };
}
