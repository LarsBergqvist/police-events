const sentencesToSkip = ['SOS Alarm', 'SOS', 'SOS alarm', 'Skadeläget', 'Polislagen'];

export function locationQueryFromTextAndAreaName(text: string, areaname: string) {
    const locationWords = locationWordsFromText(text);
    if (locationWords.length === 0) return '';

    let query = '';
    locationWords.forEach((d) => {
        query = query + ' ' + d;
    });

    if (locationWords.length === 1 && query.toLowerCase().indexOf(areaname.toLowerCase()) > -1) {
        // only area name in text, don't use as query
        return '';
    }

    if (query.toLowerCase().indexOf(areaname.toLowerCase()) < 0) {
        // use area name in query if it is not already present
        query = query + ' ' + areaname;
    }

    return query.trim();
}

export function locationWordsFromText(text: string): string[] {
    // Remove any special word combinations
    sentencesToSkip.forEach((s: string) => {
        text = text.replace(s, '');
    });
    let res: string[] = [];
    // Remove the first word of any secondary sentences
    text = text.replace(/\.\s*[A-ZÅÄÖ0-9].[A-Za-zÅÄÖåäö0-9]*/g, '');
    // Split words concatenated with /
    text = text.replace('/', ', ');
    let words = text.split(/[\s"]+/);
    for (let i = 0; i < words.length; i++) {
        let w = words[i];
        if (w === '' || w.length < 2) continue;
        if (i === 0) {
            // Use first word in first sentence if it ends with a comma
            if (w.endsWith(',') && startsWithUpperCase(w)) {
                res.push(w.slice(0, -1));
            }
        } else {
            w = w.replace(/[\s,."]+/, '');
            if (startsWithUpperCase(w)) {
                res.push(w);
            }
        }
    }

    return res;
}

export function startsWithUpperCase(w: string): boolean {
    if (w[0].match(/^\d+$/)) return false;
    if (w[0] === w[0].toUpperCase()) {
        return true;
    }
}
