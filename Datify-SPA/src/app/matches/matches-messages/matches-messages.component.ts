import { Component, OnInit, Input } from '@angular/core';
import { Message } from './../../_models/message';
import { UserService } from './../../_services/user.service';
import { AuthService } from './../../_services/auth.service';
import { AlertifyService } from './../../_services/alertify.service';

@Component({
  selector: 'app-matches-messages',
  templateUrl: './matches-messages.component.html',
  styleUrls: ['./matches-messages.component.css']
})
export class MatchesMessagesComponent implements OnInit {
  @Input() recipientId: number;
  @Input() username: string;
  messages: Message[];

  constructor(private userService: UserService, private authService: AuthService, private alertify: AlertifyService) { }

  ngOnInit() {
    this.loadMessages();
  }

  loadMessages() {
    this.userService.getMessageThread(this.authService.decodedToken.nameid, this.recipientId).subscribe((messages) => {
      this.messages = messages;
    }, error => {
      this.alertify.error(error);
    });
  }

}
