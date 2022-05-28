import { AfterViewInit, Component, OnDestroy } from '@angular/core';
import { Attribution, defaults as defaultControls } from 'ol/control';
import { boundingExtent } from 'ol/extent';
import Feature from 'ol/Feature';
import GeoJSON from 'ol/format/GeoJSON';
import { fromExtent } from 'ol/geom/Polygon';
import { Tile as TileLayer, Vector as VectorLayer } from 'ol/layer';
import Map from 'ol/Map';
import { fromLonLat } from 'ol/proj.js';
import OSM from 'ol/source/OSM';
import VectorSource from 'ol/source/Vector';
import { Fill, Stroke, Style } from 'ol/style';
import View from 'ol/View';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';
import { BoundingBox } from 'src/app/models/boundingbox';
import { MapInput } from 'src/app/models/map-input';
import { MapDataService } from 'src/app/services/map-data.service';

@Component({
    selector: 'app-map',
    templateUrl: './map.component.html',
    styleUrls: ['./map.component.scss']
})
export class MapComponent implements AfterViewInit, OnDestroy {
    map: Map;
    geoJsonVectorSource: VectorSource;
    detailedVectorSource: VectorSource;
    detailedLocation: Feature;

    private static readonly DefaultZoomLevel = 9;
    private static readonly DefaultLatitude = 59.329324;
    private static readonly DefaultLongitude = 18.068581;

    private unsubscribe$ = new Subject();

    constructor(private readonly mapDataService: MapDataService) {}

    ngAfterViewInit(): void {
        if (!this.map) {
            this.initilizeMap();
        }

        this.mapDataService.mapInput$.pipe(takeUntil(this.unsubscribe$)).subscribe((value: MapInput) => {
            if (value) {
                this.updateMap(value);
            }
        });
    }

    ngOnDestroy() {
        this.unsubscribe$.next(undefined);
        this.unsubscribe$.complete();
    }

    private updateMap(input: MapInput) {
        // Clear the previous polygons
        this.detailedLocation.setGeometry(null);
        this.geoJsonVectorSource.clear();

        if (input.geoJsonWrapper) {
            this.geoJsonVectorSource.addFeatures(
                new GeoJSON().readFeatures(input.geoJsonWrapper.geoJson, {
                    dataProjection: 'EPSG:4326',
                    featureProjection: 'EPSG:3857'
                })
            );
        }

        const view = this.map.getView();

        if (input.locationObject?.lat) {
            this.updateDetailedLocation(input?.locationObject?.boundingBox);
            var layerExtent = this.detailedVectorSource.getExtent();
            if (layerExtent) {
                view.fit(layerExtent);
            }
            view.setCenter(fromLonLat([input.locationObject.lng, input.locationObject.lat]));
        } else {
            var layerExtent = this.geoJsonVectorSource.getExtent();
            if (layerExtent) {
                view.fit(layerExtent);
            }
            view.setCenter(fromLonLat([input.centerPos.lng, input.centerPos.lat]));
        }
    }

    private updateDetailedLocation(boundingBox: BoundingBox) {
        if (!boundingBox) return;

        const bbox = boundingBox;
        const be = boundingExtent([
            [bbox.lngMin, bbox.latMin],
            [bbox.lngMax, bbox.latMax]
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
                    color: 'rgba(0, 0, 255, 0.05)'
                })
            })
        });

        this.detailedLocation = new Feature();
        this.detailedVectorSource = new VectorSource({ features: [this.detailedLocation] });
        this.map = new Map({
            controls: defaultControls({ attribution: false }).extend([attribution]),
            target: 'map',
            layers: [
                new TileLayer({ source: new OSM() }),
                geoJsonLayer,
                new VectorLayer({
                    source: this.detailedVectorSource,
                    style: new Style({
                        stroke: new Stroke({
                            color: 'red',
                            lineDash: [1],
                            width: 2
                        }),
                        fill: new Fill({
                            color: 'rgba(200, 200, 200, 0)'
                        })
                    })
                })
            ],
            view: new View({
                center: fromLonLat([MapComponent.DefaultLongitude, MapComponent.DefaultLatitude]),
                zoom: MapComponent.DefaultZoomLevel
            })
        });
    }
}
