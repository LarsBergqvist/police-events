import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { AppConfig } from './app-config.js';
import { LoggingService } from './logging.service';

@Injectable()
export class AppConfigService {
    readonly configFile = 'assets/app-config.json';
    config: AppConfig;

    constructor(private readonly http: HttpClient, private readonly logging: LoggingService) {}

    async load(): Promise<any> {
        const configPath = this.configFile;
        return new Promise<void>(async (resolve) => {
            let res = await this.http.get<AppConfig>(configPath).toPromise();
            this.logging.logInfo('loaded app-config.json');
            this.config = res;
            resolve();
        });
    }

    get apiUrl(): string {
        return this.config.apiUrl;
    }

    get nominatimUrl(): string {
        return this.config.nominatimUrl;
    }

    get municipalityUrl(): string {
        return this.config.municipalityUrl;
    }

    get countyUrl(): string {
        return this.config.countyUrl;
    }
}
