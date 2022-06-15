import { Article } from "../article-model";

export class Order{
        
    address : string ="";
    comment : string = "";
    price : number = 0;
    time = new Date();
    articles : Array<Article> = new Array<Article>();
    accepted : boolean = false;
}