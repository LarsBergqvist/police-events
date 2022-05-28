import { locationQueryFromTextAndAreaName, locationWordsFromText } from './word-query-heuristics';

describe('word-query-heuristics', () => {
    describe('locationWordsFromText', () => {
        it('Should not include first word', () => {
            const text = 'First Plats Plats2 ingen';
            const words = locationWordsFromText(text);
            expect(words.length).toBe(2);
            expect(words).toContain('Plats');
            expect(words).toContain('Plats2');
        });
        it('Should not include one-letter words', () => {
            const text = 'First P P2 ingen';
            const words = locationWordsFromText(text);
            expect(words.length).toBe(1);
            expect(words).toContain('P2');
        });
        it('Should not include reserved words', () => {
            const text = 'First Skadeläget Polislagen SOS Alarm';
            const words = locationWordsFromText(text);
            expect(words.length).toBe(0);
        });
        it('Should not include words starting with number', () => {
            const text = 'First 5apa';
            const words = locationWordsFromText(text);
            expect(words.length).toBe(0);
        });
        it('Should not include first word of next sentence', () => {
            const text = 'First Plats Plats2.  Ingen apa på Vägen. Ingen varg på Gatan.Ingen';
            const words = locationWordsFromText(text);
            expect(words.length).toBe(4);
            expect(words).toContain('Plats');
            expect(words).toContain('Plats2');
            expect(words).toContain('Gatan');
            expect(words).toContain('Vägen');
        });
    });
    describe('locationQueryFromTextAndAreaName', () => {
        it('Should append area name to query if not present in text', () => {
            const text = 'First olycka på Hemvägen. Ingen skadad.';
            const areaname = 'Kommun1';
            const query = locationQueryFromTextAndAreaName(text, areaname);
            expect(query).toEqual('Hemvägen Kommun1');
        });
        it('Should not append area name to query if already not present in text', () => {
            const text = 'First olycka på Hemvägen i Kommun1. Ingen skadad.';
            const areaname = 'Kommun1';
            const query = locationQueryFromTextAndAreaName(text, areaname);
            expect(query).toEqual('Hemvägen Kommun1');
        });
        it('Should return empty query if no location words in text', () => {
            const text = 'First olycka. Ingen skadad.';
            const areaname = 'Kommun1';
            const query = locationQueryFromTextAndAreaName(text, areaname);
            expect(query).toEqual('');
        });
        it('Should return empty query if text only produces same word as area name', () => {
            const text = 'First olycka i Kommun1. Ingen skadad.';
            const areaname = 'Kommun1';
            const query = locationQueryFromTextAndAreaName(text, areaname);
            expect(query).toEqual('');
        });
        it('Should handle Swedish characters in first word in sentence', () => {
            const text = 'Samtal befarad skottlossning i centrala Ställe. Två personer påträffade.';
            const areaname = 'Kommun1';
            const query = locationQueryFromTextAndAreaName(text, areaname);
            expect(query).toEqual('Ställe Kommun1');
        });
    });
});
