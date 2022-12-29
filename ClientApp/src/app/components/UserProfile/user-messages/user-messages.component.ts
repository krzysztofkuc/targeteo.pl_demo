import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { CommentVm } from 'src/app/model/commentVm';
import { UserContextServiceService } from 'src/app/services/user-context-service.service';
import { UserProfileService } from 'src/app/services/user-profile.service';

@Component({
  selector: 'app-user-profile',
  templateUrl: './user-messages.component.html',
  styleUrls: ['./user-messages.component.css']
})
export class UserMessagesComponent implements OnInit {

  model:CommentVm[][];
  isSiteLoaded: boolean = false;

  constructor(private compSvc: UserProfileService, 
              private usrContext: UserContextServiceService,
              private router: Router) { }

  ngOnInit(): void {
    this.compSvc.Get(this.usrContext.Login).subscribe(res => {
      res.forEach(element => {
        element[0].UnreadMessages = element.filter(x => x.Viewed == false).length;
      });
      this.model = res;

      this.isSiteLoaded = true;
    });
  }

  rowClick(row){
    this.router.navigate(['/user-profile/user-product-messages'], { queryParams: {userIdFrom: row.UserIdFrom, userIdTo: row.UserIdTo, prodId: row.ProductId, conversationId: this.model[0][0].ConversationId} });
  }

}
