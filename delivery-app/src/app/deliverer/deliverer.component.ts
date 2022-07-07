import { DatePipe } from '@angular/common';
import { Component, Input, OnInit, Output } from '@angular/core';
import { DomSanitizer, SafeUrl } from '@angular/platform-browser';
import { RegisterModel } from '../models/DTO/register-model';
import { ApiService } from '../services/api-service';

@Component({
  selector: 'app-deliverer',
  templateUrl: './deliverer.component.html',
  styleUrls: ['./deliverer.component.css']
})
export class DelivererComponent implements OnInit {

  constructor(private api: ApiService, private datePipe : DatePipe, private sanitizer : DomSanitizer) { }

  @Input() user : RegisterModel = new RegisterModel();

  imageUrl = "";

  ngOnInit(): void {
    this.api.getPicture(this.user.id).subscribe(
      data => {
        this.imageUrl = 'data:image/jpeg;base64, ' +  data.value;
      }
    );
  }

  changeStatus(status: string){
    this.api.changeStatus(this.user.id, status).subscribe();
  }

  renderDate(d : Date){
    return this.datePipe.transform(d, 'dd-MM-yyyy');
  }

  sanitizeImageUrl(imageUrl: string): SafeUrl {
    return this.sanitizer.bypassSecurityTrustUrl(imageUrl);
  }

}
