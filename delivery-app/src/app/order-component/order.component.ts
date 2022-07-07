import { formatDate } from '@angular/common';
import { Component, Input, OnInit } from '@angular/core';
import { Order } from '../models/DTO/order-model';
import { ApiService } from '../services/api-service';

@Component({
  selector: 'app-order-component',
  templateUrl: './order.component.html',
  styleUrls: ['./order.component.css']
})
export class OrderComponent implements OnInit {

  @Input() order = new Order();




  constructor(private api: ApiService) { }

  ngOnInit(): void {
  

  }

  get price(){
    let s = 0;
    this.order.articles.forEach(x => {
        s+= x.price;
    });
    return s;
  }

  get time(){
    const format = 'yyyy-MM-dd : ';
    const locale = 'en-US';

    return formatDate(this.order.time, format, locale);
    
  }

}
