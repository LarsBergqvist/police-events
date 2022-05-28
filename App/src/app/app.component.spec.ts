import { TestBed, waitForAsync } from '@angular/core/testing';
import { RouterTestingModule } from '@angular/router/testing';
import { AppComponent } from './app.component';
import { CUSTOM_ELEMENTS_SCHEMA } from '@angular/core';
import { MessageService } from 'primeng/api';

describe('AppComponent', () => {
    let service: any;
    let messageService: any;

    beforeEach(
        waitForAsync(() => {
            service = {
                fetchBaseData: jasmine.createSpy('fetchBaseData')
            };
            TestBed.configureTestingModule({
                imports: [RouterTestingModule],
                declarations: [AppComponent],
                schemas: [CUSTOM_ELEMENTS_SCHEMA],
                providers: [{ provide: MessageService, useValue: messageService }]
            }).compileComponents();
        })
    );

    it('should create the app', () => {
        const fixture = TestBed.createComponent(AppComponent);
        const app = fixture.debugElement.componentInstance;
        expect(app).toBeTruthy();
    });

    it(`should have as title 'police-events'`, () => {
        const fixture = TestBed.createComponent(AppComponent);
        const app = fixture.debugElement.componentInstance;
        expect(app.title).toEqual('police-events');
    });
});
