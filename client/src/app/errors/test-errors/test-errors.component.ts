import { HttpClient } from '@angular/common/http';
import { Component, inject } from '@angular/core';

@Component({
  selector: 'app-test-errors',
  standalone: true,
  imports: [],
  templateUrl: './test-errors.component.html',
  styleUrl: './test-errors.component.css'
})
export class TestErrorsComponent {
  baseUrl = 'http://localhost:5000/api/';

  private http = inject(HttpClient);

  validationErrors : string[]=[];

  get400(){
    this.http.get(this.baseUrl + 'buggy/bad-request').subscribe({
      next:response=>console.log(response),
      error:err=>console.log(err)
    });
  }

  get401(){
    this.http.get(this.baseUrl + 'buggy/auth').subscribe({
      next:response=>console.log(response),
      error:err=>console.log(err)
    });
  }

  get404(){
    this.http.get(this.baseUrl + 'buggy/not-found').subscribe({
      next:response=>console.log(response),
      error:err=>console.log(err)
    });
  }

  get500(){
    this.http.get(this.baseUrl + 'buggy/server-error').subscribe({
      next:response=>console.log(response),
      error:err=>console.log(err)
    });
  }

  get400validation(){
    this.http.post(this.baseUrl + 'account/register',{}).subscribe({
      next:response=>console.log(response),
      error:err=>{
        console.log(err);
        this.validationErrors = err;
        
        
      }
    });
  }

}
