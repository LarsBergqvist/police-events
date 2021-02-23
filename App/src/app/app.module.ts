import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { APP_INITIALIZER, NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { BrowserModule } from '@angular/platform-browser';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { ServiceWorkerModule } from '@angular/service-worker';
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
import { EventListComponent } from './components/event-list/event-list.component';
import { EventComponent } from './components/event-list/event.component';
import { MapSidebarComponent } from './components/map/map-sidebar.component';
import { MapComponent } from './components/map/map.component';
import { AppConfigService } from './services/app-config.service';
import { HttpInterceptorService } from './services/http-interceptor.service';
import { LoggingService } from './services/logging.service';
import { MessageBrokerService } from './services/message-broker.service';

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
        ProgressSpinnerModule,
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
