import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { CommentVm } from 'src/app/model/commentVm';
import { MsgToUserVm } from 'src/app/model/msgToUserVm';
import { ProductVm } from 'src/app/model/productVm';
import { ProductsService } from 'src/app/services/products.service';
import { UserContextServiceService } from 'src/app/services/user-context-service.service';

@Component({
  selector: 'app-ask-question-about-product',
  templateUrl: './ask-question-about-product.component.html',
  styleUrls: ['./ask-question-about-product.component.css']
})
export class AskQuestionAboutProductComponent implements OnInit {
  
  model: MsgToUserVm;
  @Input() product: ProductVm;
  @Input() userIdTo: string;
  @Input() conversationId: number;
  @Output() commentAddedEvent = new EventEmitter<CommentVm>();
  
  // @Input() isAnswear: boolean;
  form: FormGroup;
  msg: string;
  displayLoginDialog: boolean = false;
  isSending: boolean = false;
  // @Input() firstMessageFromUserId: string;
  // prodId: string;

  constructor(private productService: ProductsService,
              private route: ActivatedRoute,
              private userSvc: UserContextServiceService,
              private toastr: ToastrService,
              private authSvc: UserContextServiceService,
              private fb: FormBuilder) { }

  ngOnInit(): void {

    this.model = new MsgToUserVm();
    this.form = this.fb.group({
      msg: ['', null]
    });

    // this.route.queryParams.subscribe(params => {
    //   this.prodId = params['prodId'];
    // });
  }

  onFocus(){
    if(!this.authSvc.Login){
      this.displayLoginDialog = true;
    }
  }

  askQuestion(){
    this.isSending = true;
    this.model.UserIdFrom = this.userSvc.UserId;
    this.model.UserIdTo = this.userIdTo;
    this.model.ProductId = this.product.ProductId;
    this.model.ConversationId = this.conversationId;

    // if(this.firstMessageFromUserId){
    //   this.model.UserIdTo = this.firstMessageFromUserId;
    // }

    this.productService.AskQuestionToProduct(this.model).subscribe( res => {
      this.commentAddedEvent.emit(res);
      this.toastr.success("Wiadomość została wysłana");
    },error => {
      this.toastr.success(error);
    },
    () => {
      this.isSending = false;
      this.form.reset();
    });
  }

}
