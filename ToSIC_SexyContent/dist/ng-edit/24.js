(window.webpackJsonp=window.webpackJsonp||[]).push([[24],{BKxq:function(t,n,e){"use strict";e.r(n),e.d(n,"ExportAppPartsComponent",(function(){return T}));var o=e("D57K"),i=e("zsK+"),c=e("1C3z"),r=e("BLjT"),p=e("nPbx"),a=e("hOvr"),s=e("LuBX"),l=e("ZSGP"),b=e("8AiQ"),f=e("OeRG"),u=e("9HSk"),h=e("Qc/f"),g=e("Uk43"),d=e("udnq"),m=e("r4gC"),x=e("mPmy");function v(t,n){if(1&t&&(c.Wb(0,"mat-option",13),c.Pc(1),c.Vb()),2&t){var e=n.$implicit;c.qc("value",e.value),c.Bb(1),c.Rc("",e.name," ")}}function _(t,n){if(1&t){var e=c.Xb();c.Wb(0,"mat-icon",14),c.ec("click",(function(t){return c.Ec(e),c.ic().unlockScope(t)})),c.Pc(1,"lock"),c.Vb()}}function C(t,n){if(1&t){var e=c.Xb();c.Wb(0,"mat-icon",14),c.ec("click",(function(t){return c.Ec(e),c.ic().unlockScope(t)})),c.Pc(1,"lock_open"),c.Vb()}}function P(t,n){if(1&t){var e=c.Xb();c.Wb(0,"li",21),c.Wb(1,"div",22),c.Wb(2,"mat-checkbox",23),c.ec("ngModelChange",(function(t){return c.Ec(e),n.$implicit._export=t})),c.Wb(3,"span",24),c.Pc(4),c.Vb(),c.Vb(),c.Vb(),c.Vb()}if(2&t){var o=n.$implicit;c.Bb(2),c.qc("ngModel",o._export),c.Bb(2),c.Sc("",o.Name," (",o.Id,")")}}function W(t,n){if(1&t&&(c.Wb(0,"ul",26),c.Wb(1,"p",27),c.Pc(2,"Templates"),c.Vb(),c.Nc(3,P,5,3,"li",17),c.Vb()),2&t){var e=c.ic().$implicit;c.Bb(3),c.qc("ngForOf",e.Templates)}}function y(t,n){if(1&t){var e=c.Xb();c.Wb(0,"li",21),c.Wb(1,"div",22),c.Wb(2,"mat-checkbox",23),c.ec("ngModelChange",(function(t){return c.Ec(e),n.$implicit._export=t})),c.Wb(3,"span",24),c.Pc(4),c.Vb(),c.Vb(),c.Vb(),c.Vb()}if(2&t){var o=n.$implicit;c.Bb(2),c.qc("ngModel",o._export),c.Bb(2),c.Sc("",o.Title," (",o.Id,")")}}function S(t,n){if(1&t&&(c.Wb(0,"ul",26),c.Wb(1,"p",27),c.Pc(2,"Entities"),c.Vb(),c.Nc(3,y,5,3,"li",17),c.Vb()),2&t){var e=c.ic().$implicit;c.Bb(3),c.qc("ngForOf",e.Entities)}}function V(t,n){if(1&t){var e=c.Xb();c.Wb(0,"li",21),c.Wb(1,"div",22),c.Wb(2,"mat-checkbox",23),c.ec("ngModelChange",(function(t){return c.Ec(e),n.$implicit._export=t})),c.Wb(3,"span",24),c.Pc(4),c.Vb(),c.Vb(),c.Vb(),c.Nc(5,W,4,1,"ul",25),c.Nc(6,S,4,1,"ul",25),c.Vb()}if(2&t){var o=n.$implicit;c.Bb(2),c.qc("ngModel",o._export),c.Bb(2),c.Sc("",o.Name," (",o.Id,")"),c.Bb(1),c.qc("ngIf",o.Templates.length>0),c.Bb(1),c.qc("ngIf",o.Entities.length>0)}}function k(t,n){if(1&t){var e=c.Xb();c.Wb(0,"li",21),c.Wb(1,"div",22),c.Wb(2,"mat-checkbox",23),c.ec("ngModelChange",(function(t){return c.Ec(e),n.$implicit._export=t})),c.Wb(3,"span",24),c.Pc(4),c.Vb(),c.Vb(),c.Vb(),c.Vb()}if(2&t){var o=n.$implicit;c.Bb(2),c.qc("ngModel",o._export),c.Bb(2),c.Sc("",o.Name," (",o.Id,")")}}function O(t,n){if(1&t){var e=c.Xb();c.Wb(0,"div"),c.Wb(1,"ul",15),c.Wb(2,"p",16),c.Pc(3,"Content Types"),c.Vb(),c.Nc(4,V,7,5,"li",17),c.Vb(),c.Wb(5,"ul",15),c.Wb(6,"p",16),c.Pc(7,"Templates Without Content Types"),c.Vb(),c.Nc(8,k,5,3,"li",17),c.Vb(),c.Wb(9,"div",18),c.Wb(10,"button",19),c.ec("click",(function(){return c.Ec(e),c.ic().closeDialog()})),c.Pc(11,"Cancel"),c.Vb(),c.Wb(12,"button",20),c.ec("click",(function(){return c.Ec(e),c.ic().exportAppParts()})),c.Pc(13,"Export"),c.Vb(),c.Vb(),c.Vb()}if(2&t){var o=c.ic();c.Bb(4),c.qc("ngForOf",o.contentInfo.ContentTypes),c.Bb(4),c.qc("ngForOf",o.contentInfo.TemplatesWithoutContentTypes),c.Bb(2),c.qc("disabled",o.isExporting),c.Bb(2),c.qc("disabled",o.isExporting)}}var T=function(){function t(t,n){this.dialogRef=t,this.exportAppPartsService=n,this.exportScope=i.a.scopes.default.value,this.lockScope=!0,this.isExporting=!1}return t.prototype.ngOnInit=function(){this.scopeOptions=Object.keys(i.a.scopes).map((function(t){return i.a.scopes[t]})),this.fetchContentInfo()},t.prototype.exportAppParts=function(){this.isExporting=!0;var t=this.selectedContentTypes().map((function(t){return t.Id})),n=this.selectedTemplates().map((function(t){return t.Id})),e=this.selectedEntities().map((function(t){return t.Id}));e=e.concat(n),this.exportAppPartsService.exportParts(t,e,n),this.isExporting=!1},t.prototype.changeScope=function(t){var n=t.value;"Other"===n&&((n=prompt("This is an advanced feature to show content-types of another scope. Don't use this if you don't know what you're doing, as content-types of other scopes are usually hidden for a good reason."))?this.scopeOptions.find((function(t){return t.value===n}))||this.scopeOptions.push({name:n,value:n}):n=i.a.scopes.default.value),this.exportScope=n,this.fetchContentInfo()},t.prototype.unlockScope=function(t){t.stopPropagation(),this.lockScope=!this.lockScope,this.lockScope&&(this.exportScope=i.a.scopes.default.value,this.fetchContentInfo())},t.prototype.closeDialog=function(){this.dialogRef.close()},t.prototype.fetchContentInfo=function(){var t=this;this.exportAppPartsService.getContentInfo(this.exportScope).subscribe((function(n){t.contentInfo=n}))},t.prototype.selectedContentTypes=function(){return this.contentInfo.ContentTypes.filter((function(t){return t._export}))},t.prototype.selectedEntities=function(){var t,n,e=[];try{for(var i=Object(o.i)(this.contentInfo.ContentTypes),c=i.next();!c.done;c=i.next())e=e.concat(c.value.Entities.filter((function(t){return t._export})))}catch(r){t={error:r}}finally{try{c&&!c.done&&(n=i.return)&&n.call(i)}finally{if(t)throw t.error}}return e},t.prototype.selectedTemplates=function(){var t,n,e=[];try{for(var i=Object(o.i)(this.contentInfo.ContentTypes),c=i.next();!c.done;c=i.next())e=e.concat(c.value.Templates.filter((function(t){return t._export})))}catch(r){t={error:r}}finally{try{c&&!c.done&&(n=i.return)&&n.call(i)}finally{if(t)throw t.error}}return e.concat(this.contentInfo.TemplatesWithoutContentTypes.filter((function(t){return t._export})))},t.\u0275fac=function(n){return new(n||t)(c.Qb(r.h),c.Qb(p.a))},t.\u0275cmp=c.Kb({type:t,selectors:[["app-export-app-parts"]],decls:24,vars:7,consts:[["mat-dialog-title",""],[1,"dialog-title-box"],[1,"dialog-description"],["href","http://2sxc.org/en/help?tag=export","target","_blank"],[1,"edit-input"],["appearance","standard","color","accent"],["name","Scope",3,"ngModel","disabled","selectionChange"],[3,"value",4,"ngFor","ngForOf"],["value","Other"],["mat-icon-button","","type","button","matSuffix","",3,"matTooltip"],[3,"click",4,"ngIf"],["href","http://2sxc.org/help?tag=scope","target","_blank","appClickStopPropagation",""],[4,"ngIf"],[3,"value"],[3,"click"],[1,"content-info__list","content-info__base"],[1,"content-info__title"],["class","content-info__item",4,"ngFor","ngForOf"],[1,"dialog-actions-box"],["mat-raised-button","",3,"disabled","click"],["mat-raised-button","","color","accent",3,"disabled","click"],[1,"content-info__item"],[1,"option-box"],[3,"ngModel","ngModelChange"],[1,"option-box__text"],["class","content-info__list",4,"ngIf"],[1,"content-info__list"],[1,"content-info__subtitle"]],template:function(t,n){1&t&&(c.Wb(0,"div",0),c.Wb(1,"div",1),c.Pc(2,"Export Content and Templates from this App"),c.Vb(),c.Vb(),c.Wb(3,"p",2),c.Pc(4," This is an advanced feature to export parts of the app. The export will create an xml file which can be imported into another site or app. To export the entire content of the app (for example when duplicating the entire site), go to the app export. For further help visit "),c.Wb(5,"a",3),c.Pc(6,"2sxc Help"),c.Vb(),c.Pc(7,".\n"),c.Vb(),c.Wb(8,"div",4),c.Wb(9,"mat-form-field",5),c.Wb(10,"mat-label"),c.Pc(11,"Scope"),c.Vb(),c.Wb(12,"mat-select",6),c.ec("selectionChange",(function(t){return n.changeScope(t)})),c.Nc(13,v,2,2,"mat-option",7),c.Wb(14,"mat-option",8),c.Pc(15,"Other..."),c.Vb(),c.Vb(),c.Wb(16,"button",9),c.Nc(17,_,2,0,"mat-icon",10),c.Nc(18,C,2,0,"mat-icon",10),c.Vb(),c.Vb(),c.Wb(19,"app-field-hint"),c.Pc(20," The scope should almost never be changed - "),c.Wb(21,"a",11),c.Pc(22,"see help"),c.Vb(),c.Vb(),c.Vb(),c.Nc(23,O,14,4,"div",12)),2&t&&(c.Bb(12),c.qc("ngModel",n.exportScope)("disabled",n.lockScope),c.Bb(1),c.qc("ngForOf",n.scopeOptions),c.Bb(3),c.qc("matTooltip",n.lockScope?"Unlock":"Lock"),c.Bb(1),c.qc("ngIf",n.lockScope),c.Bb(1),c.qc("ngIf",!n.lockScope),c.Bb(5),c.qc("ngIf",n.contentInfo))},directives:[r.i,a.c,a.g,s.a,l.r,l.u,b.s,f.l,u.b,a.j,h.a,b.t,g.a,d.a,m.a,x.a],styles:[".edit-input[_ngcontent-%COMP%]{padding-bottom:8px}.mat-hint[_ngcontent-%COMP%]{font-size:12px}.content-info__title[_ngcontent-%COMP%]{font-size:18px;font-weight:700}.content-info__subtitle[_ngcontent-%COMP%]{font-size:14px;font-weight:700}.content-info__list[_ngcontent-%COMP%]{font-size:14px;list-style-type:none}.content-info__base[_ngcontent-%COMP%]{padding:0}.content-info__item[_ngcontent-%COMP%]{border-top:1px solid #ddd;padding:2px}.option-box[_ngcontent-%COMP%]{margin:8px 0}.option-box__text[_ngcontent-%COMP%]{white-space:normal;font-size:14px}"]}),t}()},Uk43:function(t,n,e){"use strict";e.d(n,"a",(function(){return u}));var o=e("1C3z"),i=e("8AiQ"),c=e("hOvr");function r(t,n){1&t&&o.Sb(0)}function p(t,n){if(1&t&&(o.Wb(0,"mat-hint"),o.Nc(1,r,1,0,"ng-container",3),o.Vb()),2&t){o.ic();var e=o.Cc(4);o.Bb(1),o.qc("ngTemplateOutlet",e)}}function a(t,n){1&t&&o.Sb(0)}function s(t,n){if(1&t&&(o.Wb(0,"mat-error"),o.Nc(1,a,1,0,"ng-container",3),o.Vb()),2&t){o.ic();var e=o.Cc(4);o.Bb(1),o.qc("ngTemplateOutlet",e)}}function l(t,n){1&t&&o.oc(0)}var b=function(t){return{"hint-box__short":t}},f=["*"],u=function(){function t(){this.isError=!1,this.isShort=!0}return t.prototype.toggleIsShort=function(){this.isShort=!this.isShort},t.\u0275fac=function(n){return new(n||t)},t.\u0275cmp=o.Kb({type:t,selectors:[["app-field-hint"]],inputs:{isError:"isError"},ngContentSelectors:f,decls:5,vars:6,consts:[[1,"hint-box",3,"ngClass","ngSwitch","click"],[4,"ngSwitchCase"],["content",""],[4,"ngTemplateOutlet"]],template:function(t,n){1&t&&(o.pc(),o.Wb(0,"div",0),o.ec("click",(function(){return n.toggleIsShort()})),o.Nc(1,p,2,1,"mat-hint",1),o.Nc(2,s,2,1,"mat-error",1),o.Vb(),o.Nc(3,l,1,0,"ng-template",null,2,o.Oc)),2&t&&(o.qc("ngClass",o.vc(4,b,n.isShort))("ngSwitch",n.isError),o.Bb(1),o.qc("ngSwitchCase",!1),o.Bb(1),o.qc("ngSwitchCase",!0))},directives:[i.q,i.x,i.y,c.f,i.A,c.b],styles:[".hint-box[_ngcontent-%COMP%]{margin-top:4px}.hint-box[_ngcontent-%COMP%]   .mat-error[_ngcontent-%COMP%], .hint-box[_ngcontent-%COMP%]   .mat-hint[_ngcontent-%COMP%]{font-size:12px;display:block}.hint-box__short[_ngcontent-%COMP%]   .mat-error[_ngcontent-%COMP%], .hint-box__short[_ngcontent-%COMP%]   .mat-hint[_ngcontent-%COMP%]{white-space:nowrap;overflow:hidden;text-overflow:ellipsis}"]}),t}()},udnq:function(t,n,e){"use strict";e.d(n,"a",(function(){return i}));var o=e("1C3z"),i=function(){function t(){}return t.prototype.onClick=function(t){t.stopPropagation()},t.\u0275fac=function(n){return new(n||t)},t.\u0275dir=o.Lb({type:t,selectors:[["","appClickStopPropagation",""]],hostBindings:function(t,n){1&t&&o.ec("click",(function(t){return n.onClick(t)}))}}),t}()}}]);
//# sourceMappingURL=24.js.map