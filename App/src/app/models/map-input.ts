import { GeoPosition } from './geo-position';
import { GeoJsonWrapper } from './geojson-wrapper';
import { LocationObjectViewModel } from './location-object-viewmodel';

export class MapInput {
    centerPos: GeoPosition;
    geoJsonWrapper: GeoJsonWrapper;
    locationObject: LocationObjectViewModel;
}
