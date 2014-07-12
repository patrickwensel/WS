/* 	ws-lib.js v1.0: Last Updated 3/10/06
-------------------------------------------------------------------------------
-wsCommon : General Functions
-wsFade : Used to change opacity of an object, fade in or out
-wsTabs : Used to create css/js based div tabs
-parseDate: parses a date
-------------------------------------------------------------------------------*/


/* 	wsCommon 
---------------------------------------------------------------------------*/
function wsCommon() {
	this.GetElementsComputedStyle = function(theObject, theCssProperty, theMozCssProperty){     		
		if(theMozCssProperty == null){
			theMozCssProperty = theCssProperty;
		}
		
        	if(theObject.currentStyle){
			return eval("theObject.currentStyle." + theCssProperty);				
		}
		else {
			return document.defaultView.getComputedStyle(theObject, null).getPropertyValue(theMozCssProperty);
		}
	};
	
	this._GlobalVarCount = 0;
	this._GlobalVarArray = new Array();
	
	this.AddGlobalVar = function (theValue) {
		var aTmpCount = this._GlobalVarCount;
		this._GlobalVarArray[aTmpCount] = theValue;		
		
		this._GlobalVarCount++;
		
		return aTmpCount;
	}
	
	this.GetGlobalVar = function (theIndex) {
		try {
			return this._GlobalVarArray[theIndex];
		}
		catch(e){
			return null;
		}
	}		
	
	this.SetGlobalVar = function (theIndex, theValue){
		try {
			this._GlobalVarArray[theIndex] = theValue;			
			return true;
		}
		catch(e){
			return false;
		}
	}
	
	this.Vars = this._GlobalVarArray;
	
	this.HideObj = function (theObject){
		theObject.style.display = "none";			
	}	
	
	this.ShowObj = function (theObject){
		theObject.style.display = "";			
	}	
	
	this.getElementsByClassName = function (theClassName, theParentNode, theNodeName){
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
	};
	
	//wsTabs	
	this._TabSets = new Array();
	this._TabSetsCount = 0;
	
	// escapes single quotes in a passed string and returns that same string back
	this.EscapeSingleQuotes = function (theString){
		var aTmpString = "";
		
		var aOrigStringLen = theString.length;
		
		for(var i=0;i<aOrigStringLen;i++){
			var aTmp = theString.substring(i,i+1);
			if(aTmp != "'"){
				aTmpString += aTmp;
			}
			else {
				aTmpString += "\\'";
			}
		}	
		
		return aTmpString;	
	};
	
	// strips out characters of a string that are not letters or numbers
	this.StripNonNormalChars = function (theString) {
		var aStringLen = theString.length;
		var aNewString = "";
		
		for(var i=0;i<aStringLen;i++){
			var aTmpString = theString.substring(i, i+1);
			
			if(isRegChar(aTmpString)){
				aNewString += aTmpString;
			}
		}
		
		return aNewString;
	};
	
	// strips whitespace
	this.Trim = function (theString) {
		var aNewString = theString;
		
		while(aNewString.charAt(aNewString.length-1) == " ") {
			aNewString = aNewString.substring(0, aNewString.length-1);
		}
		while(aNewString.charAt(0) == " ") {
			aNewString = aNewString.substring(1, aNewString.length);
		}
		
		return aNewString;
	};
	
	
	// returns true if the string passed is null or blank
	this.IsEmpty = function (theString) {
		var aTmpString = this.Trim(theString);
		
		if(theString == null || aTmpString == ""){
			return true;
		}
		else {
			return false;
		}		
	};
	
	// returns true if the string passed in is a number
	
	this.IsNumber = function (theString, typeIn) {
		var numberIn = new String(theString);
		if (typeIn == "int") {
			var numbers = "0123456789";
		}
		else if (typeIn == "dec") {
			var numbers = "0123456789.-";
			var countDot = 0;
		}
		for(var i=0;i<numberIn.length;i++) {
			if (typeIn == "dec" && i == 0 && numberIn.substring(i,i+1) == "$") {
				continue;
			}
			else if (numbers.indexOf(numberIn.substring(i,i+1)) == -1) {
				return false;
			}
			else if (numberIn.substring(i,i+1) == ".") {
				countDot++;
				if (countDot > 1) {
					return false;
				}
			}
		}
		return true;	
	};
	
	// pass a form field, sets its value to mm/dd/yyyy or blank if the value isn't a date
	this.FormatDate = function(theObject) {		
		var aDateString = theObject.value;
		aDateString = _parseDate(aDateString);
		theObject.value = aDateString;
	};
	
	
	this.ValidateEmail = function(theEmail){
		var anEmailRE = /^[,\s]*(([a-zA-Z0-9_.]*\@[a-zA-Z0-9_.]*\.\w\w\w?)|([a-zA-Z0-9_.]*\@[a-zA-Z0-9_.]*\.\w\w\w?[,\s]*)+)[,\s]*$/;	
		
		if(theEmail == ""){
		    return true;
		}
		else if(anEmailRE.test(theEmail)){
			return true;
		}
		else {
			return false;
		}
	};	
}

var ws = new wsCommon();

/* 	wsFade : Written By: Johnny T, 3/9/06
---------------------------------------------------------------------------*/
function wsFade(theObject, theOpacity, theDuration, theSteps, theEndAction){
	this.aObject = theObject; 		//object to fade
	this.aOpacity = theOpacity;		//ending opacity
	this.aSteps = theSteps;			//number of steps left till opacity
	this.aDuration = theDuration;	//duration of fade animation (ms)
	this.aOnEnd = theEndAction;
		
	this.IsFinished = function () {
		if(parseInt(this.aSteps) <= 0){
			return true;
		}	
		else {
			return false;
		}
	};
	
	this.Start = function (){
		this._DoFade();
	};
	
	this._GetOpacity = function() {
		if(this.aObject.wsOpacity == undefined){
			var aOpacity = ws.GetElementsComputedStyle(this.aObject, "opacity");			
			this.aObject.wsOpacity = aOpacity == undefined ? 1 : aOpacity;
		}
		
		return parseFloat(this.aObject.wsOpacity);
	};
	
	this._ChangeOpacityBy = function(theChangeBy) {
		var aCurOpacity = this._GetOpacity();
		
		var aNewOpacity = Math.max(0, Math.min(aCurOpacity + theChangeBy, 1));
		
		this.aObject.wsOpacity = aNewOpacity;		
		this.aObject.style.opacity = aNewOpacity;	
		this.aObject.style.filter = "progid:DXImageTransform.Microsoft.Alpha(style=0,opacity=" + Math.round(aNewOpacity*100) + ")";
	};
	
	this._DoFade = function () {
		if(this.aTimer){
			window.clearTimeout(this.aTimer);
		}
		
		if(this.IsFinished()) {
			if(this.aOnEnd == "hide"){				
				ws.HideObj(this.aObject);								
			}				
			
			if(typeof this.aOnEnd == "function"){
				this.aOnEnd(this.aObject);
			}
			
			return;
		}
		
		var aStepDuration = Math.round(this.aDuration / this.aSteps);
		var aCurOpacity = this._GetOpacity();
		
		var aOpacChange = this.aSteps > 0 ? (this.aOpacity - aCurOpacity) / this.aSteps : 0;
		
		this._ChangeOpacityBy(aOpacChange);
		this.aDuraction -= aStepDuration;
		this.aSteps--;
		
		var aGlobalKey = ws.AddGlobalVar(this);				
		this.aTimer = window.setTimeout("ws.Vars[" + aGlobalKey + "]._DoFade();", aStepDuration);
	};
}

function wsFadeIn(theObject, theEndAction){
	ws.ShowObj(theObject);
	var aNewFade = new wsFade(theObject, 1.0, 300, 10, theEndAction);		
	aNewFade.Start();
}

function wsFadeAway(theObject, theEndAction){		
	if(theEndAction == null){
		theEndAction = "hide";
	}
	var aNewFade = new wsFade(theObject, 0, 200, 10, theEndAction);		
	aNewFade.Start();
}

function wsFadeOut(theObject) {
	wsFadeAway(theObject);
} 



/* 	wsTabs
---------------------------------------------------------------------------*/
function WSTabsLoad(){		
	var aTabSets = ws.getElementsByClassName("wsTabSet");
	var aTabSetsCount = aTabSets.length;
	
	for(var i=0;i<aTabSetsCount;i++){
		var aNewSet = new WSTabSet(aTabSets[i], ws._TabSetsCount);
		ws._TabSets[ws._TabSetsCount++] = aNewSet;
		
		var aTabHeads = ws.getElementsByClassName("wsTabHead", aTabSets[i], "a");
		var aTabHeadsCount = aTabHeads.length;			
		for(var h=0;h<aTabHeadsCount;h++){
			aNewSet.addTabHead(aTabHeads[h]);				
		}
		
		var aTabPages = ws.getElementsByClassName("wsTabPage", aTabSets[i], "div");
		var aTabPagesCount = aTabPages.length;
		for(var p=0;p<aTabPagesCount;p++){
			aNewSet.addTabPage(aTabPages[p]);
		}
	} 
}

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
	
	if(theAObject.onclick != null){
		var aOldOnclick = theAObject.onclick;		
		eval("theAObject.onclick = function () { clickTab(" + this.aTabSetCount + "," + this.aTabHeadsCount + "); aOldOnclick(); return false; };");
	}
	else {
		eval("theAObject.onclick = function () { clickTab(" + this.aTabSetCount + "," + this.aTabHeadsCount + "); return false; };");
	}	
			
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
		ws._TabSets[theTabSet].selectTab(theTab);
	}
	catch(e){
		var aDivs = document.getElementById("tabSet").getElementsByTagName("div");
		
		for(var i=0;i<aDivs.length;i++){
			aDivs[i].style.display = "none";
		}	
		
		aDivs[theTab].style.display = "";		
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

/* 	wsMoveAndResize 
	: Move an object to any absolute position, resize to any height width :)
---------------------------------------------------------------------------*/
function wsMoveAndResize(theObject, theX, theY, theWidth, theHeight, theDuration, theSteps, theEndAction){
	this.aObject = theObject;
	this.aMoveToX = theX;
	this.aMoveToY = theY;
	this.aResizeWidthTo = theWidth;
	this.aResizeHeightTo = theHeight;
	this.aDuration = theDuration;
	this.aSteps = theSteps;
	this.anOnEnd = theEndAction;
	
	this.IsFinished = function () {
		if(parseInt(this.aSteps) <= 0){
			return true;
		}	
		else {
			return false;
		}
	};
	
	this._DoMoveAndResize = function(){
		if(this.aTimer){
			window.clearTimeout(this.aTimer);
		}
		
		if(this.IsFinished()) {				
			if(typeof this.anOnEnd == "function"){
				this.anOnEnd(this.aObject);
			}				
			return;
		}
		
		var aStepDuration = Math.round(this.aDuration / this.aSteps) ;

      	var aCurX = this.aObject.offsetLeft;
      	var aCurY = this.aObject.offsetTop;
      	var aCurWidth = this.aObject.offsetWidth;
      	var aCurHeight = this.aObject.offsetHeight;

		if(this.aMoveToX == null){
			this.aMoveToX = aCurX;
		}
		
		if(this.aMoveToY == null){
			this.aMoveToY = aCurY;
		}
		
		if(this.aResizeWidthTo == null){
			this.aResizeWidthTo = aCurWidth;
		}
		
		if(this.aResizeHeightTo == null){
			this.aResizeHeightTo = aCurHeight;
		}
		
		//this step's changes
		
		var aDeltX = this.aSteps >  0 ? ( this.aMoveToX - aCurX ) / this.aSteps : 0;
     	var aDeltY = this.aSteps >  0 ? ( this.aMoveToY - aCurY ) / this.aSteps : 0;
     	var aDeltWidth = this.aSteps >  0 ? ( this.aResizeWidthTo - aCurWidth ) / this.aSteps : 0;
     	var aDeltHeight = this.aSteps >  0 ? ( this.aResizeHeightTo - aCurHeight ) / this.aSteps : 0;

		this._DoMove(aDeltX, aDeltY, aCurX, aCurY);
     	this._DoResize(aDeltWidth, aDeltHeight, aCurWidth, aCurHeight);

     	this.aDuration -= aStepDuration;
     	this.aSteps--;

     	var aGlobalKey = ws.AddGlobalVar(this);				
		this.aTimer = window.setTimeout("ws.Vars[" + aGlobalKey + "]._DoMoveAndResize();", aStepDuration);		
	};
	
	this._DoMove  = function(theDeltX, theDeltY, theCurLeft, theCurTop) {
		var aDeltX = parseInt(theDeltX);
      	var aDeltY = parseInt(theDeltY);
		
		var aCurX = this.aObject.offsetLeft;
      	var aCurY = this.aObject.offsetTop;

      	if ( aDeltX != 0 ){
        	this.aObject.style.left = (aCurX + aDeltX) + "px";
		}
		
      	if ( aDeltY != 0 ){
        	this.aObject.style.top  = (aCurY + aDeltY) + "px";
		}
   	};

   	this._DoResize = function(theDeltWidth, theDeltHeight, theCurWidth, theCurHeight) {
      	var aDeltWidth = parseInt(theDeltWidth);
      	var aDeltHeight = parseInt(theDeltHeight);	
		
		var aCurWidth = this.aObject.offsetWidth;
      	var aCurHeight = this.aObject.offsetHeight;

      	if ( aDeltWidth != 0 ){
        	var aNewWidth = aCurWidth + aDeltWidth;
			this.aObject.style.width = aNewWidth + "px";
		}
		
      	if ( aDeltHeight != 0 ){
        	var aNewHeight = aCurHeight + aDeltHeight;
			this.aObject.style.height  = aNewHeight + "px";
		}
   	};		
	
	//start resize right away when called
	this._DoMoveAndResize();		
}

///////////////////////////////////////////////////////////////////////////////////////////////////////////////

/*
  Text Getter (used to make mozilla work like IE)
*/
if (navigator.product == "Gecko") 
{
	Text.prototype.__defineGetter__( "text", function ()
	{
	   return this.nodeValue;
	} );
	
	Node.prototype.__defineGetter__( "text", function ()
	{
	   var cs = this.childNodes;
	   var l = cs.length;
	   var sb = new Array( l );
	   for ( var i = 0; i < l; i++ )
	      sb[i] = cs[i].text;
	   return sb.join("");
	} );
}

///////////////////////////////////////////////////////////////////////////////////////////////////////////////
// fixes date format, private function
function _parseDate(d)
// parses a date and return the date in a standard format
// d : the date string to be parsed and formatted into standard Oracle date format
// returns: "-1" month format error, "-2" day format error, "-3" year format error
//			date string in form of mm/dd/yyyy.
// 
// Author: AMS
{
	var i;
	var mm;						// month
	var dd;						// day
	var yyyy;					// year
	var maxDay;
	var tmp;
	var sep = " \t-/\\";		// standard date seperators supported
	var nums = "0123456789";	// numeric set used for error checking
	var newd;					// new date after parsing and formatting
	var msg = ". Please Enter the date in the following format: month/day/year";
	
	i = 0;
	
	// get rid of leading white space
	while (i < d.length && sep.indexOf(d.charAt(i)) != -1)
		i++;
		
	tmp = "";
	// get first two characters of month;
	if (i < d.length && nums.indexOf(d.charAt(i) != -1))
	{
		// do not include leading zero, parse int interprets this as an octal number
		if (d.charAt(i) == "0")
			i++;
		else	
			tmp = d.charAt(i++);
	}
	
	if (i < d.length && nums.indexOf(d.charAt(i)) != -1)
		tmp += d.charAt(i++);

	mm = parseInt(tmp);

	// check range of month
	if (isNaN(mm) || mm < 1 || mm > 12)
	{
		//alert("Error in month: \"" + tmp + "\"" + msg);
		return "";
	}

	// get rid of leading white space
	while (i < d.length && sep.indexOf(d.charAt(i)) != -1)
		i++;

	tmp = "";
	// get two characters of day;
	if (i < d.length && nums.indexOf(d.charAt(i)) != -1)
	{
		// do not include leading zero, parse int interprets this as an octal number
		if (d.charAt(i) == "0")
			i++;
		else
			tmp = d.charAt(i++);
	}
	
	if (i < d.length && nums.indexOf(d.charAt(i)) != -1)
		tmp += d.charAt(i++);

	dd = parseInt(tmp);

	// preliminary check range of day: note no calender check is occuring for differences
	// in days of month that occurs at end of function
	if (isNaN(dd) || dd < 1 || dd > 31)
	{
		//alert("Error in day: \"" + tmp + "\"" + msg);
		return "";
	}
	
	
	// get rid of leading white space
	while (i < d.length && sep.indexOf(d.charAt(i)) != -1)
		i++;
		
	tmp = "";
	// get 2 or 4 characters of year
	if (i < d.length && nums.indexOf(d.charAt(i)) != -1)
		tmp = d.charAt(i++);
	if (i < d.length && nums.indexOf(d.charAt(i)) != -1)
		tmp += d.charAt(i++);
	if (i < d.length && nums.indexOf(d.charAt(i)) != -1)
		tmp += d.charAt(i++);
	if (i < d.length && nums.indexOf(d.charAt(i)) != -1)
		tmp += d.charAt(i++);

	yyyy = parseInt(tmp);
	if (isNaN(yyyy))
	{
		//alert("Error in year: \"" + tmp + "\"" + msg);
		return "";
	}
		
	// convert 2 digit
	if (yyyy < 1000)
	{
		// Y2K logic
		if (yyyy < 50)
			yyyy += 2000;
		else
			yyyy += 1900;
	}

	// final check on dd based on screwy calender THANKS ROMAN EMPIRE!!  LOVE your spaggetti though!
	if (mm == 4 || mm == 6 || mm == 9 || mm == 11)
		maxDay = 30;
	else if (mm == 2)
	{
		if (yyyy % 4 > 0)
			maxDay = 28; 		 
		else if ((yyyy % 100 == 0) && (yyyy % 400 > 0))
			maxDay = 28;
		else
			maxDay = 29;
	}
	else
		maxDay = 31;

	if (dd > maxDay)
	{
		//alert("Error in day: there aren't " + dd + " days in month " + mm + " of year " + yyyy + ".");
		return "";
	}

	// format the date string prefixing 0 with single digits for mm and dd
	newd = "";
	if (mm < 10)
		newd += "0";
	newd += mm + '/';
	if (dd < 10)
		newd += "0";
	newd += dd + '/';
	newd += yyyy;

	return newd;
}


// private function, returns true/false if a char passed is a letter or number
function _IsRegChar(theChar){
	theChar = theChar.toLowerCase();
	var aNumbers = "0123456789";
	var aLetters = "abcdefghijklmnopqrstuvwxyz";

	if(aNumbers.indexOf(theChar) != -1){
		return true;
	}
	else if(aLetters.indexOf(theChar) != -1){
		return true;
	}	
	else {
		return false;
	}
}