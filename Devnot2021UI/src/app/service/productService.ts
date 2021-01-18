import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from "@angular/common/http";
import { Product } from '../models/Product';

@Injectable({ providedIn: 'root' })
export class ProductService {
    public baseUrl: string = "http://localhost:1923/";
    constructor(private httpClient: HttpClient) { }

    public GetExchangeList(): Promise<any> {
        let headers = new HttpHeaders({
            "Content-Type": "application/json"
        });
        const options = {
            headers: headers
        };
        var url = this.baseUrl + "product/GetExchangeList";
        return this.httpClient
            .get(url, options)
            .toPromise()
            .then(
                (res: any) => {
                    return res;
                }
            )
            .catch(x => {
                if (x.status == 401) {
                    window.location.href = "http://localhost:4200";
                }
                return Promise.reject(x);
            });
    }

    public GetProductById(id: number): Promise<any> {
        let headers = new HttpHeaders({
            "Content-Type": "application/json"
        });
        const options = {
            headers: headers
        };
        var url = `${this.baseUrl}product/${id}`;
        return this.httpClient
            .get(url, options)
            .toPromise()
            .then(
                (res: any) => {
                    return res;
                }
            )
            .catch(x => {
                if (x.status == 401) {
                    window.location.href = "http://localhost:4200";
                }
                return Promise.reject(x);
            });
    }

    public GetProductList(): Promise<any> {
        let headers = new HttpHeaders({
            "Content-Type": "application/json"
        });
        const options = {
            headers: headers
        };
        var url = this.baseUrl + "product/GetProductList";
        return this.httpClient
            .get(url, options)
            .toPromise()
            .then(
                (res: any) => {
                    return res;
                }
            )
            .catch(x => {
                if (x.status == 401) {
                    window.location.href = "http://localhost:4200";
                }
                return Promise.reject(x);
            });
    }

    public ProductInsert(data: string) {

        let headers = new HttpHeaders({
            "Content-Type": "application/json",
        });
        var url = this.baseUrl + "product/InsertProduct";
        const options = {
            headers: headers
        };

        return this.httpClient
            .post(url, data, options)
            .toPromise()
            .then((res: any) => {
                return res;
            })
            .catch(x => {
                if (x.status == 401) {
                    window.localStorage.clear();
                    window.location.href = "http://localhost:4200";
                }
                return Promise.reject(x);
            });
    }
}