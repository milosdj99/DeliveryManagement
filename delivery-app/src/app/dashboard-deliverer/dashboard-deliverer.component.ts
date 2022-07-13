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

  apiErrorMessage = "";

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
        this.apiErrorMessage = error.error;
        
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
          this.showTimer = false;
          return;
        }
            let order : Order = data;
            let currentTime = new Date();
            
            order.time = new Date(order.time);

            
            let helperForSeconds = order.time.getSeconds();

            if(helperForSeconds < currentTime.getSeconds()){
              helperForSeconds += 60;
            }

            let raspon = (order.time.getMinutes() - currentTime.getMinutes()) * 60 + order.time.getSeconds() - currentTime.getSeconds();

            this.deliveryMinutes = Math.floor(raspon / 60);
            this.deliverySeconds = raspon % 60;

           /* this.deliveryMinutes = order.time.getMinutes() - currentTime.getMinutes() - 1;
            this.deliverySeconds = helperForSeconds - currentTime.getSeconds(); */

            this.orders.push(data);

            console.clear();

            console.log("orderMinutes: " + order.time.getMinutes());
            console.log("currentMinutes: " + currentTime.getMinutes());

            console.log("deliveryMinutes: " + this.deliveryMinutes);

            console.log("orderSeconds: " + helperForSeconds);
            console.log("currentSeconds: " + currentTime.getSeconds());

            
            console.log("deliverySeconds: " + this.deliverySeconds);
            
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
