import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';

import { MessagesComponent } from './messages/messages.component';
import { MatchesListComponent } from './matches/matches-list/matches-list.component';
import { MatchDetailComponent } from './matches/match-detail/match-detail.component';
import { HomeComponent } from './home/home.component';
import { LikesListComponent } from './likes-list/likes-list.component';
import { ProfileComponent } from './user/profile/profile.component';

import { AuthGuard } from './_guards/auth.guard';
import { MatchDetailResolver } from './_resolvers/match-detail.resolver';
import { MatchListResolver } from './_resolvers/match-list.resolver';
import { ProfileResolver } from './_resolvers/profile.resolver';
import { LikesResolver } from './_resolvers/likes.resolver';
import { PreventUnsavedChangesGuard } from './_guards/prevent-unsaved-changes.guard';
import { MessagesResolver } from './_resolvers/messages.resolver';

const routes: Routes = [
  { path: '', component: HomeComponent },
  {
    path: '',
    runGuardsAndResolvers: 'always',
    canActivate: [AuthGuard],
    children: [
      { path: 'matches', component: MatchesListComponent, resolve: { users: MatchListResolver } },
      { path: 'matches/:id', component: MatchDetailComponent, resolve: { user: MatchDetailResolver } },
      {
        path: 'profile', component: ProfileComponent, resolve: { user: ProfileResolver },
        canDeactivate: [PreventUnsavedChangesGuard]
      },
      { path: 'likes', component: LikesListComponent, resolve: { users: LikesResolver } },
      { path: 'messages', component: MessagesComponent, resolve: { messages: MessagesResolver } },
    ]
  },
  { path: '**', redirectTo: '', pathMatch: 'full' }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
