import { BrowserModule } from '@angular/platform-browser';
import { APP_INITIALIZER, NgModule } from '@angular/core';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { ToolbarModule } from 'primeng/toolbar';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { SidebarModule } from 'primeng/sidebar';
import { ServiceWorkerModule } from '@angular/service-worker';
import { environment } from '../environments/environment';
import { ToastModule } from 'primeng/toast';
import { ButtonModule } from 'primeng/button';
import { MessageService } from 'primeng/api';
import { HttpInterceptorService } from './services/http-interceptor.service';
import { LoggingService } from './services/logging.service';
import { MessageBrokerService } from './services/message-broker.service';
import { InputTextModule } from 'primeng/inputtext';
import { MapComponent } from './components/map/map.component';
import { FormsModule } from '@angular/forms';
import { MapSidebarComponent } from './components/map/map-sidebar.component';
import { EventListComponent } from './components/event-list/event-list.component';
import { EventComponent } from './components/event-list/event.component';
import { AppConfigService } from './services/app-config.service';

export function appConfigInit(configService: AppConfigService, logging: LoggingService) {
    return () => {
        return new Promise<void>((resolve) => {
            configService.load().then(() => {
                resolve();
            });
        });
    };
}

@NgModule({
    declarations: [MapComponent, AppComponent, MapSidebarComponent, EventListComponent, EventComponent],
    imports: [
        BrowserModule,
        AppRoutingModule,
        HttpClientModule,
        ToolbarModule,
        BrowserAnimationsModule,
        SidebarModule,
        ToastModule,
        ButtonModule,
        FormsModule,
        InputTextModule,
        ServiceWorkerModule.register('ngsw-worker.js', {
            enabled: environment.production
        })
    ],
    providers: [
        MessageService,
        LoggingService,
        AppConfigService,
        {
            provide: APP_INITIALIZER,
            useFactory: appConfigInit,
            multi: true,
            deps: [AppConfigService, LoggingService]
        },
        {
            provide: HTTP_INTERCEPTORS,
            useClass: HttpInterceptorService,
            multi: true,
            deps: [MessageBrokerService, LoggingService]
        }
    ],
    bootstrap: [AppComponent]
})
export class AppModule {}
