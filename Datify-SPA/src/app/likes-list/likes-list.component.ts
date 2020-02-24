import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { User } from '../_models/user';
import { Pagination, PaginatedResult } from '../_models/pagination';
import { UserService } from './../_services/user.service';
import { AuthService } from '../_services/auth.service';
import { AlertifyService } from '../_services/alertify.service';

@Component({
  selector: 'app-likes-list',
  templateUrl: './likes-list.component.html',
  styleUrls: ['./likes-list.component.css']
})
export class LikesListComponent implements OnInit {
  users: User[];
  pagination: Pagination;
  likesParam: string;

  constructor(private userService: UserService, private authService: AuthService, private alertify: AlertifyService,
    private route: ActivatedRoute) { }

  ngOnInit() {
    this.route.data.subscribe(data => {
      this.users = data['users'].result;
      this.pagination = data['users'].pagination;
    });
    this.likesParam = 'Likers';
  }

  pageChanged(event: any): void {
    this.pagination.currentPage = event.page;
    this.loadUsers();
  }

  loadUsers() {
    this.userService.getUsers(this.pagination.currentPage, this.pagination.itemsPerPage, null, this.likesParam)
      .subscribe((res: PaginatedResult<User[]>) => {
      this.users = res.result;
      this.pagination = res.pagination;
    }, error => {
      this.alertify.error(error);
    });
  }

}
