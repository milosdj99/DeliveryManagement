import { Component, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { ApiService } from '../services/api-service';

@Component({
  selector: 'app-profile',
  templateUrl: './profile.component.html',
  styleUrls: ['./profile.component.css']
})
export class ProfileComponent implements OnInit {

  formGroupProfile = new FormGroup({
    username : new FormControl("", Validators.required),
    email : new FormControl("", [Validators.required, Validators.email]),
    password1 : new FormControl("", Validators.required),
    password2 : new FormControl("", Validators.required),
    name : new FormControl("", Validators.required),
    surname : new FormControl("", Validators.required),
    dateOfBirth : new FormControl("", Validators.required),
    address : new FormControl("", Validators.required),
    imageUrl : new FormControl("", Validators.required),
    type : new FormControl("", Validators.required),
  })

  constructor(private api: ApiService, private router: Router) { }

  ngOnInit(): void {
    let id = "fad"
      this.api.getUserById(id).subscribe(
        data =>{
          this.formGroupProfile.get('username')?.patchValue(data.username);
          this.formGroupProfile.get('email')?.patchValue(data.username);
          this.formGroupProfile.get('username')?.patchValue(data.username);
          this.formGroupProfile.get('username')?.patchValue(data.username);
          this.formGroupProfile.get('username')?.patchValue(data.username);
          this.formGroupProfile.get('username')?.patchValue(data.username);
          this.formGroupProfile.get('username')?.patchValue(data.username);
          this.formGroupProfile.get('username')?.patchValue(data.username);

        }
      )
  }

  submitChange(){

  }

}
