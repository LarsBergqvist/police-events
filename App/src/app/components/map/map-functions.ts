import { click } from 'ol/events/condition';
import Feature from 'ol/Feature';
import { Icon, Style } from 'ol/style';

export function styleUser(feature: Feature) {
    if (!feature) return;

    feature.setStyle(
        new Style({
            image: new Icon({
                src: 'assets/user.png',
                imgSize: [60, 60],
                anchor: [0.5, 1],
                opacity: 0.7,
                scale: 0.5
            })
        })
    );
}
