export function durationToTime(duration: number): Date {
    const t = new Date(1970, 0, 1);
    t.setSeconds(duration);
    return t;
}

export function getUTCDateStringFromLocalDateTime(dateTime: Date): string {
    const utcDateTimeString = dateTime.toISOString();
    const utcDateString = utcDateTimeString.substring(0, 10);
    return utcDateString;
}

export function getDateTimeNDaysFromNow(dateTime: Date, days: number): Date {
    const newDateTime = new Date(dateTime.getTime() + days * 24 * 3600 * 1000);
    return newDateTime;
}

export function getDateFromDateTime(dateTime: Date): Date {
    const date = new Date(dateTime.getTime());
    date.setHours(0);
    date.setMinutes(0);
    date.setSeconds(0);
    date.setMilliseconds(0);
    return date;
}
