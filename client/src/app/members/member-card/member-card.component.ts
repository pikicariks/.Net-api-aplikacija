import { Component, computed, inject, input, ViewEncapsulation } from '@angular/core';
import { Member } from '../../_models/member';
import { RouterLink } from '@angular/router';
import { LikesService } from '../../_services/likes.service';

@Component({
  selector: 'app-member-card',
  standalone: true,
  imports: [RouterLink],
  templateUrl: './member-card.component.html',
  styleUrl: './member-card.component.css',

})
export class MemberCardComponent {
  private likeS = inject(LikesService);
  member = input.required<Member>();


  hasLiked = computed(()=>this.likeS.likeIds().includes(this.member().id));

  toggleLike(){
    this.likeS.toggleLike(this.member().id).subscribe({
      next:()=>{
        if(this.hasLiked()){
          this.likeS.likeIds.update(ids=>ids.filter(x=>x!==this.member().id))
        }
        else{
          this.likeS.likeIds.update(ids=>[...ids,this.member().id])
        }
      }
    })
  }
}
