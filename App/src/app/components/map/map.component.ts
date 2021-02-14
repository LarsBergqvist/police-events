import { Component, Input } from '@angular/core';
import GeoJSON from 'ol/format/GeoJSON';
import { Attribution, defaults as defaultControls } from 'ol/control';
import { Tile as TileLayer, Vector as VectorLayer } from 'ol/layer';
import Map from 'ol/Map';
import { fromLonLat } from 'ol/proj.js';
import OSM from 'ol/source/OSM';
import VectorSource from 'ol/source/Vector';
import View from 'ol/View';
import { GeoPosition } from 'src/app/models/geo-position';
import { Fill, Stroke, Style } from 'ol/style';
import { LocationObjectViewModel } from 'src/app/models/location-object-viewmodel';
import Feature from 'ol/Feature';
import { boundingExtent } from 'ol/extent';
import { fromExtent } from 'ol/geom/Polygon';

export class MapInput {
    centerPos: GeoPosition;
    geoJson: {};
    showCounty = false;
    locationObject: LocationObjectViewModel;
}

@Component({
    selector: 'app-map',
    templateUrl: './map.component.html',
    styleUrls: ['./map.component.scss']
})
export class MapComponent {
    map: Map;
    geoJsonVectorSource: VectorSource;
    showCounty = false;
    detailedLocation: Feature;

    private static readonly ZoomLevelObject = 13;
    private static readonly ZoomLevelMunicipality = 9;
    private static readonly ZoomLevelCounty = 6;
    private static readonly DefaultLatitude = 59.329324;
    private static readonly DefaultLongitude = 18.068581;

    @Input('mapInput') set setInputData(input: MapInput) {
        if (!input) return;
        this.updateMap(input);
    }

    private updateMap(input: MapInput) {
        this.showCounty = input.showCounty;

        if (!this.map) {
            this.initilizeMap();
        }

        if (input.geoJson) {
            this.geoJsonVectorSource.clear();
            this.geoJsonVectorSource.addFeatures(
                new GeoJSON().readFeatures(input.geoJson, {
                    dataProjection: 'EPSG:4326',
                    featureProjection: 'EPSG:3857'
                })
            );
        }

        const view = this.map.getView();
        if (input.locationObject?.lat) {
            view.setCenter(fromLonLat([input.locationObject.lon, input.locationObject.lat]));
        } else {
            view.setCenter(fromLonLat([input.centerPos.lng, input.centerPos.lat]));
        }

        let zoomLevel = this.showCounty ? MapComponent.ZoomLevelCounty : MapComponent.ZoomLevelMunicipality;
        if (input.locationObject) {
            zoomLevel = MapComponent.ZoomLevelObject;
        }
        view.setZoom(zoomLevel);

        this.updateDetailedLocation(input.locationObject);
    }

    private updateDetailedLocation(locationObject: LocationObjectViewModel) {
        this.detailedLocation.setGeometry(null);
        if (!locationObject) return;

        const bbox = locationObject.boundingBox;
        const be = boundingExtent([
            [bbox.lon1, bbox.lat1],
            [bbox.lon2, bbox.lat2]
        ]);
        const locationPolygon = fromExtent(be).clone().transform('EPSG:4326', 'EPSG:3857');
        this.detailedLocation.setGeometry(locationPolygon);
    }

    private initilizeMap(): void {
        //
        // Create a map with an OpenStreetMap-layer,
        // a geoJson-polygon layer, a detailed location layer and a view
        //
        const attribution = new Attribution({
            // Attach the attribution information
            // to an element outside of the map
            target: 'attribution'
        });
        this.geoJsonVectorSource = new VectorSource({});
        const geoJsonLayer = new VectorLayer({
            source: this.geoJsonVectorSource,
            style: new Style({
                stroke: new Stroke({
                    color: 'blue',
                    lineDash: [1],
                    width: 2
                }),
                fill: new Fill({
                    color: 'rgba(0, 0, 255, 0.1)'
                })
            })
        });

        this.detailedLocation = new Feature();
        this.map = new Map({
            controls: defaultControls({ attribution: false }).extend([attribution]),
            target: 'map',
            layers: [
                new TileLayer({ source: new OSM() }),
                geoJsonLayer,
                new VectorLayer({
                    source: new VectorSource({ features: [this.detailedLocation] }),
                    style: new Style({
                        stroke: new Stroke({
                            color: 'red',
                            lineDash: [5],
                            width: 2
                        }),
                        fill: new Fill({
                            color: 'rgba(200, 2000, 200, 0.2)'
                        })
                    })
                })
            ],
            view: new View({
                center: fromLonLat([MapComponent.DefaultLongitude, MapComponent.DefaultLatitude]),
                zoom: MapComponent.ZoomLevelMunicipality
            })
        });
    }
}
