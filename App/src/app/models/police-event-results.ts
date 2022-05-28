import { PoliceEvent } from "./police-event";

export interface PoliceEventsResult {
    totalPages: number;
    page: number;
    events: PoliceEvent[];
}