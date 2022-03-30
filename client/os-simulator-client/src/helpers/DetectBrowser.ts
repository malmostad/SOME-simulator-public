export default class DetectBrowser {
    ua: string;

    constructor() {
        this.ua = navigator.userAgent.toLocaleLowerCase();
    }

    public isSafari(): boolean {
        return this.ua.indexOf('safari') !== -1 && this.ua.indexOf('chrome') < 0;
    }

    public isChrome(): boolean {
        return this.ua.indexOf('chrome') !== -1;
    }

    public isFirefox(): boolean {
        return this.ua.indexOf('firefox') !== -1;
    }
}
