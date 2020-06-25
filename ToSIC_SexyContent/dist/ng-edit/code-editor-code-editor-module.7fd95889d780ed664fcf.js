(window.webpackJsonp=window.webpackJsonp||[]).push([[15],{"+UDY":function(e,t,n){"use strict";n.d(t,"a",(function(){return o}));var i=n("1C3z"),o=function(){function e(){this.cleanBadPath=function(e){return e?e.replace(/[\?<>\\:\*\|":]/g,"_").replace(/[\x00-\x1f\x80-\x9f]/g,"_").replace(/^\.+$/,"_").replace(/^(con|prn|aux|nul|com[0-9]|lpt[0-9])(\..*)?$/i,"_").replace(/[\. ]+$/,"_"):e}}return e.prototype.removeFromStart=function(e,t){if(!e)return e;for(;e.substring(0,1)===t;)e=e.substring(1);return e},e.prototype.removeFromEnd=function(e,t){if(!e)return e;for(;e.substring(e.length-1,e.length)===t;)e=e.substring(0,e.length-1);return e},e.prototype.sanitizePath=function(e){return e?(e=this.removeFromStart(e,"/"),e=this.removeFromEnd(e,"/"),e=this.removeFromStart(e,"\\"),e=this.removeFromEnd(e,"\\"),e=this.cleanBadPath(e)):e},e.prototype.sanitizeName=function(e){return e?this.sanitizePath(e).replace(/\//g,"_"):e},e.\u0275prov=i.Mb({token:e,factory:e.\u0275fac=function(t){return new(t||e)}}),e}()},iluC:function(e,t,n){"use strict";n.r(t);var i=n("8AiQ"),o=n("t5c9"),r=n("ZSGP"),a=n("D3iX"),s=n("AcfQ"),c=n("BLjT"),p=n("9HSk"),l=n("r4gC"),u=n("Qc/f"),d=n("2pW/"),f=n("LuBX"),h=n("OeRG"),g=n("5/c3"),b=n("cQOC"),v=n("0ELX"),m=n("Q+Kz"),y=n("1C3z"),x=n("Iv+g"),w=n("D57K"),S=n("fQLH"),C=function(){function e(e){this.snackBar=e,this.defaultDuration=3e3,this.processingMessage=!1,this.messageQueue=[]}return e.prototype.add=function(e,t,n){void 0===n&&(n={duration:this.defaultDuration});var i=new S.a;return this.messageQueue.push({message:e,action:t,config:n,triggered:i}),this.processingMessage||this.showSnackBar(),i.asObservable()},e.prototype.showSnackBar=function(){var e=this,t=this.messageQueue.shift();if(null!=t){this.processingMessage=!0;var n=this.snackBar.open(t.message,t.action,t.config);n.afterDismissed().subscribe((function(){t.triggered.complete(),e.showSnackBar()})),n.onAction().subscribe((function(){t.triggered.next()}))}else this.processingMessage=!1},e.prototype.ngOnDestroy=function(){var e,t;try{for(var n=Object(w.i)(this.messageQueue),i=n.next();!i.done;i=n.next())i.value.triggered.complete()}catch(o){e={error:o}}finally{try{i&&!i.done&&(t=n.return)&&t.call(n)}finally{if(e)throw e.error}}this.messageQueue=null},e.\u0275fac=function(t){return new(t||e)(y.ac(d.b))},e.\u0275prov=y.Mb({token:e,factory:e.\u0275fac,providedIn:"root"}),e}(),k=n("Jg5f"),O=n("dkRO"),T=function(){function e(e,t,n){this.http=e,this.context=t,this.dnnContext=n}return e.prototype.get=function(e){return this.http.get(this.dnnContext.$2sxc.http.apiUrl("app-sys/appassets/asset"),{params:Object(w.a)({appId:this.context.appId.toString()},this.templateIdOrPath(e))}).pipe(Object(k.a)((function(e){if("auto"===e.Type.toLowerCase())switch(e.Extension.toLowerCase()){case".cs":case".cshtml":e.Type="Razor";break;case".html":case".css":case".js":e.Type="Token"}return e})))},e.prototype.save=function(e,t){return this.http.post(this.dnnContext.$2sxc.http.apiUrl("app-sys/appassets/asset"),t,{params:Object(w.a)({appId:this.context.appId.toString()},this.templateIdOrPath(e))})},e.prototype.getTemplates=function(){return this.http.get(this.dnnContext.$2sxc.http.apiUrl("app-sys/appassets/list"),{params:{appId:this.context.appId.toString(),global:"false",withSubfolders:"true"}})},e.prototype.createTemplate=function(e){return this.http.post(this.dnnContext.$2sxc.http.apiUrl("app-sys/appassets/create"),{},{params:{appId:this.context.appId.toString(),global:"false",path:e}})},e.prototype.templateIdOrPath=function(e){return"number"==typeof e?{templateId:e.toString()}:{path:e}},e.\u0275fac=function(t){return new(t||e)(y.ac(o.b),y.ac(x.a),y.ac(O.a))},e.\u0275prov=y.Mb({token:e,factory:e.\u0275fac}),e}(),P=n("9RHM"),V=n("DGvA"),I=function(){function e(e,t,n){this.http=e,this.dnnContext=t,this.translate=n,this.keyPrefixes=["@","["],this.keyPrefixIndex=function(e){return e.Type.indexOf("Razor")>-1?0:1}}return e.prototype.getSnippets=function(e){return Object(w.b)(this,void 0,void 0,(function(){var t,n,i;return Object(w.e)(this,(function(o){switch(o.label){case 0:return[4,this.http.get("../ng-assets/snippets.json.js").toPromise()];case 1:return t=o.sent(),n=this.filterAwayNotNeededSnippetsList(t.snippets,e),i=this.extractInputTypeSnippets(n),[2,{sets:this.initSnippetsWithConfig(i.standardArray,e,i.inputTypeSnippets),list:i.standardArray}]}}))}))},e.prototype.filterAwayNotNeededSnippetsList=function(e,t){var n,i,o=[];try{for(var r=Object(w.i)(e),a=r.next();!a.done;a=r.next()){var s=a.value,c=this.keyPrefixes.indexOf(s.set[0]);-1!==c&&c!==this.keyPrefixIndex(t)||(c===this.keyPrefixIndex(t)&&(s.set=s.set.substr(1)),o.push(s))}}catch(p){n={error:p}}finally{try{a&&!a.done&&(i=r.return)&&i.call(r)}finally{if(n)throw n.error}}return o},e.prototype.extractInputTypeSnippets=function(e){var t,n,i=[],o=[];try{for(var r=Object(w.i)(e),a=r.next();!a.done;a=r.next()){var s=a.value;"\\"===s.set[0]?(s.set=s.set.substr(1),o.push(s)):i.push(s)}}catch(c){t={error:c}}finally{try{a&&!a.done&&(n=r.return)&&n.call(r)}finally{if(t)throw t.error}}return{standardArray:i,inputTypeSnippets:this.catalogInputTypeSnippets(o)}},e.prototype.catalogInputTypeSnippets=function(e){var t,n,i={};try{for(var o=Object(w.i)(e),r=o.next();!r.done;r=o.next()){var a=r.value;void 0===i[a.subset]&&(i[a.subset]=[]),i[a.subset].push(a)}}catch(s){t={error:s}}finally{try{r&&!r.done&&(n=o.return)&&n.call(o)}finally{if(t)throw t.error}}return i},e.prototype.initSnippetsWithConfig=function(e,t,n){return(e=this.makeTree(e)).Content=Object.assign({},e.Content,{Fields:{},PresentationFields:{}}),t.TypeContent&&this.loadContentType(e.Content.Fields,t.TypeContent,"Content",t,n),t.TypeContentPresentation&&this.loadContentType(e.Content.PresentationFields,t.TypeContentPresentation,"Content.Presentation",t,n),t.HasList?(e.List=Object.assign({},e.List,{Fields:{},PresentationFields:{}}),t.TypeList&&this.loadContentType(e.List.Fields,t.TypeList,"Header",t,n),t.TypeListPresentation&&this.loadContentType(e.List.PresentationFields,t.TypeListPresentation,"Header.Presentation",t,n)):delete e.List,t.HasApp&&(e.App.Resources={},e.App.Settings={},this.loadContentType(e.App.Resources,"App-Resources","App.Resources",t,n),this.loadContentType(e.App.Settings,"App-Settings","App.Settings",t,n)),e},e.prototype.makeTree=function(e){var t,n,i={};try{for(var o=Object(w.i)(e),r=o.next();!r.done;r=o.next()){var a=r.value;void 0===i[a.set]&&(i[a.set]={}),void 0===i[a.set][a.subset]&&(i[a.set][a.subset]=[]);var s={key:a.name,label:this.label(a.set,a.subset,a.name),snip:a.content,help:a.help||this.help(a.set,a.subset,a.name),links:this.linksList(a.links)};i[a.set][a.subset].push(s)}}catch(c){t={error:c}}finally{try{r&&!r.done&&(n=o.return)&&n.call(o)}finally{if(t)throw t.error}}return i},e.prototype.label=function(e,t,n){var i=this.getHelpKey(e,t,n,".Key"),o=this.translate.instant(i);return o===i&&(o=n),o},e.prototype.getHelpKey=function(e,t,n,i){return"SourceEditorSnippets."+e+"."+t+"."+n+i},e.prototype.help=function(e,t,n){var i=this.getHelpKey(e,t,n,".Help"),o=this.translate.instant(i);return o===i&&(o=""),o},e.prototype.linksList=function(e){var t,n;if(!e)return null;var i=[],o=e.split("\n");try{for(var r=Object(w.i)(o),a=r.next();!a.done;a=r.next()){var s=a.value.split(":");3===s.length&&i.push({name:s[0].trim(),url:s[1].trim()+":"+s[2].trim()})}}catch(c){t={error:c}}finally{try{a&&!a.done&&(n=r.return)&&n.call(r)}finally{if(t)throw t.error}}return 0===i.length?null:i},e.prototype.loadContentType=function(e,t,n,i,o){var r=this;this.getFields(i.AppId,t).then((function(t){var a,s;try{for(var c=Object(w.i)(t),p=c.next();!p.done;p=c.next()){var l=p.value,u=l.StaticName;e[u]={key:u,label:u,snip:r.valuePlaceholder(n,u,i),help:l.Metadata.merged.Notes||" ("+l.Type.toLowerCase()+") "};var d=Object(P.a)(e[u]);r.attachSnippets(e,n,u,l.InputType,d,o)}}catch(g){a={error:g}}finally{try{p&&!p.done&&(s=c.return)&&s.call(c)}finally{if(a)throw a.error}}var f=["EntityId","EntityTitle","EntityGuid","EntityType","IsPublished","Modified"];if(t.length)for(var h=0;h<f.length;h++)e[f[h]]={key:f[h],label:f[h],snip:r.valuePlaceholder(n,f[h],i),help:r.translate.instant("SourceEditorSnippets.StandardFields."+f[h]+".Help")}}))},e.prototype.valuePlaceholder=function(e,t,n){return n.Type.indexOf("Razor")>-1?"@"+e+"."+t:"["+e.replace(".",":")+":"+t+"]"},e.prototype.getFields=function(e,t){return this.http.get(this.dnnContext.$2sxc.http.apiUrl("eav/contenttype/getfields"),{params:{appid:e.toString(),staticName:t}}).toPromise().then((function(e){var t,n;if(e=e.filter((function(e){return e.Type!==V.a.Empty})))try{for(var i=Object(w.i)(e),o=i.next();!o.done;o=i.next()){var r=o.value;if(r.Metadata){var a=r.Metadata,s=a.All,c=a[r.Type],p=a[r.InputType];a.merged=Object(w.a)(Object(w.a)(Object(w.a)({},s),c),p)}}}catch(l){t={error:l}}finally{try{o&&!o.done&&(n=i.return)&&n.call(i)}finally{if(t)throw t.error}}return e}))},e.prototype.attachSnippets=function(e,t,n,i,o,r){var a=r[i];if(i.indexOf("-")){var s=i.substr(0,i.indexOf("-"));if(s){var c=r[s];c&&(a=a?a.concat(c):c)}}if(a){void 0===e[n].more&&(e[n].more=[]);for(var p=e[n].more,l=0;l<a.length;l++)try{p[n+"-"+a[l].name]=Object.assign({},o,{key:n+" - "+a[l].name,label:a[l].name,snip:this.localizeGenericSnippet(a[l].content,t,n),collapse:!0})}finally{}}},e.prototype.localizeGenericSnippet=function(e,t,n){return e.replace(/(\$\{[0-9]+\:)var(\})/gi,"$1"+t+"$2").replace(/(\$\{[0-9]+\:)prop(\})/gi,"$1"+n+"$2")},e.\u0275fac=function(t){return new(t||e)(y.ac(o.b),y.ac(O.a),y.ac(a.e))},e.\u0275prov=y.Mb({token:e,factory:e.\u0275fac}),e}(),M=n("Ata6"),F=n("H0VJ"),B=n("+UDY"),j=n("Zfm5"),_=function(){function e(){}return e.prototype.transform=function(e){return null==e?e:(e.sort((function(e,t){return e.isFolder<t.isFolder?1:e.isFolder>t.isFolder?-1:0})),e)},e.\u0275fac=function(t){return new(t||e)},e.\u0275pipe=y.Pb({name:"sortItems",type:e,pure:!0}),e}(),W=function(){function e(e){this.sanitizer=e}return e.prototype.transform=function(e,t){return this.sanitizer.bypassSecurityTrustStyle("padding-left: "+(t?8*e:0===e?8:8*e+16)+"px;")},e.\u0275fac=function(t){return new(t||e)(y.Qb(M.b))},e.\u0275pipe=y.Pb({name:"depthPadding",type:e,pure:!0}),e}();function z(e,t){1&e&&y.Sb(0)}var E=function(e){return{item:e}};function L(e,t){if(1&e&&(y.Ub(0),y.Nc(1,z,1,0,"ng-container",5),y.Tb()),2&e){var n=t.$implicit;y.ic();var i=y.Bc(5);y.Bb(1),y.oc("ngTemplateOutlet",i)("ngTemplateOutletContext",y.tc(2,E,n))}}var A=function(e){return{active:e}};function D(e,t){if(1&e){var n=y.Xb();y.Wb(0,"div",8),y.ec("click",(function(){y.Dc(n);var e=y.ic().item;return y.ic().openTemplate(e.pathFromRoot)})),y.jc(1,"depthPadding"),y.Pc(2),y.Vb()}if(2&e){var i=y.ic().item,o=y.ic();y.Lc(y.lc(1,5,i.depth,i.isFolder)),y.oc("ngClass",y.tc(8,A,o.toggledItems.includes(i.pathFromRoot)))("matTooltip",i.name),y.Bb(2),y.Rc(" ",i.name," ")}}function N(e,t){1&e&&y.Sb(0)}function R(e,t){if(1&e&&(y.Ub(0),y.Nc(1,N,1,0,"ng-container",5),y.Tb()),2&e){var n=t.$implicit;y.ic(4);var i=y.Bc(5);y.Bb(1),y.oc("ngTemplateOutlet",i)("ngTemplateOutletContext",y.tc(2,E,n))}}function $(e,t){if(1&e&&(y.Ub(0),y.Nc(1,R,2,4,"ng-container",1),y.jc(2,"sortItems"),y.Tb()),2&e){var n=y.ic(2).item;y.Bb(1),y.oc("ngForOf",y.kc(2,1,n.children))}}function Q(e,t){if(1&e){var n=y.Xb();y.Wb(0,"div"),y.Wb(1,"div",9),y.ec("click",(function(){y.Dc(n);var e=y.ic().item;return y.ic().toggleItem(e.pathFromRoot)})),y.jc(2,"depthPadding"),y.Wb(3,"div",10),y.Wb(4,"mat-icon"),y.Pc(5),y.Vb(),y.Wb(6,"span"),y.Pc(7),y.Vb(),y.Vb(),y.Wb(8,"div",11),y.ec("click",(function(){y.Dc(n);var e=y.ic().item;return y.ic().addFile(e.pathFromRoot)})),y.Wb(9,"mat-icon"),y.Pc(10,"add"),y.Vb(),y.Vb(),y.Vb(),y.Nc(11,$,3,3,"ng-container",7),y.Vb()}if(2&e){var i=y.ic().item,o=y.ic();y.Bb(1),y.Lc(y.lc(2,6,i.depth,i.isFolder)),y.Bb(2),y.oc("matTooltip",i.name),y.Bb(2),y.Rc(" ",o.toggledItems.includes(i.pathFromRoot)?"keyboard_arrow_down":"keyboard_arrow_right"," "),y.Bb(2),y.Qc(i.name),y.Bb(4),y.oc("ngIf",o.toggledItems.includes(i.pathFromRoot))}}function H(e,t){if(1&e&&(y.Nc(0,D,3,10,"div",6),y.Nc(1,Q,12,9,"div",7)),2&e){var n=t.item;y.oc("ngIf",!n.isFolder),y.Bb(1),y.oc("ngIf",n.isFolder)}}var K=function(){function e(e){this.dialogService=e,this.createTemplate=new y.n,this.toggledItems=[]}return e.prototype.ngOnInit=function(){},e.prototype.ngOnChanges=function(e){var t,n;(null===(t=e.templates)||void 0===t?void 0:t.currentValue)&&(this.tree=function(e){var t,n;if(!e)return[];var i=[];try{for(var o=Object(w.i)(e),r=o.next();!r.done;r=o.next())for(var a=i,s=r.value.split("/"),c=s[s.length-1],p="",l=function(e){var t=s[e];p+=e?"/"+t:t;var n=a.find((function(e){return e.name===t}));if(n)a=n.children;else{var i=Object(w.a)({depth:e,name:t,pathFromRoot:p,isFolder:t!==c},t!==c&&{children:[]});a.push(i),a=i.children}},u=0;u<s.length;u++)l(u)}catch(d){t={error:d}}finally{try{r&&!r.done&&(n=o.return)&&n.call(o)}finally{if(t)throw t.error}}return i}(this.templates)),(null===(n=e.view)||void 0===n?void 0:n.currentValue)&&this.showFileInTree(this.view.FileName)},e.prototype.openTemplate=function(e){this.dialogService.openCodeFile(e)},e.prototype.toggleItem=function(e){var t,n,i;-1===(i=(n=this.toggledItems).indexOf(t=e))?n.push(t):n.splice(i,1)},e.prototype.addFile=function(e){this.createTemplate.emit(e)},e.prototype.showFileInTree=function(e){var t,n;if(null!=e&&!this.toggledItems.includes(e)){var i=e.split("/"),o="";try{for(var r=Object(w.i)(i),a=r.next();!a.done;a=r.next()){var s=a.value;this.toggledItems.includes(o=o?o+"/"+s:s)||this.toggleItem(o)}}catch(c){t={error:c}}finally{try{a&&!a.done&&(n=r.return)&&n.call(r)}finally{if(t)throw t.error}}}},e.\u0275fac=function(t){return new(t||e)(y.Qb(F.a))},e.\u0275cmp=y.Kb({type:e,selectors:[["app-code-templates"]],inputs:{view:"view",templates:"templates"},outputs:{createTemplate:"createTemplate"},features:[y.zb],decls:10,vars:3,consts:[[1,"editor-active-explorer","editor-fancy-scrollbar","explorer-wrapper"],[4,"ngFor","ngForOf"],["fileOrFolder",""],[1,"create-button-wrapper"],["mat-icon-button","","matTooltip","Create file",3,"click"],[4,"ngTemplateOutlet","ngTemplateOutletContext"],["class","file","matTooltipShowDelay","750",3,"style","ngClass","matTooltip","click",4,"ngIf"],[4,"ngIf"],["matTooltipShowDelay","750",1,"file",3,"ngClass","matTooltip","click"],[1,"folder-name-wrapper",3,"click"],["matTooltipShowDelay","750",1,"folder-name",3,"matTooltip"],["matTooltipShowDelay","750","matTooltip","Create file in this folder","appClickStopPropagation","",1,"add-item",3,"click"]],template:function(e,t){1&e&&(y.Wb(0,"div",0),y.Wb(1,"div"),y.Nc(2,L,2,4,"ng-container",1),y.jc(3,"sortItems"),y.Nc(4,H,2,2,"ng-template",null,2,y.Oc),y.Vb(),y.Wb(6,"div",3),y.Wb(7,"button",4),y.ec("click",(function(){return t.addFile()})),y.Wb(8,"mat-icon"),y.Pc(9,"add"),y.Vb(),y.Vb(),y.Vb(),y.Vb()),2&e&&(y.Bb(2),y.oc("ngForOf",y.kc(3,1,t.tree)))},directives:[i.s,p.b,u.a,l.a,i.A,i.t,i.q,j.a],pipes:[_,W],styles:[".explorer-wrapper[_ngcontent-%COMP%]{display:flex;flex-direction:column;justify-content:space-between}.create-button-wrapper[_ngcontent-%COMP%]{display:flex;justify-content:flex-end}.file[_ngcontent-%COMP%]{cursor:pointer;padding:4px 0 4px 8px;overflow:hidden;white-space:nowrap;text-overflow:ellipsis;-webkit-user-select:none;-moz-user-select:none;-ms-user-select:none;user-select:none}.file[_ngcontent-%COMP%]:hover{background-color:#2a2d2e}.file.active[_ngcontent-%COMP%]{background-color:#37373d}.folder-name-wrapper[_ngcontent-%COMP%]{display:flex;cursor:pointer;-webkit-user-select:none;-moz-user-select:none;-ms-user-select:none;user-select:none;align-items:center}.folder-name-wrapper[_ngcontent-%COMP%]   .mat-icon[_ngcontent-%COMP%]{width:18px;height:18px;font-size:18px}.folder-name-wrapper[_ngcontent-%COMP%]   .folder-name[_ngcontent-%COMP%]{display:flex;align-items:center;overflow:hidden;white-space:nowrap;padding:3px 0;flex:1 1 auto}.folder-name-wrapper[_ngcontent-%COMP%]   .add-item[_ngcontent-%COMP%]{margin-right:4px;display:none;align-items:center;justify-content:center;padding:3px}.folder-name-wrapper[_ngcontent-%COMP%]:hover{background-color:#2a2d2e}.folder-name-wrapper[_ngcontent-%COMP%]:hover   .add-item[_ngcontent-%COMP%]{display:flex}"],changeDetection:0}),e}();function U(e,t){var n=t.indexOf(e);-1===n?t.push(e):t.splice(n,1)}var X=function(){function e(){}return e.prototype.transform=function(e){return typeof e!=typeof{}?e:Object.keys(e).map((function(t){return e[t]}))},e.\u0275fac=function(t){return new(t||e)},e.\u0275pipe=y.Pb({name:"objectToArray",type:e,pure:!0}),e}(),q=n("KSFr");function G(e,t){if(1&e){var n=y.Xb();y.Wb(0,"mat-icon",12),y.ec("click",(function(){y.Dc(n);var e=y.ic().$implicit;return y.ic(5).toggleMore(e)})),y.Pc(1),y.Vb()}if(2&e){var i=y.ic().$implicit,o=y.ic(5);y.Bb(1),y.Rc(" ",o.toggledMores.includes(i)?"more_vert":"more_horiz"," ")}}function J(e,t){if(1&e&&(y.Wb(0,"div"),y.Wb(1,"a",15),y.Pc(2),y.Vb(),y.Vb()),2&e){var n=t.$implicit;y.Bb(1),y.oc("href",n.url,y.Gc),y.Bb(1),y.Qc(n.name)}}function Y(e,t){if(1&e&&(y.Wb(0,"div",13),y.Rb(1,"div",14),y.jc(2,"safeHtml"),y.Nc(3,J,3,2,"div",1),y.Vb()),2&e){var n=y.ic().$implicit;y.Bb(1),y.oc("innerHTML",y.kc(2,2,n.help),y.Ec),y.Bb(2),y.oc("ngForOf",n.links)}}function Z(e,t){if(1&e&&(y.Wb(0,"div"),y.Wb(1,"a",15),y.Pc(2),y.Vb(),y.Vb()),2&e){var n=t.$implicit;y.Bb(1),y.oc("href",n.url,y.Gc),y.Bb(1),y.Qc(n.name)}}function ee(e,t){if(1&e&&(y.Wb(0,"div",18),y.Rb(1,"div",14),y.jc(2,"safeHtml"),y.Nc(3,Z,3,2,"div",1),y.Vb()),2&e){var n=y.ic().$implicit;y.Bb(1),y.oc("innerHTML",y.kc(2,2,n.help),y.Ec),y.Bb(2),y.oc("ngForOf",n.links)}}function te(e,t){if(1&e){var n=y.Xb();y.Wb(0,"div"),y.Wb(1,"div",7),y.Wb(2,"div",16),y.ec("click",(function(){y.Dc(n);var e=t.$implicit;return y.ic(7).addSnippet(e.snip)})),y.Pc(3),y.Vb(),y.Wb(4,"mat-icon",10),y.ec("click",(function(){y.Dc(n);var e=t.$implicit;return y.ic(7).toggleInfo(e)})),y.Pc(5," info "),y.Vb(),y.Vb(),y.Nc(6,ee,4,4,"div",17),y.Vb()}if(2&e){var i=t.$implicit,o=y.ic(7);y.Bb(2),y.oc("matTooltip",i.snip),y.Bb(1),y.Rc(" ",i.label," "),y.Bb(3),y.oc("ngIf",o.toggledInfos.includes(i))}}function ne(e,t){if(1&e&&(y.Ub(0),y.Nc(1,te,7,3,"div",1),y.jc(2,"objectToArray"),y.Tb()),2&e){var n=y.ic().$implicit;y.Bb(1),y.oc("ngForOf",y.kc(2,1,n.more))}}function ie(e,t){if(1&e){var n=y.Xb();y.Wb(0,"div",6),y.Wb(1,"div",7),y.Wb(2,"div",8),y.ec("click",(function(){y.Dc(n);var e=t.$implicit;return y.ic(5).addSnippet(e.snip)})),y.Pc(3),y.Vb(),y.Nc(4,G,2,1,"mat-icon",9),y.Wb(5,"mat-icon",10),y.ec("click",(function(){y.Dc(n);var e=t.$implicit;return y.ic(5).toggleInfo(e)})),y.Pc(6," info "),y.Vb(),y.Vb(),y.Nc(7,Y,4,4,"div",11),y.Nc(8,ne,3,3,"ng-container",3),y.Vb()}if(2&e){var i=t.$implicit,o=y.ic(5);y.Bb(2),y.oc("matTooltip",i.snip),y.Bb(1),y.Rc(" ",i.label," "),y.Bb(1),y.oc("ngIf",i.more),y.Bb(3),y.oc("ngIf",o.toggledInfos.includes(i)),y.Bb(1),y.oc("ngIf",o.toggledMores.includes(i))}}function oe(e,t){if(1&e&&(y.Ub(0),y.Nc(1,ie,9,5,"div",5),y.jc(2,"objectToArray"),y.Tb()),2&e){var n=y.ic().$implicit;y.Bb(1),y.oc("ngForOf",y.kc(2,1,n.value))}}function re(e,t){if(1&e){var n=y.Xb();y.Wb(0,"div"),y.Wb(1,"div",4),y.ec("click",(function(){y.Dc(n);var e=t.$implicit;return y.ic(3).toggleFolder(e)})),y.jc(2,"translate"),y.Wb(3,"mat-icon"),y.Pc(4),y.Vb(),y.Wb(5,"span"),y.Pc(6),y.jc(7,"translate"),y.Vb(),y.Vb(),y.Nc(8,oe,3,3,"ng-container",3),y.Vb()}if(2&e){var i=t.$implicit,o=y.ic(2).$implicit,r=y.ic();y.Bb(1),y.oc("matTooltip",y.kc(2,4,"SourceEditorSnippets."+o.key+"."+i.key+".Help")),y.Bb(3),y.Qc(r.toggledFolders.includes(i)?"keyboard_arrow_down":"keyboard_arrow_right"),y.Bb(2),y.Qc(y.kc(7,6,"SourceEditorSnippets."+o.key+"."+i.key+".Title")),y.Bb(2),y.oc("ngIf",r.toggledFolders.includes(i))}}function ae(e,t){if(1&e&&(y.Ub(0),y.Nc(1,re,9,8,"div",1),y.jc(2,"keyvalue"),y.Tb()),2&e){var n=y.ic().$implicit;y.Bb(1),y.oc("ngForOf",y.kc(2,1,n.value))}}function se(e,t){if(1&e){var n=y.Xb();y.Wb(0,"div"),y.Wb(1,"div",2),y.ec("click",(function(){y.Dc(n);var e=t.$implicit;return y.ic().toggleSection(e)})),y.jc(2,"translate"),y.Wb(3,"mat-icon"),y.Pc(4),y.Vb(),y.Wb(5,"span"),y.Pc(6),y.jc(7,"translate"),y.Vb(),y.Vb(),y.Nc(8,ae,3,3,"ng-container",3),y.Vb()}if(2&e){var i=t.$implicit,o=y.ic();y.Bb(1),y.oc("matTooltip",y.kc(2,4,"SourceEditorSnippets."+i.key+".Help")),y.Bb(3),y.Qc(o.toggledSections.includes(i)?"keyboard_arrow_down":"keyboard_arrow_right"),y.Bb(2),y.Qc(y.kc(7,6,"SourceEditorSnippets."+i.key+".Title")),y.Bb(2),y.oc("ngIf",o.toggledSections.includes(i))}}var ce=function(){function e(){this.insertSnippet=new y.n,this.toggledSections=[],this.toggledFolders=[],this.toggledInfos=[],this.toggledMores=[]}return e.prototype.ngOnInit=function(){},e.prototype.addSnippet=function(e){this.insertSnippet.emit(e)},e.prototype.toggleSection=function(e){U(e,this.toggledSections)},e.prototype.toggleFolder=function(e){U(e,this.toggledFolders)},e.prototype.toggleInfo=function(e){U(e,this.toggledInfos)},e.prototype.toggleMore=function(e){U(e,this.toggledMores)},e.\u0275fac=function(t){return new(t||e)},e.\u0275cmp=y.Kb({type:e,selectors:[["app-code-snippets"]],inputs:{snippets:"snippets"},outputs:{insertSnippet:"insertSnippet"},decls:3,vars:3,consts:[[1,"editor-active-explorer","editor-fancy-scrollbar"],[4,"ngFor","ngForOf"],["matTooltipShowDelay","750",1,"collapsible-header","section",3,"matTooltip","click"],[4,"ngIf"],["matTooltipShowDelay","750",1,"collapsible-header","folder",3,"matTooltip","click"],["class","snippet",4,"ngFor","ngForOf"],[1,"snippet"],[1,"snippet-label"],["matRipple","","matTooltipShowDelay","750",1,"text",3,"matTooltip","click"],["matTooltip","Show More","matTooltipShowDelay","750",3,"click",4,"ngIf"],["matTooltip","Show Help","matTooltipShowDelay","750","appClickStopPropagation","",3,"click"],["class","snippet-help",4,"ngIf"],["matTooltip","Show More","matTooltipShowDelay","750",3,"click"],[1,"snippet-help"],[1,"text",3,"innerHTML"],["target","_blank",3,"href"],["matRipple","","matTooltipShowDelay","750",1,"text","text-more",3,"matTooltip","click"],["class","snippet-help snippet-help-more",4,"ngIf"],[1,"snippet-help","snippet-help-more"]],template:function(e,t){1&e&&(y.Wb(0,"div",0),y.Nc(1,se,9,8,"div",1),y.jc(2,"keyvalue"),y.Vb()),2&e&&(y.Bb(1),y.oc("ngForOf",y.kc(2,1,t.snippets)))},directives:[i.s,u.a,l.a,i.t,h.q,j.a],pipes:[i.l,a.d,X,q.a],styles:[".collapsible-header[_ngcontent-%COMP%]{display:flex;align-items:center;cursor:pointer;overflow:hidden;white-space:nowrap;-webkit-user-select:none;-moz-user-select:none;-ms-user-select:none;user-select:none;padding:3px 0}.collapsible-header.section[_ngcontent-%COMP%]{background-color:#383838;font-weight:500}.collapsible-header.folder[_ngcontent-%COMP%]{padding-left:8px}.collapsible-header.folder[_ngcontent-%COMP%]:hover{background-color:#2a2d2e}.collapsible-header[_ngcontent-%COMP%]   .mat-icon[_ngcontent-%COMP%]{width:18px;height:18px;font-size:18px}.snippet[_ngcontent-%COMP%]{padding-right:8px}.snippet[_ngcontent-%COMP%]:hover{background-color:#2a2d2e}.snippet-label[_ngcontent-%COMP%]{display:flex;align-items:center;justify-content:space-between;-webkit-user-select:none;-moz-user-select:none;-ms-user-select:none;user-select:none}.snippet-label[_ngcontent-%COMP%]   .text[_ngcontent-%COMP%]{cursor:pointer;padding:3px 0 3px 32px;flex:1 1 auto;overflow:hidden;white-space:nowrap;text-overflow:ellipsis}.snippet-label[_ngcontent-%COMP%]   .text-more[_ngcontent-%COMP%]{padding-left:40px}.snippet-label[_ngcontent-%COMP%]   .mat-icon[_ngcontent-%COMP%]{cursor:pointer;width:18px;height:18px;font-size:18px}.snippet-help[_ngcontent-%COMP%]{padding:3px 18px 3px 32px}.snippet-help-more[_ngcontent-%COMP%]{padding-left:40px}.snippet-help[_ngcontent-%COMP%]   .text[_ngcontent-%COMP%]{font-size:12px;font-style:oblique;word-break:break-word}.snippet-help[_ngcontent-%COMP%]   .text[_ngcontent-%COMP%]     p{margin:0}"],changeDetection:0}),e}(),pe={mode:"ace/mode/razor",wrap:!0,useSoftTabs:!0,theme:"ace/theme/sqlserver",fontSize:14,fontFamily:"Consolas, Courier New, monospace",showGutter:!0,enableBasicAutocompletion:!0,enableLiveAutocompletion:!0,enableSnippets:!0},le=["editor"],ue=function(){function e(e){this.zone=e,this.value="",this.propagateChange=function(){},this.propagateTouched=function(){}}return e.prototype.ngOnInit=function(){!function e(t,n,i){void 0===i&&(i=0);var o=t.length===i+1?n:e.bind(this,t,n,i+1),r=t[i];if(window[r.globalVar])n();else{var a=document.querySelector('script[src="'+r.src+'"]');if(a)a.addEventListener("load",o,{once:!0});else{var s=document.createElement("script");s.src=r.src,s.addEventListener("load",o,{once:!0}),document.head.appendChild(s)}}}([{globalVar:"ace",src:"https://cdnjs.cloudflare.com/ajax/libs/ace/1.4.11/ace.min.js"},{globalVar:null,src:"https://cdnjs.cloudflare.com/ajax/libs/ace/1.4.11/ext-modelist.min.js"},{globalVar:null,src:"https://cdnjs.cloudflare.com/ajax/libs/ace/1.4.11/ext-language_tools.min.js"}],this.aceLoaded.bind(this))},e.prototype.ngOnChanges=function(e){var t,n,i,o,r,a=this,s=null===(t=e.filename)||void 0===t?void 0:t.currentValue,c=null===(n=e.snippets)||void 0===n?void 0:n.currentValue;if(this.updateValues(s,c),this.editor){(null===(i=e.toggleResize)||void 0===i?void 0:i.currentValue)!==(null===(o=e.toggleResize)||void 0===o?void 0:o.previousValue)&&this.zone.runOutsideAngular((function(){setTimeout((function(){a.editor.resize()}),50)}));var p=null===(r=e.insertSnippet)||void 0===r?void 0:r.currentValue;p&&this.zone.runOutsideAngular((function(){ace.require("ace/snippets").snippetManager.insertSnippet(a.editor,p),a.editor.focus()}))}},e.prototype.writeValue=function(e){var t=this;this.value=e||"",this.editor&&this.zone.runOutsideAngular((function(){var e=t.editor.getCursorPosition();t.editor.setValue(t.value,-1),t.editor.moveCursorToPosition(e)}))},e.prototype.registerOnChange=function(e){this.propagateChange=e},e.prototype.registerOnTouched=function(e){this.propagateTouched=e},e.prototype.ngOnDestroy=function(){var e=this;this.zone.runOutsideAngular((function(){e.editor.destroy(),e.editor.container.remove(),e.editor=null}))},e.prototype.aceLoaded=function(){var e=this;this.zone.runOutsideAngular((function(){ace.config.set("basePath","https://cdnjs.cloudflare.com/ajax/libs/ace/1.4.11"),e.editor=ace.edit(e.editorRef.nativeElement,pe),e.editor.$blockScrolling=1/0,e.editor.session.setValue(e.value),e.updateValues(e.filename,e.snippets),e.editor.on("change",e.onEditorValueChange.bind(e)),e.editor.on("blur",e.onEditorBlurred.bind(e)),e.editor.focus()}))},e.prototype.onEditorValueChange=function(){var e=this;this.zone.run((function(){e.propagateChange(e.editor.getValue())}))},e.prototype.onEditorBlurred=function(){var e=this;this.zone.run((function(){e.propagateTouched(e.editor.getValue())}))},e.prototype.updateValues=function(e,t){var n=this;this.editor&&this.zone.runOutsideAngular((function(){if(e){var i=ace.require("ace/ext/modelist").getModeForPath(e).mode;n.editor.session.setMode(i)}t&&ace.require("ace/snippets").snippetManager.register(n.snippets)}))},e.\u0275fac=function(t){return new(t||e)(y.Qb(y.z))},e.\u0275cmp=y.Kb({type:e,selectors:[["app-ace-editor"]],viewQuery:function(e,t){var n;1&e&&y.Vc(le,!0),2&e&&y.Ac(n=y.fc())&&(t.editorRef=n.first)},inputs:{filename:"filename",snippets:"snippets",insertSnippet:"insertSnippet",toggleResize:"toggleResize"},features:[y.Ab([{provide:r.p,useExisting:Object(y.T)((function(){return e})),multi:!0}]),y.zb],decls:2,vars:0,consts:[[2,"width","100%","height","100%"],["editor",""]],template:function(e,t){1&e&&y.Rb(0,"div",0,1)},styles:["[_nghost-%COMP%] {display: block; width: 100%; height: 100%}"],changeDetection:0}),e}();function de(e,t){if(1&e){var n=y.Xb();y.Wb(0,"app-ace-editor",8),y.ec("ngModelChange",(function(e){return y.Dc(n),y.ic().view.Code=e})),y.Vb()}if(2&e){var i=y.ic();y.oc("filename",i.view.FileName)("ngModel",i.view.Code)("snippets",i.editorSnipps)("insertSnippet",i.insertSnipp)("toggleResize",!i.activeExplorer)}}var fe=function(e){return{active:e}},he=[{path:"",component:function(){function e(e,t,n,i,o,r,a,s,c,p){this.context=e,this.route=t,this.snackBar=n,this.snackBarStack=i,this.sourceService=o,this.snippetsService=r,this.zone=a,this.titleService=s,this.dialogService=c,this.sanitizeService=p,this.explorer={templates:"templates",snippets:"snippets"},this.activeExplorer=this.explorer.templates,this.eventListeners=[],this.context.init(this.route),this.calculateViewKey(),this.attachListeners()}return e.prototype.ngOnInit=function(){var e=this;Object(b.a)({view:this.sourceService.get(this.viewKey),templates:this.sourceService.getTemplates()}).subscribe((function(t){e.view=t.view,e.savedCode=e.view.Code,e.titleService.setTitle(e.view.FileName+" - Code Editor"),e.templates=t.templates,e.showCodeAndEditionWarnings(t.view,t.templates),e.snippetsService.getSnippets(e.view).then((function(t){e.explorerSnipps=t.sets,e.editorSnipps=t.list}))}))},e.prototype.ngOnDestroy=function(){this.detachListeners()},e.prototype.toggleExplorer=function(e){this.activeExplorer=this.activeExplorer===e?null:e},e.prototype.createTemplate=function(e){var t=this,n="File name:",i=m.b;("api"===e||(null==e?void 0:e.startsWith("api/")))&&(n="Controller name:",i=m.a);var o=prompt(n,i);null!==o&&0!==o.length&&(o=this.sanitizeService.sanitizePath(o),null!=e&&(o=e+"/"+o),this.sourceService.createTemplate(o).subscribe((function(e){t.sourceService.getTemplates().subscribe((function(e){t.templates=e}))})))},e.prototype.changeInsertSnipp=function(e){this.insertSnipp=e},e.prototype.save=function(){var e=this;this.snackBar.open("Saving...");var t=this.view.Code;this.sourceService.save(this.viewKey,this.view).subscribe({next:function(n){n?(e.savedCode=t,t=null,e.snackBar.open("Saved",null,{duration:2e3})):e.snackBar.open("Failed",null,{duration:2e3})},error:function(){e.snackBar.open("Failed",null,{duration:2e3})}})},e.prototype.calculateViewKey=function(){var e=sessionStorage.getItem(v.i),t=JSON.parse(e)[0];this.viewKey=t.EntityId||t.Path},e.prototype.showCodeAndEditionWarnings=function(e,t){var n=this,i=e.FileName,o=i.indexOf("/")>-1?i.lastIndexOf("/")+1:0,r=0===o?"":i.substring(0,o),a=i.substring(o),s=a.substring(0,a.length-e.Extension.length)+".code"+e.Extension,c=t.find((function(e){return e===r+s})),p=t.filter((function(e){return e.endsWith(a)})).length-1;c&&this.snackBarStack.add("This template also has a code-behind file '"+c+"'.","Open").subscribe((function(){n.dialogService.openCodeFile(c)})),p&&this.snackBarStack.add("There are "+p+" other editions of this. You may be editing an edition which is not the one you see.","Help").subscribe((function(){window.open("https://r.2sxc.org/polymorphism","_blank")}))},e.prototype.attachListeners=function(){var e=this;this.zone.runOutsideAngular((function(){var t=e.stopClose.bind(e),n=e.keyboardSave.bind(e);window.addEventListener("beforeunload",t),window.addEventListener("keydown",n),e.eventListeners.push({element:window,type:"beforeunload",listener:t}),e.eventListeners.push({element:window,type:"keydown",listener:n})}))},e.prototype.detachListeners=function(){var e=this;this.zone.runOutsideAngular((function(){e.eventListeners.forEach((function(e){e.element.removeEventListener(e.type,e.listener)})),e.eventListeners=null}))},e.prototype.stopClose=function(e){this.savedCode!==this.view.Code&&(e.preventDefault(),e.returnValue="")},e.prototype.keyboardSave=function(e){var t=this;83===e.keyCode&&(navigator.platform.match("Mac")?e.metaKey:e.ctrlKey)&&(e.preventDefault(),this.zone.run((function(){t.save()})))},e.\u0275fac=function(t){return new(t||e)(y.Qb(x.a),y.Qb(g.a),y.Qb(d.b),y.Qb(C),y.Qb(T),y.Qb(I),y.Qb(y.z),y.Qb(M.d),y.Qb(F.a),y.Qb(B.a))},e.\u0275cmp=y.Kb({type:e,selectors:[["app-code-editor"]],decls:14,vars:12,consts:[[1,"editor-root"],[1,"editor-side-toolbar"],["matTooltip","Templates",1,"button",3,"ngClass","click"],["matTooltip","Snippets",1,"button",3,"ngClass","click"],[3,"hidden","view","templates","createTemplate"],[3,"hidden","snippets","insertSnippet"],[3,"filename","ngModel","snippets","insertSnippet","toggleResize","ngModelChange",4,"ngIf"],["mat-fab","","mat-elevation-z24","","matTooltip","Click to save or CTRL + S",3,"click"],[3,"filename","ngModel","snippets","insertSnippet","toggleResize","ngModelChange"]],template:function(e,t){1&e&&(y.Wb(0,"div",0),y.Wb(1,"div",1),y.Wb(2,"div",2),y.ec("click",(function(){return t.toggleExplorer(t.explorer.templates)})),y.Wb(3,"mat-icon"),y.Pc(4,"file_copy"),y.Vb(),y.Vb(),y.Wb(5,"div",3),y.ec("click",(function(){return t.toggleExplorer(t.explorer.snippets)})),y.Wb(6,"mat-icon"),y.Pc(7,"code"),y.Vb(),y.Vb(),y.Vb(),y.Wb(8,"app-code-templates",4),y.ec("createTemplate",(function(e){return t.createTemplate(e)})),y.Vb(),y.Wb(9,"app-code-snippets",5),y.ec("insertSnippet",(function(e){return t.changeInsertSnipp(e)})),y.Vb(),y.Nc(10,de,1,5,"app-ace-editor",6),y.Wb(11,"button",7),y.ec("click",(function(){return t.save()})),y.Wb(12,"mat-icon"),y.Pc(13,"done"),y.Vb(),y.Vb(),y.Vb()),2&e&&(y.Bb(2),y.oc("ngClass",y.tc(8,fe,t.activeExplorer===t.explorer.templates)),y.Bb(3),y.oc("ngClass",y.tc(10,fe,t.activeExplorer===t.explorer.snippets)),y.Bb(3),y.oc("hidden",t.activeExplorer!==t.explorer.templates)("view",t.view)("templates",t.templates),y.Bb(1),y.oc("hidden",t.activeExplorer!==t.explorer.snippets)("snippets",t.explorerSnipps),y.Bb(1),y.oc("ngIf",t.view))},directives:[u.a,i.q,l.a,K,ce,i.t,p.b,ue,r.r,r.u],styles:[".mat-fab[_ngcontent-%COMP%]{position:absolute;right:48px;bottom:32px;z-index:10}"]}),e}()}],ge=function(){function e(){}return e.\u0275mod=y.Ob({type:e}),e.\u0275inj=y.Nb({factory:function(t){return new(t||e)},imports:[[g.g.forChild(he)],g.g]}),e}(),be=n("O6Xb");function ve(e){return new s.a(e,"./i18n/code-editor.",".js?"+sxcVersion)}n.d(t,"translateLoaderFactory",(function(){return ve})),n.d(t,"CodeEditorModule",(function(){return me}));var me=function(){function e(){}return e.\u0275mod=y.Ob({type:e}),e.\u0275inj=y.Nb({factory:function(t){return new(t||e)},providers:[x.a,T,F.a,I,B.a,a.e],imports:[[ge,be.a,i.c,c.g,p.c,l.b,u.b,d.d,r.l,f.b,h.r,a.c.forChild({loader:{provide:a.b,useFactory:ve,deps:[o.b]},defaultLanguage:"en",isolate:!0})]]}),e}()}}]);
//# sourceMappingURL=https://sources.2sxc.org/11.03.00/ng-edit/code-editor-code-editor-module.7fd95889d780ed664fcf.js.map