import { Component, OnInit, ViewChild } from '@angular/core';
import { FormGroup, NgForm } from '@angular/forms';
import { ButtonRendererComponent } from './button-renderer.component';
import { ExchangeType } from './models/ExchangeType';
import { Product } from './models/Product';
import { ProductService } from './service/productService';

import { HubConnection, HubConnectionBuilder, LogLevel } from '@aspnet/signalr';
import * as signalR from '@aspnet/signalr';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent implements OnInit {
  title = 'devnot2021UI';
  public product: Product = new Product();
  @ViewChild("myForm") myForm: NgForm;

  selectedTable: string = null;
  exchangeTypeList: ExchangeType[] = [];

  private gridApi;
  private gridColumnApi;
  frameworkComponents: any;
  rowData = [];

  _hubConnection: HubConnection;
  _connectionId: string;
  signalRServiceIp: string = "http://localhost:1923/productHub";

  constructor(private service: ProductService) {
    this.frameworkComponents = {
      iconRenderer: ButtonRendererComponent
    };
  }

  ngOnInit(): void {
    this.getExchangeList();
    this.getProductList();

    this._hubConnection = new HubConnectionBuilder()
      .withUrl(`${this.signalRServiceIp}`, {
        skipNegotiation: true,
        transport: signalR.HttpTransportType.WebSockets
      })
      .build();

    this._hubConnection.start().then(
      () => console.log("Hub Connection Start"))
      .catch(err => console.log(err));

    this._hubConnection.on('GetConnectionId', (connectionId: string) => {
      this._connectionId = connectionId;
      //alert(`ConnectionID:${this._connectionId}`);
      console.log("ConnectionID :" + connectionId);
    });

    this._hubConnection.on('PushProduct', (product: Product) => {
      //console.log("Updated Product:" + JSON.stringify(product));
      //console.log("Row Data Push:"+JSON.stringify(this.rowData));
      var item = this.rowData.find(rd => rd.ID == product.ID);
      //console.log("Current Product:" + JSON.stringify(item));
      this.rowData = this.rowData.filter(pro => pro != item);            
      this.rowData.push(product);
      //console.log("Row Data Push:" + JSON.stringify(this.rowData));
    });
  }

  public clearForm() {
    //Formdaki temizle butonu
    this.product = new Product();
    this.myForm.form.markAsUntouched();
    /* if (this.selectedTable != undefined) {
      this.selectedTable = undefined;
      this.getUserlist();
    } */
  }

  public saveForm(form: FormGroup) {
    this.product.ConnectionId = this._connectionId;
    let data = JSON.stringify(this.product);
    this.service.ProductInsert(data)
      .then((result) => {
        this.rowData = result;
        //console.log("Row Data Save:"+JSON.stringify(this.rowData));
        this.clearForm();
      })
      .catch((err) => {
        console.log('Hata:' + JSON.stringify(err));
      });
  }

  public getExchangeList() {
    this.service
      .GetExchangeList()
      .then((result) => {
        this.exchangeTypeList = result;
        //console.log(this.exchangeTypeList);
      })
      .catch((err) => {
        console.log('Hata:' + JSON.stringify(err));
      });
  }

  public getProductById(Id: number) {
    this.service
      .GetProductById(Id)
      .then((result) => {
        this.product = result;
        //console.log(result);
      })
      .catch((err) => {
        console.log('Hata:' + JSON.stringify(err));
      });
  }

  public getProductList() {
    this.service
      .GetProductList()
      .then((result) => {
        this.rowData = result;
        //console.log(result);
      })
      .catch((err) => {
        console.log('Hata:' + JSON.stringify(err));
      });
  }

  public getSelectedRow(e) {
    this.getProductById(e.rowData.ID);
    //this.product = e.rowData;
    //console.log(JSON.stringify(e.rowData));
  }

  onGridReady(params) {
    this.gridApi = params.api;
    this.gridColumnApi = params.columnApi;
    //params.api.sizeColumnsToFit();
  }

  columnDefs = [
    {
      headerName: "Seç",
      cellRenderer: "iconRenderer",
      cellRendererParams: {
        onClick: this.getSelectedRow.bind(this),
        label: 'Güncelle',
        btnClass: 'far fa-edit fa-sm',
        imageButton: true
      },
      pinned: 'left',
      lockPinned: true,
      autoHeight: true,
      filter: false,
      sortable: false,
      //suppressSizeToFit: true,
      width: 60,
    },
    { headerName: "Adı", field: "Name" },
    { headerName: "Seri No", field: "SeriNo" },
    { headerName: "Toplam Sayı", field: "TotalCount" },
    { headerName: "Fiyat", field: "Price" },
    { headerName: "₺ Fiyat", field: "PriceTL" },
    { headerName: "E-Döviz Tipi", field: "ExchangeTypeName" },
    { headerName: "E-Döviz Değeri", field: "ExchangeTL" },
    { headerName: "Oluşturma Tarihi", field: "CreatedDate" },
  ];
  defaultColDef = { flex: 1, sortable: true, filter: true }
}
