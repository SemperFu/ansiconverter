
//AANIMVAR
//AMAPPINGSVAR

function d2h(d) {return d.toString(16).toUpperCase();}
function h2d(h) {return parseInt(h,16);}
function thiscell(obj) {
   var o=obj; 
   var id=obj.id;
   this.o = obj; 
   this.id=id;
   this.x=(h2d(id) % 80)+1;
   this.y=Math.floor(h2d(id)/80)+1;
   this.resetchr = resetchar;
   function resetchar(back){
      if (!back) {
		this.o.className="II"; 
	  } else {
	    this.o.className="b"+back; 
      }	  
	  this.o.innerHTML="&nbsp;";
	  this.o.title=this.o.className;
  };
}
thiscell.prototype.setcolor = function(fore, back) {this.o.className='C'+fore+' B'+back;this.o.title=this.o.className;};	
var cells=[];
var cnt=0;
var scnt=0;
var i=0;
var j=0;
var e1=0;
var e2=0;
var a1;
var d1;
var m1;
var xpos=1;
var ypos=1;
var xposmin=1;
var xposmax=80;
var yposmin=1;
var yposmax=25;
var fc="7";
var bc="0";  
var minid=0;
var maxid=(xposmax*yposmax)-1;
var o;
var cw=0;
var debOB='';
  
function StartAnsiAnimation(anmid,aAnm,aMaps) {
  o=document.getElementById(anmid);
  var q=o.getElementsByTagName("span");
  for (var i=0;i<q.length;i++) {
    var p=new thiscell(q[i]);
    cells.push(p);
  }
  a1=aAnm;
  i=0;
  m1=aMaps;
  cnt=0;
  if (aAnm.length) {
     e1 = aAnm.length;
     if (e1>0) {
	    j=0;
	    d1=aAnm[i].split(' ');	
		e2=d1.length;
		process();
	 }
  }
}

function adde(par,newid,col,ba) {
  var sp=document.createElement("span");
  sp.className='II';
  sp.setAttribute('id', newid);
  var tx=document.createTextNode(String.fromCharCode(32));
  sp.appendChild(tx);
  sp.data='&nbsp;';
  var p=new thiscell(sp);
  col.push(p);
  if (ba=='b') {par.insertBefore(sp,par.firstChild);}
  if (ba=='a') {par.appendChild(sp);}
}

function addla() {
  var w=o.getElementsByTagName("pre");  
  var tx=document.createTextNode(String.fromCharCode(10)+String.fromCharCode(13));
  w[0].appendChild(tx);
  for (var cx2=0;cx2<80;cx2++){
      var w=o.getElementsByTagName("pre");
      maxid++;
	  adde(w[0],d2h(maxid),cells,'a');
  }
  yposmax++;
}

function idfromcoord(x,y) {xpos=chkx(x);ypos=chky(y);cnt=((parseInt(ypos)-1)*80)+parseInt(xpos)-1;}
function clearline(y) {for (var a=(y-1)*80;a<(y)*80;a++) {colls[a].resetchr(bc);}}
function cltc(y,x) {for (var a=(y-1)*80;a<((y-1)*80)+x;a++) {colls[a].resetchr(bc);}}
function clfc(y,x) {for (var a=((y-1)*80)+x;a<(y)*80;a++) {colls[a].resetchr(bc);}}
function movc(numpos,charins) {
	 var curx=cells[cnt].x;
	 if (curx+numpos>xposmax) {
	    if (charins===false) {
		  numpos=xposmax-curx;
		}
	 }
	 if (numpos>0) {
        cnt+=numpos;if (cnt>maxid){addla();};xpos=cells[cnt].x;ypos=cells[cnt].y;
	 }
}
function movb(numpos) {
	var curx=cells[cnt].x;
	if (curx-numpos<xposmin) {
	   numpos=curx-xposmin;
	}
	if (numpos>0) {
		cnt-=numpos;xpos=cells[cnt].x;ypos=cells[cnt].y;
	}	
}
function movu() {
	var cury=cells[cnt].y;
	if (cury>yposmin) {
		cnt-=80;xpos=cells[cnt].x;ypos=cells[cnt].y;
	}	
}
function movd() {
   var cury=cells[cnt].y;
   if (cury===yposmax) {
	 addla();
   } 
   cnt+=80;xpos=cells[cnt].x;ypos=cells[cnt].y;   
}
function chkx(x) {if (x<xposmin) {return xposmin;} else {if (x>xposmax) {return xposmax;} else {return x;}}}
function chky(y) {if (y<yposmin) {return yposmin;} else {if (y>yposmax) {
  //for (var l1=yposmax;l1<=y;l1++) {addla()};return y;
  return yposmax;
  } else {return y;}}}

function process() {
  var iwait=0;
  if (i<e1)
  {
    d1=a1[i].split(' ');	
	e2=d1.length;
	if (j<e2)
	{ 
	  var d=d1[j];
	  var c=d.substr(0,1); 
  	  switch(c) {
		case 'A': //up
		    for (var a=0;a<h2d(d.substr(1,10));a++) {movu();}
			break;
		case 'B': //down
		    for (var a=0;a<h2d(d.substr(1,10));a++) {movd();}
			break;
		case 'C': //right
		    movc(h2d(d.substr(1,10)),false);
			break;
		case 'D': //left
		    movb(h2d(d.substr(1,10)));
			break;
		case 'X': //1char
			//cells[cnt].o.innerHTML=m1[h2d(d.substr(1,10))-33];
			cells[cnt].o.innerHTML=m1[h2d(d.substr(1,10))];
			cells[cnt].setcolor(fc,bc);
			movc(1,true);cw++;
			break;
		case 'x'://multi chars
			var xa=d.substr(1,d.length-1).split(";");
			for (var a=0;a<xa.length;a++) {
			  //cells[cnt].o.innerHTML=m1[h2d(xa[a])-33];
			  cells[cnt].o.innerHTML=m1[h2d(xa[a])];
			  cells[cnt].setcolor(fc,bc);
			  movc(1,true);cw++;
			}
			break;
		case 'T'://color
			fc=d.substr(2,1);
			bc=d.substr(1,1);
		    cells[cnt].setcolor(fc,bc);
			break;
		case 'Z'://spaces
		    for (var a=0;a<h2d(d.substr(1,10));a++) {
			  cells[cnt].o.innerHTML="&nbsp;";
			  cells[cnt].setcolor(fc,bc);
			  movc(1,true);cw++;
			}
			break;
		case 'W'://wait
		    iwait=Math.floor(h2d(d.substr(1,10))/10);
			if (iwait===0) {iwait=1;};
			break;
		case 'S'://save pos
		    scnt=cnt;
			break;
		case 'R'://restore pos
		    cnt=scnt;
			xpos=cells[cnt].x;
			ypos=cells[cnt].y;
			break;
		case 'P'://pos x,y
			idfromcoord(d.substr(1,1).charCodeAt(0)-45,d.substr(2,1).charCodeAt(0)-96);
			break;
		case 'G'://clear to eol
		    clfc(cells[cnt].y,cells[cnt].x);
			break;
		case 'H'://clear to sol
		    cltc(cells[cnt].y,cells[cnt].x);
			break;
		case 'I'://clear line
		    clearline(cells[cnt].y);
			break;
		case 'K'://clear to end
		    clfc(cells[cnt].y,cells[cnt].x);
			for (var a=ypos++;a<=yposmax;a++) {
				clearline(a);
			}
			break;
		case 'J'://clear to start
		    cltc(cells[cnt].y,cells[cnt].x);
			for (var a=yposmin;a<cells[cnt].y;a--) {
				clearline(a);
			}
			break;
		case 'F'://clear screen
			for (var a=minid;a<=maxid;a++) {
			  cells[a].resetchr(bc);
			}
			cnt=minid;
			xpos=xposmin;
			ypos=yposmin;
			iwait=1;
			break;
		case 'N'://new line
			if (ypos===yposmax) {addla();};
		    idfromcoord(1,ypos++);
			break;			
		default:
			break;
	  }	  
	  debOB=d1[j];
	  j++;
	} else {
	   i++;
	   j=0;
	}
	
    if (iwait!==0){
	   f=setTimeout('process()',iwait);
	}else{
	   if (cw>=400){
	      cw=0;
	      f=setTimeout('process()',1);
	   }else{
	      process();}
	   }
	
  }
}


//CALLPLACEHOLDER

//CALLSTART
StartAnsiAnimation('anm1',aAnim1,aMappings1);
//CALLEND
