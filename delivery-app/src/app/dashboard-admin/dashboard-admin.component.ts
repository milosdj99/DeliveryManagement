import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { Order } from '../models/DTO/order-model';
import { RegisterModel } from '../models/DTO/register-model';
import { ApiService } from '../services/api-service';

@Component({
  selector: 'app-dashboard-admin',
  templateUrl: './dashboard-admin.component.html',
  styleUrls: ['./dashboard-admin.component.css']
})
export class DashboardAdminComponent implements OnInit {

  constructor(private api: ApiService, private router: Router) { }

  deliverers : Array<RegisterModel> = [];

  orders : Array<Order> = [];

  showDeliverers: boolean = false;

  showOrders: boolean = false;

  ngOnInit(): void {

    this.api.getAllOrders().subscribe(
      data => {
        this.orders = data;
      }
    );

    this.showOrders = true;
    this.showDeliverers = false;
  }




  logOut(){
    localStorage.setItem("isLoggedIn", "false");
    localStorage.removeItem('token');

    this.router.navigateByUrl("/login");
  }

  allOrders(){
    
    this.api.getAllOrders().subscribe(
      data => {
        this.orders = data;
      }
    );

    this.showOrders = true;
    this.showDeliverers = false;
  }


  allDeliverers(){
      this.api.getAllDeliveres().subscribe(
        data => {
          this.deliverers = data;
        }
      );

      this.showDeliverers = true;
      this.showOrders = false;
  }


  addArticle(){
    this.router.navigateByUrl("add-article");
  }

}
