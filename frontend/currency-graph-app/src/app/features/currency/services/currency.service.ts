import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { CurrencyRate } from '../models/cur.model';

@Injectable({
  providedIn: 'root'
})
export class CurrencyService {
  
  private baseUrl = 'https://localhost:7019/api/currency';

  constructor(private http: HttpClient) {}

  getWeek() {
    return this.http.get<CurrencyRate[]>(`${this.baseUrl}/week`);
  }

  getMonth() {
    return this.http.get<CurrencyRate[]>(`${this.baseUrl}/month`);
  }

  getHalfYear() {
    return this.http.get<CurrencyRate[]>(`${this.baseUrl}/half-year`);
  }

  getYear() {
    return this.http.get<CurrencyRate[]>(`${this.baseUrl}/year`);
  }
}
