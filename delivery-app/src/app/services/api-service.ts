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

    changePicture(picture: FormData, id: string){
        return this.http.put<string>(`${this.baseUrl}/change-picture/${id}`, picture);
    }

    facebookLogin(user: RegisterModel){
        return this.http.post<TokenModel>(`${this.baseUrl}/facebook-login`, user);
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
        return this.http.post(`${this.baseUrl}/customer/add-order/${this.id}`, order);
    }

    getCurrentOrderCustomer(){
        return this.http.get<Order>(`${this.baseUrl}/customer/current-order/${this.id}`)
    }

    getCustomerOrders(){
        return this.http.get<Array<Order>>(`${this.baseUrl}/customer/previous-orders/${this.id}`);
    }

    getNewOrders(){
        return this.http.get<Array<Order>>(`${this.baseUrl}/deliverer/new-orders/${this.id}`);
    }

    getDelivererOrders(){
        return this.http.get<Array<Order>>(`${this.baseUrl}/deliverer/previous-orders/${this.id}`);
    }

    getCurrentOrderDeliverer(){
        return this.http.get<Order>(`${this.baseUrl}/deliverer/current-order/${this.id}`);
    }

    confirmOrder(orderId: string){
        return this.http.get(`${this.baseUrl}/deliverer/confirm-order/${this.id}/${orderId}`);
    }

    getAllDeliveres(){
        return this.http.get<Array<RegisterModel>>(`${this.baseUrl}/admin/all-deliverers`);
    }

    addArticle(article: Article){
        return this.http.post(`${this.baseUrl}/admin/add-article`, article);
    }

    changeStatus(id : string, status: string){
        return this.http.get(`${this.baseUrl}/admin/change-state/${id}/${status}`);
    }

    getAllOrders(){
        return this.http.get<Array<Order>>(`${this.baseUrl}/admin/all-orders`);
    }



    
}