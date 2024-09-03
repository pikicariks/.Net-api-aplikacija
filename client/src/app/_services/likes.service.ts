import { inject, Injectable, signal } from '@angular/core';
import { environment } from '../../environments/environment';
import { HttpClient } from '@angular/common/http';
import { Member } from '../_models/member';
import { PaginationResult } from '../_models/pagination';
import { setPaginatedResponse, setPaginationHeaders } from './paginationHelper';

@Injectable({
  providedIn: 'root'
})
export class LikesService {
  baseUrl = environment.apiUrl;
  private http = inject(HttpClient);
  likeIds = signal<number[]>([]);
  paginatedR = signal<PaginationResult<Member[]> | null>(null);

  toggleLike(targetId:number){
    return this.http.post(`${this.baseUrl}likes/${targetId}`,{})
  }

  getLikes(predicate:string,pagenum:number,pages:number){

    let params = setPaginationHeaders(pagenum,pages);
    params=params.append('predicate',predicate);
    return this.http.get<Member[]>(`${this.baseUrl}likes`,
      {observe:'response',params}).subscribe({
        next:res=>setPaginatedResponse(res,this.paginatedR)
      })
  }

  getLikeIds(){
    return this.http.get<number[]>(`${this.baseUrl}likes/list`).subscribe({
      next:res=>this.likeIds.set(res)
    })
  }
}
