            ///////////////////////////////////////////////////////////////////////////
            //displaySearchRow
            //
            //shows the hidden detail table for a row
            //when clicked first hides the currently displayed hidden row if there is on,
            //then displays the clicked row		    
            var currSearchUSRow;
		    var currSearchCARow;
		    function displaySearchRow(row,loc){
		        if (loc == 'US')
		        {
		            if( currSearchUSRow != null ) {
		                document.getElementById("mHiddenSearchRowUS_"   + currSearchUSRow).style.display = "none";
		                document.getElementById("mSearchScrollRowUS_" + currSearchUSRow).className   = "scrollRow";
		                document.getElementById("idSearchTDUS_"     + currSearchUSRow).className   = "rowID";
			            document.getElementById("typeSearchTDUS_"   + currSearchUSRow).className = "scrollType";
		            }

		            if( currSearchUSRow == row ) {
			            currSearchUSRow = null;
		            }
		            else {
			            document.getElementById("mHiddenSearchRowUS_"   + row).style.display = "";
			            document.getElementById("mSearchScrollRowUS_" + row).className   = "scrollExpandedRow";
			            document.getElementById("idSearchTDUS_"     + row).className   = "expLeft";
			            document.getElementById("typeSearchTDUS_"   + row).className = "expCenter";
			            currSearchUSRow = row;
		            }
		        }
		        else
		        {
		            if( currSearchCARow != null ) {
		                document.getElementById("mHiddenSearchRowCA_"   + currSearchCARow).style.display = "none";
		                document.getElementById("mSearchScrollRowCA_" + currSearchCARow).className   = "scrollRow";
		                document.getElementById("idSearchTDCA_"     + currSearchCARow).className   = "rowID";
			            document.getElementById("typeSearchTDCA_"   + currSearchCARow).className = "scrollType";
		            }

		            if( currSearchCARow == row ) {
			            currSearchCARow = null;
		            }
		            else {
			            document.getElementById("mHiddenSearchRowCA_"   + row).style.display = "";
			            document.getElementById("mSearchScrollRowCA_" + row).className   = "scrollExpandedRow";
			            document.getElementById("idSearchTDCA_"     + row).className   = "expLeft";
			            document.getElementById("typeSearchTDCA_"   + row).className = "expCenter";
			            currSearchCARow = row;
		            }
		        }
		    }
            ///////////////////////////////////////////////////////////////////////////
            //updateSearch
            //
            // updates arrays which hold changed values on search page
            // used when update is clicked to update all values changed in the database
            // used when sort is clicked to update all values changed in the dataview
           
            //arrays to hold updated Search Results
            var gUSSearchEmailUpdated = new Array();
            var gUSSearchStatusUpdated = new Array();
            var gUSSearchNotesUpdated = new Array();
            var gCASearchEmailUpdated = new Array();
            var gCASearchStatusUpdated = new Array();
            var gCASearchNotesUpdated = new Array();
            function updateSearch(obj){
                var idx = obj.id.indexOf('_');
                var row = obj.id.substring(idx+1);
                var name = obj.id.substring(0,idx-2);
                var loc = obj.id.substring(idx-2,idx);
                
                if (loc == 'US'){
                    errorMessage("&nbsp;","SearchUS","");
                    if (gUSSearchEmailUpdated[row] == null)  { gUSSearchEmailUpdated[row] = null; }
                    if (gUSSearchStatusUpdated[row] == null) { gUSSearchStatusUpdated[row] = null; }
                    if (gUSSearchNotesUpdated[row] == null)  { gUSSearchNotesUpdated[row] = null; }
                    
                    switch(name){
                        case 'emailSearch':
                            if (ws.ValidateEmail(obj.value)) { 
                                gUSSearchEmailUpdated[row] = obj.value;       
                            }
                            else{
                               errorMessage(obj.value+" is not a vaild email.","SearchUS","Error");
                               obj.value = gUSSearchEmailUpdated[row];
                               objFocus(obj); 
                            }
                            break;
                        case 'statusSearch':
                            gUSSearchStatusUpdated[row] = obj.value;
                            break;
                        case 'notesSearch':
                            gUSSearchNotesUpdated[row] = obj.value;
                            break;
                    }
                }
                if (loc == 'CA'){
                    errorMessage("&nbsp;","SearchCA","");
                    if (gCASearchEmailUpdated[row] == null)  { gCASearchEmailUpdated[row] = null; }
                    if (gCASearchStatusUpdated[row] == null) { gCASearchStatusUpdated[row] = null; }
                    if (gCASearchNotesUpdated[row] == null)  { gCASearchNotesUpdated[row] = null; }
                    
                    switch(name){
                        case 'emailSearch':
                            if (ws.ValidateEmail(obj.value)) { 
                                gCASearchEmailUpdated[row] = obj.value;       
                            }
                            else{
                               errorMessage(obj.value+" is not a vaild email.","SearchCA","Error");
                               obj.value = gCASearchEmailUpdated[row];
                               objFocus(obj); 
                            }
                            break;
                        case 'statusSearch':
                            gCASearchStatusUpdated[row] = obj.value;
                            break;
                        case 'notesSearch':
                            gCASearchNotesUpdated[row] = obj.value;
                            break;
                    }
                }
            }
            ///////////////////////////////////////////////////////////////////////////
            //search
            //
            //validates the search criteria
            //if ok runs search at index.aspx.cs
            //displays the searching message, clears current expanded row, disables buttons, hides tab, and shows loading message
            function search(){
                //might need to make these dates to compare
                if (document.mForm.mSearchSentTo.value != "" &&
                    document.mForm.mSearchSentFrom.value > document.mForm.mSearchSentTo.value){
                    document.getElementById("mSearchErrorTD").innerHTML = "First Sent Date must occur before second date.";
                    document.mForm.mSearchSentFrom.focus();
                    document.mForm.mSearchSentFrom.select();
                    return;
                }
                
                CCT.index.search(document.mForm.mSearchTransID.value
                                ,document.mForm.mSearchCustNum.value
                                ,document.mForm.mSearchCustName.value
                                ,document.mForm.mSearchCSNum.value
                                ,document.mForm.mSearchODOC.value
                                ,document.mForm.mSearchOrderNum.value
                                ,document.mForm.mSearchDCT.value
                                ,document.mForm.mSearchDOC.value
                                ,document.mForm.mSearchKCO.value
                                ,document.mForm.mSearchCCSufix.value
                                ,document.mForm.mSearchSentFrom.value
                                ,document.mForm.mSearchSentTo.value
                                ,document.mForm.mSearchStatusSelect.value
                                ,document.mForm.mSearchTypeSelect.value
                                ,document.mForm.mSearchCardType.value
                                ,searchCallback);
            
                document.getElementById("mSearchErrorTD").innerHTML = "<br>";
                errorMessage("Searching ...","SearchUS","Message");
                errorMessage("&nbsp;","SearchCA","");
                currSearchUSRow = null;
		        currSearchCARow = null;
		        
                document.mForm.mSearchButton.disabled = true; 
                document.getElementById('searchTabSet').style.display = 'none';
                document.getElementById('mSearchLoadingDiv').style.display = '';         
            }
            ///////////////////////////////////////////////////////////////////////////
            //searchCallback
            //
            // if there is no error displays returned tables
            // if there is no error, but the session has died refreshes page
            // if there is an error displays default error message (check ws_messages for error message)
            // enables buttons, shows the tabs and clears loading message
            function searchCallback(res){
                document.mForm.mSearchButton.disabled = false; 
                document.getElementById('mSearchLoadingDiv').style.display = 'none';
                document.getElementById('searchTabSet').style.display = '';
                 
                if (!res.error && !res.value.error){   
                    if (res.value.message == "REFRESH"){
                        reloadSession();
                    }
                    else{ 
                        document.getElementById('mSearchUSTableDiv').innerHTML = res.value.usHTML;
                        document.getElementById('mSearchCATableDiv').innerHTML = res.value.caHTML;
                        gUSSearchEmailUpdated = res.value.usEmail;
                        gCASearchEmailUpdated = res.value.caEmail;
                        errorMessage("&nbsp;","SearchUS","");
                        //clicks the ca tab if no us results, not the best way of ref which tabset to click
                        //but the code in ws-lib doesn't seem to ref them anyother way
                        if (gUSSearchEmailUpdated.length == 0) { clickTab(1,1); }
                        else                                   { clickTab(1,0); }
                    }
                 }
                 else{
                    document.getElementById('mSearchUSTableDiv').innerHTML = '';
                    document.getElementById('mSearchCATableDiv').innerHTML = '';
                    errorMessage("Error Loading US Search Results","SearchUS","Error");
                    errorMessage("Error Loading Canada Search Results","SearchCA","Error");
                    document.mForm.mSearchUSUpdateButton.disabled = true;
                    document.mForm.mSearchCAUpdateButton.disabled = true;
                 }
            } 
            ///////////////////////////////////////////////////////////////////////////
            //searchClear
            //
            //clears the search values from the page and from the session
            function searchClear(){
                CCT.index.searchClear();
                
                document.getElementById('mSearchTransID').value = "";
                document.getElementById('mSearchCustNum').value = "";
                document.getElementById('mSearchCustName').value = "";
                document.getElementById('mSearchCSNum').value = "";
                document.getElementById('mSearchODOC').value = "";
                document.getElementById('mSearchOrderNum').value = "";
                document.getElementById('mSearchDCT').value = "";
                document.getElementById('mSearchDOC').value = "";
                document.getElementById('mSearchKCO').value = "";
                document.getElementById('mSearchCCSufix').value = "";
                document.getElementById('mSearchSentFrom').value = "";
                document.getElementById('mSearchSentTo').value = "";
                document.mForm.mSearchStatusSelect.value = "";
                document.mForm.mSearchTypeSelect.value = "";
            }
            ///////////////////////////////////////////////////////////////////////////
            //updateUSSearch
            //
            //if something on the table has been updated calls updateUSSearch in index.aspx.cs
            //clears any error message, displays Updating message and disables the buttons
            function updateUSSearch(){   
                CCT.index.updateUSSearch(gUSSearchStatusUpdated,gUSSearchEmailUpdated,gUSSearchNotesUpdated,updateUSSearchCallback);
                
                document.mForm.mSearchUSUpdateButton.disabled = true; 
                document.mForm.mSearchButton.disabled = true; 
                
                document.getElementById("mSearchUSTableDiv").style.display = "none";
                errorMessage("Updating ...","SearchUS","Message");
            }
            ///////////////////////////////////////////////////////////////////////////
            // updateUSSearchCallback
            //
            // if there is no error displays returned table and displays search complete message
            // if there is no error, but the session has died refreshes page
            // if there is an error displays default error message (check ws_messages for error message)
            // enables buttons 
            function updateUSSearchCallback(res){ 
                document.mForm.mSearchButton.disabled = false; 
                document.getElementById("mSearchUSTableDiv").style.display = "";
                  
                if (!res.error && !res.value.error){   
                    if (res.value.message == "REFRESH"){
                        reloadSession();
                    }
                    else{   
                        document.getElementById('mSearchUSTableDiv').innerHTML = res.value.usHTML;
                        gUSSearchEmailUpdated = res.value.usEmail;
                        errorMessage(res.value.message,"SearchUS","Message");
                        gUSSearchStatusUpdated = new Array();
                        gUSSearchNotesUpdated  = new Array();
                        document.mForm.mSearchUSUpdateButton.disabled = false;  
                    }
                }
                else{
                    document.getElementById('mSearchUSTableDiv').innerHTML = "";
                    errorMessage("Error Updating Transactions","SearchUS","Error");
                } 
            }
            ///////////////////////////////////////////////////////////////////////////
            //updateCASearch
            //
            //calls updateCASearch in index.aspx.cs
            //clears any error message, displays Saving message and disables buttons
            function updateCASearch(){ 
                CCT.index.updateCASearch(gCASearchStatusUpdated,gCASearchEmailUpdated,gCASearchNotesUpdated,updateCASearchCallback);
                 
                document.mForm.mSearchCAUpdateButton.disabled = true; 
                document.mForm.mSearchButton.disabled = true; 
                document.getElementById("mSearchCATableDiv").style.display = "none";
                errorMessage("Updating ...","SearchCA","Message");                    
            }
            ///////////////////////////////////////////////////////////////////////////
            // updateCASearchCallback
            //
            // if there is no error displays returned table, displays save complete message
            // if there is no error, but the session has died refreshes page
            // if there is an error displays default error message (check ws_messages for error message)
            // enables buttons and clears loading message from the page box
            function updateCASearchCallback(res){  
                document.mForm.mSearchButton.disabled = false;  
                document.getElementById("mSearchCATableDiv").style.display = ""; 
                
                if (!res.error && !res.value.error){   
                    if (res.value.message == "REFRESH"){
                        reloadSession();
                    }
                    else{
                        document.getElementById('mSearchCATableDiv').innerHTML = res.value.caHTML;
                        gCASearchEmailUpdated = res.value.caEmail;
                        errorMessage(res.value.message,"SearchCA","Message");
                        gCASearchStatusUpdated = new Array();
                        gCASearchNotesUpdated  = new Array(); 
                        document.mForm.mSearchCAUpdateButton.disabled = false; 
                    }
                }
                else{
                    document.getElementById('mSearchCATableDiv').innerHTML = "";
                    errorMessage("Error Updating Transactions","SearchCA","Error");
                }
            }
            ///////////////////////////////////////////////////////////////////////////
            //sortSearchUS
            //
            //calls sortSearchUS which saves any changed data to the dataview and sorts the us search table by the col given
            //displays loading messages and disables buttons
            function sortSearchUS(col){       
                CCT.index.sortSearchUS(col,gUSSearchEmailUpdated,gUSSearchStatusUpdated,gUSSearchNotesUpdated,sortSearchUSCallBack);

                document.getElementById("mSearchUSTableDiv").style.display = 'none';
                errorMessage("Sorting ...","SearchUS","Message");
                document.mForm.mSearchUSUpdateButton.disabled = true; 
                document.mForm.mSearchButton.disabled = true;  
            }
            ///////////////////////////////////////////////////////////////////////////
            //sortSearchUSCallBack
            //
            // if there is no error displays returned table
            // if there is no error, but the session has died refreshes page
            // if there is an error displays default error message (check ws_messages for error message)
            // enables buttons 
            function sortSearchUSCallBack(res){       
                document.mForm.mSearchUSUpdateButton.disabled = false; 
                document.mForm.mSearchButton.disabled = false; 
                
                if (!res.error && !res.value.error){   
                    if (res.value.message == "REFRESH"){
                        reloadSession();
                    }
                    else{    
                        document.getElementById('mSearchUSTableDiv').innerHTML = res.value.usHTML;
                        gUSSearchEmailUpdated = res.value.usEmail;
                        errorMessage("&nbsp;","SearchUS","");
                        
                        gUSSearchStatusUpdated = new Array();
                        gUSSearchNotesUpdated  = new Array();
                        document.getElementById("mSearchUSTableDiv").style.display = '';
                    }
                }
                else{
                    document.getElementById('mSearchUSTableDiv').innerHTML = "";
                    errorMessage("Error Sorting Transactions","SearchUS","Error");
                }    
            }
            ///////////////////////////////////////////////////////////////////////////
            //sortSearchCA
            //
            //calls sortSearchCA which saves any changed data to the dataview and sorts the us search table by the col given
            //displays loading messages and disables buttons
            function sortSearchCA(col){       
                CCT.index.sortSearchCA(col,gCASearchEmailUpdated,gCASearchStatusUpdated,gCASearchNotesUpdated,sortSearchCACallBack);

                document.getElementById("mSearchCATableDiv").style.display = 'none';
                errorMessage("Sorting ...","SearchCA","Message");
                document.mForm.mSearchCAUpdateButton.disabled = true; 
                document.mForm.mSearchButton.disabled = true; 
            }
            ///////////////////////////////////////////////////////////////////////////
            //sortSearchCACallBack
            //
            // if there is no error displays returned table
            // if there is no error, but the session has died refreshes page
            // if there is an error displays default error message (check ws_messages for error message)
            // enables buttons 
            function sortSearchCACallBack(res){
                document.mForm.mSearchCAUpdateButton.disabled = false; 
                document.mForm.mSearchButton.disabled = false;
                       
                if (!res.error && !res.value.error){   
                    if (res.value.message == "REFRESH"){
                        reloadSession();
                    }
                    else{    
                        document.getElementById('mSearchCATableDiv').innerHTML = res.value.caHTML;
                        gCASearchEmailUpdated = res.value.caEmail;
                        errorMessage("&nbsp;","SearchCA","");

                        gCASearchStatusUpdated = new Array();
                        gCASearchNotesUpdated  = new Array();
                        document.getElementById("mSearchCATableDiv").style.display = '';
                    }
                }
                else{
                    document.getElementById('mSearchCATableDiv').innerHTML = "";
                    errorMessage("Error Sorting Transactions","SearchCA","Error");
                }   
            }