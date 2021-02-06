import { Component, EventEmitter, Input, Output } from '@angular/core';
import GeoJSON from 'ol/format/GeoJSON';
import { Attribution, defaults as defaultControls } from 'ol/control';
import Feature from 'ol/Feature';
import Point from 'ol/geom/Point';
import { Tile as TileLayer, Vector as VectorLayer } from 'ol/layer';
import Map from 'ol/Map';
import { fromLonLat } from 'ol/proj.js';
import OSM from 'ol/source/OSM';
import VectorSource from 'ol/source/Vector';
import Vector from 'ol/source/Vector';
import View from 'ol/View';
import { GeoPosition } from 'src/app/view-models/geo-position';
import { styleUser } from './map-functions';
import { Fill, Stroke, Style } from 'ol/style';

export class MapInput {
    markerPositions: GeoPosition[];
    userPos: GeoPosition;
    geoJson: {};
    showCounty = false;
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

    private static readonly ZoomLevelMunicipality = 9;
    private static readonly ZoomLevelCounty = 6;
    private userMarker: Feature;
    private positions: GeoPosition[];
    private userPos: GeoPosition;

    @Input('mapInput') set setInputData(input: MapInput) {
        if (!input) return;
        this.setupMap(input);
    }
    @Output() onMarkerClicked = new EventEmitter<number>();

    private setupMap(input: MapInput) {
        this.positions = input.markerPositions;
        this.userPos = input.userPos;
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
        if (this.positions.length > 0) {
            view.setCenter(fromLonLat([this.positions[0].lng, this.positions[0].lat]));
        } else if (this.userPos && this.userPos.lat && this.userPos.lng) {
            view.setCenter(fromLonLat([this.userPos.lng, this.userPos.lat]));
        }

        const zoomLevel = this.showCounty ? MapComponent.ZoomLevelCounty : MapComponent.ZoomLevelMunicipality;
        view.setZoom(zoomLevel);

        this.updateUserMarker(this.userPos);
    }

    private updateUserMarker(userPos: GeoPosition) {
        this.userPos = userPos;
        this.userMarker.setGeometry(null);
        if (!userPos) return;

        this.userMarker.setGeometry(new Point(fromLonLat([userPos.lng, userPos.lat])));
        styleUser(this.userMarker);
    }

    private initilizeMap(): void {
        this.userMarker = new Feature();

        //
        // Create a map with an OpenStreetMap-layer,
        // a marker layer and a view
        var attribution = new Attribution({
            // Attach the attribution information
            // to an element outside of the map
            target: 'attribution'
        });
        this.geoJsonVectorSource = new VectorSource({});
        let geoJsonLayer = new VectorLayer({
            source: this.geoJsonVectorSource,
            style: new Style({
                stroke: new Stroke({
                    color: 'blue',
                    lineDash: [4],
                    width: 3
                }),
                fill: new Fill({
                    color: 'rgba(0, 0, 255, 0.1)'
                })
            })
        });
        this.map = new Map({
            controls: defaultControls({ attribution: false }).extend([attribution]),
            target: 'map',
            layers: [
                new TileLayer({ source: new OSM() }),
                new VectorLayer({
                    source: new Vector({ features: [this.userMarker] })
                }),
                geoJsonLayer
            ],
            view: new View({
                center: fromLonLat([0, 0]),
                zoom: MapComponent.ZoomLevelMunicipality
            })
        });
    }
}
