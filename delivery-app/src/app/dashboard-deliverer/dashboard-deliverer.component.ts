import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { Order } from '../models/DTO/order-model';
import { ApiService } from '../services/api-service';

@Component({
  selector: 'app-dashboard-deliverer',
  templateUrl: './dashboard-deliverer.component.html',
  styleUrls: ['./dashboard-deliverer.component.css']
})
export class DashboardDelivererComponent implements OnInit {

  constructor(private api: ApiService, private router: Router) { }

  orders : Array<Order> = new Array<Order>();

  newOrders: boolean = true;

  apiError: boolean = false;

  showTimer: boolean = false;

  deliveryMinutes: number = 0;

  deliverySeconds: number = 0;

  ngOnInit(): void {

    this.showTimer = false;

    this.apiError = false;

    this.newOrders = true;

    this.api.getNewOrders().subscribe(
      data => {
        this.orders = data;
      }
    );

  }

  
  confirmOrder(order: Order){
    this.api.confirmOrder(order.id).subscribe(
      data => {
        this.currentOrder();
      },
      error => {
        this.apiError = true;
      }
    );
    
    
  }

  previousOrders(){

    this.showTimer = false;

    this.newOrders = false;

    this.apiError = false;

    this.api.getDelivererOrders().subscribe(
      data => {
        this.orders = data;
      }
    );

  }

  currentOrder(){

    this.newOrders = false;

    this.apiError = false;

    this.orders = [];

    this.api.getCurrentOrderDeliverer().subscribe(
      data => {
        if(data == null){
          return;
        }
            let order : Order = data;
            let currentTime = new Date();
            
            order.time = new Date(order.time);

            
            let helperForSeconds = 0;

            if(order.time.getSeconds < currentTime.getSeconds){
              helperForSeconds = order.time.getSeconds() + 60;
            }
            this.deliveryMinutes = order.time.getMinutes() - currentTime.getMinutes();
            this.deliverySeconds = order.time.getSeconds() + helperForSeconds - currentTime.getSeconds();

            this.orders.push(data);

            
              this.showTimer = true;
            
      }
    );
  }

  newOrdersRedirect(){

    this.showTimer = false;

    this.apiError = false;

    this.newOrders = true;

    this.api.getNewOrders().subscribe(
      data => {
        this.orders = data;
      }
    );
  }

  
  logOut(){
    localStorage.setItem("isLoggedIn", "false");
    localStorage.setItem('role', "null");
    localStorage.removeItem('token');

    this.router.navigateByUrl("login");
  }

}
