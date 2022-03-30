export default class Format {
    private constructor() {}

    public static minutes(minutes: any): string {
        if (typeof minutes !== 'string' && typeof minutes !== 'number') {
            return '';
        }
        let internalMinutes: number;
        if (typeof minutes === 'string') {
            // tslint:disable-next-line:radix
            internalMinutes = parseInt(minutes);
        } else {
            internalMinutes = minutes;
        }

        const hours =
            internalMinutes / 60 < 1 ? 0 : Math.floor(internalMinutes / 60);
        internalMinutes = internalMinutes - hours * 60;
        return (
            hours +
            ':' +
            (internalMinutes < 10 ? '0' + internalMinutes : internalMinutes)
        );
    }
}
