(window.webpackJsonp=window.webpackJsonp||[]).push([[35],{NzEc:function(t,n,o){"use strict";o.r(n);var i=o("8AiQ"),e=o("9Saw"),a=o("BLjT"),s=o("9HSk"),r=o("r4gC"),l=o("Qc/f"),c=o("2pW/"),u=o("5/c3"),d=o("nXrb"),g=o("D57K"),h={name:"MANAGE_CONTENT_LIST_DIALOG",initContext:!0,panelSize:"small",panelClass:null,getComponent:function(){return Object(g.b)(this,void 0,void 0,(function(){return Object(g.e)(this,(function(t){switch(t.label){case 0:return[4,o.e(34).then(o.bind(null,"EqWC"))];case 1:return[2,t.sent().ManageContentListComponent]}}))}))}},f=o("it7M"),p=o("1C3z"),b=[{path:"",component:d.a,data:{dialog:h},children:[{matcher:f.a,loadChildren:function(){return Promise.all([o.e(1),o.e(2),o.e(3),o.e(7),o.e(8),o.e(10),o.e(9),o.e(0),o.e(26)]).then(o.bind(null,"B+J5")).then((function(t){return t.EditModule}))}}]}],C=function(){function t(){}return t.\u0275mod=p.Ob({type:t}),t.\u0275inj=p.Nb({factory:function(n){return new(n||t)},imports:[[u.g.forChild(b)],u.g]}),t}(),w=o("O6Xb"),m=o("Iv+g"),v=o("PlBB");o.d(n,"ManageContentListModule",(function(){return O}));var O=function(){function t(){}return t.\u0275mod=p.Ob({type:t}),t.\u0275inj=p.Nb({factory:function(n){return new(n||t)},providers:[m.a,v.a],imports:[[i.c,C,w.a,a.g,s.c,r.b,l.b,e.c,c.d]]}),t}()},nXrb:function(t,n,o){"use strict";o.d(n,"a",(function(){return u}));var i=o("D57K"),e=o("LR82"),a=o("50eG"),s=o("1C3z"),r=o("BLjT"),l=o("5/c3"),c=o("Iv+g"),u=function(){function t(t,n,o,i,a){if(this.dialog=t,this.viewContainerRef=n,this.router=o,this.route=i,this.context=a,this.subscription=new e.a,this.dialogConfig=this.route.snapshot.data.dialog,!this.dialogConfig)throw new Error("Could not find config for dialog. Did you forget to add DialogConfig to route data?")}return t.prototype.ngOnInit=function(){return Object(i.b)(this,void 0,void 0,(function(){var t,n=this;return Object(i.e)(this,(function(o){switch(o.label){case 0:return Object(a.a)("Open dialog:",this.dialogConfig.name,"Context id:",this.context.id,"Context:",this.context),t=this,[4,this.dialogConfig.getComponent()];case 1:return t.component=o.sent(),this.dialogConfig.initContext&&this.context.init(this.route),this.dialogRef=this.dialog.open(this.component,{backdropClass:"dialog-backdrop",panelClass:Object(i.g)(["dialog-panel","dialog-panel-"+this.dialogConfig.panelSize,this.dialogConfig.showScrollbar?"show-scrollbar":"no-scrollbar"],this.dialogConfig.panelClass?this.dialogConfig.panelClass:[]),viewContainerRef:this.viewContainerRef,autoFocus:!1,closeOnNavigation:!1,position:{top:"0"}}),this.subscription.add(this.dialogRef.afterClosed().subscribe((function(t){if(Object(a.a)("Dialog was closed:",n.dialogConfig.name,"Data:",t),n.route.pathFromRoot.length<=3)try{window.parent.$2sxc.totalPopup.close()}catch(o){}else n.router.navigate(["./"],n.route.snapshot.url.length>0?{relativeTo:n.route.parent,state:t}:{relativeTo:n.route.parent.parent,state:t})}))),[2]}}))}))},t.prototype.ngOnDestroy=function(){this.subscription.unsubscribe(),this.subscription=null,this.dialogConfig=null,this.component=null,this.dialogRef.close(),this.dialogRef=null},t.\u0275fac=function(n){return new(n||t)(s.Qb(r.b),s.Qb(s.O),s.Qb(l.c),s.Qb(l.a),s.Qb(c.a))},t.\u0275cmp=s.Kb({type:t,selectors:[["app-dialog-entry"]],decls:0,vars:0,template:function(t,n){},styles:[""]}),t}()}}]);
//# sourceMappingURL=https://sources.2sxc.org/11.03.00/ng-edit/manage-content-list-manage-content-list-module.5a546a0135746d7230eb.js.map