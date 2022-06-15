import { Injectable } from "@angular/core";
import { HttpClient } from "@angular/common/http";
import { environment } from "src/environments/environment";
import { RegisterModel } from "../models/DTO/register-model";
import { LoginModel } from "../models/DTO/login-model";
import { TokenModel } from "../models/token-model";
import { Article } from "../models/article-model";
import { Order } from "../models/DTO/order-model";


@Injectable({ providedIn: 'root' })
export class ApiService{

    baseUrl = `${environment.apiUrl}`;

    

    constructor(private http: HttpClient){

    }

    get id(){
        return localStorage.getItem('id');
    }

    register(registerModel: RegisterModel){
        return this.http.post<RegisterModel>(`${this.baseUrl}/register`, registerModel);
    }

    login(model: LoginModel){
        return this.http.post<TokenModel>(`${this.baseUrl}/login`, model);
    }

    getUserById(){
        return this.http.get<RegisterModel>(`${this.baseUrl}/users/${this.id}`);
    }

    changeProfile(registerModel: RegisterModel){

        return this.http.put<RegisterModel>(`${this.baseUrl}/modify-profile/${this.id}`, registerModel);
    }

    getArticles(){
        return this.http.get<Array<Article>>(`${this.baseUrl}/articles`);
    }

    addOrder(order: Order){
        return this.http.post(`${this.baseUrl}/add-order/${this.id}`, order);
    }

    getCurrentOrder(){
        return this.http.get<Order>(`${this.baseUrl}/current-order/${this.id}`)
    }

    
}