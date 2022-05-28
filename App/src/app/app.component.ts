import { Component, OnInit, OnDestroy } from '@angular/core';
import { MessageService } from 'primeng/api';
import { MessageBrokerService } from './services/message-broker.service';
import { filter, takeUntil } from 'rxjs/operators';
import { SuccessInfoMessage } from './messages/success-info.message';
import { Subject } from 'rxjs';
import { ErrorOccurredMessage } from './messages/error-occurred.message';

@Component({
    selector: 'app-root',
    templateUrl: './app.component.html',
    styleUrls: ['./app.component.scss']
})
export class AppComponent implements OnInit, OnDestroy {
    title = 'police-events';
    isLoading = false;
    private unsubscribe$ = new Subject();

    constructor(
        private readonly broker: MessageBrokerService,
        private readonly primeNGmessageService: MessageService
    ) {}

    async ngOnInit() {
        const messages = this.broker.getMessage();
        messages
            .pipe(
                takeUntil(this.unsubscribe$),
                filter((message) => message instanceof SuccessInfoMessage)
            )
            .subscribe((message: SuccessInfoMessage) => {
                this.primeNGmessageService.add({ severity: 'success', summary: '', detail: message.info });
            });
        messages
            .pipe(
                takeUntil(this.unsubscribe$),
                filter((message) => message instanceof ErrorOccurredMessage)
            )
            .subscribe((message: ErrorOccurredMessage) => {
                this.primeNGmessageService.add({ severity: 'error', summary: '', detail: message.errorMessage });
            });

        await this.fetchBaseData();
    }

    async fetchBaseData() {
        try {
            this.isLoading = true;
        } finally {
            this.isLoading = false;
        }
    }

    ngOnDestroy(): void {
        this.unsubscribe$.next(undefined);
        this.unsubscribe$.complete();
    }
}
