import { Component, Input, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { ChatVm } from 'src/app/model/chatVm';
import { CommentVm } from 'src/app/model/commentVm';
import { ProductsService } from 'src/app/services/products.service';
import { UserContextServiceService } from 'src/app/services/user-context-service.service';
import { UserProfileService } from 'src/app/services/user-profile.service';

@Component({
  selector: 'app-user-product-messages',
  templateUrl: './user-product-messages.component.html',
  styleUrls: ['./user-product-messages.component.css']
})
export class UserProductMessagesComponent implements OnInit {

  constructor(private route: ActivatedRoute, 
              private compSvc: UserProfileService, 
              private usrContextSvc: UserContextServiceService,
              private prodSvc: ProductsService) { }

  model: ChatVm;
  @Input() userIdTo: string;
  userIdToForAnswear: string;
  conversationId: number;
  isSiteLoaded: boolean = false;
  
  ngOnInit(): void {

    this.route.queryParams
      .subscribe(params => {

        //use switchmap here
        let userIdFrom = params.userIdFrom;
        this.userIdTo = params.userIdTo;
        let prodId = params.prodId;
        this.conversationId = +params.conversationId;
        this.userIdToForAnswear = this.userIdTo;

        if(this.userIdTo == this.usrContextSvc.UserId){
          this.userIdToForAnswear = params.userIdFrom;
        }

        this.compSvc.GetProductMessages(userIdFrom, this.userIdTo, prodId).subscribe(res => {
          this.model = res;

          //this should be refacrtoreed
          this.prodSvc.GetCountedUnreadMessages().subscribe( res => {
            this.prodSvc.numberOfUnreadMessaes = res;
            
            // this.numberOfUnreadMessages = res;
          });

          this.isSiteLoaded = true;
        });

      });
  }

  commentAdded(comment: CommentVm){
    this.model.Comments.push(comment);
  }
}
