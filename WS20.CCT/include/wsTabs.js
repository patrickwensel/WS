// WS Tabs 
// Written by: J Tornow
// 2/7/06

var gTabSets = new Array();
var gTabSetsCount = 0;

//get all elements on the page with class="whatever"	
function getElementsByClassName(theClassName, theParentNode, theNodeName){
	var aObjects = new Array();
	
	if(theParentNode == null)
		theParentNode = document;
	if(theNodeName == null)
		theNodeName = '*';
		
	var aAllObjects = theParentNode.getElementsByTagName(theNodeName);
	var aAllObjectsCount = aAllObjects.length;
	
	var aRegExpPattern = new RegExp("(^|\\s)" + theClassName + "(\\s|$)");
	
	for (i = 0, j = 0; i < aAllObjectsCount; i++) {
		if(aRegExpPattern.test(aAllObjects[i].className)){
			aObjects[j] = aAllObjects[i];
			j++;
		}
	}
	
	return aObjects;
}


///////////////////////////////////////////////////////////////////////
//the initial page load function that builds the tabs
function WSTabsLoad(){	
	var aTabSets = getElementsByClassName("wsTabSet");
	var aTabSetsCount = aTabSets.length;
	
	for(var i=0;i<aTabSetsCount;i++){
		var aNewSet = new WSTabSet(aTabSets[i], gTabSetsCount);
		gTabSets[gTabSetsCount++] = aNewSet;
		
		var aTabHeads = getElementsByClassName("wsTabHead", aTabSets[i], "a");
		var aTabHeadsCount = aTabHeads.length;		

		for(var h=0;h<aTabHeadsCount;h++){
			aNewSet.addTabHead(aTabHeads[h]);				
		}
		
		var aTabPages = getElementsByClassName("wsTabPage", aTabSets[i], "div");
		var aTabPagesCount = aTabPages.length;
		for(var p=0;p<aTabPagesCount;p++){
			aNewSet.addTabPage(aTabPages[p]);
		}
	}
	
	//sets the class in ie 5.5 for the first tab
	var mLinks = document.getElementById("tabSet").getElementsByTagName("a");
	if (aTabSetsCount == 0 && mLinks.length > 0){
		mLinks[0].className = 'wsTabHead_selected';
	}
}
//
/////////////////////////////////////////////////////////////////////// 

//create the tabset
function WSTabSet(theDivObject, theTabSetCount) {
	this.aTabSetCount = theTabSetCount;
	
	this.mElement = theDivObject;
	this.aTabHeadsArray = new Array();
	this.aTabPagesArray = new Array();
	
	this.aTabHeadsCount = 0;
	this.aTabPagesCount = 0;
	
	this.aSelectedTabIndex = 0;	 //defaults to tab1 selected	
}

//add the header object to the tabSet
WSTabSet.prototype.addTabHead = function (theAObject){
	//add to the array
	this.aTabHeadsArray[this.aTabHeadsCount] = theAObject;		
	
	//sets up the header
	theAObject.href = "#";

	theAObject.onmouseover = function () { window.status=this.innerHTML; return true; };
	theAObject.onmousedown = function () { window.status=this.innerHTML; return true; };
	theAObject.onmouseout = function () { window.status=''; return true; };
	
	
	eval("theAObject.onclick = function () { clickTab(" + this.aTabSetCount + "," + this.aTabHeadsCount + ");  };");
			
	if(this.aTabHeadsCount == 0){
		theAObject.className = "wsTabHead_selected";
	}
	
	this.aTabHeadsCount++;
}

//add the page object to the tabSet
WSTabSet.prototype.addTabPage = function (theDivObject){
	//add to the array
	this.aTabPagesArray[this.aTabPagesCount] = theDivObject;
	this.aTabPagesCount++;
	
	//sets up the page
	if(this.aTabPagesCount > 1){
		theDivObject.style.display = "none";
	}
}

WSTabSet.prototype.selectTab = function (theTabIndex){
	if(theTabIndex < 0){
		theTabIndex = 0;
	}
	
	if(theTabIndex >= this.aTabPagesCount){
		theTabIndex = this.aTabPagesCount - 1;
	}
	
	if(this.aSelectedTabIndex != theTabIndex){
		//clear old selected
		this.aTabHeadsArray[this.aSelectedTabIndex].className = "wsTabHead";
		this.aTabPagesArray[this.aSelectedTabIndex].style.display = "none";
		
		//select the new one			
		this.aTabHeadsArray[theTabIndex].className = "wsTabHead_selected";
		this.aTabPagesArray[theTabIndex].style.display = "block";
		this.aSelectedTabIndex = theTabIndex;
	}
}

function clickTab(theTabSet, theTab){ 
	try {
		gTabSets[theTabSet].selectTab(theTab);
	}
	catch(e){
		var aDivs = document.getElementById("tabSet").getElementsByTagName("div");
		var aLink = document.getElementById("tabSet").getElementsByTagName("a");		
		var wsTabCT = 0;
		for(var i=0;i<aDivs.length;i++){
			if (aDivs[i].className == "wsTabPage"){ 
				if (theTab == wsTabCT){ 
					aDivs[i].style.display = "";		
				}
				else{
					aDivs[i].style.display = "none";
				}
				wsTabCT++;
			}
				
			if (aLink[i].className == 'wsTabHead_selected'){
				aLink[i].className = "wsTabHead";
			}
		}	

		aLink[theTab].className = "wsTabHead_selected";
	}
}

// setup the init function to run on page load
if(typeof window.addEventListener != "undefined"){ 
	window.addEventListener( "load", WSTabsLoad, false );
}
else if( typeof window.attachEvent != "undefined" ){
	window.attachEvent( "onload", WSTabsLoad );
}	
else {
	if(window.onload != null){
		var aOldOnLoadStuff = window.onload;
		window.onload = function ( e ) {
			WSTabsLoad();
			aOldOnLoadStuff(e);			
		};
	}
	else {
		window.onload = WSTabsLoad;
	}
}