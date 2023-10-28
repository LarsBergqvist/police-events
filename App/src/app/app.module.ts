import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { APP_INITIALIZER, NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { BrowserModule } from '@angular/platform-browser';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { ServiceWorkerModule } from '@angular/service-worker';
import { AccordionModule } from 'primeng/accordion';
import { MessageService } from 'primeng/api';
import { ButtonModule } from 'primeng/button';
import { InputTextModule } from 'primeng/inputtext';
import { ProgressSpinnerModule } from 'primeng/progressspinner';
import { SidebarModule } from 'primeng/sidebar';
import { ToastModule } from 'primeng/toast';
import { ToolbarModule } from 'primeng/toolbar';
import { environment } from '../environments/environment';
import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { EventViewComponent } from './components/events/event-view.component';
import { EventComponent } from './components/events/event.component';
import { SearchEventsComponent } from './components/events/search-events.component';
import { MapSidebarComponent } from './components/map/map-sidebar.component';
import { MapComponent } from './components/map/map.component';
import { AppConfigService } from './services/app-config.service';
import { HttpInterceptorService } from './services/http-interceptor.service';
import { LoggingService } from './services/logging.service';
import { MessageBrokerService } from './services/message-broker.service';
import { LOCALE_ID } from '@angular/core';
import '@angular/common/locales/global/sv';

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
    declarations: [
        MapComponent,
        AppComponent,
        MapSidebarComponent,
        SearchEventsComponent,
        EventComponent,
        EventViewComponent
    ],
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
        ProgressSpinnerModule,
        AccordionModule,
        ServiceWorkerModule.register('ngsw-worker.js', {
            enabled: environment.production
        }),
        ServiceWorkerModule.register('ngsw-worker.js', {
          enabled: environment.production,
          // Register the ServiceWorker as soon as the app is stable
          // or after 30 seconds (whichever comes first).
          registrationStrategy: 'registerWhenStable:30000'
        })
    ],
    providers: [
        MessageService,
        LoggingService,
        AppConfigService,
        { provide: LOCALE_ID, useValue: 'sv' },
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
