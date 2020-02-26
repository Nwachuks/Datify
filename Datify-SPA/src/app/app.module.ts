import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { HttpClientModule } from '@angular/common/http';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { JwtModule } from '@auth0/angular-jwt';
import { BsDropdownModule, TabsModule, BsDatepickerModule, PaginationModule, ButtonsModule } from 'ngx-bootstrap';
import { NgxGalleryModule } from 'ngx-gallery';
import { FileUploadModule } from 'ng2-file-upload';
import { TimeAgoPipe } from 'time-ago-pipe';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { NavComponent } from './nav/nav.component';
import { HomeComponent } from './home/home.component';
import { RegisterComponent } from './register/register.component';
import { LikesListComponent } from './likes-list/likes-list.component';
import { MessagesComponent } from './messages/messages.component';
import { MatchesListComponent } from './matches/matches-list/matches-list.component';
import { MatchCardComponent } from './matches/match-card/match-card.component';
import { MatchDetailComponent } from './matches/match-detail/match-detail.component';
import { ProfileComponent } from './user/profile/profile.component';
import { PhotoEditorComponent } from './user/photo-editor/photo-editor.component';
import { MatchesMessagesComponent } from './matches/matches-messages/matches-messages.component';

import { AuthService } from './_services/auth.service';
import { AlertifyService } from './_services/alertify.service';
import { ErrorInterceptorProvider } from './_services/error.interceptor';
import { UserService } from './_services/user.service';
import { AuthGuard } from './_guards/auth.guard';
import { PreventUnsavedChangesGuard } from './_guards/prevent-unsaved-changes.guard';
import { MatchDetailResolver } from './_resolvers/match-detail.resolver';
import { MatchListResolver } from './_resolvers/match-list.resolver';
import { ProfileResolver } from './_resolvers/profile.resolver';
import { LikesResolver } from './_resolvers/likes.resolver';
import { MessagesResolver } from './_resolvers/messages.resolver';

export function tokenGetter() {
  return localStorage.getItem('token');
}
@NgModule({
  declarations: [
    AppComponent,
    NavComponent,
    HomeComponent,
    RegisterComponent,
    MatchesListComponent,
    MatchCardComponent,
    MatchDetailComponent,
    LikesListComponent,
    MessagesComponent,
    ProfileComponent,
    PhotoEditorComponent,
    TimeAgoPipe,
    MatchesMessagesComponent
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    HttpClientModule,
    FormsModule,
    ReactiveFormsModule,
    BsDropdownModule.forRoot(),
    BsDatepickerModule.forRoot(),
    PaginationModule.forRoot(),
    ButtonsModule.forRoot(),
    TabsModule.forRoot(),
    NgxGalleryModule,
    FileUploadModule,
    JwtModule.forRoot({
      config: {
        tokenGetter: tokenGetter,
        whitelistedDomains: ['localhost:3000'],
        blacklistedRoutes: ['localhost:3000/auth']
      }
    })
  ],
  providers: [
    AuthService,
    AlertifyService,
    UserService,
    ErrorInterceptorProvider,
    AuthGuard,
    PreventUnsavedChangesGuard,
    MatchDetailResolver,
    MatchListResolver,
    ProfileResolver,
    LikesResolver,
    MessagesResolver
  ],
  bootstrap: [
    AppComponent
  ]
})
export class AppModule { }
