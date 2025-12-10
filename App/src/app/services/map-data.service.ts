import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';
import { MapInput } from '../models/map-input';

@Injectable({
    providedIn: 'root'
})
export class MapDataService {
    private mapInput: MapInput | null = null;
    mapInput$ = new BehaviorSubject<MapInput | null>(null);

    addNewMapInput(mapInput: MapInput) {
        this.mapInput = mapInput;
        this.mapInput$.next(this.mapInput);
    }
}
