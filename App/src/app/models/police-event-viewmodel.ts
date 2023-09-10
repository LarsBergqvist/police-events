import { GeoPosition } from './geo-position';

export interface PoliceEventViewModel {
    id: number;
    name: string;
    datetime: Date;
    summary: string;
    url: string;
    type: string;
    details: string;
    description: string;
    location: {
        name: string;
        pos: GeoPosition;
    };
}
