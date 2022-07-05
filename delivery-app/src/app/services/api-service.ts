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
        return this.http.post<RegisterModel>(`${this.baseUrl}/users/register`, registerModel);
    }

    login(model: LoginModel){
        return this.http.post<TokenModel>(`${this.baseUrl}/users/login`, model);
    }

    changePicture(picture: FormData, id: string){
        return this.http.put<string>(`${this.baseUrl}/users/change-picture/${id}`, picture);
    }

    getPicture(){
        return this.http.get<TokenModel>(`${this.baseUrl}/users/picture/${this.id}`);
    }

    facebookLogin(user: RegisterModel){
        return this.http.post<TokenModel>(`${this.baseUrl}/users/facebook-login`, user);
    }

    getUserById(){
        return this.http.get<RegisterModel>(`${this.baseUrl}/users/user/${this.id}`);
    }

    changeProfile(registerModel: RegisterModel){

        return this.http.put<RegisterModel>(`${this.baseUrl}/users/modify-profile/${this.id}`, registerModel);
    }

    getArticles(){
        return this.http.get<Array<Article>>(`${this.baseUrl}/delivery/articles`);
    }

    //customer

    addOrder(order: Order){
        return this.http.post(`${this.baseUrl}/delivery/customer/add-order/${this.id}`, order);
    }

    getCurrentOrderCustomer(){
        return this.http.get<Order>(`${this.baseUrl}/delivery/customer/current-order/${this.id}`)
    }

    getCustomerOrders(){
        return this.http.get<Array<Order>>(`${this.baseUrl}/delivery/customer/previous-orders/${this.id}`);
    }

    //deliverer

    getNewOrders(){
        return this.http.get<Array<Order>>(`${this.baseUrl}/delivery/deliverer/new-orders/${this.id}`);
    }

    getDelivererOrders(){
        return this.http.get<Array<Order>>(`${this.baseUrl}/delivery/deliverer/previous-orders/${this.id}`);
    }

    getCurrentOrderDeliverer(){
        return this.http.get<Order>(`${this.baseUrl}/delivery/deliverer/current-order/${this.id}`);
    }

    confirmOrder(orderId: string){
        return this.http.get(`${this.baseUrl}/deliverer/confirm-order/${this.id}/${orderId}`);
    }

    //admin
    getAllDeliveres(){
        return this.http.get<Array<RegisterModel>>(`${this.baseUrl}/users/admin/all-deliverers`);
    }

    changeStatus(id : string, status: string){
        return this.http.get(`${this.baseUrl}/users/admin/change-state/${id}/${status}`);
    }

    addArticle(article: Article){
        return this.http.post(`${this.baseUrl}/delivery/admin/add-article`, article);
    }

    getAllOrders(){
        return this.http.get<Array<Order>>(`${this.baseUrl}/delivery/admin/all-orders`);
    }



    
}