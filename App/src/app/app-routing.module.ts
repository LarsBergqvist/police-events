import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { EventViewComponent } from './components/events/event-view.component';
import { SearchEventsComponent } from './components/events/search-events.component';

const routes: Routes = [
    { path: 'event/:id', component: EventViewComponent },
    { path: '**', component: SearchEventsComponent }
];

@NgModule({
    imports: [RouterModule.forRoot(routes, {})],
    exports: [RouterModule]
})
export class AppRoutingModule {}
