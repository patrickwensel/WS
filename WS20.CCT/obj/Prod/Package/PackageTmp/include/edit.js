            ///////////////////////////////////////////////////////////////////////////
            //displayEditRow
            //
            //shows the hidden detail table for a row
            //when clicked first hides the currently displayed hidden row if there is on,
            //then displays the clicked row		    
            var currEditRow;
		    function displayEditRow(row){
	            if( currEditRow != null ) {
	                document.getElementById("mHiddenEditRow_"   + currEditRow).style.display = "none";
	                document.getElementById("mEditScrollRow_" + currEditRow).className   = "scrollRow";
	                document.getElementById("idEditTD_"     + currEditRow).className   = "rowID";
		            document.getElementById("typeEditTD_"   + currEditRow).className = "scrollType";
	            }

	            if( currEditRow == row ) {
		            currEditRow = null;
	            }
	            else {
		            document.getElementById("mHiddenEditRow_"   + row).style.display = "";
		            document.getElementById("mEditScrollRow_" + row).className   = "scrollExpandedRow";
		            document.getElementById("idEditTD_"     + row).className   = "expLeft";
		            document.getElementById("typeEditTD_"   + row).className = "expCenter";
		            currEditRow = row;
	            }
		    }
            ///////////////////////////////////////////////////////////////////////////
            //edit
            //
            //validates the search criteria
            //if ok runs search at index.aspx.cs
            //displays the searching message, clears current expanded row, disables buttons, hides tab, and shows loading message
            function editSearch(){
                //might need to make these dates to compare
                if (document.mForm.mEditTransID.value == ""){
                    document.getElementById("mEditErrorTD").innerHTML = "Must enter a trans id.";
                    document.mForm.mEditTransID.focus();
                    return;
                }
                
                CCT.index.editSearch(document.mForm.mEditTransID.value
                               ,editCallback);
            
                document.getElementById("mEditErrorTD").innerHTML = "<br>";
                currEditRow = null;
                
                document.mForm.mEditButton.disabled = true; 
                document.getElementById('mEditTableDiv').style.display = 'none';
                document.getElementById('mEditLoadingDiv').style.display = '';         
            }
            ///////////////////////////////////////////////////////////////////////////
            //editCallback
            //
            // if there is no error displays returned tables
            // if there is no error, but the session has died refreshes page
            // if there is an error displays default error message (check ws_messages for error message)
            // enables buttons, shows the tabs and clears loading message
            function editCallback(res){
                document.mForm.mEditButton.disabled = false; 
                document.getElementById('mEditLoadingDiv').style.display = 'none';
                document.getElementById('mEditTableDiv').style.display = '';
                
                if (!res.error && !res.value.error){   
                    if (res.value.message == "REFRESH"){
                        reloadSession();
                    }
                    else{ 
                        document.getElementById('mEditTableDiv').innerHTML = res.value.usHTML;
                        gEditEmailUpdated = res.value.usEmail;
                        document.getElementById('mEditErrorDiv').innerHTML = "&nbsp;";
                        //clicks the ca tab if no us results, not the best way of ref which tabset to click
                        //but the code in ws-lib doesn't seem to ref them anyother way
                    }
                 }
                 else{
                   document.getElementById('mEditErrorDiv').innerHTML = "Error Loading Edit Results";
                 }
            } 
            ///////////////////////////////////////////////////////////////////////////
            //searchClear
            //
            //clears the search values from the page and from the session
            function editClear(){
                document.getElementById('mEditTransID').value = "";
            }
            ///////////////////////////////////////////////////////////////////////////
            //editRow
            //
            //if something on the table has been updated calls updateUSSearch in index.aspx.cs
            //clears any error message, displays Updating message and disables the buttons
            function editRow(row){   
                var obj = document.getElementById("amtEdit_"+row);
                var num = obj.value.substring(0,1) == "$"? obj.value.substring(1,obj.value.length) : obj.value;
                num = parseFloat(num.replace(',',''));
                
                if (!ws.IsNumber(num,'dec')) {
                   document.getElementById('mEditErrorDiv').innerHTML = "Amount must be a number."; 
                   objFocus(obj);   
                   return false;
                }

                CCT.index.editRow(row,num,document.getElementById('emailEdit_'+row).value,document.getElementById('notesEdit_'+row).value,document.getElementById('statusEdit_'+row).value,editRowCallback);
                
                document.mForm.mEditButton.disabled = true; 
                document.getElementById('mEditErrorDiv').innerHTML = "Editing...";
                document.getElementById("mEditTableDiv").style.display = "none";
            }
            ///////////////////////////////////////////////////////////////////////////
            // updateEditCallback
            //
            // if there is no error displays returned table and displays search complete message
            // if there is no error, but the session has died refreshes page
            // if there is an error displays default error message (check ws_messages for error message)
            // enables buttons 
            function editRowCallback(res){ 
                document.mForm.mEditButton.disabled = false; 
                document.getElementById("mEditTableDiv").style.display = "";
                  
                if (!res.error && !res.value.error){   
                    if (res.value.message == "REFRESH"){
                        reloadSession();
                    }
                    else{   
                        document.getElementById('mEditTableDiv').innerHTML = res.value.usHTML;
                        document.getElementById('mEditErrorDiv').innerHTML = "";
                        gEditEmailUpdated = res.value.usEmail;
                        gEditStatusUpdated = new Array();
                        gEditAmountUpdated = new Array();
                        gEditNotesUpdated = new Array();
                    }
                }
                else{
                    document.getElementById('mEditTableDiv').innerHTML = "";
                    document.getElementById('mEditErrorDiv').innerHTML = "Error Saving Row";
                } 
            }
 
          
