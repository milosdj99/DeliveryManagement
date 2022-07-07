import { Component, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { Article } from '../models/article-model';
import { ApiService } from '../services/api-service';

@Component({
  selector: 'app-add-article',
  templateUrl: './add-article.component.html',
  styleUrls: ['./add-article.component.css']
})
export class AddArticleComponent implements OnInit {

  constructor(private api: ApiService, private router: Router) { }

  formGroupAddArticle = new FormGroup({
    name : new FormControl("", Validators.required),
    price : new FormControl("", Validators.required),
    ingredients : new FormControl("", Validators.required)
  });

  requiredError = false;
  apiError = false;
  successfulAdd = false;
  numberError = false;
  

  ngOnInit(): void {
  }

  addArticle(){

    this.requiredError = false;
    this.successfulAdd = false;
    this.apiError = false;
    this.numberError = false;

    if(this.formGroupAddArticle.get('price')?.value < 1){
      this.numberError = true;
      return;
    }

    if(this.formGroupAddArticle.get('name')?.errors?['required']:"" || this.formGroupAddArticle.get('price')?.errors?['required']:"" || this.formGroupAddArticle.get('ingredients')?.errors?['required']:""){
        this.requiredError = true;
        return;
    }


      let namee = this.formGroupAddArticle.get('name')?.value;
      let pricee = this.formGroupAddArticle.get('price')?.value;
      let ingredientss = this.formGroupAddArticle.get('ingredients')?.value;


      let a = new Article();
      a.id = "3fa85f64-5717-4562-b3fc-2c963f66afa6";
      a.name = namee,
      a.price = pricee,
      a.ingredients = ingredientss,

      this.api.addArticle(a).subscribe(
        data => {
          this.successfulAdd = true;
        },
        error => {
          this.apiError = true;
        }
      );
      
  }

  logOut(){
    localStorage.setItem("isLoggedIn", "false");
    localStorage.setItem('role', "null");
    localStorage.removeItem('token');

    this.router.navigateByUrl("/login");
  }

  dashboard(){
    this.router.navigateByUrl("/dashboard/admin");

  }

}
