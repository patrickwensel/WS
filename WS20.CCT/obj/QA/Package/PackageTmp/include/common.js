		    ///////////////////////////////////////////////////////////////////////////
		    //menu
		    //
		    // loads the menu and calls menu in index.aspx.cs which checks for login
		    // and saves the current page on the session
		    function menu(page){
		        loadPage(page,'');
		        //CCT.index.menu(page,menuCallback);
		    }
		    ///////////////////////////////////////////////////////////////////////////
		    //menuCallback
		    //
		    //if you are still logged in does nothing, if the session has died
		    //reloads the session
		    function menuCallback(res){
		        if (res.error || !res.value){
		           reloadSession();
		        }
		    }
		    ///////////////////////////////////////////////////////////////////////////
		    //reloadSession
		    //
		    // reloads the session by refreshing the page
		    function reloadSession(){
		        window.location.reload();
		    }
		    ///////////////////////////////////////////////////////////////////////////
		    //changeMenu
		    //
		    // changes a div to the small menu box in the top left of the screen
		    // does this by changes the style of the div and its contents, and changing the text/link
		    // to menu
		    function changeMenu(name){
		        //document.getElementById("mMenuTrans").innerHTML = "";
		        
		        document.getElementById(name).className = "NavMenuFont";
		        document.getElementById(name+'A').className = "NavMenuFont";
		        document.getElementById(name+'USSpan').className = "NavMenuFont";
		        document.getElementById(name+'CASpan').className = "NavMenuFont";
		        
		        document.getElementById(name+'USSpan').style.display = "none";
		        document.getElementById(name+'CASpan').style.display = "none";
		        document.getElementById(name+'Span').innerHTML = "Menu<br>";
		        document.getElementById(name+'A').onclick = function(){menu('menu')};
		        document.getElementById("mMainMenu").style.height = 50;		        
		    }
		    ///////////////////////////////////////////////////////////////////////////
		    //changeStyle
		    //
		    // changes a large div on the main menu page to a small div that appears at the 
		    // top of the other 3 pages, also changes a menu div back to a normal one
		    function changeStyle(name,loc,menu_loc,title){
		        document.getElementById(name).className = loc+"MenuFont";
		        document.getElementById(name+'A').className = loc+"MenuFont";
		        document.getElementById(name+'USSpan').className = loc+"MenuFont";
		        document.getElementById(name+'CASpan').className = loc+"MenuFont";
		        
		        //needs to change back from menu
		        //erase anything written in the message
		        if (menu != ""){
		            document.getElementById(name+'USSpan').style.display = "";
		            document.getElementById(name+'CASpan').style.display = "";
		            document.getElementById(name+'Span').innerHTML = title;
		            document.getElementById(name+'A').onclick = function(){menu(menu_loc)};
		        }
		    }
		    ///////////////////////////////////////////////////////////////////////////
		    //loadPage
		    //
		    // first hides all divs, then displays the divs for the current page
		    // also moves the menu divs around and adjust their size
		    function loadPage(page,err){
		        document.getElementById('gErrorDiv').innerHTML = err; 
		        document.getElementById('mBOASearch').style.display = "none";
		      //  document.getElementById('mMainMenu').style.display = "none";
		        document.getElementById('mTransactions').style.display = "none";
		        document.getElementById('mSearch').style.display = "none";
		        document.getElementById('mCredit').style.display = "none";
		        document.getElementById('mEdit').style.display = "none";
		        document.getElementById('mBOASearch').style.display = "none";
		        //document.getElementById('main_loading').style.display = "none";
		    
		        switch(page){
		            case 'login':
		                document.getElementById('mBOASearch').style.display = "";
		                document.getElementById(document.mForm.mHeaderClientID.value+"_mAppPageName").innerHTML = "CCT";
		                break;
		            case 'menu':
		                var aMoveObj = new wsMoveAndResize(document.getElementById("mMenuTrans")  , 332, 180, 130, 70, 200, 10,changeStyle('mMenuTrans','Main','trans','Transactions'));
		                var aMoveObj1 = new wsMoveAndResize(document.getElementById("mMenuSearch"), 144, 275, 130, 70, 200, 10,changeStyle('mMenuSearch','Main','search','Search'));
		            	var aMoveObj2 = new wsMoveAndResize(document.getElementById("mMenuCredit"), 520, 275, 130, 70, 200, 10,changeStyle('mMenuCredit','Main','credit','Credit'));
                        document.getElementById("mMainMenu").style.height = 220;

		                document.getElementById('mMainMenu').style.display = "";
		                document.getElementById(document.mForm.mHeaderClientID.value+"_mAppPageName").innerHTML = "Menu";
		                break;
		            case 'trans': 
		                var aMoveObj = new wsMoveAndResize(document.getElementById("mMenuTrans"), 6, 155, 80, 40, 200, 10, changeMenu('mMenuTrans'));
		                var aMoveObj1 = new wsMoveAndResize(document.getElementById("mMenuSearch"), 600, 155, 80, 40, 200, 10,changeStyle('mMenuSearch','Nav','search','Search'));
		            	var aMoveObj2 = new wsMoveAndResize(document.getElementById("mMenuCredit"), 697, 155, 80, 40, 200, 10,changeStyle('mMenuCredit','Nav','credit','Credit'));

		                document.getElementById('mMainMenu').style.display = "";
		                document.getElementById('mTransactions').style.display = "";
		                document.getElementById(document.mForm.mHeaderClientID.value+"_mAppPageName").innerHTML = "Transactions";
		                break;
		            case 'search':
		                var aMoveObj = new wsMoveAndResize(document.getElementById("mMenuSearch"), 6, 155, 80, 40, 200, 10, changeMenu('mMenuSearch'));
		                var aMoveObj1 = new wsMoveAndResize(document.getElementById("mMenuTrans"), 600, 155, 80, 40, 200, 10, changeStyle('mMenuTrans','Nav','trans','Transactions'));
		            	var aMoveObj2 = new wsMoveAndResize(document.getElementById("mMenuCredit"), 697, 155, 80, 40, 200, 10,changeStyle('mMenuCredit','Nav','credit','Credit'));

		                document.getElementById('mMainMenu').style.display = "";
		                document.getElementById('mSearch').style.display = "";
		                document.getElementById(document.mForm.mHeaderClientID.value+"_mAppPageName").innerHTML = "Search";
		                break;
		            case 'credit':
			            var aMoveObj = new wsMoveAndResize(document.getElementById("mMenuCredit"), 6, 155, 80, 40, 200, 10, changeMenu('mMenuCredit'));
                        var aMoveObj1 = new wsMoveAndResize(document.getElementById("mMenuTrans"), 600, 155, 80, 40, 200, 10, changeStyle('mMenuTrans','Nav','trans','Transactions'));
		                var aMoveObj2 = new wsMoveAndResize(document.getElementById("mMenuSearch"), 697, 155, 80, 40, 200, 10,changeStyle('mMenuSearch','Nav','search','Search'));

		                document.getElementById('mMainMenu').style.display = "";
		                document.getElementById('mCredit').style.display = "";
		                document.getElementById(document.mForm.mHeaderClientID.value+"_mAppPageName").innerHTML = "Credit";
		                break;
		            case 'boaSearch':
		                var aMoveObj = new wsMoveAndResize(document.getElementById("mMenuSearch"), 6, 155, 80, 40, 200, 10, changeMenu('mMenuSearch'));
		                var aMoveObj1 = new wsMoveAndResize(document.getElementById("mMenuTrans"), 600, 155, 80, 40, 200, 10, changeStyle('mMenuTrans','Nav','trans','Transactions'));
		            	var aMoveObj2 = new wsMoveAndResize(document.getElementById("mMenuCredit"), 697, 155, 80, 40, 200, 10,changeStyle('mMenuCredit','Nav','credit','Credit'));
                        document.getElementById('gErrorDiv').innerHTML = ""; 
		                document.getElementById('mBOASearch').style.display = "";
		                //document.getElementById(document.mForm.mHeaderClientID.value+"_mAppPageName").innerHTML = "BOA Search";
		                break;     
		           case 'edit':
		                document.getElementById('mEdit').style.display = "";
		                document.getElementById(document.mForm.mHeaderClientID.value+"_mAppPageName").innerHTML = "Edit";
		                break;
		        }
		    }
		    ///////////////////////////////////////////////////////////////////////////
		    //getSearchSelects
		    //
		    // loads the type and status drop downs for the search and credit page
		    // don't needs to disable search/clear buttons here because this is only
		    // called on page load and the buttons are already disabled
            function getSearchSelects(){
                CCT.index.getSearchSelects(getSearchSelectsCallback);
                document.getElementById('mMenuSearchUSSpan').innerHTML = "Loading ...";
                document.getElementById('mMenuSearchCASpan').innerHTML = "&nbsp;";
                document.getElementById('mMenuCreditUSSpan').innerHTML = "Loading ...";
                document.getElementById('mMenuCreditCASpan').innerHTML = "&nbsp;";
            }
            ///////////////////////////////////////////////////////////////////////////
            //getSearchSelectsCallback
            //
            // loads the type and status drops on the page, and enable the buttons
            // so you can search, if SEARCH_FLAG or CREDIT_FLAG is set to Y
            // then runs search/credit search 
            // had to put a small delay between the too calls because for some reason the first
            // call won't come back if they are call too fast.. not sure if this is just a bug
            // when running localy or not.
            // dont' need to check for login here because this is only called on page load
            // so user is def. logged in
            function getSearchSelectsCallback(res){
                if (!res.error && res.value != null){   
                    document.getElementById('mSearchStatusSelectDiv').innerHTML = res.value[0];
                    document.getElementById('mSearchTypeSelectDiv').innerHTML = res.value[1];
                    document.getElementById('mCreditTypeSelectDiv').innerHTML = res.value[2];
                    document.getElementById('mMenuSearchUSSpan').innerHTML = "&nbsp;";
                    document.getElementById('mMenuCreditUSSpan').innerHTML = "&nbsp;";
                    document.mForm.mSearchButton.disabled = false;
                    document.mForm.mSearchClearButton.disabled = false; 
                    document.mForm.mCreditButton.disabled = false;
                    document.mForm.mCreditClearButton.disabled = false; 
                    
                    if (SEARCH_FLAG == 'Y') { 
                        search(); 
                        //for some reason if I dont' wait a sec then search doesn't work if
                        //creditSearch is called ...very strange
                        date = new Date();
                        var curDate = null;

                        do { var curDate = new Date(); }
                        while(curDate-date < 1000);
                    }
                    
                    if (CREDIT_SEARCH_FLAG == 'Y') { creditSearch(); } 
                }
                else{
                    document.getElementById('mSearchStatusSelectDiv').innerHTML = 'Error Loading Status';
                    document.getElementById('mSearchTypeSelectDiv').innerHTML   = 'Error Loading Types';
                    document.getElementById('mCreditTypeSelectDiv').innerHTML   = 'Error Loading Types';
                    document.getElementById('mMenuSearchUSSpan').innerHTML = "Error";
                    document.getElementById('mMenuCreditUSSpan').innerHTML = "Error";
                }
            }
            ///////////////////////////////////////////////////////////////////////////
            //getFiles
            //
            // gets a file dropdown for page given
            function getFiles(all,thePage){
                document.getElementById('m'+thePage+'FileDiv').innerHTML  = "loading ..."; 
                CCT.index.getFiles(all,thePage,getFilesCallback);                
            }
            ///////////////////////////////////////////////////////////////////////////
            //getFilesCallback
            //
            // loads the file drop down for page given
            // don't need to check for login because it would require this function to 
            // check the session which it currently doesn't need to do (and make it sync)
            function getFilesCallback(res){
                if (!res.error){
                    document.getElementById('m'+res.value[0]+'FileDiv').innerHTML  = res.value[1]; 
                }
                else{
                   document.getElementById('m'+res.value[0]+'FileDiv').innerHTML  = "Error Loading Files."
                }
            }
            ///////////////////////////////////////////////////////////////////////////
            //getAbsoluteTop
            //
            // recursive function to find the top of a obj on the page
            function getAbsoluteTop(obj)
            {
                if (obj.offsetParent != null){
                    return obj.offsetTop + getAbsoluteTop(obj.offsetParent);
                }	
                else{
                    return obj.offsetTop;
                }
            }
            ///////////////////////////////////////////////////////////////////////////
            //getAbsoluteLeft
            //
            // recursive function to find the left of a obj on the page
            function getAbsoluteLeft(obj)
            {
                if (obj.offsetParent != null){
                    return obj.offsetLeft + getAbsoluteLeft(obj.offsetParent);
                }	
                else{
                    return obj.offsetTop;
                }
            }
            ///////////////////////////////////////////////////////////////////////////
            //viewHistory
            //
            // displays the history for a row in the app
            // first displays a transparent iframe over everything, then displays loading div in the 
            // center of the screen
            function viewHistory(theRow,theLoc){
                //throw a move on this, maybe from the center out
                CCT.index.viewHistory(theRow,theLoc,viewHistoryCallBack);  
            
                var absTop = getAbsoluteTop(document.getElementById('mBody'));
                var absLeft = getAbsoluteLeft(document.getElementById('mBody'));
                document.getElementById(document.mForm.mHeaderClientID.value+'_mAppMenuBar').style.display = 'none';
                document.getElementById('grayedOutDiv').style.top = absTop;
                document.getElementById('grayedOutDiv').style.left = absLeft;
                document.getElementById('grayedOutDiv').style.width = screen.width;//document.getElementById('mBody').offsetWidth;
                document.getElementById('grayedOutDiv').style.height = screen.height;//document.getElementById('mBody').offsetHeight;
                document.getElementById('grayedOutDiv').style.display = '';
                    
                document.getElementById('grayedOutIframe').style.top = absTop;
                document.getElementById('grayedOutIframe').style.left = absLeft;
                document.getElementById('grayedOutIframe').style.width = document.getElementById('mBody').offsetWidth;
                document.getElementById('grayedOutIframe').style.height = document.getElementById('mBody').offsetHeight;
                document.getElementById('grayedOutIframe').style.display = '';
                
                document.getElementById('historyDiv').style.display = '';
                document.getElementById('historyDiv').innerHTML = 'Loading ...';
                document.getElementById('historyDiv').style.top = document.getElementById('mBody').offsetHeight/2-20;
                document.getElementById('historyDiv').style.left = document.getElementById('mBody').offsetWidth/2 - document.getElementById('historyDiv').offsetWidth/2;
                
                document.getElementById('historyHead').style.display = '';
                document.getElementById('historyHead').style.top = document.getElementById('historyDiv').offsetTop-document.getElementById('historyHead').offsetHeight;
                document.getElementById('historyHead').style.left = document.getElementById('historyDiv').offsetLeft; 
            }
            ///////////////////////////////////////////////////////////////////////////
            //viewHistoryCallBack
            //
            // displays the history in the center of the screen
            function viewHistoryCallBack(res){   
                if (!res.error && !res.value.error){   
                    if (res.value.message == "REFRESH"){
                        reloadSession();
                    }
                    else{
                        document.getElementById('historyDiv').innerHTML = res.value.usHTML;  
                        document.getElementById('historyDiv').style.display = '';
                        if (document.getElementById('mBody').offsetHeight > document.getElementById('historyDiv').offsetHeight){
                            document.getElementById('historyDiv').style.top = document.getElementById('mBody').offsetHeight/2 - document.getElementById('historyDiv').offsetHeight/2;
                        }
                        else{
                            document.getElementById('historyDiv').style.top = 30;
                        }  
                        document.getElementById('historyDiv').style.left = document.getElementById('mBody').offsetWidth/2 - document.getElementById('historyDiv').offsetWidth/2; 

                        document.getElementById('historyHead').style.display = '';
                        document.getElementById('historyHead').style.top = document.getElementById('historyDiv').offsetTop-document.getElementById('historyHead').offsetHeight;
                        document.getElementById('historyHead').style.left = document.getElementById('historyDiv').offsetLeft;   

                    }
                }
                else{
                    document.getElementById('historyDiv').innerHTML = "Error Loading History";
                }
            }
            ///////////////////////////////////////////////////////////////////////////
            //closeHistory
            //
            // hides the history div and iframe
            function closeHistory(){
                document.getElementById('grayedOutDiv').style.display = 'none';
                document.getElementById('grayedOutIframe').style.display = 'none';
                document.getElementById('historyHead').style.display = 'none';
                document.getElementById('historyDiv').style.display = 'none';
                document.getElementById(document.mForm.mHeaderClientID.value+'_mAppMenuBar').style.display = '';
            }
            ///////////////////////////////////////////////////////////////////////////
            //printPage
            //
            // prints a page
            function printPage(theType,theSource){
                if (theType == "P"){
                    window.print();
                }
                else{
                    window.location.href = "downloadFile.aspx?type="+theType+"&source="+theSource;
                }
            }
            ///////////////////////////////////////////////////////////////////////////
            //errorMessage
            //
            //writes an error message and focues on it if the div is not hidden
            //need to display all divs before calling this function, otherwise will not focues correctly
            function errorMessage(theMessage, theLoc, theType){
                if (theType == "Error"){
                    document.getElementById("m"+theLoc+"ErrorDiv").className = "errorDiv";
                    document.getElementById("mMenu"+theLoc+"Span").innerHTML = "Error";
                }
                else{
                    document.getElementById("m"+theLoc+"ErrorDiv").className = "loadingDiv";
                    document.getElementById("mMenu"+theLoc+"Span").innerHTML = theMessage.length>15?"&nbsp;":theMessage;
                }
                document.getElementById("m"+theLoc+"ErrorDiv").innerHTML = theMessage;
                
                if (theMessage != "&nbsp;" && document.getElementById("m"+theLoc+"ErrorDiv").offsetHeight != 0){
                    document.getElementById("m"+theLoc+"ErrorDiv").focus();
                }
            } 
            ///////////////////////////////////////////////////////////////////////////
            //objFocus
            //
            //makes sure object is visable before focusing on it
            function objFocus(obj){
                if (obj.offsetHeight != 0){
                    obj.focus();
                    obj.select();
                }
            } 
            ///////////////////////////////////////////////////////////////////////////
            //alphaBlock
            //
            // blocks none alpha chars
	        function alphaBlock() {
		        if (event.keyCode < 48 || event.keyCode > 57) {
			        event.returnValue = false;
		        }
	        }
	        /////////////////////////////////////////////////////////////////////////////////////////////////
	        //toDollarsAndCents
	        //
	        // formats a nunber with 2 dec places
	        function toDollarsAndCents(n) {
		         var n = new String(n);
    //			     if (n.substring(0,1) == "$") {
    //			 	    n = n.substring(1,n.length);
    //			     }
		         var s = "" + Math.round(n * 100) / 100;
		         var i = s.indexOf('.');
		         if (i < 0) {
		           return  s + ".00";
		          }
		         var t = s.substring(0, i + 1) + s.substring(i + 1, i + 3);
		         if (i + 2 == s.length) {
		           t += "0";
		         }
		         t =  t;
		         return t;		 
	        }
	        ///////////////////////////////////////////////////////////////////////////
            //ajaxTimeOut
            //
            // writes an error message when ajax times out
            // need to make a reference to this function when time out is set for this to work
	        function ajaxTimeOut(){
	            document.getElementById('gErrorDiv').innerHTML = "Ajax Timed Out."; 
	        }
	        ///////////////////////////////////////////////////////////////////////////
            //ajaxError
            //
            // writes an error message when ajax errors
            // need to make a reference to this function when time out is set for this to work
	        function ajaxError(e){
	            document.getElementById('gErrorDiv').innerHTML = "Ajax Error."+e.Message; 
	        }
	        
