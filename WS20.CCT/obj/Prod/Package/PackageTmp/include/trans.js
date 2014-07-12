		    ///////////////////////////////////////////////////////////////////////////
            //displayRow
            //
            //shows the hidden detail table for a row
            //when clicked first hides the currently displayed hidden row if there is on,
            //then displays the clicked row	
		    var currUSRow;
		    var currCARow;
		    function displayRow(row,loc){
		        if (loc == 'US')
		        {
		            if( currUSRow != null ) {
		                document.getElementById("mHiddenRowUS_"   + currUSRow).style.display = "none";
		                document.getElementById("mScrollRowUS_" + currUSRow).className   = "scrollRow";
		                document.getElementById("idTDUS_"     + currUSRow).className   = "scrollID";
			            document.getElementById("typeTDUS_"   + currUSRow).className = "scrollType";
		            }

		            if( currUSRow == row ) {
			            currUSRow = null;
		            }
		            else {
			            document.getElementById("mHiddenRowUS_"   + row).style.display = "";
			            document.getElementById("mScrollRowUS_" + row).className   = "scrollExpandedRow";
			            document.getElementById("idTDUS_"     + row).className   = "expTransLeft";
			            document.getElementById("typeTDUS_"   + row).className = "expCenter";
			            currUSRow = row;
		            }
		        }
		        else
		        {
		            if( currCARow != null ) {
		                document.getElementById("mHiddenRowCA_"   + currCARow).style.display = "none";
		                document.getElementById("mScrollRowCA_" + currCARow).className   = "scrollRow";
		                document.getElementById("idTDCA_"     + currCARow).className   = "scrollID";
			            document.getElementById("typeTDCA_"   + currCARow).className = "scrollType";
		            }

		            if( currCARow == row ) {
			            currCARow = null;
		            }
		            else {
			            document.getElementById("mHiddenRowCA_"   + row).style.display = "";
			            document.getElementById("mScrollRowCA_" + row).className   = "scrollExpandedRow";
			            document.getElementById("idTDCA_"     + row).className   = "expTransLeft";
			            document.getElementById("typeTDCA_"   + row).className = "expCenter";
			            currCARow = row;
		            }
		        }
		    }
		    /////////////////////////////////////////////////////////////////////////////////////////////
	        //toggleCB
	        //
	        // Toggles all check boxes 
	        // checks to see how many check boxes there are because a list of 1 box is referenece different then more than one
	        // checks to see if the check box is disabled (if status is set to delete), if it is leaves the checkbox alone
	        // if box isn't disabled checks it and calls update function
	        function toggleCB(selectAll,selectRow,loc){
	            if (selectRow){
	                if (loc == "US"){
	                    errorMessage("&nbsp;","TransUS","");
		                if (!selectRow.length){
		                    if (!selectRow.disabled){
			                    selectRow.checked = selectAll;
			                    gUSCKUpdated[0] = selectAll;
			                    updateIt(document.getElementById("selectRow"+loc+"_0"));
			                }
		                }
		                else{
			                for (var i = 0; i < selectRow.length; i++){
			                    if (!selectRow[i].disabled){
				                    selectRow[i].checked = selectAll;
				                    gUSCKUpdated[i] = selectAll;
				                    updateIt(document.getElementById("selectRow"+loc+"_"+i));
				                }
				            }
		                }
		            }
		            else{
		                errorMessage("&nbsp;","TransCA","");
		                if (!selectRow.length){
		                     if (!selectRow.disabled){
			                    selectRow.checked = selectAll;
			                    gCACKUpdated[0] = selectAll;
			                    updateIt(document.getElementById('selectRow'+loc+'_0'));
			                 }
		                }
		                else{
			                for (var i = 0; i < selectRow.length; i++){
			                    if (!selectRow[i].disabled){
				                    selectRow[i].checked = selectAll;
				                    gCACKUpdated[i] = selectAll;
				                    updateIt(document.getElementById('selectRow'+loc+'_'+i));
				                }
				            }
		                }
		            }
		        }
	        }
	        ///////////////////////////////////////////////////////////////////////////
            //updateIt
            //
            // updates arrays which hold changed values on trans page
            // used when update is clicked to update all values changed in the database
            // used when sort is clicked to update all values changed in the dataview
            
            //arrays to hold updated us values
            var gUSCKUpdated     = new Array(); //hold checked boxes
            var gUSEmailUpdated  = new Array(); //holds updated email address
            var gUSStatusUpdated = new Array(); //holds status value to update
            var gUSNotesUpdated  = new Array(); //holds status value to update
            
            //arrays to hold updated CA values
            var gCACKUpdated     = new Array();
            var gCAEmailUpdated  = new Array();
            var gCAStatusUpdated = new Array();
            var gCANotesUpdated = new Array();
            function updateIt(obj){
                var idx = obj.id.indexOf('_');
                var row = obj.id.substring(idx+1);
                var name = obj.id.substring(0,idx-2);
                var loc = obj.id.substring(idx-2,idx);
                
                if (loc == 'US'){
                    errorMessage("&nbsp;","TransUS","");

                    //if (gUSCKUpdated[row] == null) { gUSCKUpdated[row] = (document.getElementById["selectRowUS_"+row].checked)?'true':'false'; }
                    if (gUSCKUpdated[row] == null) { gUSCKUpdated[row] = 'false'; }
                    //i know this doesn't make any sense but it works
                    //if this isn't called then the code behind throws an out-of-bounds index error
                    //really only need to do this for the last row, because it fills all prev rows in with null if they have no value
                    if (gUSEmailUpdated[row] == null)  { gUSEmailUpdated[row] = null; }
                    if (gUSStatusUpdated[row] == null) { gUSStatusUpdated[row] = null; }
                    if (gUSNotesUpdated[row] == null)  { gUSNotesUpdated[row] = null; }

                    switch(name){
                        case 'selectRow':   if (obj.checked){
                                                gUSCKUpdated[row] = 'true'; 
                                                eval("document.mForm.status"+loc+"_"+row+".disabled = true");
                                            }
                                            else{
                                                gUSCKUpdated[row] = 'false';   
                                                eval("document.mForm.status"+loc+"_"+row+".disabled = false");
                                            }
                                                                                    break;
                        case 'email':       if (ws.ValidateEmail(obj.value)) { 
                                                gUSEmailUpdated[row] = obj.value;       
                                            }
                                            else{
                                               errorMessage(obj.value+" is not a vaild email.","TransUS","Error");
                                               obj.value = gUSEmailUpdated[row];
                                               objFocus(obj); 
                                            }
                                            break;
                        case 'status':      gUSStatusUpdated[row] = obj.value;
                                            if (obj.value != STATUS_UNSENT && obj.value != STATUS_DECLINED){
                                                document.getElementById("selectRowUS_"+row).disabled = true;
                                            }
                                            else{
                                                document.getElementById("selectRowUS_"+row).disabled = false;
                                            }         
                                                                                    break;
                        case 'notes':       gUSNotesUpdated[row] = obj.value;       break;
                    }
                }
                else if (loc == 'CA'){
                    errorMessage("&nbsp;","TransCA","");
                    
                    //if (gCACKUpdated[row] == null) { gCACKUpdated[row] = (document.getElementById["selectRowCA_"+row].checked)?'true':'false'; }
                    if (gCACKUpdated[row] == null) { gCACKUpdated[row] = 'false'; }
                    if (gCAEmailUpdated[row] == null) { gCAEmailUpdated[row] = null; }
                    if (gCAStatusUpdated[row]== null) { gCAStatusUpdated[row] = null; }
                    if (gCANotesUpdated[row] == null) { gCANotesUpdated[row] = null; }

                    switch(name){
                        case 'selectRow':   if (obj.checked){
                                                gCACKUpdated[row] = 'true'; 
                                                eval("document.mForm.status"+loc+"_"+row+".disabled = true");
                                            }
                                            else{
                                                gCACKUpdated[row] = 'false';   
                                                eval("document.mForm.status"+loc+"_"+row+".disabled = false");
                                            }
                                            break;
                        case 'email':       if (ws.ValidateEmail(obj.value)) { 
                                                gCAEmailUpdated[row] = obj.value;       
                                            }
                                            else{
                                               errorMessage(obj.value+" is not a vaild email.","TransCA","Error");
                                               obj.value = gCAEmailUpdated[row];
                                               objFocus(obj); 
                                            }
                                            break;
                        case 'status':      gCAStatusUpdated[row] = obj.value;
                                            if (obj.value != STATUS_UNSENT && obj.value != STATUS_DECLINED){
                                                document.getElementById("selectRowCA_"+row).disabled = true;
                                            }
                                            else{
                                                document.getElementById("selectRowCA_"+row).disabled = false;
                                            }         
                                            break;
                        case 'notes':       gCANotesUpdated[row] = obj.value;
                                            break;
                    }
                }
            }
            ///////////////////////////////////////////////////////////////////////////
            //getTrans
            //
            //  runs getTrans at index.aspx.cs
            // displays the loading message, clears current expanded row, disables buttons, hides tab, 
            // gets the ca file select and shows loading message
            function getTrans(){ 
                CCT.index.getTrans(getTransCallback);
                
                getFiles(false,"CATrans");
                
            	document.getElementById("mTransLoadingDiv").style.display="";
            	document.getElementById("tabSet").style.display="none";
            	
            	errorMessage("Loading ...","TransUS","Message");
                errorMessage("&nbsp;","TransCA","");
                
            	currUSRow = null;
		        currCARow = null;
            }
            ///////////////////////////////////////////////////////////////////////////
            //getTransCallback
            //
            // if there is no error displays returned tables
            // if there is no error, but the session has died refreshes page
            // if there is an error displays default error message (check ws_messages for error message)
            // enables buttons, shows the tabs and clears loading message
            function getTransCallback(res){
                document.getElementById("mTransLoadingDiv").style.display="none";
                document.getElementById("tabSet").style.display="";
                
                if (!res.error && !res.value.error){   
                    if (res.value.message == "REFRESH"){
                        reloadSession();
                    }
                    else{ 
                        document.getElementById('mTransUSTableDiv').innerHTML = res.value.usHTML;
                        document.getElementById('mTransCATableDiv').innerHTML = res.value.caHTML;
                        gUSEmailUpdated = res.value.usEmail;
                        gCAEmailUpdated = res.value.caEmail;
                        errorMessage("&nbsp;","TransUS","");
                        //clicks the ca tab if no us results, not the best way of ref which tabset to click
                        //but the code in ws-lib doesn't seem to ref them anyother way
                        if (gUSEmailUpdated.length == 0) { clickTab(0,1); } 
                        else                             { clickTab(0,0); }  
                    }
                }
                else{
                    document.getElementById("mTransUSErrorDiv").innerHTML = "";
                    document.getElementById("mTransCAErrorDiv").innerHTML = "";
                    errorMessage("Error Loading US Transactions","TransUS","Error");
                    errorMessage("Error Loading Canada Transactions","TransCA","Error");
                    document.mForm.mSubmitTransUS.disabled = true;
                    document.mForm.mSubmitTransCA.disabled = true;
                }
            }
            ///////////////////////////////////////////////////////////////////////////
            //updateUSTrans
            //
            //if something on the table has been updated calls updateUSTrans in index.aspx.cs
            //clears any error message, displays Updating message and disables the buttons
            function updateUSTrans(){ 
                CCT.index.updateUSTrans(gUSCKUpdated,gUSEmailUpdated,gUSStatusUpdated,gUSNotesUpdated,getUpdateUSTransCallback);  

                document.mForm.mSubmitTransUS.disabled = true;
                document.mForm.mRefreshUSTrans.disabled = true;
                
                document.getElementById("mTransUSTableDiv").style.display = "none";
                errorMessage("Updating ...","TransUS","Message");   
            }
            ///////////////////////////////////////////////////////////////////////////
            // getUpdateUSTransCallback
            //
            // if there is no error displays returned table and displays update complete message
            // if there is no error, but the session has died refreshes page
            // if there is an error displays default error message (check ws_messages for error message)
            // enables buttons 
            function getUpdateUSTransCallback(res){  
                document.mForm.mRefreshUSTrans.disabled = false; 
                document.getElementById("mTransUSTableDiv").style.display = "";
                 
                if (!res.error && !res.value.error){ 
                    if (res.value.message == "REFRESH"){
                        reloadSession();
                    }
                    else{ 
                        document.getElementById('mTransUSTableDiv').innerHTML = res.value.usHTML;
                        gUSEmailUpdated = res.value.usEmail;
                        errorMessage(res.value.message,"TransUS","Message");
                        
                        gUSCKUpdated     = new Array();
                        gUSStatusUpdated = new Array();
                        gUSNotesUpdated = new Array();
                        document.mForm.mSubmitTransUS.disabled = false;
                    }
                }
                else{
                    document.getElementById("mTransUSErrorDiv").innerHTML = "";
                    errorMessage("Error Updating Transactions","TransUS","Error");
                }
            }
            ///////////////////////////////////////////////////////////////////////////
            //updateCATrans
            //
            //calls updateCATrans in index.aspx.cs
            //clears any error message, displays Saving message and disables buttons
            function updateCATrans(){ 
                CCT.index.updateCATrans(gCACKUpdated,gCAEmailUpdated,gCAStatusUpdated,gCANotesUpdated,getUpdateCATransCallback);
                 
                document.mForm.mSubmitTransCA.disabled = true;
                document.mForm.mRefreshCATrans.disabled = true;
                
                //add check to display correct message if file isn't being created
                document.getElementById("mTransCATableDiv").style.display = "none";
                errorMessage("Updating ...","TransCA","Message");
            }
            ///////////////////////////////////////////////////////////////////////////
            // getUpdateCATransCallback
            //
            // if there is no error displays returned table, displays complete message,
            //    and opens file save box
            // if there is no error, but the session has died refreshes page
            // if there is an error displays default error message (check ws_messages for error message)
            // enables buttons  
            function getUpdateCATransCallback(res){  
                document.getElementById("mTransCATableDiv").style.display = "";
                document.mForm.mRefreshCATrans.disabled = false;
                
                if (!res.error && !res.value.error){ 
                    if (res.value.message == "REFRESH"){
                        reloadSession();
                    }
                    else{    
                        document.getElementById('mTransCATableDiv').innerHTML = res.value.caHTML;
                        gCAEmailUpdated = res.value.caEmail;
                        getFiles(false,"CATrans");
                        errorMessage(res.value.message,"TransCA","Message");
                        
                        //download file
                        if (res.value.filePath != null && res.value.filePath != ""){
                            printPage("F",res.value.filePath);
                        }
                        
                        gCACKUpdated     = new Array();
                        gCAStatusUpdated = new Array();
                        gCANotesUpdated = new Array();
                        document.mForm.mSubmitTransCA.disabled = false;  
                    }
                }
                else{
                    document.getElementById('mTransCATableDiv').innerHTML = "";
                    errorMessage("Error Updating Transactions","TransCA","Error");
                }
            }
            ///////////////////////////////////////////////////////////////////////////
            //sortTransUS
            //
            //calls sortTransUS which saves any changed data to the dataview and sorts the us trans table by the col given
            //displays loading messages and disables buttons
            function sortTransUS(col){       
                CCT.index.sortTransUS(col,gUSCKUpdated,gUSEmailUpdated,gUSStatusUpdated,gUSNotesUpdated,sortTransUSCallBack);

                document.getElementById("mTransUSTableDiv").style.display = 'none';
                errorMessage("Sorting ...","TransUS","Message");
                document.mForm.mSubmitTransUS.disabled = true;  
                document.mForm.mRefreshUSTrans.disabled = true; 
            }
            ///////////////////////////////////////////////////////////////////////////
            //sortTransUSCallBack
            //
            // if there is no error displays returned table
            // if there is no error, but the session has died refreshes page
            // if there is an error displays default error message (check ws_messages for error message)
            // enables buttons 
            function sortTransUSCallBack(res){       
                document.getElementById("mTransUSTableDiv").style.display = '';
                document.mForm.mRefreshUSTrans.disabled = false;
                
                if (!res.error && !res.value.error){   
                    if (res.value.message == "REFRESH"){
                        reloadSession();
                    }
                    else{    
                        document.getElementById('mTransUSTableDiv').innerHTML = res.value.usHTML;
                        gUSEmailUpdated = res.value.usEmail;
                        errorMessage("&nbsp;","TransUS","");
                        
                        gUSCKUpdated     = new Array();
                        gUSStatusUpdated = new Array();
                        gUSNotesUpdated = new Array();
                        document.mForm.mSubmitTransUS.disabled = false;  
                    }
                }
                else{
                    document.getElementById('mTransUSTableDiv').innerHTML = "";
                    errorMessage("Error Sorting Transactions","TransUS","Error");
                }     
            }
            ///////////////////////////////////////////////////////////////////////////
            //sortTransCA
            //
            //calls sortTransCA which saves any changed data to the dataview and sorts the us search table by the col given
            //displays loading messages and disables buttons
            function sortTransCA(col){       
                CCT.index.sortTransCA(col,gCACKUpdated,gCAEmailUpdated,gCAStatusUpdated,gCANotesUpdated,sortTransCACallBack);

                document.getElementById("mTransCATableDiv").style.display = 'none';
                errorMessage("Sorting ...","TransCA","Message");
                document.mForm.mSubmitTransCA.disabled = true;  
                document.mForm.mRefreshCATrans.disabled = true; 
            }
            ///////////////////////////////////////////////////////////////////////////
            //sortTransCACallBack
            //
            // if there is no error displays returned table
            // if there is no error, but the session has died refreshes page
            // if there is an error displays default error message (check ws_messages for error message)
            // enables buttons 
            function sortTransCACallBack(res){  
                document.getElementById("mTransCATableDiv").style.display = '';
                document.mForm.mRefreshCATrans.disabled = false; 
                     
                if (!res.error && !res.value.error){   
                    if (res.value.message == "REFRESH"){
                        reloadSession();
                    }
                    else{    
                        document.getElementById('mTransCATableDiv').innerHTML = res.value.caHTML;
                        gCAEmailUpdated = res.value.caEmail;
                        errorMessage("&nbsp;","TransCA","");
                        
                        gCACKUpdated     = new Array();
                        gCAStatusUpdated = new Array();
                        gCANotesUpdated = new Array();  
                        document.mForm.mSubmitTransCA.disabled = false;  
                    }
                }
                else{
                    document.getElementById('mTransCATableDiv').innerHTML = "";
                    errorMessage("Error Sorting Transactions","TransCA","Error");
                }   
            }