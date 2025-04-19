import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class WeatherService {
  private apiUrl = 'https://api.data.gov.sg/v1/environment/air-temperature';

  constructor(private http: HttpClient) { }

  getWeatherData(date: string): Observable<any> {
    return this.http.get(`${this.apiUrl}?date=${date}`);
  }
}