import { Component, OnInit } from '@angular/core';
import { Order } from '../models/DTO/order-model';
import { ApiService } from '../services/api-service';

@Component({
  selector: 'app-dashboard-deliverer',
  templateUrl: './dashboard-deliverer.component.html',
  styleUrls: ['./dashboard-deliverer.component.css']
})
export class DashboardDelivererComponent implements OnInit {

  constructor(private api: ApiService) { }

  orders : Array<Order> = new Array<Order>();

  newOrders: boolean = true;

  ngOnInit(): void {
    this.newOrders = true;

    this.api.getNewOrders().subscribe(
      data => {
        this.orders = data;
      }
    );

  }

  
  confirmOrder(order: Order){
    this.api.confirmOrder(order.id);
  }

  previousOrders(){

    this.api.getDelivererOrders().subscribe(
      data => {
        this.orders = data;
      }
    );

  }

  currentOrder(){

    this.orders = [];

    this.api.getCurrentOrderDeliverer().subscribe(
      data => {
        this.orders.push(data);
      }
    );
  }

  
  logOut(){
    localStorage.setItem("isLoggedIn", "false");
    localStorage.removeItem('token');
  }

}
