// Author: T4professor

import { Component } from '@angular/core';
import { ICellRendererAngularComp } from 'ag-grid-angular';
//import { ICellRendererParams, IAfterGuiAttachedParams } from 'ag-grid-angular';

@Component({
  selector: 'app-button-renderer',
  template: `
    <button [hidden]="imageButton" type="button" class="{{btnClass}}"  (click)="onClick($event)">{{label}}</button>
    <i [hidden]="!imageButton" style="cursor:pointer; font-size: 1.3em;" class="{{btnClass}}" (click)="onClick($event)" title="{{label}}"></i>
 `
})

export class ButtonRendererComponent implements ICellRendererAngularComp {
  params;
  label: string;
  btnClass: string;
  imageButton : boolean;

  agInit(params): void {
    this.params = params;
    this.label = this.params.label || null;
    this.btnClass = this.params.btnClass || '';
    this.imageButton = this.params.imageButton || false;
  }

  refresh(params?: any): boolean {
    return true;
  }

  onClick($event) {
    if (this.params.onClick instanceof Function) {
      // put anything into params u want pass into parents component
      const params = {
        event: $event,
        rowData: this.params.node.data
        // ...something
      }
      this.params.onClick(params);

    }
  }
}