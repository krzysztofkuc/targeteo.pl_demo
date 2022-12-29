import { Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { ToastrService } from 'ngx-toastr';
import { LazyLoadEvent, SelectItem } from 'primeng/api';
import { OneUserOrderVm } from 'src/app/model/oneUserOrderVm';
import { OrderStatus } from 'src/app/model/enums';
import { OrderVm } from 'src/app/model/orderVm';
import { OrderService } from 'src/app/services/order.service';
import {ConfirmDialogModule} from 'primeng/confirmdialog';
import {ConfirmationService} from 'primeng/api';

@Component({
  selector: 'app-all-orders',
  templateUrl: './all-orders.component.html',
  styleUrls: ['./all-orders.component.css']
})
export class AllOrdersComponent implements OnInit {

  orders: OrderVm[];
  loading: boolean = true;
  totalRecords: number;
  cols: any[];
  status: SelectItem[];
  trackingNo: string;
  displayModalChangeStatusConfirmation: boolean = false;
  selectedOrder: OrderVm;
  changeStatusTo: string;
  changeStatusOld: string;
  @ViewChild('confirmModal')  confirmModal: ElementRef ;


  constructor(private orderService: OrderService,
              private confirmationService: ConfirmationService,
              private toastr: ToastrService) { }

  ngOnInit(): void {

    this.orderService.GetAllorders().subscribe( res => {
      this.orders = res;

      //Need to hanfle dropdown
      this.orders.forEach(order => {
        order.OldStatus = order.Status;
      });

      this.loading = false;
      }, 
      (error: any) => {
        this.toastr.error("Błąd - nie można pobrać metod wysyłki");
      });

      this.status=Object.keys(OrderStatus).map(key => ({ label: OrderStatus[key], value: key }));
  }

  statusChanged(order, event, trackingNo){
    this.selectedOrder = order;
    this.displayModalChangeStatusConfirmation = true;
    this.changeStatusTo = event.value;
    this.changeStatusOld = order.Status;

    // this.confirmationService.confirm({
    //   message: 'Na pewno chcesz zmienić status zamówienia ? Zostanie wysłany email do klienta.',
    //   accept: () => {
      
    //   },
    //   reject: () => {
        
    //     order.Status = order.Status;
    //     order.OldStatus = order.Status;
    //   }
    // });
  }

  confirmChangeStatus(){
    this.orderService.ChangeStatus(this.selectedOrder.OrderId, this.changeStatusTo , this.trackingNo).subscribe( res => {
      this.selectedOrder.Status = res.Status;

      this.loading = false;
      }, 
      (error: any) => {
        this.toastr.warning("Nie zmieniono statusu. Error techniczny");
      });

      this.displayModalChangeStatusConfirmation = false;
  }

  rejectChangeStatus(){
    this.selectedOrder.Status = this.changeStatusOld;
    this.selectedOrder.OldStatus = this.changeStatusOld;
    this.displayModalChangeStatusConfirmation = false;
  }
}
