import { Component, Input, OnInit, Output } from '@angular/core';
import { RegisterModel } from '../models/DTO/register-model';
import { ApiService } from '../services/api-service';

@Component({
  selector: 'app-deliverer',
  templateUrl: './deliverer.component.html',
  styleUrls: ['./deliverer.component.css']
})
export class DelivererComponent implements OnInit {

  constructor(private api: ApiService) { }

  @Input() user : RegisterModel = new RegisterModel();


  ngOnInit(): void {
    document.getElementById("id1")?.setAttribute("name", "radioGroup" + this.user.username);
    document.getElementById("id2")?.setAttribute("name", "radioGroup" + this.user.username);
    document.getElementById("id3")?.setAttribute("name", "radioGroup" + this.user.username);
  }

  changeStatus(status: string){
    this.api.changeStatus(this.user.id, status).subscribe();
  }

}
