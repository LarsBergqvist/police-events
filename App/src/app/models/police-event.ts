export interface PoliceEvent {
    id: number;
    utcDateTime: Date;
    summary: string;
    url: string;
    type: string;
    details: string;
    description: string;
    location: {
        name: string;
        lat: number;
        lng: number;
    };
}
