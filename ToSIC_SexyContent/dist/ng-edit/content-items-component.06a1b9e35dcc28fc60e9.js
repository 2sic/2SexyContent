(window.webpackJsonp=window.webpackJsonp||[]).push([[20],{gur7:function(t,e,i){"use strict";i.r(e);var n=i("D57K"),a=i("5/c3"),o=i("LR82"),r=i("z5yO"),l=i("KLQV"),s=i("o9tz"),c=i("1C3z"),u=i("+raR"),d=i("ZSGP"),p=function(){function t(){this.published="",this.metadata=""}return t.prototype.agInit=function(t){this.params=t},t.prototype.isFilterActive=function(){return""!==this.published||""!==this.metadata},t.prototype.doesFilterPass=function(t){var e,i,n=this.params.valueGetter(t.node);return e=""===this.published||null!=n.published&&n.published.toString()===this.published,i=""===this.metadata||null!=n.metadata&&n.metadata.toString()===this.metadata,e&&i},t.prototype.getModel=function(){if(this.isFilterActive())return{filterType:"pub-meta",published:this.published,metadata:this.metadata}},t.prototype.setModel=function(t){this.published=t?t.published:"",this.metadata=t?t.metadata:""},t.prototype.afterGuiAttached=function(t){},t.prototype.filterChanged=function(){this.params.filterChangedCallback()},t.\u0275fac=function(e){return new(e||t)},t.\u0275cmp=c.Kb({type:t,selectors:[["app-pub-meta-filter"]],decls:18,vars:2,consts:[[1,"title"],[3,"ngModel","ngModelChange"],["value",""],["value","true"],["value","false"]],template:function(t,e){1&t&&(c.Wb(0,"div",0),c.Pc(1,"Published"),c.Vb(),c.Wb(2,"mat-radio-group",1),c.ec("ngModelChange",(function(t){return e.published=t}))("ngModelChange",(function(){return e.filterChanged()})),c.Wb(3,"mat-radio-button",2),c.Pc(4,"All"),c.Vb(),c.Wb(5,"mat-radio-button",3),c.Pc(6,"Published"),c.Vb(),c.Wb(7,"mat-radio-button",4),c.Pc(8,"Not published"),c.Vb(),c.Vb(),c.Wb(9,"div",0),c.Pc(10,"Metadata"),c.Vb(),c.Wb(11,"mat-radio-group",1),c.ec("ngModelChange",(function(t){return e.metadata=t}))("ngModelChange",(function(){return e.filterChanged()})),c.Wb(12,"mat-radio-button",2),c.Pc(13,"All"),c.Vb(),c.Wb(14,"mat-radio-button",3),c.Pc(15,"Is metadata"),c.Vb(),c.Wb(16,"mat-radio-button",4),c.Pc(17,"Is not metadata"),c.Vb(),c.Vb()),2&t&&(c.Bb(2),c.oc("ngModel",e.published),c.Bb(9),c.oc("ngModel",e.metadata))},directives:[u.b,d.r,d.u,u.a],styles:[".title[_ngcontent-%COMP%]{padding:12px 12px 0}.mat-radio-group[_ngcontent-%COMP%]{display:flex;flex-direction:column;justify-content:space-between;overflow:hidden;padding:12px;width:160px;height:104px}"]}),t}(),m=i("8AiQ"),b=i("r4gC"),f=i("Qc/f");function h(t,e){1&t&&(c.Wb(0,"mat-icon",4),c.Pc(1,"visibility"),c.Vb())}function y(t,e){1&t&&(c.Wb(0,"mat-icon",5),c.Pc(1,"visibility_off"),c.Vb())}function g(t,e){if(1&t&&(c.Ub(0),c.Wb(1,"mat-icon",6),c.Pc(2,"local_offer"),c.Vb(),c.Tb()),2&t){var i=c.ic();c.Bb(1),c.oc("matTooltip",i.metadataTooltip)}}var v=function(){function t(){}return t.prototype.agInit=function(t){this.value=t.value;var e=t.data;e.Metadata&&(this.metadataTooltip="Metadata\nType: "+e.Metadata.TargetType+(e.Metadata.KeyNumber?"\nNumber: "+e.Metadata.KeyNumber:"")+(e.Metadata.KeyString?"\nString: "+e.Metadata.KeyString:"")+(e.Metadata.KeyGuid?"\nGuid: "+e.Metadata.KeyGuid:""))},t.prototype.refresh=function(t){return!0},t.\u0275fac=function(e){return new(e||t)},t.\u0275cmp=c.Kb({type:t,selectors:[["app-content-items-status"]],decls:4,vars:3,consts:[[1,"icon-container"],["matTooltip","Published",4,"ngIf"],["matTooltip","Not published",4,"ngIf"],[4,"ngIf"],["matTooltip","Published"],["matTooltip","Not published"],[1,"meta-icon",3,"matTooltip"]],template:function(t,e){1&t&&(c.Wb(0,"div",0),c.Nc(1,h,2,0,"mat-icon",1),c.Nc(2,y,2,0,"mat-icon",2),c.Nc(3,g,3,1,"ng-container",3),c.Vb()),2&t&&(c.Bb(1),c.oc("ngIf",e.value.published),c.Bb(1),c.oc("ngIf",!e.value.published),c.Bb(1),c.oc("ngIf",e.value.metadata))},directives:[m.t,b.a,f.a],styles:[".meta-icon[_ngcontent-%COMP%]{margin-left:8px}"]}),t}(),T=i("OeRG"),C=function(){function t(){}return t.prototype.agInit=function(t){this.params=t,this.item=t.data},t.prototype.refresh=function(t){return!0},t.prototype.clone=function(){this.params.onClone(this.item)},t.prototype.export=function(){this.params.onExport(this.item)},t.prototype.delete=function(){this.params.onDelete(this.item)},t.\u0275fac=function(e){return new(e||t)},t.\u0275cmp=c.Kb({type:t,selectors:[["app-content-items-actions"]],decls:10,vars:0,consts:[[1,"actions-component"],["matRipple","","matTooltip","Copy",1,"like-button","highlight",3,"click"],["matRipple","","matTooltip","Export",1,"like-button","highlight",3,"click"],["matRipple","","matTooltip","Delete",1,"like-button","highlight",3,"click"]],template:function(t,e){1&t&&(c.Wb(0,"div",0),c.Wb(1,"div",1),c.ec("click",(function(){return e.clone()})),c.Wb(2,"mat-icon"),c.Pc(3,"file_copy"),c.Vb(),c.Vb(),c.Wb(4,"div",2),c.ec("click",(function(){return e.export()})),c.Wb(5,"mat-icon"),c.Pc(6,"cloud_download"),c.Vb(),c.Vb(),c.Wb(7,"div",3),c.ec("click",(function(){return e.delete()})),c.Wb(8,"mat-icon"),c.Pc(9,"delete"),c.Vb(),c.Vb(),c.Vb())},directives:[T.q,f.a,b.a],styles:[""]}),t}();function I(t,e){if(1&t&&(c.Wb(0,"span",2),c.Pc(1),c.Vb()),2&t){var i=c.ic();c.Bb(1),c.Qc(i.entities)}}var k=function(){function t(){}return t.prototype.agInit=function(t){this.params=t,Array.isArray(t.value)&&(this.encodedValue=this.htmlEncode(t.value.join(", ")),t.colDef.allowMultiValue&&(this.entities=t.value.length))},t.prototype.refresh=function(t){return!0},t.prototype.htmlEncode=function(t){return t.replace(/&/g,"&amp;").replace(/"/g,"&quot;").replace(/'/g,"&#39;").replace(/</g,"&lt;").replace(/>/g,"&gt;")},t.\u0275fac=function(e){return new(e||t)},t.\u0275cmp=c.Kb({type:t,selectors:[["app-content-items-entity"]],decls:3,vars:3,consts:[[3,"matTooltip"],["class","more-entities",4,"ngIf"],[1,"more-entities"]],template:function(t,e){1&t&&(c.Wb(0,"div",0),c.Nc(1,I,2,1,"span",1),c.Pc(2),c.Vb()),2&t&&(c.oc("matTooltip",e.encodedValue),c.Bb(1),c.oc("ngIf",e.entities),c.Bb(1),c.Rc(" ",e.encodedValue,"\n"))},directives:[f.a,m.t],styles:[".more-entities[_ngcontent-%COMP%]{padding:0 8px;border-radius:10px;border:1px solid rgba(29,39,61,.44);-webkit-user-select:none;-moz-user-select:none;-ms-user-select:none;user-select:none}"]}),t}(),S=i("CeOT"),V=i("0ELX"),W=i("JzAw"),x=i("50eG"),M=i("QjRa"),N=i("BLjT"),P=i("S36y"),w=i("55Ui"),D=i("GTfO"),O=i("Xi8o"),G=i("2pW/"),R=i("9HSk"),A=i("G6Ml");i.d(e,"ContentItemsComponent",(function(){return F}));var F=function(){function t(t,e,i,a,r,s,c,u){this.dialogRef=t,this.contentTypesService=e,this.router=i,this.route=a,this.contentItemsService=r,this.entitiesService=s,this.contentExportService=c,this.snackBar=u,this.modules=l.a,this.gridOptions=Object(n.a)(Object(n.a)({},M.a),{frameworkComponents:{pubMetaFilterComponent:p,booleanFilterComponent:S.a,idFieldComponent:W.a,contentItemsStatusComponent:v,contentItemsActionsComponent:C,contentItemsEntityComponent:k}}),this.subscription=new o.a,this.hasChild=!!this.route.snapshot.firstChild,this.contentTypeStaticName=this.route.snapshot.paramMap.get("contentTypeStaticName")}return t.prototype.ngOnInit=function(){var t=this;this.fetchContentType(),this.fetchItems(),this.refreshOnChildClosed(),this.contentItemsService.getColumns(this.contentTypeStaticName).subscribe((function(e){var i;t.columnDefs=t.buildColumnDefs(e),null===(i=t.gridApi)||void 0===i||i.setColumnDefs(t.columnDefs);var a=function(t){var e,i;if(t){var a;"="===t.charAt(t.length-1)&&(t=atob(t));try{a=JSON.parse(t)}catch(p){console.error("Can't parse JSON with filters from url:",t)}if(a){var o={};(a.IsPublished||a.IsMetadata)&&(o.Status=d={filterType:"pub-meta",published:a.IsPublished?a.IsPublished:"",metadata:a.IsMetadata?a.IsMetadata:""});var r=Object.keys(a);try{for(var l=Object(n.i)(r),s=l.next();!s.done;s=l.next()){var c=s.value;if("IsPublished"!==c&&"IsMetadata"!==c){var u=a[c];if("string"==typeof u)o[c]=d={filterType:"text",type:"equals",filter:u};else if("number"==typeof u)o[c]=d={filterType:"number",type:"equals",filter:u,filterTo:null};else if(typeof u==typeof!0){var d={filterType:"boolean",filter:u.toString()};o[c]=d}}}}catch(m){e={error:m}}finally{try{s&&!s.done&&(i=l.return)&&i.call(l)}finally{if(e)throw e.error}}return o}}}(sessionStorage.getItem(V.h));a&&(Object(x.a)("Will try to apply filter:",a),t.gridApi.setFilterModel(a))}))},t.prototype.ngOnDestroy=function(){this.subscription.unsubscribe(),this.subscription=null},t.prototype.onGridReady=function(t){this.gridApi=t.api,this.columnDefs&&this.gridApi.setColumnDefs(this.columnDefs)},t.prototype.fetchContentType=function(){var t=this;this.contentTypesService.retrieveContentType(this.contentTypeStaticName).subscribe((function(e){t.contentType=e}))},t.prototype.fetchItems=function(){var t=this;this.contentItemsService.getAll(this.contentTypeStaticName).subscribe((function(e){t.items=e}))},t.prototype.editItem=function(t){var e;e=null===t?{items:[{ContentTypeName:this.contentTypeStaticName}]}:{items:[{EntityId:t.data.Id.toString()}]},this.router.navigate(["edit/"+JSON.stringify(e)],{relativeTo:this.route})},t.prototype.exportContent=function(){var t=this.gridApi.getFilterModel(),e=Object.keys(t).length>0,i=[];e&&this.gridApi.forEachNodeAfterFilterAndSort((function(t){i.push(t.data.Id)})),this.router.navigate(["export/"+this.contentTypeStaticName+(i.length>0?"/"+i:"")],{relativeTo:this.route})},t.prototype.importItem=function(){this.router.navigate(["import"],{relativeTo:this.route})},t.prototype.addMetadata=function(){var t,e;if(confirm("This is a special operation to add an item which is metadata for another item. If you didn't understand that, this is not for you :). Continue?")){var i=Object.keys(s.a.metadata),a=i.map((function(t){return s.a.metadata[t].type})),o=parseInt(prompt("What kind of assignment do you want?"+i.map((function(t){return"\n"+s.a.metadata[t].type+": "+s.a.metadata[t].target})),s.a.metadata.entity.type.toString()),10);if(!o)return alert("No target type entered. Cancelled");if(!a.includes(o))return alert("Invalid target type. Cancelled");var r=prompt("What key do you want?");if(!r)return alert("No key entered. Cancelled");var l,c=Object.keys(s.a.keyTypes),u=c.map((function(t){return s.a.keyTypes[t]})),d=prompt("What key type do you want?"+c.map((function(t){return"\n"+s.a.keyTypes[t]})),s.a.keyTypes.number);if(!d)return alert("No key type entered. Cancelled");if(!u.includes(d))return alert("Invalid key type. Cancelled");if(d===s.a.keyTypes.number&&!parseInt(r,10))return alert("Key type number and key don't match. Cancelled");try{for(var p=Object(n.i)(i),m=p.next();!m.done;m=p.next()){var b=m.value;o===s.a.metadata[b].type&&(l=s.a.metadata[b].target)}}catch(h){t={error:h}}finally{try{m&&!m.done&&(e=p.return)&&e.call(p)}finally{if(t)throw t.error}}var f={items:[{ContentTypeName:this.contentTypeStaticName,For:Object(n.a)(Object(n.a)(Object(n.a)({Target:l},d===s.a.keyTypes.guid&&{Guid:r}),d===s.a.keyTypes.number&&{Number:parseInt(r,10)}),d===s.a.keyTypes.string&&{String:r})}]};this.router.navigate(["edit/"+JSON.stringify(f)],{relativeTo:this.route})}},t.prototype.debugFilter=function(){console.warn("Current filter:",this.gridApi.getFilterModel()),alert("Check console for filter information")},t.prototype.closeDialog=function(){this.dialogRef.close()},t.prototype.refreshOnChildClosed=function(){var t=this;this.subscription.add(this.router.events.pipe(Object(r.a)((function(t){return t instanceof a.b}))).subscribe((function(e){var i=t.hasChild;t.hasChild=!!t.route.snapshot.firstChild,!t.hasChild&&i&&t.fetchItems()})))},t.prototype.buildColumnDefs=function(t){var e,i,a=[{headerName:"ID",field:"Id",width:70,headerClass:"dense",cellClass:"id-action no-padding no-outline",cellRenderer:"idFieldComponent",sortable:!0,filter:"agTextColumnFilter",valueGetter:this.idValueGetter},{headerName:"Status",field:"Status",width:80,headerClass:"dense",cellClass:"no-outline",filter:"pubMetaFilterComponent",cellRenderer:"contentItemsStatusComponent",valueGetter:this.valueGetterStatus},{headerName:"Item (Entity)",field:"_Title",flex:2,minWidth:250,cellClass:"primary-action highlight",sortable:!0,filter:"agTextColumnFilter",onCellClicked:this.editItem.bind(this)},{cellClass:"secondary-action no-padding",width:120,cellRenderer:"contentItemsActionsComponent",cellRendererParams:{onClone:this.clone.bind(this),onExport:this.export.bind(this),onDelete:this.delete.bind(this)}},{headerName:"Stats",headerTooltip:"Used by others / uses others",field:"_Used",width:70,headerClass:"dense",cellClass:"no-outline",sortable:!0,filter:"agTextColumnFilter",valueGetter:this.valueGetterUsage}];try{for(var o=Object(n.i)(t),r=o.next();!r.done;r=o.next()){var l=r.value,s={headerName:l.StaticName,field:l.StaticName,flex:2,minWidth:250,cellClass:"no-outline",sortable:!0};switch(l.Type){case"Entity":try{s.allowMultiValue=l.Metadata.Entity.AllowMultiValue}catch(c){s.allowMultiValue=!0}s.cellRenderer="contentItemsEntityComponent",s.valueGetter=this.valueGetterEntityField,s.filter="agTextColumnFilter";break;case"DateTime":try{s.useTimePicker=l.Metadata.DateTime.UseTimePicker}catch(c){s.useTimePicker=!1}s.valueGetter=this.valueGetterDateTime,s.filter="agTextColumnFilter";break;case"Boolean":s.valueGetter=this.valueGetterBoolean,s.filter="booleanFilterComponent";break;case"Number":s.filter="agNumberColumnFilter";break;default:s.filter="agTextColumnFilter"}a.push(s)}}catch(u){e={error:u}}finally{try{r&&!r.done&&(i=o.return)&&i.call(o)}finally{if(e)throw e.error}}return a},t.prototype.clone=function(t){this.router.navigate(["edit/"+JSON.stringify({items:[{ContentTypeName:this.contentTypeStaticName,DuplicateEntity:t.Id}]})],{relativeTo:this.route})},t.prototype.export=function(t){this.contentExportService.exportEntity(t.Id,this.contentTypeStaticName,!0)},t.prototype.delete=function(t){var e=this;confirm("Delete '"+t._Title+"' ("+t._RepositoryId+")?")&&(this.snackBar.open("Deleting..."),this.entitiesService.delete(this.contentTypeStaticName,t._RepositoryId,!1).subscribe({next:function(){e.snackBar.open("Deleted",null,{duration:2e3}),e.fetchItems()},error:function(i){e.snackBar.dismiss(),confirm(i.error.ExceptionMessage+"\n\nDo you want to force delete '"+t._Title+"' ("+t._RepositoryId+")?")&&(e.snackBar.open("Deleting..."),e.entitiesService.delete(e.contentTypeStaticName,t._RepositoryId,!0).subscribe((function(){e.snackBar.open("Deleted",null,{duration:2e3}),e.fetchItems()})))}}))},t.prototype.idValueGetter=function(t){var e=t.data;return"ID: "+e.Id+"\nRepoID: "+e._RepositoryId+"\nGUID: "+e.Guid},t.prototype.valueGetterStatus=function(t){var e=t.data;return{published:e.IsPublished,metadata:!!e.Metadata}},t.prototype.valueGetterUsage=function(t){var e=t.data;return e._Used+" / "+e._Uses},t.prototype.valueGetterEntityField=function(t){var e=t.data[t.colDef.field];return 0===e.length?null:e.map((function(t){return t.Title}))},t.prototype.valueGetterDateTime=function(t){var e=t.data[t.colDef.field];return e?t.colDef.useTimePicker?e.substring(0,19).replace("T"," "):e.substring(0,10):null},t.prototype.valueGetterBoolean=function(t){var e=t.data[t.colDef.field];return typeof e!=typeof!0?null:e.toString()},t.\u0275fac=function(e){return new(e||t)(c.Qb(N.h),c.Qb(P.a),c.Qb(a.c),c.Qb(a.a),c.Qb(w.a),c.Qb(D.a),c.Qb(O.a),c.Qb(G.b))},t.\u0275cmp=c.Kb({type:t,selectors:[["app-content-items"]],decls:27,vars:4,consts:[[1,"nav-component-wrapper"],["mat-dialog-title",""],[1,"dialog-title-box"],["mat-icon-button","","matTooltip","Close dialog",3,"click"],[1,"grid-wrapper"],[1,"ag-theme-material",3,"rowData","modules","gridOptions","gridReady"],[1,"actions-box"],["mat-icon-button","","matTooltip","Export",3,"click"],["mat-icon-button","","matTooltip","Import",3,"click"],["mat-icon-button","","matTooltip","Add metadata",3,"click"],["mat-icon-button","","matTooltip","Debug filter",3,"click"],["mat-fab","","mat-elevation-z24","","matTooltip","Add item",1,"grid-fab",3,"click"]],template:function(t,e){1&t&&(c.Wb(0,"div",0),c.Wb(1,"div",1),c.Wb(2,"div",2),c.Wb(3,"div"),c.Pc(4),c.Vb(),c.Wb(5,"button",3),c.ec("click",(function(){return e.closeDialog()})),c.Wb(6,"mat-icon"),c.Pc(7,"close"),c.Vb(),c.Vb(),c.Vb(),c.Vb(),c.Rb(8,"router-outlet"),c.Wb(9,"div",4),c.Wb(10,"ag-grid-angular",5),c.ec("gridReady",(function(t){return e.onGridReady(t)})),c.Vb(),c.Wb(11,"div",6),c.Wb(12,"button",7),c.ec("click",(function(){return e.exportContent()})),c.Wb(13,"mat-icon"),c.Pc(14,"cloud_download"),c.Vb(),c.Vb(),c.Wb(15,"button",8),c.ec("click",(function(){return e.importItem()})),c.Wb(16,"mat-icon"),c.Pc(17,"cloud_upload"),c.Vb(),c.Vb(),c.Wb(18,"button",9),c.ec("click",(function(){return e.addMetadata()})),c.Wb(19,"mat-icon"),c.Pc(20,"local_offer"),c.Vb(),c.Vb(),c.Wb(21,"button",10),c.ec("click",(function(){return e.debugFilter()})),c.Wb(22,"mat-icon"),c.Pc(23,"filter_list"),c.Vb(),c.Vb(),c.Vb(),c.Wb(24,"button",11),c.ec("click",(function(){return e.editItem(null)})),c.Wb(25,"mat-icon"),c.Pc(26,"add"),c.Vb(),c.Vb(),c.Vb(),c.Vb()),2&t&&(c.Bb(4),c.Rc("",null==e.contentType?null:e.contentType.Name," Data"),c.Bb(6),c.oc("rowData",e.items)("modules",e.modules)("gridOptions",e.gridOptions))},directives:[N.i,R.b,f.a,b.a,a.h,A.a],styles:[".actions-box[_ngcontent-%COMP%]{margin-right:66px;margin-left:8px;display:flex}"]}),t}()}}]);
//# sourceMappingURL=https://sources.2sxc.org/11.03.00/ng-edit/content-items-component.06a1b9e35dcc28fc60e9.js.map