(window.webpackJsonp=window.webpackJsonp||[]).push([[34],{GYuQ:function(e,t,i){"use strict";i.r(t),i.d(t,"ContentImportComponent",(function(){return k}));var n=i("D57K"),o=i("d3IL"),c=i("1C3z"),a=i("BLjT"),r=i("5/c3"),l=i("Ujoz"),s=i("8AiQ"),b=i("ZSGP"),u=i("9HSk"),f=i("Qc/f"),d=i("+raR");function p(e,t){if(1&e&&(c.Ub(0),c.Pc(1),c.Tb()),2&e){var i=c.ic();c.Bb(1),c.Rc("Step ",i.viewStateSelected," of 3")}}function m(e,t){1&e&&(c.Wb(0,"span"),c.Pc(1,"Select file"),c.Vb())}function g(e,t){if(1&e&&(c.Wb(0,"span"),c.Pc(1),c.Vb()),2&e){var i=c.ic(2);c.Bb(1),c.Qc(i.formValues.file.name)}}function v(e,t){if(1&e){var i=c.Xb();c.Wb(0,"form",null,8),c.Wb(2,"div"),c.Wb(3,"button",9),c.ec("click",(function(){return c.Ec(i),c.Cc(7).click()})),c.Nc(4,m,2,0,"span",2),c.Nc(5,g,2,1,"span",2),c.Vb(),c.Wb(6,"input",10,11),c.ec("change",(function(e){return c.Ec(i),c.ic().fileChange(e)})),c.Vb(),c.Vb(),c.Wb(8,"div"),c.Wb(9,"p",12),c.Pc(10,"References to pages / files"),c.Vb(),c.Wb(11,"mat-radio-group",13),c.ec("ngModelChange",(function(e){return c.Ec(i),c.ic().formValues.resourcesReferences=e})),c.Wb(12,"mat-radio-button",14),c.Pc(13," Import links as written in the file (for example /Portals/...) "),c.Vb(),c.Wb(14,"mat-radio-button",15),c.Pc(15," Try to resolve paths to references "),c.Vb(),c.Vb(),c.Vb(),c.Wb(16,"div"),c.Wb(17,"p",12),c.Pc(18,"Clear all other entities"),c.Vb(),c.Wb(19,"mat-radio-group",16),c.ec("ngModelChange",(function(e){return c.Ec(i),c.ic().formValues.clearEntities=e})),c.Wb(20,"mat-radio-button",17),c.Pc(21," Keep all entities not found in import "),c.Vb(),c.Wb(22,"mat-radio-button",18),c.Pc(23," Remove all entities not found in import "),c.Vb(),c.Vb(),c.Vb(),c.Wb(24,"p",19),c.Pc(25,"Remember to backup your DNN first!"),c.Vb(),c.Wb(26,"div",20),c.Wb(27,"button",21),c.ec("click",(function(){return c.Ec(i),c.ic().closeDialog()})),c.Pc(28,"Cancel"),c.Vb(),c.Wb(29,"button",22),c.ec("click",(function(){return c.Ec(i),c.ic().evaluateContent()})),c.Pc(30," Preview Import "),c.Vb(),c.Vb(),c.Vb()}if(2&e){var n=c.ic();c.Bb(4),c.qc("ngIf",!n.formValues.file),c.Bb(1),c.qc("ngIf",n.formValues.file),c.Bb(6),c.qc("ngModel",n.formValues.resourcesReferences),c.Bb(8),c.qc("ngModel",n.formValues.clearEntities),c.Bb(10),c.qc("disabled",!n.formValues.file||!n.formValues.file.name)}}function h(e,t){1&e&&(c.Wb(0,"p",23),c.Pc(1,"Please wait while processing..."),c.Vb())}function V(e,t){if(1&e&&(c.Wb(0,"div"),c.Wb(1,"p",24),c.Pc(2),c.Vb(),c.Wb(3,"p",24),c.Pc(4,"File contains:"),c.Vb(),c.Wb(5,"ul",25),c.Wb(6,"li"),c.Pc(7),c.Vb(),c.Wb(8,"li"),c.Pc(9),c.Vb(),c.Wb(10,"li"),c.Pc(11),c.Vb(),c.Vb(),c.Wb(12,"p",24),c.Pc(13,"If you press Import, it will:"),c.Vb(),c.Wb(14,"ul",25),c.Wb(15,"li"),c.Pc(16),c.Vb(),c.Wb(17,"li"),c.Pc(18),c.Vb(),c.Wb(19,"li"),c.Pc(20),c.Vb(),c.Wb(21,"li"),c.Pc(22),c.Vb(),c.Vb(),c.Wb(23,"p",19),c.Pc(24,"Note: The import validates much data and may take several minutes."),c.Vb(),c.Vb()),2&e){var i=c.ic(3);c.Bb(2),c.Rc("Try to import file '",i.formValues.file.name,"'"),c.Bb(5),c.Rc("",i.evaluationResult.Detail.DocumentElementsCount," content-items (records/entities)"),c.Bb(2),c.Rc("",i.evaluationResult.Detail.LanguagesInDocumentCount," languages"),c.Bb(2),c.Sc("",i.evaluationResult.Detail.AttributeNamesInDocument.length," columns: ",i.evaluationResult.Detail.AttributeNamesInDocument.join(", "),""),c.Bb(5),c.Rc("Create ",i.evaluationResult.Detail.AmountOfEntitiesCreated," content-items"),c.Bb(2),c.Rc("Update ",i.evaluationResult.Detail.AmountOfEntitiesUpdated," content-items"),c.Bb(2),c.Rc("Delete ",i.evaluationResult.Detail.AmountOfEntitiesDeleted," content-items"),c.Bb(2),c.Sc("Ignore ",i.evaluationResult.Detail.AttributeNamesNotImported.length," columns: ",i.evaluationResult.Detail.AttributeNamesNotImported.join(", "),"")}}function W(e,t){if(1&e&&(c.Wb(0,"div"),c.Wb(1,"i"),c.Pc(2),c.Vb(),c.Vb()),2&e){var i=c.ic().$implicit;c.Bb(2),c.Rc("Details: ",i.ErrorDetail,"")}}function P(e,t){if(1&e&&(c.Wb(0,"div"),c.Wb(1,"i"),c.Pc(2),c.Vb(),c.Vb()),2&e){var i=c.ic().$implicit;c.Bb(2),c.Rc("Line-no: ",i.LineNumber,"")}}function S(e,t){if(1&e&&(c.Wb(0,"div"),c.Wb(1,"i"),c.Pc(2),c.Vb(),c.Vb()),2&e){var i=c.ic().$implicit;c.Bb(2),c.Rc("Line-details: ",i.LineDetail,"")}}function w(e,t){if(1&e&&(c.Wb(0,"li"),c.Wb(1,"div"),c.Pc(2),c.Vb(),c.Nc(3,W,3,1,"div",2),c.Nc(4,P,3,1,"div",2),c.Nc(5,S,3,1,"div",2),c.Vb()),2&e){var i=t.$implicit,n=c.ic(4);c.Bb(2),c.Qc(n.errors[i.ErrorCode]),c.Bb(1),c.qc("ngIf",i.ErrorDetail),c.Bb(1),c.qc("ngIf",i.LineNumber),c.Bb(1),c.qc("ngIf",i.LineDetail)}}function R(e,t){if(1&e&&(c.Wb(0,"div"),c.Wb(1,"p",24),c.Pc(2),c.Vb(),c.Wb(3,"ul",25),c.Nc(4,w,6,4,"li",26),c.Vb(),c.Vb()),2&e){var i=c.ic(3);c.Bb(2),c.Rc("Try to import file '",i.formValues.file.name,"'"),c.Bb(2),c.qc("ngForOf",i.evaluationResult.Detail)}}function C(e,t){if(1&e){var i=c.Xb();c.Wb(0,"div"),c.Nc(1,V,25,10,"div",2),c.Nc(2,R,5,2,"div",2),c.Wb(3,"div",20),c.Wb(4,"button",21),c.ec("click",(function(){return c.Ec(i),c.ic(2).back()})),c.Pc(5,"Back"),c.Vb(),c.Wb(6,"button",22),c.ec("click",(function(){return c.Ec(i),c.ic(2).importContent()})),c.Pc(7," Import "),c.Vb(),c.Vb(),c.Vb()}if(2&e){var n=c.ic(2);c.Bb(1),c.qc("ngIf",n.evaluationResult.Succeeded),c.Bb(1),c.qc("ngIf",!n.evaluationResult.Succeeded),c.Bb(4),c.qc("disabled",!n.evaluationResult.Succeeded)}}function B(e,t){if(1&e&&(c.Wb(0,"div"),c.Nc(1,C,8,3,"div",2),c.Vb()),2&e){var i=c.ic();c.Bb(1),c.qc("ngIf",i.evaluationResult)}}function I(e,t){1&e&&(c.Wb(0,"p"),c.Pc(1,"Import done."),c.Vb())}function y(e,t){1&e&&(c.Wb(0,"p"),c.Pc(1,"Import failed."),c.Vb())}function N(e,t){if(1&e&&(c.Wb(0,"div",23),c.Nc(1,I,2,0,"p",2),c.Nc(2,y,2,0,"p",2),c.Vb()),2&e){var i=c.ic(2);c.Bb(1),c.qc("ngIf",i.importResult.Succeeded),c.Bb(1),c.qc("ngIf",!i.importResult.Succeeded)}}function D(e,t){if(1&e){var i=c.Xb();c.Wb(0,"div"),c.Nc(1,N,3,2,"div",27),c.Wb(2,"div",20),c.Wb(3,"button",28),c.ec("click",(function(){return c.Ec(i),c.ic().closeDialog()})),c.Pc(4,"Close"),c.Vb(),c.Vb(),c.Vb()}if(2&e){var n=c.ic();c.Bb(1),c.qc("ngIf",n.importResult)}}var k=function(){function e(e,t,i){this.dialogRef=e,this.route=t,this.contentImportService=i,this.errors={0:"Unknown error occured.",1:"Selected content-type does not exist.",2:"Document is not a valid XML file.",3:"Selected content-type does not match the content-type in the XML file.",4:"The language is not supported.",5:"The document does not specify all languages for all entities.",6:"Language reference cannot be parsed, the language is not supported.",7:"Language reference cannot be parsed, the read-write protection is not supported.",8:"Value cannot be read, because of it has an invalid format."},this.viewStates={waiting:0,default:1,evaluated:2,imported:3},this.viewStateSelected=this.viewStates.default,this.formValues={defaultLanguage:sessionStorage.getItem(o.k),contentType:this.route.snapshot.paramMap.get("contentTypeStaticName"),file:null,resourcesReferences:"Keep",clearEntities:"None"}}return e.prototype.ngOnInit=function(){},e.prototype.evaluateContent=function(){return Object(n.b)(this,void 0,void 0,(function(){var e=this;return Object(n.e)(this,(function(t){switch(t.label){case 0:return this.viewStateSelected=this.viewStates.waiting,[4,this.contentImportService.evaluateContent(this.formValues)];case 1:return[2,t.sent().subscribe((function(t){e.evaluationResult=t,e.viewStateSelected=e.viewStates.evaluated}))]}}))}))},e.prototype.importContent=function(){return Object(n.b)(this,void 0,void 0,(function(){var e=this;return Object(n.e)(this,(function(t){switch(t.label){case 0:return this.viewStateSelected=this.viewStates.waiting,[4,this.contentImportService.importContent(this.formValues)];case 1:return[2,t.sent().subscribe((function(t){e.importResult=t,e.viewStateSelected=e.viewStates.imported}))]}}))}))},e.prototype.back=function(){this.viewStateSelected=this.viewStates.default,this.evaluationResult=void 0},e.prototype.closeDialog=function(){this.dialogRef.close()},e.prototype.fileChange=function(e){this.formValues.file=e.target.files[0]},e.\u0275fac=function(t){return new(t||e)(c.Qb(a.h),c.Qb(r.a),c.Qb(l.a))},e.\u0275cmp=c.Kb({type:e,selectors:[["app-content-import"]],decls:14,vars:6,consts:[["mat-dialog-title",""],[1,"dialog-title-box"],[4,"ngIf"],[1,"dialog-description"],["href","http://2sxc.org/help","target","_blank"],[3,"ngSwitch"],[4,"ngSwitchCase"],["class","progress-message",4,"ngSwitchCase"],["ngForm","ngForm"],["mat-raised-button","","matTooltip","Open file browser",3,"click"],["type","file",1,"hide",3,"change"],["fileInput",""],[1,"field-label"],["name","ResourcesReferences",3,"ngModel","ngModelChange"],["value","Keep"],["value","Resolve"],["name","ClearEntities",3,"ngModel","ngModelChange"],["value","None"],["value","All"],[1,"hint"],[1,"dialog-actions-box"],["mat-raised-button","","type","button",3,"click"],["mat-raised-button","","type","button","color","accent",3,"disabled","click"],[1,"progress-message"],[1,"evaluation__title"],[1,"evaluation__content"],[4,"ngFor","ngForOf"],["class","progress-message",4,"ngIf"],["mat-raised-button","","type","button","color","accent",3,"click"]],template:function(e,t){1&e&&(c.Wb(0,"div",0),c.Wb(1,"div",1),c.Pc(2," Import Content / Data "),c.Nc(3,p,2,1,"ng-container",2),c.Vb(),c.Vb(),c.Wb(4,"p",3),c.Pc(5," This will import content-items into 2sxc. It requires that you already defined the content-type before you try importing, and that you created the import-file using the template provided by the Export. Please visit "),c.Wb(6,"a",4),c.Pc(7,"http://2sxc.org/help"),c.Vb(),c.Pc(8," for more instructions.\n"),c.Vb(),c.Wb(9,"div",5),c.Nc(10,v,31,5,"form",6),c.Nc(11,h,2,0,"p",7),c.Nc(12,B,2,1,"div",6),c.Nc(13,D,5,1,"div",6),c.Vb()),2&e&&(c.Bb(3),c.qc("ngIf",t.viewStateSelected>0),c.Bb(6),c.qc("ngSwitch",t.viewStateSelected),c.Bb(1),c.qc("ngSwitchCase",1),c.Bb(1),c.qc("ngSwitchCase",0),c.Bb(1),c.qc("ngSwitchCase",2),c.Bb(1),c.qc("ngSwitchCase",3))},directives:[a.i,s.t,s.x,s.y,b.G,b.s,b.t,u.b,f.a,d.b,b.r,b.u,d.a,s.s],styles:[".field-label[_ngcontent-%COMP%]{font-size:18px;margin:24px 0 0}.mat-radio-group[_ngcontent-%COMP%]{display:flex;flex-direction:column;margin:8px 0}.mat-radio-button[_ngcontent-%COMP%]{margin:5px;font-size:14px}.hint[_ngcontent-%COMP%]{margin-top:24px;margin-bottom:16px}.hint[_ngcontent-%COMP%], .progress-message[_ngcontent-%COMP%]{font-size:18px}.evaluation__title[_ngcontent-%COMP%]{font-size:18px;margin:24px 0 0;font-weight:700}.evaluation__content[_ngcontent-%COMP%]{font-size:14px}.evaluation__content[_ngcontent-%COMP%]   li[_ngcontent-%COMP%]{padding:2px 0}"]}),e}()}}]);
//# sourceMappingURL=34.js.map