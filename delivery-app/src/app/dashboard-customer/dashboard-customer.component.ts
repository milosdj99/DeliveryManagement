import { DatePipe } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { Article } from '../models/article-model';
import { Order } from '../models/DTO/order-model';
import { ApiService } from '../services/api-service';

@Component({
  selector: 'app-dashboard',
  templateUrl: './dashboard-customer.component.html',
  styleUrls: ['./dashboard-customer.component.css']
})
export class DashboardCustomerComponent implements OnInit {

  price: number = 0;

  formGroupOrder = new FormGroup({
    address: new FormControl("", Validators.required),
    comment: new FormControl
  })

  apiError : boolean = false;

  articles: Article[] = [];

  basket: Article[] = [];

  orders: Order[] = [];

  showBasket : boolean = false;

  showTimer: boolean = false;

  deliveryMinutes: number = 0;

  deliverySeconds: number = 0;

  constructor(private api: ApiService, private router: Router) { }

  ngOnInit(): void {

    this.showBasket = true;

    this.orders = [];

    this.apiError = false;

    this.api.getArticles().subscribe(
      data => {
        this.articles = data;
      }
    )
  }

  submitOrder(){
    let order = new Order();

    
    let price = 0;
    let articles = new Array<Article>();

    this.basket.forEach(x => {
      price += x.price * x.amount;

      for(let i = 0; i < x.amount; i++){
        articles.push(x);
      }
    });

    order.id = "3fa85f64-5717-4562-b3fc-2c963f66afa6";
    order.address = this.formGroupOrder.get('address')?.value;
    order.comment = this.formGroupOrder.get('comment')?.value;
    order.price = price;
    order.articles = articles;
    order.time = new Date();
    
    
    this.api.addOrder(order).subscribe(
      error => {
        this.apiError = true;
      },
      data => {
        this.apiError = false;
      }
    );

  }



  addArticle(articleAmount: string){


    let articleName = articleAmount.split(',')[0];
    let amount = parseInt(articleAmount.split(',')[1]);

    if(amount < 1){
      return;
    }
    let a = this.articles.find(x => x.name == articleName);
    
    let basketArticle = new Article();
    basketArticle.id = a!.id;
    basketArticle.name = a!.name; 
    basketArticle.ingredients = a!.ingredients; 
    basketArticle.amount = amount;
    basketArticle.price = a!.price;

    if(this.basket.find(x => x.name == articleName) != null){     //vec postoji u korpi
      this.basket.find(x => x.name == articleName)!.amount += basketArticle.amount;
    } else {
      this.basket.push(basketArticle);
    }
    
    
  }

  removeArticle(article: Article){
    this.basket.splice(this.basket.indexOf(article), 1);
  }

  myOrders(){

    this.showTimer = false;
    this.apiError = false;

    this.orders = [];
    this.articles = [];
    
      this.api.getCustomerOrders().subscribe(
        data => {
          this.orders = data;
        }
      );
  }

  currentOrder(){

    this.showTimer = false;

    this.apiError = false;

    this.orders = [];

    this.showBasket = false;

    this.articles = [];

    this.api.getCurrentOrderCustomer().subscribe(
      data => {
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

            if(order.accepted == true){
              this.showTimer = true;
            }
        }
      )
  }

  newOrder(){

    this.apiError = false;

    this.showBasket = true;

    this.orders = [];

    this.api.getArticles().subscribe(
      data => {
        this.articles = data;
      }
    )
  }

  logOut(){
    localStorage.setItem("isLoggedIn", "false");
    localStorage.removeItem('token');

    this.router.navigateByUrl("/login");
  }

}
