import { DatePipe } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
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

  articles: Article[] = [];

  basket: Article[] = [];

  constructor(private api: ApiService) { }

  ngOnInit(): void {
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

    
    order.address = this.formGroupOrder.get('address')?.value;
    order.comment = this.formGroupOrder.get('comment')?.value;
    order.price = price;
    order.articles = articles;
    order.time = new Date();
    
    
    this.api.addOrder(order).subscribe();

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

}
