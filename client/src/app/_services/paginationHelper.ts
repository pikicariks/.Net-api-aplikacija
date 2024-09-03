import { HttpParams, HttpResponse } from "@angular/common/http";

import { signal } from "@angular/core";
import { PaginationResult } from "../_models/pagination";





export function  setPaginatedResponse<T>(response:HttpResponse<T>
    ,paginatedResultSignal:ReturnType<typeof signal<PaginationResult<T> | null>>) {
    
        paginatedResultSignal.set({
      items:response.body as T,
      pagination:JSON.parse(response.headers.get('Pagination')!)
    })
  }
  export function setPaginationHeaders(pageNum:number,pageSize:number){
    let params = new HttpParams();

    if (pageNum && pageSize) {
      params = params.append('pageNumber',pageNum);
      params = params.append('pageSize',pageSize);
    }
return params;

  }