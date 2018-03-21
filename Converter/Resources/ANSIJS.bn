//AANIMVAR
//AMAPPINGSVAR
function d2h(d){return d.toString(16).toUpperCase();};function h2d(h){return parseInt(h,16);};
function tc(obj){var o=obj,id=obj.id;this.o=obj;this.id=id;this.x=(h2d(id)%80)+1;this.y=Math.floor(h2d(id)/80)+1;this.f='7';this.b='0';this.f2='7';this.blink=false;this.r=rc;function rc(back){if(!back){this.o.className="II";}else{this.o.className="b"+back;};this.o.innerHTML="&nbsp;";this.f='7';this.f2='7';this.b='0';this.blink=false;};};tc.prototype.sc=function(f,b,bl){this.o.className='C'+f+' B'+b;this.o.title=this.o.className;if(!bl){this.f=f;this.f2=f;this.b=b;};};	
var cells=[],cnt=0,scnt=0,i=0,j=0,e1=0,e2=0,a1,d1,m1,xp=1,yp=1,xpmn=1,xpmx=80,ypmn=1,ypmx=25,fc="7",bc="0",minid=0,maxid=(xpmx*ypmx)-1,blink=false,yoffset=0;
var o,mydiff=0,mytimer=0,mystart=new Date().getTime();
var todur=100,numit=150 //timing
//         todur numit         todur numit    general ratio
//General 100   150     Chrome 20   50     todur/numit = 2/3  
function StartAnsiAnimation(anmid,aAnm,aMaps){o=document.getElementById(anmid);var q=o.getElementsByTagName("span");for(var i=0;i<q.length;i++){var p=new tc(q[i]);cells.push(p);};a1=aAnm;i=0;m1=aMaps;cnt=0;if(aAnm.length){e1=aAnm.length;if(e1>0){j=0;d1=aAnm[i].split(' ');e2=d1.length;if((e1>0)&&(e2>0)){window.setTimeout(process,todur);};};};};
function adde(par,newid,col,ba){var sp=document.createElement("span");sp.className='II';sp.setAttribute('id',newid);var tx=document.createTextNode(String.fromCharCode(32));sp.appendChild(tx);sp.data='&nbsp;';var p=new tc(sp);col.push(p);if(ba=='b'){par.insertBefore(sp,par.firstChild);};if(ba=='a'){par.appendChild(sp);};};
function addla(){var w=o.getElementsByTagName("pre");var tx=document.createTextNode(String.fromCharCode(10)+String.fromCharCode(13));w[0].appendChild(tx);for(var cx2=0;cx2<80;cx2++){var w=o.getElementsByTagName("pre");maxid++;adde(w[0],d2h(maxid),cells,'a');};ypmx++;yoffset++;};
function idc(x,y) {xp=chkx(x);yp=chky(y+yoffset);cnt=((parseInt(yp)-1)*80)+parseInt(xp)-1;};function cl(y) {for (var a=(y-1)*80;a<(y)*80;a++) {colls[a].r(bc);}};
function cltc(y,x) {for (var a=(y-1)*80;a<((y-1)*80)+x;a++) {colls[a].r(bc);}};function clfc(y,x){for (var a=((y-1)*80)+x;a<(y)*80;a++) {colls[a].r(bc);}};
function movc(np,ci){var curx=cells[cnt].x;if (curx+np>xpmx) {if (ci===false){np=xpmx-curx;};};if (np>0){cnt+=np;if (cnt>maxid){addla();};xp=cells[cnt].x;yp=cells[cnt].y;};};
function movb(np){var curx=cells[cnt].x;if (curx-np<xpmn){np=curx-xpmn;};if (np>0){cnt-=np;xp=cells[cnt].x;yp=cells[cnt].y;};};
function movu(){var cury=cells[cnt].y;if (cury>ypmn){cnt-=80;xp=cells[cnt].x;yp=cells[cnt].y;};};
function movd(){var cury=cells[cnt].y;if (cury===ypmx){addla();};cnt+=80;xp=cells[cnt].x;yp=cells[cnt].y;};
function chkx(x) {if (x<xpmn) {return xpmn;} else {if (x>xpmx) {return xpmx;} else {return x;}}};
function chky(y) {if (y<ypmn+yoffset) {return ypmn+yoffset;} else {if (y>ypmx){return ypmx;}else{return y;}}};
function process(){var iwait=0;if(i<e1){d1=a1[i].split(' ');e2=d1.length;if (j<e2){for (k=0;k<numit;k++) {var d=d1[j];var c=d.substr(0,1);switch(c) {
case 'A':for (var a=0;a<h2d(d.substr(1,10));a++) {movu();};break; //up
case 'B':for (var a=0;a<h2d(d.substr(1,10));a++) {movd();};break; //down
case 'C':movc(h2d(d.substr(1,10)),false);break; //right
case 'D':movb(h2d(d.substr(1,10)));break; //left
case 'X':cells[cnt].o.innerHTML=m1[h2d(d.substr(1,10))];cells[cnt].sc(fc,bc);if(blink===true){sblink(cnt);};movc(1,true);break; //1 char
case 'x':var xa=d.substr(1,d.length-1).split(";");for(var a=0;a<xa.length;a++){cells[cnt].o.innerHTML=m1[h2d(xa[a])];cells[cnt].sc(fc,bc);if(blink==true){sblink(cnt);};movc(1,true);};break;//multichars
case 'T':fc=d.substr(2,1);bc=d.substr(1,1);cells[cnt].sc(fc,bc);break; //color
case 'Z':for (var a=0;a<h2d(d.substr(1,10));a++){cells[cnt].o.innerHTML="&nbsp;";cells[cnt].sc(fc,bc);if(blink===true){sblink(cnt);};movc(1,true);};break; //spaces
case 'W':iwait=Math.floor(h2d(d.substr(1,10)))*5;if(iwait===0){iwait=1;};break; //wait
case 'S':scnt=cnt;break; //save pos
case 'R':cnt=scnt;xp=cells[cnt].x;yp=cells[cnt].y;break; //restore pos
case 'P':idc(d.substr(1,1).charCodeAt(0)-45,d.substr(2,1).charCodeAt(0)-96);break; //pos x,y
case 'G':clfc(cells[cnt].y,cells[cnt].x);break; //clear to eol
case 'H':cltc(cells[cnt].y,cells[cnt].x);break; //clear to sol
case 'I':cl(cells[cnt].y);break; //clear line
case 'K':clfc(cells[cnt].y,cells[cnt].x);for (var a=yp++;a<=ypmx;a++) {cl(a);};break; //clear to end
case 'J':cltc(cells[cnt].y,cells[cnt].x);for (var a=ypmn;a<cells[cnt].y;a--) {cl(a);};break; //clear to start
case 'F':for (var a=minid;a<=maxid;a++){cells[a].r(bc);};cnt=minid;xp=xpmn;yp=ypmn;break; //clear screen
case 'N':if(yp===ypmx){addla();};idc(1,yp++);break; //new line
case 'L':if (d.substr(1,1)=='1') {blink=true;sblink(cnt);} else {blink=false;cells[cnt].blink=false;};break; //blinking on/off 
default: break;};j++;if(j>=e2){break;};};}else{i++;j=0;};mytimer+= todur;mydiff=(new Date().getTime()-mystart)-mytimer;window.setTimeout(process,(todur-mydiff));};};
function sblink(id){cells[id].blink=true;f=setTimeout('bon('+id+')',250);}
function bon(id){if (cells[id].blink==true){cells[id].sc(cells[id].b,cells[id].b,true);f=setTimeout('boff('+id+')',250);};}
function boff(id){cells[id].sc(cells[id].f2,cells[id].b,true); f=setTimeout('bon('+id+')',250);}
//CALLPLACEHOLDER
//CALLSTART
StartAnsiAnimation('anm1',aAnim1,aMappings1);
//CALLEND
