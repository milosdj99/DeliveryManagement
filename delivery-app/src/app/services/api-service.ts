import { Injectable } from "@angular/core";
import { HttpClient } from "@angular/common/http";
import { environment } from "src/environments/environment";
import { RegisterModel } from "../models/DTO/register-model";
import { LoginModel } from "../models/DTO/login-model";
import { TokenModel } from "../models/token-model";


@Injectable({ providedIn: 'root' })
export class ApiService{

    baseUrl = `${environment.apiUrl}`;

    constructor(private http: HttpClient){

    }

    register(registerModel: RegisterModel){
        return this.http.post<RegisterModel>(`${this.baseUrl}/register`, registerModel);
    }

    login(model: LoginModel){
        return this.http.post<TokenModel>(`${this.baseUrl}/login`, model);
    }

    getUserById(id: string){
        return this.http.get<RegisterModel>(`${this.baseUrl}/users/${id}`);
    }

    changeProfile(registerModel: RegisterModel){

        let id = localStorage.getItem('id');
        return this.http.put<RegisterModel>(`${this.baseUrl}/modify-profile/${id}`, registerModel);
    }

    
}