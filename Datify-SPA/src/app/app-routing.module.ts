import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { MessagesComponent } from './messages/messages.component';
import { MatchesListComponent } from './matches-list/matches-list.component';
import { HomeComponent } from './home/home.component';
import { LikesListComponent } from './likes-list/likes-list.component';

const routes: Routes = [
  { path: 'home', component: HomeComponent },
  { path: 'matches', component: MatchesListComponent },
  { path: 'likes', component: LikesListComponent },
  { path: 'messages', component: MessagesComponent },
  { path: '**', redirectTo: 'home', pathMatch: 'full' }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
