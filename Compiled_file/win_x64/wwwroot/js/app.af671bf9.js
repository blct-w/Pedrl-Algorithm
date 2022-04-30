(function(e){function t(t){for(var a,o,s=t[0],l=t[1],c=t[2],u=0,m=[];u<s.length;u++)o=s[u],i[o]&&m.push(i[o][0]),i[o]=0;for(a in l)Object.prototype.hasOwnProperty.call(l,a)&&(e[a]=l[a]);d&&d(t);while(m.length)m.shift()();return r.push.apply(r,c||[]),n()}function n(){for(var e,t=0;t<r.length;t++){for(var n=r[t],a=!0,o=1;o<n.length;o++){var l=n[o];0!==i[l]&&(a=!1)}a&&(r.splice(t--,1),e=s(s.s=n[0]))}return e}var a={},i={app:0},r=[];function o(e){return s.p+"js/"+({about:"about"}[e]||e)+"."+{about:"ef3c67a4"}[e]+".js"}function s(t){if(a[t])return a[t].exports;var n=a[t]={i:t,l:!1,exports:{}};return e[t].call(n.exports,n,n.exports,s),n.l=!0,n.exports}s.e=function(e){var t=[],n=i[e];if(0!==n)if(n)t.push(n[2]);else{var a=new Promise(function(t,a){n=i[e]=[t,a]});t.push(n[2]=a);var r,l=document.createElement("script");l.charset="utf-8",l.timeout=120,s.nc&&l.setAttribute("nonce",s.nc),l.src=o(e),r=function(t){l.onerror=l.onload=null,clearTimeout(c);var n=i[e];if(0!==n){if(n){var a=t&&("load"===t.type?"missing":t.type),r=t&&t.target&&t.target.src,o=new Error("Loading chunk "+e+" failed.\n("+a+": "+r+")");o.type=a,o.request=r,n[1](o)}i[e]=void 0}};var c=setTimeout(function(){r({type:"timeout",target:l})},12e4);l.onerror=l.onload=r,document.head.appendChild(l)}return Promise.all(t)},s.m=e,s.c=a,s.d=function(e,t,n){s.o(e,t)||Object.defineProperty(e,t,{enumerable:!0,get:n})},s.r=function(e){"undefined"!==typeof Symbol&&Symbol.toStringTag&&Object.defineProperty(e,Symbol.toStringTag,{value:"Module"}),Object.defineProperty(e,"__esModule",{value:!0})},s.t=function(e,t){if(1&t&&(e=s(e)),8&t)return e;if(4&t&&"object"===typeof e&&e&&e.__esModule)return e;var n=Object.create(null);if(s.r(n),Object.defineProperty(n,"default",{enumerable:!0,value:e}),2&t&&"string"!=typeof e)for(var a in e)s.d(n,a,function(t){return e[t]}.bind(null,a));return n},s.n=function(e){var t=e&&e.__esModule?function(){return e["default"]}:function(){return e};return s.d(t,"a",t),t},s.o=function(e,t){return Object.prototype.hasOwnProperty.call(e,t)},s.p="",s.oe=function(e){throw console.error(e),e};var l=window["webpackJsonp"]=window["webpackJsonp"]||[],c=l.push.bind(l);l.push=t,l=l.slice();for(var u=0;u<l.length;u++)t(l[u]);var d=c;r.push([0,"chunk-vendors"]),n()})({0:function(e,t,n){e.exports=n("56d7")},"034f":function(e,t,n){"use strict";var a=n("64a9"),i=n.n(a);i.a},"3e29":function(e,t,n){"use strict";(function(e){var a=n("0a0d"),i=n.n(a),r=(n("c5f6"),n("ac6a"),n("bd86")),o=n("e814"),s=n.n(o),l=n("59ad"),c=n.n(l),u=(n("28a5"),n("ec35")),d=n.n(u);n("6f8a");t["a"]={name:"CesiumViewer",data:function(){return{lastlon:0,lastlat:0,viewer:null,terrainProvider:null,scene:null,ellipsoid:null,seeHeight:5,testLong:119.5,testLat:41.5,testToLong:119.55,testToLat:41.55,realTimeTestR:.05,realTimeTest:!1,realTimeTestOnDrawing:!1,realTestTime:null,realTestTimeStr:"",realTestSendCount:0,realTestReciveCount:0,realTimeTestDrawArr:new Array,realTimeTestTimer:null,CaculateCount:0,CompareCount:0,TimeSpan:null,ondrawing:!1,mouseLocation:{lon:"...",lat:"...",h:"..."},info:"infomation....",startPoint:null,startCartographic:null,toCartographic:null,startPointStr:"...",startPointServerHeight:0,dist:10,lastDist:0,circle:null,shapes:[],updateShape:null,context:null}},mounted:function(){var t=this,n=new WebSocket("ws://localhost:8000");n.onopen=function(e){},n.onclose=function(e){},n.onmessage=function(e){t.realTestReciveCount++,t.realTimeTestDrawArr.push(e.data),t.realTestTimeStr+="|接收"+((new Date).valueOf()-t.realTestTime.valueOf()),t.realTestTime=new Date},t.realTimeTestTimer=window.setInterval(function(){t.realTimeTestDrawArr.length>0&&(t.updateVisibleLines(t.realTimeTestDrawArr.pop()),t.realTimeTestDrawArr.length=0)},200),d.a.Ion.defaultAccessToken="eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJqdGkiOiJhZjg4ZDBjZi1lN2JiLTQwNzQtYjRiNS03ZDJiZTYzOTBkYTciLCJpZCI6OTgxOSwic2NvcGVzIjpbImFzbCIsImFzciIsImdjIl0sImlhdCI6MTU4NDk3MzIwNX0.busTyop6rf2-AWuiBtNnFOQnfXXHc6Dm-BVVg4Hs1hk";var a=this.ondrawing,i=new d.a.CallbackProperty(function(){return t.dist},!1),r=t.terrainProvider=d.a.createWorldTerrain(),o=t.viewer=new d.a.Viewer("cesiumContainer",{selectionIndicator:!1,infoBox:!1,terrainProvider:r});o._cesiumWidget._creditContainer.style.display="none",o.cesiumWidget.screenSpaceEventHandler.removeInputAction(d.a.ScreenSpaceEventType.LEFT_DOUBLE_CLICK);var s=t.scene=o.scene,l=t.ellipsoid=o.scene.globe.ellipsoid;o.camera.lookAt(d.a.Cartesian3.fromDegrees(97.41,28.67,8500),new d.a.Cartesian3(0,-1e3,1e3)),o.camera.lookAtTransform(d.a.Matrix4.IDENTITY);var c=new d.a.ScreenSpaceEventHandler(o.canvas);function u(){t.testLong=d.a.Math.toDegrees(t.startCartographic.longitude).toFixed(4),t.testLat=d.a.Math.toDegrees(t.startCartographic.latitude).toFixed(4),t.startPointStr="lon:"+d.a.Math.toDegrees(t.startCartographic.longitude).toFixed(4)+"°  lat:"+d.a.Math.toDegrees(t.startCartographic.latitude).toFixed(4)+"°",t.lastDist=t.dist,t.axios.get("/api/dem/height/"+t.testLong+"/"+t.testLat).then(function(e){t.startPointServerHeight=e.data},function(e){}),o.entities.remove(t.circle),t.circle=null,t.dist=10,i=new d.a.CallbackProperty(function(){return t.dist},!1),t.viewshedDefault(t.testLong,t.testLat,t.testToLong,t.testToLat,t.seeHeight)}c.setInputAction(function(e){if(d.a.Entity.supportsPolylinesOnTerrain(o.scene)){var n=o.scene.pickPosition(e.position),r=l.cartesianToCartographic(n);if(a)a=!1,t.testToLong=d.a.Math.toDegrees(r.longitude).toFixed(6),t.testToLat=d.a.Math.toDegrees(r.latitude).toFixed(6),u();else{a=!0,t.startPoint=n,t.startCartographic=r;var s=o.entities.add({position:t.startPoint,point:{color:d.a.Color.RED,outlineColor:d.a.Color.WHITE,outlineWidth:2,pixelSize:5,heightReference:d.a.HeightReference.CLAMP_TO_GROUND}});t.shapes.push(s),t.circle=o.entities.add({position:t.startPoint,name:"Green circle at height",ellipse:{outlineColor:d.a.Color.WHITE,outlineWidth:2,semiMinorAxis:i,semiMajorAxis:i,material:d.a.Color.BLUE.withAlpha(.3)}})}}else console.log("This browser does not support polylines on terrain.")},d.a.ScreenSpaceEventType.LEFT_CLICK),c.setInputAction(function(e){if(a){var i=o.scene.pickPosition(e.endPosition);if(d.a.defined(i)){var r=Math.sqrt(Math.pow(i.x-t.startPoint.x,2)+Math.pow(i.y-t.startPoint.y,2)+Math.pow(i.z-t.startPoint.z,2));t.dist=r}}var l=o.scene.pickPosition(e.endPosition);if(l){var c=d.a.Cartographic.fromCartesian(l),u=d.a.Math.toDegrees(c.longitude),m=d.a.Math.toDegrees(c.latitude),p=u.toFixed(4),f=m.toFixed(4),v=(s.globe.ellipsoid.cartesianToCartographic(l),o.camera.getPickRay(e.endPosition)),h=o.scene.globe.pick(v,o.scene),g=d.a.Cartographic.fromCartesian(h);if(t.mouseLocation.lon=p+"°",t.mouseLocation.lat=f+"°",t.mouseLocation.h=g.height.toFixed(4)+"米（概略）","true"==t.realTimeTest){if(Math.abs(t.lastlon-u)>1e-4||Math.abs(t.lastlat-m)>1e-4){t.realTestTime=new Date,t.realTestTimeStr="发送";var b=u+t.realTimeTestR;n.send(u+","+m+","+b+","+m+","+t.seeHeight),t.realTestTime=new Date,t.realTestSendCount++,t.lastlon=u,t.lastlat=m}}else null!=t.updateShape&&(t.viewer.entities.remove(t.updateShape),t.updateShape=null),t.realTestTimeStr=""}},d.a.ScreenSpaceEventType.MOUSE_MOVE),c.setInputAction(function(n){a=!1,e.each(t.shapes,function(e,t){o.entities.remove(t)}),t.shapes=[]},d.a.ScreenSpaceEventType.RIGHT_CLICK)},methods:{stringToByte:function(e){var t,n,a=new Array;t=e.length;for(var i=0;i<t;i++)n=e.charCodeAt(i),n>=65536&&n<=1114111?(a.push(n>>18&7|240),a.push(n>>12&63|128),a.push(n>>6&63|128),a.push(63&n|128)):n>=2048&&n<=65535?(a.push(n>>12&15|224),a.push(n>>6&63|128),a.push(63&n|128)):n>=128&&n<=2047?(a.push(n>>6&31|192),a.push(63&n|128)):a.push(255&n);return a},viewshedDefault:function(e,t,n,a,i){var r=this;this.axios.get("/api/dem/analysis/"+e+"/"+t+"/"+n+"/"+a+"/"+i).then(function(e){new Array;r.CaculateCount=e.data.allPointCount,r.TimeSpan=e.data.time,r.drawVisibleLines(e.data.visiblePoints)},function(e){})},updateVisibleLines:function(e){var t=this,n=e.split("|"),a=n[0].split(","),i=d.a.Cartesian3.fromDegreesArray([c()(a[0]),c()(a[1]),c()(a[2]),c()(a[3]),c()(a[4]),c()(a[5]),c()(a[6]),c()(a[7]),c()(a[8]),c()(a[9])]),o=n[1].split(","),l=s()(o[0]),u=s()(o[1]),m=t.stringToByte(n[2]),p=document.createElement("canvas");p.width=l,p.height=u;var f=p.getContext("2d");f.fillStyle="rgba(0,255,0,0.2)";for(var v=0;v<l;v++)for(var h=0;h<u;h++)1==m[v*l+h]&&f.fillRect(v,u-1-h,1,1);if(t.context=p,null==t.updateShape){var g={name:"Red polygon on surface",polygon:{hierarchy:i,material:new d.a.ImageMaterialProperty(Object(r["a"])({image:t.material,transparent:!0,color:d.a.Color.WHITE},"transparent",.5)),classificationType:d.a.ClassificationType.BOTH}};t.updateShape=t.viewer.entities.add(g)}else t.updateShape._polygon.hierarchy=i,t.updateShape._polygon.material.image=t.context},drawVisibleLines:function(e,t,n){var a=this,i=a.viewer.entities.add({name:"Red polygon on surface",polygon:{hierarchy:d.a.Cartesian3.fromDegreesArray(e.hierarchy),material:new d.a.ImageMaterialProperty(Object(r["a"])({image:a.drawPointFaces(e,t,n),transparent:!0,color:d.a.Color.WHITE},"transparent",.5)),classificationType:d.a.ClassificationType.BOTH}});a.shapes.push(i),i=a.viewer.entities.add({position:a.startPoint,name:"Green circle at height",ellipse:{outlineColor:d.a.Color.WHITE,outlineWidth:5,semiMinorAxis:5,semiMajorAxis:5,material:d.a.Color.RED}}),a.shapes.push(i)},drawPointFaces:function(e,t,n){var a=document.createElement("canvas");a.width=2*e.x,a.height=2*e.y;var i=a.getContext("2d");i.fillStyle="rgba(0,255,0,0.2)";for(var r=0;r<e.x;r++)for(var o=0;o<e.y;o++)1==e.values[r][o]&&i.fillRect(2*r,2*(e.y-1-o),2,2);if(void 0!=t){i.fillStyle="rgba(0,0,255,0.8)";for(r=0;r<t.length;r++)i.fillRect(2*t[r].i,2*(e.y-1-t[r].j),2,2)}if(void 0!=n){i.fillStyle="rgba(255,0,0,0.8)";for(r=0;r<n.length;r++)i.fillRect(2*n[r].i,2*(e.y-1-n[r].j),2,2)}return a},testOSD:function(){this.testVisibleAnalysis("osd")},testFOSD:function(){this.testVisibleAnalysis("fast_osd")},testXDraw:function(){this.testVisibleAnalysis("xdraw")},testRefF:function(){this.testVisibleAnalysis("reff")},testVisibleAnalysis:function(e){var t=this;t.axios.get("/api/dem/analysis_"+e+"/"+t.testLong+"/"+t.testLat+"/"+t.testToLong+"/"+t.testToLat+"/"+t.seeHeight).then(function(e){new Array;t.CaculateCount=e.data.allPointCount,t.TimeSpan=e.data.time,t.drawVisibleLines(e.data.visiblePoints,e.data.visibleErrPoints,e.data.unVisibleErrPoints),t.info="计算点数："+e.data.allPointCount+";\r\n通视率："+(100*e.data.visibleRate).toFixed(4)+"%;\r\n可见点错误数："+e.data.visibleErrCount+";\r\n可见点错误率："+(100*e.data.visibleErrRate).toFixed(4)+"%;\r\n不可见点错误数："+e.data.unVisibleErrCount+";\r\n不可见点错误率："+(100*e.data.unVisibleErrRate).toFixed(4)+"%;\r\n错误率："+(100*e.data.errRate).toFixed(4)+"%;"},function(e){})},lookAtTestPoint:function(){var e=this;e.axios.get("/api/dem/height/"+e.testLong+"/"+e.testLat).then(function(t){e.startPointServerHeight=t.data,e.viewer.camera.setView({destination:d.a.Cartesian3.fromDegrees(Number(e.testLong),Number(e.testLat),e.startPointServerHeight+Number(e.seeHeight)),orientation:{heading:d.a.Math.toRadians(0),pitch:d.a.Math.toRadians(0),roll:0}})},function(e){})},lookLeft10:function(){this.viewer.camera.lookLeft(d.a.Math.toRadians(10))},lookRight10:function(){this.viewer.camera.lookRight(d.a.Math.toRadians(10))},lookUp5:function(){this.viewer.camera.lookUp(d.a.Math.toRadians(5))},lookDown5:function(){this.viewer.camera.lookDown(d.a.Math.toRadians(5))},computHeight:function(){var e=i()(),t=[d.a.Cartographic.fromDegrees(Number(this.testLong),Number(this.testLat))],n=this,a=d.a.sampleTerrain(n.terrainProvider,15,t);d.a.when(a,function(t){n.info=t,console.log(i()()-e)})}},computed:{startPointServerHeightStr:function(){return 0==this.startPointServerHeight?"未计算":this.startPointServerHeight.toFixed(4)+"米"},realTimeTestRStr:function(){return"大约实地"+108*this.realTimeTestR+" km"}}}}).call(this,n("1157"))},"429a":function(e,t){function n(e){var t=new Error("Cannot find module '"+e+"'");throw t.code="MODULE_NOT_FOUND",t}n.keys=function(){return[]},n.resolve=n,e.exports=n,n.id="429a"},"440b":function(e,t,n){"use strict";var a=n("49e3"),i=n.n(a);i.a},4678:function(e,t,n){var a={"./af":"2bfb","./af.js":"2bfb","./ar":"8e73","./ar-dz":"a356","./ar-dz.js":"a356","./ar-kw":"423e","./ar-kw.js":"423e","./ar-ly":"1cfd","./ar-ly.js":"1cfd","./ar-ma":"0a84","./ar-ma.js":"0a84","./ar-sa":"8230","./ar-sa.js":"8230","./ar-tn":"6d83","./ar-tn.js":"6d83","./ar.js":"8e73","./az":"485c5","./az.js":"485c5","./be":"1fc1","./be.js":"1fc1","./bg":"84aa","./bg.js":"84aa","./bm":"a7fa","./bm.js":"a7fa","./bn":"9043","./bn.js":"9043","./bo":"d26a","./bo.js":"d26a","./br":"6887","./br.js":"6887","./bs":"2554","./bs.js":"2554","./ca":"d716","./ca.js":"d716","./cs":"3c0d","./cs.js":"3c0d","./cv":"03ec","./cv.js":"03ec","./cy":"9797","./cy.js":"9797","./da":"0f14","./da.js":"0f14","./de":"b469","./de-at":"b3eb","./de-at.js":"b3eb","./de-ch":"bb718","./de-ch.js":"bb718","./de.js":"b469","./dv":"598a","./dv.js":"598a","./el":"8d47","./el.js":"8d47","./en-au":"0e6b","./en-au.js":"0e6b","./en-ca":"3886","./en-ca.js":"3886","./en-gb":"39a6","./en-gb.js":"39a6","./en-ie":"e1d3","./en-ie.js":"e1d3","./en-nz":"6f50","./en-nz.js":"6f50","./eo":"65db","./eo.js":"65db","./es":"898b","./es-do":"0a3c","./es-do.js":"0a3c","./es-us":"55c9","./es-us.js":"55c9","./es.js":"898b","./et":"ec18","./et.js":"ec18","./eu":"0ff2","./eu.js":"0ff2","./fa":"8df4","./fa.js":"8df4","./fi":"81e9","./fi.js":"81e9","./fo":"0721","./fo.js":"0721","./fr":"9f26","./fr-ca":"d9f8","./fr-ca.js":"d9f8","./fr-ch":"0e49","./fr-ch.js":"0e49","./fr.js":"9f26","./fy":"7118","./fy.js":"7118","./gd":"f6b4","./gd.js":"f6b4","./gl":"8840","./gl.js":"8840","./gom-latn":"0caa","./gom-latn.js":"0caa","./gu":"e0c5","./gu.js":"e0c5","./he":"c7aa","./he.js":"c7aa","./hi":"dc4d","./hi.js":"dc4d","./hr":"4ba9","./hr.js":"4ba9","./hu":"5b14","./hu.js":"5b14","./hy-am":"d6b6","./hy-am.js":"d6b6","./id":"5038","./id.js":"5038","./is":"0558","./is.js":"0558","./it":"6e98","./it.js":"6e98","./ja":"079e","./ja.js":"079e","./jv":"b540","./jv.js":"b540","./ka":"201b","./ka.js":"201b","./kk":"6d79","./kk.js":"6d79","./km":"e81d","./km.js":"e81d","./kn":"3e92","./kn.js":"3e92","./ko":"22f8","./ko.js":"22f8","./ky":"9609","./ky.js":"9609","./lb":"440c","./lb.js":"440c","./lo":"b29d","./lo.js":"b29d","./lt":"26f9","./lt.js":"26f9","./lv":"b97c","./lv.js":"b97c","./me":"293c","./me.js":"293c","./mi":"688b","./mi.js":"688b","./mk":"6909","./mk.js":"6909","./ml":"02fb","./ml.js":"02fb","./mr":"39bd","./mr.js":"39bd","./ms":"ebe4","./ms-my":"6403","./ms-my.js":"6403","./ms.js":"ebe4","./mt":"1b45","./mt.js":"1b45","./my":"8689","./my.js":"8689","./nb":"6ce3","./nb.js":"6ce3","./ne":"3a39","./ne.js":"3a39","./nl":"facd","./nl-be":"db29","./nl-be.js":"db29","./nl.js":"facd","./nn":"b84c","./nn.js":"b84c","./pa-in":"f3ff","./pa-in.js":"f3ff","./pl":"8d57","./pl.js":"8d57","./pt":"f260","./pt-br":"d2d4","./pt-br.js":"d2d4","./pt.js":"f260","./ro":"972c","./ro.js":"972c","./ru":"957c","./ru.js":"957c","./sd":"6784","./sd.js":"6784","./se":"ffff","./se.js":"ffff","./si":"eda5","./si.js":"eda5","./sk":"7be6","./sk.js":"7be6","./sl":"8155","./sl.js":"8155","./sq":"c8f3","./sq.js":"c8f3","./sr":"cf1e","./sr-cyrl":"13e9","./sr-cyrl.js":"13e9","./sr.js":"cf1e","./ss":"52bd","./ss.js":"52bd","./sv":"5fbd","./sv.js":"5fbd","./sw":"74dc","./sw.js":"74dc","./ta":"3de5","./ta.js":"3de5","./te":"5cbb","./te.js":"5cbb","./tet":"576c","./tet.js":"576c","./th":"10e8","./th.js":"10e8","./tl-ph":"0f38","./tl-ph.js":"0f38","./tlh":"cf75","./tlh.js":"cf75","./tr":"0e81","./tr.js":"0e81","./tzl":"cf51","./tzl.js":"cf51","./tzm":"c109","./tzm-latn":"b53d","./tzm-latn.js":"b53d","./tzm.js":"c109","./uk":"ada2","./uk.js":"ada2","./ur":"5294","./ur.js":"5294","./uz":"2e8c","./uz-latn":"010e","./uz-latn.js":"010e","./uz.js":"2e8c","./vi":"2921","./vi.js":"2921","./x-pseudo":"fd7e","./x-pseudo.js":"fd7e","./yo":"7f33","./yo.js":"7f33","./zh-cn":"5c3a","./zh-cn.js":"5c3a","./zh-hk":"49ab","./zh-hk.js":"49ab","./zh-tw":"90ea","./zh-tw.js":"90ea"};function i(e){var t=r(e);return n(t)}function r(e){var t=a[e];if(!(t+1)){var n=new Error("Cannot find module '"+e+"'");throw n.code="MODULE_NOT_FOUND",n}return t}i.keys=function(){return Object.keys(a)},i.resolve=r,e.exports=i,i.id="4678"},"49e3":function(e,t,n){},"56d7":function(e,t,n){"use strict";n.r(t);n("cadf"),n("551c"),n("f751"),n("097d");var a=n("a026"),i=function(){var e=this,t=e.$createElement,n=e._self._c||t;return n("div",{attrs:{id:"app"}},[n("router-view")],1)},r=[],o=(n("034f"),n("2877")),s={},l=Object(o["a"])(s,i,r,!1,null,null,null),c=l.exports,u=n("8c4f"),d=function(){var e=this,t=e.$createElement,n=e._self._c||t;return n("div",{staticClass:"hello"},[e._m(0),n("br"),n("div",{staticStyle:{"margin-left":"40px","margin-right":"40px",overflow:"auto"}},[n("h3",{staticStyle:{"text-align":"left"}},[e._v("说明")]),e._m(1),n("h3",{staticStyle:{"text-align":"left"}},[e._v("PDERL/XPDERL算法测试（测试用DEM在下面“切换测试用DEM数据”中选择）")]),n("div",{staticStyle:{"text-align":"justify",margin:"auto"}},[e._v("\n                1. 如果您想通过交互的方式了解XPDERL的性能，请"),n("router-link",{attrs:{to:"/ui"}},[e._v("在交互页面测试")]),e._v("(需要国际互联网)。"),n("br"),e._v("\n                2. 如果您想重复论文《Fast approximate viewshed analysis based on regular grid digital elevation model——X-type partition proximity-direction-elevation spatial reference line algorithm》\n                的实验，请点击以下链接。"),n("br"),e._v("\n                    👉"),n("a",{attrs:{href:"/api/dem/x_analysis_auto_test_time_without_r3"}},[e._v("3.1 Experiment 1: speed；")]),e._v("(需要8个小时以上，结果保存在程序目录的\\RunningLog\\固定半径速度测试.csv)"),n("br"),e._v("\n                    👉"),n("a",{attrs:{href:"/api/dem/x_analysis_auto_test_accuracy"}},[e._v("3.2 Experiment 2: accuracy；")]),e._v("(需要8个小时以上，结果保存在程序目录的\\RunningLog\\各高度各算法精度速度随机测试.csv)"),n("br"),e._v("\n                    👉"),n("a",{attrs:{href:"/api/dem/x_analysis_auto_test_neighbor_err?p=1"}},[e._v("3.3 Experiment 3: aggregation of error points；")]),e._v("(需要8个小时以上，结果保存在程序目录的\\RunningLog\\测试各邻域错误数【DEM文件名】.csv，具体操作请阅读代码)"),n("br"),e._v("\n                3. 其它可用的测试请阅读源代码\\Code\\XPDERL\\Controllers\\DemController.cs中的路由规则。\n            ")],1),n("h3",{staticStyle:{"text-align":"left"}},[e._v("切换测试用DEM数据")]),n("ul",{staticStyle:{"text-align":"left"}},e._l(e.demfiles,function(t,a){return n("li",{key:a,on:{click:function(t){return e.ClickActive(a)}}},[n("input",{attrs:{type:"radio",id:a},domProps:{value:t,checked:e.currentDem==t}}),n("label",{attrs:{for:a}},[e._v(e._s(t))])])}),0)])])},m=[function(){var e=this,t=e.$createElement,a=e._self._c||t;return a("div",{staticStyle:{"background-color":"#41b883",padding:"40px",height:"200px"}},[a("img",{attrs:{alt:"Vue logo",src:n("6c92")}})])},function(){var e=this,t=e.$createElement,n=e._self._c||t;return n("div",{staticStyle:{"text-align":"justify",margin:"auto"}},[e._v("\n\t\t\t\t这是快速精确通视域分析算法PDERL和XPDERL的验证和测试程序。\n\t\t\t\t该算法目前正在申请国家专利。您可免授权运用于:研究、学习、非盈利性质的公益建设。\n\t\t\t\t虽然目前专利申请人尚未明确商业运用的授权政策，\n\t\t\t\t但如果您计划将之运用于您的商业项目，建议您先联系以下邮箱咨询："),n("a",{attrs:{href:"mailto:blct_w@foxmail.com"}},[e._v("blct_w@foxmail.com")])])}],p={name:"HelloWorld",data:function(){return{currentDem:null,demfiles:null,windowHeight:document.documentElement.clientHeight}},mounted:function(){this.Initial()},methods:{Initial:function(){var e=this;e.axios.get("/api/dem").then(function(t){e.currentDem=t.data.current,e.demfiles=t.data.allFiles},function(e){alert("没有找到后台服务")})},ClickActive:function(e){var t=this;t.axios.get("http://localhost:8000/api/dem/setdem/"+encodeURIComponent(t.demfiles[e])).then(function(e){t.Initial()},function(e){alert("没有找到后台服务")})}}},f=p,v=(n("c0f8"),Object(o["a"])(f,d,m,!1,null,"1de877a6",null)),h=v.exports,g=function(){var e=this,t=e.$createElement,n=e._self._c||t;return n("cesiumViewer")},b=[],T=function(){var e=this,t=e.$createElement,n=e._self._c||t;return n("div",[n("table",{staticStyle:{width:"100%"}},[n("tr",[n("td",[n("div",{staticStyle:{"background-color":"#2fa8ec"}},[e._v("\n          Cesium场景\n          "),n("router-link",{attrs:{to:"/"}},[e._v("返回首页")])],1)]),e._m(0)]),n("tr",[e._m(1),n("td",[n("div",{staticStyle:{width:"300px","text-align":"left"}},[n("div",{staticStyle:{"overflow-y":"scroll"}},[n("div",[e._v("\n              鼠标实时位置：\n              "),n("hr",{staticStyle:{height:"1px",border:"none","border-top":"2px ridge #185598"}}),n("div",[e._v("\n                经度："+e._s(e.mouseLocation.lon)+" 纬度："+e._s(e.mouseLocation.lat)+"\n              ")]),n("div",[e._v("高度："+e._s(e.mouseLocation.h))]),n("div",[e._v("\n                观测者视点离地高度(米)：\n                "),n("input",{directives:[{name:"model",rawName:"v-model",value:e.seeHeight,expression:"seeHeight"}],staticStyle:{width:"40px"},attrs:{placeholder:"观测点高"},domProps:{value:e.seeHeight},on:{input:function(t){t.target.composing||(e.seeHeight=t.target.value)}}})]),n("br"),e._v("实时测试:\n              "),n("br"),n("hr",{staticStyle:{height:"1px",border:"none","border-top":"2px ridge #185598"}}),n("input",{directives:[{name:"model",rawName:"v-model",value:e.realTimeTest,expression:"realTimeTest"}],attrs:{type:"checkbox","true-value":"true","false-value":"false"},domProps:{checked:Array.isArray(e.realTimeTest)?e._i(e.realTimeTest,null)>-1:e._q(e.realTimeTest,"true")},on:{change:function(t){var n=e.realTimeTest,a=t.target,i=a.checked?"true":"false";if(Array.isArray(n)){var r=null,o=e._i(n,r);a.checked?o<0&&(e.realTimeTest=n.concat([r])):o>-1&&(e.realTimeTest=n.slice(0,o).concat(n.slice(o+1)))}else e.realTimeTest=i}}}),e._v("\n              执行实时测试"),n("br"),e._v("\n              测试半径(度):\n              "),n("input",{directives:[{name:"model",rawName:"v-model",value:e.realTimeTestR,expression:"realTimeTestR"}],staticStyle:{width:"40px"},attrs:{placeholder:"单位为度"},domProps:{value:e.realTimeTestR},on:{input:function(t){t.target.composing||(e.realTimeTestR=t.target.value)}}}),e._v("("+e._s(e.realTimeTestRStr)+")\n              "),n("div",[e._v("\n                发送计数："+e._s(e.realTestSendCount)+" 接收计数："+e._s(e.realTestReciveCount)+"\n              ")]),n("div",[e._v(e._s(e.realTestTimeStr))]),n("br"),e._v("与R3精度对比（R3较慢，需等待）:\n              "),n("br"),n("hr",{staticStyle:{height:"1px",border:"none","border-top":"2px ridge #185598"}}),e._v("\n              起点:\n              "),n("input",{directives:[{name:"model",rawName:"v-model",value:e.testLong,expression:"testLong"}],staticStyle:{width:"100px"},attrs:{placeholder:"测试经度"},domProps:{value:e.testLong},on:{input:function(t){t.target.composing||(e.testLong=t.target.value)}}}),n("input",{directives:[{name:"model",rawName:"v-model",value:e.testLat,expression:"testLat"}],staticStyle:{width:"100px"},attrs:{placeholder:"测试纬度"},domProps:{value:e.testLat},on:{input:function(t){t.target.composing||(e.testLat=t.target.value)}}}),n("br"),e._v("终点:\n              "),n("input",{directives:[{name:"model",rawName:"v-model",value:e.testToLong,expression:"testToLong"}],staticStyle:{width:"100px"},attrs:{placeholder:"测试经度"},domProps:{value:e.testToLong},on:{input:function(t){t.target.composing||(e.testToLong=t.target.value)}}}),n("input",{directives:[{name:"model",rawName:"v-model",value:e.testToLat,expression:"testToLat"}],staticStyle:{width:"100px"},attrs:{placeholder:"测试纬度"},domProps:{value:e.testToLat},on:{input:function(t){t.target.composing||(e.testToLat=t.target.value)}}}),n("br"),n("button",{on:{click:e.testOSD}},[e._v("PDERL")]),n("button",{on:{click:e.testFOSD}},[e._v("XPDERL")]),n("button",{on:{click:e.testXDraw}},[e._v("XDraw")]),n("button",{on:{click:e.testRefF}},[e._v("参考面")]),n("br"),n("button",{on:{click:e.computHeight}},[e._v("获取高度")]),n("button",{on:{click:e.lookAtTestPoint}},[e._v("定位视角")]),n("button",{on:{click:e.lookLeft10}},[e._v("←")]),n("button",{on:{click:e.lookRight10}},[e._v("→")]),n("button",{on:{click:e.lookUp5}},[e._v("↑")]),n("button",{on:{click:e.lookDown5}},[e._v("↓")]),n("br"),n("br"),e._v("通视分析信息：\n              "),n("hr",{staticStyle:{height:"1px",border:"none","border-top":"2px ridge #185598"}}),n("div",[e._v("\n                半径: "+e._s(e.lastDist.toFixed(4))+"米；高程:\n                "+e._s(e.startPointServerHeightStr)+"\n              ")]),n("div",[e._v("\n                计算时间（不包括显示时间）：\n                "),n("br"),e._v("\n                "+e._s(e.TimeSpan)+"\n              ")]),n("div",[e._v("\n                计算点量：\n                "+e._s(e.CaculateCount)+"\n              ")]),n("br"),e._v("输出信息：\n              "),n("hr",{staticStyle:{height:"1px",border:"none","border-top":"2px ridge #185598"}}),n("div",[e._v(e._s(e.info))])])])])])])])])},y=[function(){var e=this,t=e.$createElement,n=e._self._c||t;return n("td",[n("div",{staticStyle:{"background-color":"yellow"}},[e._v("调试操作")])])},function(){var e=this,t=e.$createElement,n=e._self._c||t;return n("td",{attrs:{valign:"top"}},[n("div",{staticClass:"cesium-viewer",attrs:{id:"cesiumContainer"}})])}],_=n("3e29"),x=_["a"],C=(n("7397"),Object(o["a"])(x,T,y,!1,null,"595977a1",null)),w=C.exports,j={components:{cesiumViewer:w}},S=j,R=Object(o["a"])(S,g,b,!1,null,null,null),D=R.exports,k=function(){var e=this,t=e.$createElement,n=e._self._c||t;return n("div",{staticClass:"home"},[n("OSDTestReport",{attrs:{MethodName:"高精度OSD参考线算法",analysisMethod:"analysis_osd_r"}})],1)},L=[],O=function(){var e=this,t=e.$createElement,n=e._self._c||t;return n("div",[n("HeaderTittle",{attrs:{Title:e.MethodName}}),n("div",{staticStyle:{margin:"10px"}},[n("div",{staticClass:"left-content"},[e._v("\n      测试区域起始经度(度)：\n      "),n("input",{directives:[{name:"model",rawName:"v-model",value:e.minlon,expression:"minlon"}],domProps:{value:e.minlon},on:{input:function(t){t.target.composing||(e.minlon=t.target.value)}}}),e._v("\n      起始纬度(度)：\n      "),n("input",{directives:[{name:"model",rawName:"v-model",value:e.minlat,expression:"minlat"}],domProps:{value:e.minlat},on:{input:function(t){t.target.composing||(e.minlat=t.target.value)}}}),e._v("\n      截止经度(度)：\n      "),n("input",{directives:[{name:"model",rawName:"v-model",value:e.maxlon,expression:"maxlon"}],domProps:{value:e.maxlon},on:{input:function(t){t.target.composing||(e.maxlon=t.target.value)}}}),e._v("\n      截止纬度(度)：\n      "),n("input",{directives:[{name:"model",rawName:"v-model",value:e.maxlat,expression:"maxlat"}],domProps:{value:e.maxlat},on:{input:function(t){t.target.composing||(e.maxlat=t.target.value)}}}),e._v("\n      视点高度(米)：\n      "),n("input",{directives:[{name:"model",rawName:"v-model",value:e.seeHeight,expression:"seeHeight"}],domProps:{value:e.seeHeight},on:{input:function(t){t.target.composing||(e.seeHeight=t.target.value)}}}),e._v("\n      随机测试次数：\n      "),n("input",{directives:[{name:"model",rawName:"v-model",value:e.testCount,expression:"testCount"}],attrs:{value:"5"},domProps:{value:e.testCount},on:{input:function(t){t.target.composing||(e.testCount=t.target.value)}}}),n("button",{staticStyle:{"margin-left":"10px"},on:{click:e.startTest}},[e._v("开始测试")])]),n("div",{staticClass:"left-content"},[e._v("\n      默认测试区域DEM格网为30米，数据为东经119°~120°，北纬41°~42°，避免边界运算出现错误，填写测试范围不要超过此范围，且应当适当缩小。\n      同时，为避免计算区域过小，范围直径不可小于0.027(30米格网大概10个网格间距)。\n    ")]),n("br"),n("HotTable",{ref:"testHot",staticStyle:{overflow:"auto","min-height":"400px"},attrs:{root:e.root,settings:e.hotSettings}}),n("div",{staticClass:"left-content"},[e._v(e._s(e.info))])],1),n("div",{class:{coverLayout:e.isOnTesting}},[n("div",{staticStyle:{"font-size":"50px","margin-top":"400px"}},[e._v("正在计算...")])])],1)},P=[],E=n("3c78"),H=(n("8033"),n("1699"),n("3e8f"),function(){var e=this,t=e.$createElement,n=e._self._c||t;return n("div",{staticStyle:{"background-color":"#41b883","padding-top":"30px","padding-bottom":"20px"}},[n("h1",[e._v(e._s(e.Title))]),n("router-link",{attrs:{to:"/"}},[e._v("返回首页")])],1)}),M=[],F={name:"headertittle",props:{Title:String}},N=F,A=Object(o["a"])(N,H,M,!1,null,null,null),z=A.exports,I={props:{analysisMethod:String,MethodName:String,HeaderTittle:z},data:function(){return{info:"",minlon:119.15,maxlon:119.95,minlat:41.15,maxlat:41.95,seeHeight:2,testCount:10,isOnTesting:!1,root:"test-hot",hotSettings:{data:[],minRows:1,minCols:5,maxRows:1e4,maxCols:20,rowHeaders:!0,colHeaders:["中心点坐标","计算点总数","通视率","R3耗时(秒)","OSD算法耗时(秒)","OSD单点耗时(纳秒)","误点数","总错误率","参考线交叉点数","交叉点计算量","交叉点求解失败量"],columns:[{data:"location",type:"text"},{data:"allPointCount",type:"numeric"},{data:"visibleRate",type:"numeric",renderer:this.percentageCellRender},{data:"r3Time",type:"numeric",numericFormat:{pattern:"0.00000"}},{data:"time",type:"numeric",numericFormat:{pattern:"0.00000"}},{data:"perPointTime",type:"numeric",numericFormat:{pattern:"0.00"}},{data:"osdAllErrCount",type:"numeric"},{data:"osdErrRate",type:"numeric",renderer:this.percentageCellRender},{data:"crossCount",type:"numeric"},{data:"crossCaculateCount",type:"numeric"},{data:"crossFailCount",type:"numeric"}],currentRowClassName:"currentRow",currentColClassName:"currentCol",autoWrapRow:!0,fillHandle:!0,fixedColumnsLeft:0,fixedRowsTop:0,manualColumnFreeze:!0,manualColumnMove:!0,manualRowMove:!0,manualColumnResize:!0,manualRowResize:!0,comments:!0,columnSorting:!0,stretchH:"all",licenseKey:"non-commercial-and-evaluation"}}},name:"handsonTable",components:{HotTable:E["a"],HeaderTittle:z},methods:{percentageCellRender:function(e,t,n,a,i,r,o){return null!=r&&(t.innerHTML=0==r?"0":(100*r).toFixed(4)+"%"),t},startTest:function(){var e=this;e.info="",e.isOnTesting=!0,e.axios.get("http://localhost:8000/api/dem/"+e.analysisMethod+"/"+e.minlon+"/"+e.maxlat+"/"+e.maxlon+"/"+e.minlat+"/"+e.seeHeight+"/"+e.testCount).then(function(t){e.hotSettings.data=t.data,e.isOnTesting=!1,e.info="共"+t.data.length+"条数据，本表格最大能显示50条。"},function(t){alert("出现错误"),e.isOnTesting=!1})}}},V=I,U=(n("7792"),Object(o["a"])(V,O,P,!1,null,"a8abcc0a",null)),X=U.exports,W={name:"home",components:{OSDTestReport:X}},$=W,B=Object(o["a"])($,k,L,!1,null,null,null),J=B.exports,G=function(){var e=this,t=e.$createElement,n=e._self._c||t;return n("div",{staticClass:"home"},[n("OSDTestReport",{attrs:{MethodName:"快速OSD参考线算法",analysisMethod:"analysis_fosd_r"}})],1)},q=[],K={name:"home",components:{OSDTestReport:X}},Z=K,Y=Object(o["a"])(Z,G,q,!1,null,null,null),Q=Y.exports,ee=function(){var e=this,t=e.$createElement,n=e._self._c||t;return n("div",{staticClass:"home"},[n("AllTestReport")],1)},te=[],ne=function(){var e=this,t=e.$createElement,n=e._self._c||t;return n("div",[n("HeaderTittle",{attrs:{Title:"各类算法综合对比页面"}}),n("div",{staticStyle:{margin:"10px"}},[n("div",{staticClass:"left-content"},[e._v("\n      测试半径(度)：\n      "),n("input",{directives:[{name:"model",rawName:"v-model",value:e.r,expression:"r"}],domProps:{value:e.r},on:{input:function(t){t.target.composing||(e.r=t.target.value)}}}),e._v("\n      视点高度(米)：\n      "),n("input",{directives:[{name:"model",rawName:"v-model",value:e.seeHeight,expression:"seeHeight"}],domProps:{value:e.seeHeight},on:{input:function(t){t.target.composing||(e.seeHeight=t.target.value)}}}),e._v("\n      随机测试最大次数(大于0有效)：\n      "),n("input",{directives:[{name:"model",rawName:"v-model",value:e.testCount,expression:"testCount"}],attrs:{value:"0"},domProps:{value:e.testCount},on:{input:function(t){t.target.composing||(e.testCount=t.target.value)}}}),n("button",{staticStyle:{"margin-left":"10px"},on:{click:e.startTest}},[e._v("开始测试")])]),n("div",{staticClass:"left-content"},[e._v("半径不可小于0.001,不可大于0.4999。")]),n("br"),n("HotTable",{ref:"testHot",staticStyle:{overflow:"auto","min-height":"400px"},attrs:{root:e.root,settings:e.hotSettings}}),n("div",{staticClass:"left-content"},[e._v(e._s(e.info))])],1),n("div",{class:{coverLayout:e.isOnTesting}},[n("div",{staticStyle:{"font-size":"50px","margin-top":"400px"}},[e._v("正在计算...")])])],1)},ae=[],ie={name:"handsonTable",data:function(){return{info:"",r:.01,seeHeight:2,testCount:10,isOnTesting:!1,root:"test-hot",hotSettings:{data:[],minRows:1,minCols:5,maxRows:1e4,maxCols:20,rowHeaders:!0,colHeaders:["半径","总点量","通视率","R3耗时(秒)","OSD算法耗时(秒)","FOSD算法耗时(秒)","参考面算法耗时(秒)","XDraw算法耗时(秒)","OSD总错误率","OSD可视错误率","OSD不可视错误率","FOSD总错误率","FOSD可视错误率","FOSD不可视错误率","参考面总错误率","参考面可视错误率","参考面不可视错误率","XDraw总错误率","XDraw可视错误率","XDraw不可视错误率"],columns:[{data:"r",type:"numeric"},{data:"allPointCount",type:"numeric"},{data:"visibleRate",type:"numeric",renderer:this.percentageCellRender},{data:"r3Time",type:"numeric",numericFormat:{pattern:"0.00000"}},{data:"osdTime",type:"numeric",numericFormat:{pattern:"0.00000"}},{data:"fosdTime",type:"numeric",numericFormat:{pattern:"0.00000"}},{data:"refFTime",type:"numeric",numericFormat:{pattern:"0.00000"}},{data:"xDrawTime",type:"numeric",numericFormat:{pattern:"0.00000"}},{data:"osdErrRate",type:"numeric",renderer:this.percentageCellRender},{data:"osdVisibleErrRate",type:"numeric",renderer:this.percentageCellRender},{data:"osdUnVisibleErrRate",type:"numeric",renderer:this.percentageCellRender},{data:"fosdErrRate",type:"numeric",renderer:this.percentageCellRender},{data:"fosdVisibleErrRate",type:"numeric",renderer:this.percentageCellRender},{data:"fosdUnVisibleErrRate",type:"numeric",renderer:this.percentageCellRender},{data:"refFErrRate",type:"numeric",renderer:this.percentageCellRender},{data:"refFVisibleErrRate",type:"numeric",renderer:this.percentageCellRender},{data:"refFUnVisibleErrRate",type:"numeric",renderer:this.percentageCellRender},{data:"xDrawErrRate",type:"numeric",renderer:this.percentageCellRender},{data:"xDrawVisibleErrRate",type:"numeric",renderer:this.percentageCellRender},{data:"xDrawUnVisibleErrRate",type:"numeric",renderer:this.percentageCellRender}],currentRowClassName:"currentRow",currentColClassName:"currentCol",autoWrapRow:!0,fillHandle:!0,fixedColumnsLeft:0,fixedRowsTop:0,manualColumnFreeze:!0,manualColumnMove:!0,manualRowMove:!0,manualColumnResize:!0,manualRowResize:!0,comments:!0,columnSorting:!0,stretchH:"all",licenseKey:"non-commercial-and-evaluation"}}},components:{HotTable:E["a"],HeaderTittle:z},methods:{percentageCellRender:function(e,t,n,a,i,r,o){return null!=r&&(t.innerHTML=0==r?"0":(100*r).toFixed(4)+"%"),t},startTest:function(){var e=this;e.info="",e.isOnTesting=!0,e.axios.get("http://localhost:8000/api/dem/analysis_all_fixed_r/"+e.r+"/"+e.seeHeight+"/"+e.testCount).then(function(t){e.hotSettings.data=t.data,e.isOnTesting=!1,e.info="共"+t.data.length+"条数据，本表格最大能显示50条。"},function(t){alert("出现错误"),e.isOnTesting=!1})}}},re=ie,oe=(n("440b"),Object(o["a"])(re,ne,ae,!1,null,"158b6a82",null)),se=oe.exports,le={name:"home",components:{AllTestReport:se}},ce=le,ue=Object(o["a"])(ce,ee,te,!1,null,null,null),de=ue.exports,me=function(){var e=this,t=e.$createElement,n=e._self._c||t;return n("div",{staticClass:"home"},[n("AllSpeedTestReport")],1)},pe=[],fe=function(){var e=this,t=e.$createElement,n=e._self._c||t;return n("div",[n("HeaderTittle",{attrs:{Title:"各类算法速度比较页面"}}),n("div",{staticStyle:{margin:"10px"}},[n("div",{staticClass:"left-content"},[e._v("\n      测试半径(度)：\n      "),n("input",{directives:[{name:"model",rawName:"v-model",value:e.r,expression:"r"}],domProps:{value:e.r},on:{input:function(t){t.target.composing||(e.r=t.target.value)}}}),e._v("\n      视点高度(米)：\n      "),n("input",{directives:[{name:"model",rawName:"v-model",value:e.seeHeight,expression:"seeHeight"}],domProps:{value:e.seeHeight},on:{input:function(t){t.target.composing||(e.seeHeight=t.target.value)}}}),e._v("\n      采样多少个点：\n      "),n("input",{directives:[{name:"model",rawName:"v-model",value:e.maxCount,expression:"maxCount"}],domProps:{value:e.maxCount},on:{input:function(t){t.target.composing||(e.maxCount=t.target.value)}}}),n("button",{staticStyle:{"margin-left":"10px"},on:{click:e.startTest}},[e._v("开始测试")])]),n("div",{staticClass:"left-content"},[e._v("\n      默认测试区域DEM格网为30米，数据为东经119°~120°，北纬41°~42°，避免边界运算出现错误，填写测试范围不要超过此范围，且应当适当缩小。\n      同时，为避免计算区域过小，范围直径不可小于0.027(30米格网大概10个网格间距)。\n    ")]),n("br"),n("HotTable",{ref:"testHot",staticStyle:{overflow:"auto","min-height":"400px"},attrs:{root:e.root,settings:e.hotSettings}}),n("div",{staticClass:"left-content"},[e._v(e._s(e.info))])],1),n("div",{class:{coverLayout:e.isOnTesting}},[n("div",{staticStyle:{"font-size":"50px","margin-top":"400px"}},[e._v("正在计算...")])])],1)},ve=[],he={data:function(){return{info:"",r:.01,seeHeight:2,maxCount:10,isOnTesting:!1,root:"test-hot",hotSettings:{data:[],minRows:1,minCols:5,maxRows:1e7,maxCols:20,rowHeaders:!0,colHeaders:["半径","总点量","通视率","OSD算法耗时(秒)","FOSD算法耗时(秒)","参考面算法耗时(秒)","XDraw算法耗时(秒)"],columns:[{data:"r",type:"numeric"},{data:"allPointCount",type:"numeric"},{data:"visibleRate",type:"numeric",renderer:this.percentageCellRender},{data:"osdTime",type:"numeric",numericFormat:{pattern:"0.00000"}},{data:"fosdTime",type:"numeric",numericFormat:{pattern:"0.00000"}},{data:"refFTime",type:"numeric",numericFormat:{pattern:"0.00000"}},{data:"xDrawTime",type:"numeric",numericFormat:{pattern:"0.00000"}}],currentRowClassName:"currentRow",currentColClassName:"currentCol",autoWrapRow:!0,fillHandle:!0,fixedColumnsLeft:0,fixedRowsTop:0,manualColumnFreeze:!0,manualColumnMove:!0,manualRowMove:!0,manualColumnResize:!0,manualRowResize:!0,comments:!0,columnSorting:!0,stretchH:"all",licenseKey:"non-commercial-and-evaluation"}}},name:"handsonTable",components:{HotTable:E["a"],HeaderTittle:z},methods:{percentageCellRender:function(e,t,n,a,i,r,o){return null!=r&&(t.innerHTML=0==r?"0":(100*r).toFixed(4)+"%"),t},startTest:function(){var e=this;e.info="",e.isOnTesting=!0,e.axios.get("http://localhost:8000/api/dem/analysis_time_fixed_r/"+e.r+"/"+e.seeHeight+"/"+e.maxCount).then(function(t){e.hotSettings.data=t.data,e.isOnTesting=!1,e.info="共"+t.data.length+"条数据，本表格最大能显示50条。"},function(t){alert("出现错误"),e.isOnTesting=!1})}}},ge=he,be=(n("984d"),Object(o["a"])(ge,fe,ve,!1,null,"48662ad4",null)),Te=be.exports,ye={name:"home",components:{AllSpeedTestReport:Te}},_e=ye,xe=Object(o["a"])(_e,me,pe,!1,null,null,null),Ce=xe.exports;a["a"].use(u["a"]);var we=new u["a"]({routes:[{path:"/",name:"主页",component:h},{path:"/ui",name:"通视交互操作页面",component:D},{path:"/osd",name:"OSD测试页面",component:J},{path:"/fosd",name:"FOSD测试页面",component:Q},{path:"/speed",name:"各类算法速度对比",component:Ce},{path:"/all",name:"各类算法综合对比",component:de},{path:"/about",name:"about",component:function(){return n.e("about").then(n.bind(null,"f820"))}}]}),je=n("bc3a"),Se=n.n(je);a["a"].config.productionTip=!1,a["a"].prototype.axios=Se.a,new a["a"]({router:we,render:function(e){return e(c)}}).$mount("#app")},"64a9":function(e,t,n){},"6c92":function(e,t,n){e.exports=n.p+"img/osd.56930385.png"},"6f83":function(e,t,n){},7397:function(e,t,n){"use strict";var a=n("8061"),i=n.n(a);i.a},7792:function(e,t,n){"use strict";var a=n("91a3"),i=n.n(a);i.a},8061:function(e,t,n){},"91a3":function(e,t,n){},"984d":function(e,t,n){"use strict";var a=n("deb1"),i=n.n(a);i.a},c0f8:function(e,t,n){"use strict";var a=n("6f83"),i=n.n(a);i.a},deb1:function(e,t,n){},f7d3:function(e,t){function n(e){var t=new Error("Cannot find module '"+e+"'");throw t.code="MODULE_NOT_FOUND",t}n.keys=function(){return[]},n.resolve=n,e.exports=n,n.id="f7d3"}});
//# sourceMappingURL=app.af671bf9.js.map