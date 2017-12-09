export class Mscc {
    cookieName: string = 'MSCC';
    interactiveConsentEnabled: boolean = true;
    version: string = '0.3.6';

    private bannerElement: any;
    private bannerShownClass: any = 'active';
    private hasLogged: boolean = false;
    private events: any[] = [];
    private hostname: string = window.location.hostname;
    private learnMoreLinkElement: any;
    private shouldLog: boolean = true;
    private telemetryUrl = 'https://uhf.microsoft.com/_log';

    constructor(refBanner: any, refLearnMore: any) {
        this.bannerElement = refBanner;
        this.learnMoreLinkElement = refLearnMore;

        this.initDomNeeded();
    }

    hasConsent(): boolean {
        return !!this.getCookie(this.cookieName);
    }

    isVisible(): boolean {
        return this.bannerElement != undefined && this.hasClass(this.bannerElement, this.bannerShownClass);
    }

    setConsent(): void {
        let hasConsentNow = this.hasConsent();
        if (this.bannerElement) {
            this.hideBanner();
        }
        if (!hasConsentNow) {
            this.setCookieOnRootDomain(this.cookieName, '' + this.nsToMs(Date.now()), 390);
            this.emit('consent');
        }
    }

    private addClass(el: any, className: string): void {
        if (el.classList) {
            el.classList.add(className);
        } else {
            el.className += ' ' + className;
        }
    }

    private emit(name: any): void {
        let args: any[] = [];

        for (let i = 1; i < arguments.length; i++) {
            args[i - 1] = arguments[i];
        }

        if (!this.events[name]) {
            return;
        }

        this.events[name].forEach((x: any) => {
            x.apply(null, args);
        });
    }

    private getCookie(name: string): string {
        if (!!name) {
            for (let i = 0, a = document.cookie.split('; '); i < a.length; i++) {
                let cookie = a[i];
                let delimiterIndex = cookie.indexOf('=');
                let cookieName_1 = cookie.substring(0, delimiterIndex);
                if (cookieName_1 === name) {
                    return cookie.substring(cookieName_1.length + 1);
                }
            }
        }
        return null;
    }

    private hasClass(el: any, className: string): boolean {
        if (el.classList) {
            return el.classList.contains(className);
        } else {
            return new RegExp('(^| )' + className + '( |$)', 'gi').test(el.className);
        }
    }

    private hideBanner(): void {
        this.removeClass(this.bannerElement, this.bannerShownClass);
        this.emit('hide');
    }

    private initDomNeeded(): void {
        let hasConsentNow: boolean = this.hasConsent();
        if (this.bannerElement && !hasConsentNow && !this.isVisible()) {
            this.showBanner();
        }
        if (this.bannerElement && hasConsentNow && this.isVisible()) {
            this.hideBanner();
        }
    }

    private log(message: string): void {
        if (!this.shouldLog || this.hasLogged) {
            return;
        }
        let image: any = new Image();
        let siteName: any = this.bannerElement.getAttribute('data-site-name');
        let nugetVersion: any = this.bannerElement.getAttribute('data-nver');
        let settingsVersion: any = this.bannerElement.getAttribute('data-sver');
        let siteParam: string = !!siteName ? '&s=' + siteName : '';
        let nugetParam: string = !!nugetVersion ? '&nv=' + nugetVersion : '';
        let settingsParam: string = !!settingsVersion ? '&sv=' + settingsVersion : '';
        let messageParam: string = !!message ? '&m=' + message : '';
        image.src = this.telemetryUrl + '?o=mscc' + siteParam + messageParam + nugetParam + settingsParam;
        this.hasLogged = true;
    }

    private nsToMs(nanoSeconds: any): any {
        return Math.floor(nanoSeconds / 1000);
    }

    private removeClass(el: any, className: string): void {
        if (el.classList) {
            el.classList.remove(className);
        } else {
            el.className = el.className.replace(new RegExp('(^|\\b)' + className.split(' ').join('|') + '(\\b|$)', 'gi'), ' ');
        }
    }

    private setCookieOnRootDomain(name: string, value: string, expiryDays: any): void {
        let expiryDate = new Date();
        expiryDate.setDate(expiryDate.getDate() + expiryDays);
        let domainParts = this.hostname.split('.'), testDomain = domainParts.pop();
        if (testDomain == 'localhost') {
            document.cookie = name + '=' + value + ';expires=' + expiryDate.toUTCString() + ';path=/';
        } else {
            while (this.getCookie(name) !== value && domainParts.length !== 0) {
                testDomain = domainParts.pop() + '.' + testDomain;
                document.cookie = name + '=' + value + ';expires=' + expiryDate.toUTCString() + ';path=/;domain=.' + testDomain;
            }
        }
    }

    private showBanner(): void {
        this.addClass(this.bannerElement, this.bannerShownClass);
        this.emit('show');
        this.log('show');
    }
}