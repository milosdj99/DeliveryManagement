import { Component, Input, OnInit, Output, EventEmitter } from '@angular/core';
import { Article } from '../models/article-model';

@Component({
  selector: 'app-article',
  templateUrl: './article.component.html',
  styleUrls: ['./article.component.css']
})
export class ArticleComponent implements OnInit {

  @Input() article: Article = new Article();

  @Output() addEvent = new EventEmitter();
  

  constructor() { }

  ngOnInit(): void {
  }

  get ingredients(){
    return this.article.ingredients.split(',');
  }

  addArticle(amount: any){
    this.addEvent.emit(this.article.name + ',' + amount);
  }

}
