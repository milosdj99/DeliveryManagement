import { Component, OnInit } from '@angular/core';
import { Order } from '../models/DTO/order-model';
import { ApiService } from '../services/api-service';

@Component({
  selector: 'app-current-order',
  templateUrl: './current-order.component.html',
  styleUrls: ['./current-order.component.css']
})
export class CurrentOrderComponent implements OnInit {

  order = new Order();


  constructor(private api: ApiService) { }

  ngOnInit(): void {
      this.api.getCurrentOrder().subscribe(
      data => {
            this.order = data;
        }
      )

  }

}
