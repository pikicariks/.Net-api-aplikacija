import { Component, inject, OnDestroy, OnInit } from '@angular/core';
import { LikesService } from '../_services/likes.service';
import { Member } from '../_models/member';
import { ButtonsModule } from 'ngx-bootstrap/buttons';
import { FormsModule } from '@angular/forms';
import { MemberCardComponent } from "../members/member-card/member-card.component";
import { PaginationModule } from 'ngx-bootstrap/pagination';

@Component({
  selector: 'app-lists',
  standalone: true,
  imports: [ButtonsModule, FormsModule, MemberCardComponent,PaginationModule],
  templateUrl: './lists.component.html',
  styleUrl: './lists.component.css'
})
export class ListsComponent implements OnInit,OnDestroy {
  ngOnDestroy(): void {
    this.likeS.paginatedR.set(null);
  }
ngOnInit(): void {
  this.loadLikes();
}
 likeS = inject(LikesService);



predicate = 'liked';
pagenum = 1;
pages = 5;

getTitle(){
  switch (this.predicate) {
    case 'liked':return 'Members you liked';
    case 'likedby':return 'Members who like you';
    
  
    default:
     return 'mutual'
  }
}

loadLikes(){
  this.likeS.getLikes(this.predicate,this.pagenum,this.pages);
}

pageChanged(event:any){
  if (this.pagenum!==event.page) {
    this.pagenum = event.page;
    this.loadLikes();
    
  }
}
}
