                
            ///////////////////////////////////////////////////////////////////////////
            //formatAmount
            //
            //takes in an input box with a number in it and US or CA
            //alerts if the values is not a number, or if it is greater than the open amount / < than 0
            //if number is in the right range, formates it, figures out the new amount total, saves the value in an array
            //and updates the amount in updateCredit
            //old and start arrays are used because start used to be open amount and i used to check onblur of amount
            //field so needed the orignal value incase they want to change it back. 
            //can take out start arry and use old now if sticking with just checking on submit
            var gOldUSCreditAmt;
            var gOldCACreditAmt;
            function formatAmount(obj){
               //var num = parseFloat((obj.value.substring(0,1) == "$")? obj.value.substring(1,obj.value.length) : obj.value);
                var num = obj.value.substring(0,1) == "$"? obj.value.substring(1,obj.value.length) : obj.value;
                num = parseFloat(num.replace(',',''));
                var idx = obj.id.indexOf('_');
                var row = obj.id.substring(idx+1);
                var loc = obj.id.substring(idx-2,idx);
                
                if (loc == 'US'){
                    errorMessage("&nbsp;","CreditUS","");
                    if (!ws.IsNumber(num,'dec')) {
                        errorMessage("Amount must be a number.","CreditUS","Error");
                        obj.value = "$" + gOldUSCreditAmt[row];  
                        objFocus(obj);   
                        return false;
                    }
                    else if (num > gOldUSCreditAmt[row] || num <= 0){
                        errorMessage("Amount must be less than the processed amount $"+gOldUSCreditAmt[row]+" and greater than $0.00.","CreditUS","Error");
                        obj.value = "$" + gOldUSCreditAmt[row];
                        objFocus(obj);  
                        return false;
                    }
                    else{
                        gOldUSCreditAmt[gOldUSCreditAmt.length-1] = toDollarsAndCents(gOldUSCreditAmt[gOldUSCreditAmt.length-1] - gOldUSCreditAmt[row] - (-num));
                        gOldUSCreditAmt[row] = toDollarsAndCents(num);
                    
                        obj.value = "$"+ gOldUSCreditAmt[row];
                        document.getElementById("mCreditUSTAmt").innerHTML = "$"+gOldUSCreditAmt[gOldUSCreditAmt.length-1];
                        updateCredit(obj);
                        return true;
                    }
                }
                else {
                    errorMessage("&nbsp;","CreditCA","");
                    if (!ws.IsNumber(num,'dec')) {
                        errorMessage("Amount must be a number.","CreditCA","Error");
                        obj.value = "$" + gOldCACreditAmt[row];
                        objFocus(obj);
                        return false;
                    }
                    else if (num > gOldCACreditAmt[row] || num <= 0){
                        errorMessage("Amount must be less than the processed amount $"+gOldCACreditAmt[row]+" and greater than $0.00.","CreditCA","Error");
                        obj.value = "$" + gOldCACreditAmt[row];
                        objFocus(obj); 
                        return false;
                    }
                    else{
                        gOldCACreditAmt[gOldCACreditAmt.length-1] = toDollarsAndCents(gOldCACreditAmt[gOldCACreditAmt.length-1] - gOldCACreditAmt[row] - (-num));
                        gOldCACreditAmt[row] = toDollarsAndCents(num);
                    
                        obj.value = "$"+ gOldCACreditAmt[row];
                        document.getElementById("mCreditCATAmt").innerHTML = "$"+gOldCACreditAmt[gOldCACreditAmt.length-1];
                        updateCredit(obj);
                        return true;
                    }
               }
            }
            ///////////////////////////////////////////////////////////////////////////
            //displayCreditRow
            //
            //shows the hidden detail table for a row
            //when clicked first hides the currently displayed hidden row if there is on,
            //then displays the clicked row
		    var currCreditUSRow;
		    var currCreditCARow;
		    function displayCreditRow(row,loc){
		        if (loc == 'US')
		        {
		            if( currCreditUSRow != null ) {
		                document.getElementById("mHiddenCreditRowUS_"   + currCreditUSRow).style.display = "none";
		                document.getElementById("mCreditScrollRowUS_" + currCreditUSRow).className   = "scrollRow";
		                document.getElementById("idCreditTDUS_"     + currCreditUSRow).className   = "rowID";
			            document.getElementById("typeCreditTDUS_"   + currCreditUSRow).className = "scrollType";
		            }

		            if( currCreditUSRow == row ) {
			            currCreditUSRow = null;
		            }
		            else {
			            document.getElementById("mHiddenCreditRowUS_"   + row).style.display = "";
			            document.getElementById("mCreditScrollRowUS_" + row).className   = "scrollExpandedRow";
			            document.getElementById("idCreditTDUS_"     + row).className   = "expLeft";
			            document.getElementById("typeCreditTDUS_"   + row).className = "expCenter";
			            currCreditUSRow = row;
		            }
		        }
		        else
		        {
		            if( currCreditCARow != null ) {
		                document.getElementById("mHiddenCreditRowCA_"   + currCreditCARow).style.display = "none";
		                document.getElementById("mCreditScrollRowCA_" + currCreditCARow).className   = "scrollRow";
		                document.getElementById("idCreditTDCA_"     + currCreditCARow).className   = "rowID";
			            document.getElementById("typeCreditTDCA_"   + currCreditCARow).className = "scrollType";
		            }

		            if( currCreditCARow == row ) {
			            currCreditCARow = null;
		            }
		            else {
			            document.getElementById("mHiddenCreditRowCA_"   + row).style.display = "";
			            document.getElementById("mCreditScrollRowCA_" + row).className   = "scrollExpandedRow";
			            document.getElementById("idCreditTDCA_"     + row).className   = "expLeft";
			            document.getElementById("typeCreditTDCA_"   + row).className = "expCenter";
			            currCreditCARow = row;
		            }
		        }
		    }
		   ///////////////////////////////////////////////////////////////////////////
           //updateCredit
           //
           // updates arrays which hold changed values on credit page
           // only used when table is sorted, so unsaved values are still displayed
           
           //arrays to hold updated Credit Results
           var gUSCreditEmailUpdated = new Array();
           var gUSCreditAmountUpdated = new Array();
           var gUSCreditNotesUpdated = new Array();
           var gCACreditEmailUpdated = new Array();
           var gCACreditAmountUpdated = new Array();
           var gCACreditNotesUpdated = new Array();
           function updateCredit(obj){
                var idx = obj.id.indexOf('_');
                var row = obj.id.substring(idx+1);
                var name = obj.id.substring(0,idx-2);
                var loc = obj.id.substring(idx-2,idx);

                //currently only status can be updated in search
                if (loc == 'US'){
                    errorMessage("&nbsp;","CreditUS","");
                    if (gUSCreditEmailUpdated[row] == null)  { gUSCreditEmailUpdated[row] = null; }
                    if (gUSCreditAmountUpdated[row] == null) { gUSCreditAmountUpdated[row] = null; }
                    if (gUSCreditNotesUpdated[row] == null)  { gUSCreditNotesUpdated[row] = null; }
                    
                    switch(name){
                        case 'emailCredit':
                            if (ws.ValidateEmail(obj.value)) { 
                                gUSCreditEmailUpdated[row] = obj.value;       
                            }
                            else{
                               errorMessage(obj.value+" is not a vaild email.","CreditUS","Error");
                               obj.value = gUSCreditEmailUpdated[row];
                               objFocus(obj); 
                            }
                            break;
                        case 'amtCredit':
                            gUSCreditAmountUpdated[row] = obj.value;
                            break;
                        case 'notesCredit':
                            gUSCreditNotesUpdated[row] = obj.value;
                            break;
                    }
                }
                if (loc == 'CA'){
                    errorMessage("&nbsp;","CreditCA","");
                    if (gCACreditEmailUpdated[row] == null)  { gCACreditEmailUpdated[row] = null; }
                    if (gCACreditAmountUpdated[row] == null) { gCACreditAmountUpdated[row] = null; }
                    if (gCACreditNotesUpdated[row] == null)  { gCACreditNotesUpdated[row] = null; }
                    
                    switch(name){
                        case 'emailCredit':
                            if (ws.ValidateEmail(obj.value)) { 
                                gCACreditEmailUpdated[row] = obj.value;       
                            }
                            else{
                               errorMessage(obj.value+" is not a vaild email.","CreditCA","Error");
                               obj.value = gCACreditEmailUpdated[row];
                               objFocus(obj); 
                            }
                            break;
                        case 'amtCredit':
                            gCACreditAmountUpdated[row] = obj.value;
                            break;
                        case 'notesCredit':
                            gCACreditNotesUpdated[row] = obj.value;
                            break;
                    }
                }
            }
            ///////////////////////////////////////////////////////////////////////////
            //creditSearch
            //
            //validates the search criteria
            //if ok runs creditSearch at index.aspx.cs
            //gets the file drop down
            //displays the searching message, clears current expanded row, disables buttons, hides tab, and shows loading message
            function creditSearch(){
                //might need to make these dates to compare
                if (document.mForm.mCreditSentTo.value != "" &&
                    document.mForm.mCreditSentFrom.value > document.mForm.mCreditSentTo.value){
                    document.getElementById("mCreditErrorTD").innerHTML = "First Sent Date must occur before second date.";
                    document.mForm.mCreditSentFrom.focus();
                    document.mForm.mCreditSentFrom.select();
                    return;
                }
                CCT.index.creditSearch(document.mForm.mCreditTransID.value
                                ,document.mForm.mCreditCustNum.value
                                ,document.mForm.mCreditCustName.value
                                ,document.mForm.mCreditCSNum.value
                                ,document.mForm.mCreditODOC.value
                                ,document.mForm.mCreditOrderNum.value
                                ,document.mForm.mCreditDCT.value
                                ,document.mForm.mCreditDOC.value
                                ,document.mForm.mCreditKCO.value
                                ,document.mForm.mCreditCCSufix.value
                                ,document.mForm.mCreditSentFrom.value
                                ,document.mForm.mCreditSentTo.value
                                ,document.mForm.mCreditTypeSelect.value
                                ,document.mForm.mCreditCardType.value
                                ,creditSearchCallback); 
                getFiles(false,"CACredit");
  
                document.getElementById("mCreditErrorTD").innerHTML = "<br>";
                errorMessage("Searching ...","CreditUS","Message");
                errorMessage("&nbsp;","CreditCA","");
                currCreditUSRow = null;
		        currCreditCARow = null;

                document.getElementById('creditTabSet').style.display = 'none';
                document.getElementById('mCreditLoadingDiv').style.display = "";
                document.mForm.mCreditButton.disabled = true;      
            }
            ///////////////////////////////////////////////////////////////////////////
            //creditSearchCallback
            //
            // if there is no error displays returned tables, updates array of amounts
            // if there is no error, but the session has died refreshes page
            // if there is an error displays default error message (check ws_messages for error message)
            // enables buttons, shows the tabs and clears loading message
            function creditSearchCallback(res){
                document.getElementById('mCreditLoadingDiv').style.display = 'none';
                document.getElementById('creditTabSet').style.display = '';
                document.mForm.mCreditButton.disabled = false;
                
                if (!res.error && !res.value.error){    
                    if (res.value.message == "REFRESH"){
                        reloadSession();
                    }
                    else{    
                        document.getElementById('mCreditUSTableDiv').innerHTML = res.value.usHTML;
                        document.getElementById('mCreditCATableDiv').innerHTML = res.value.caHTML;
                        gOldUSCreditAmt = res.value.usAmt;
                        gOldCACreditAmt = res.value.caAmt; 
                        gUSCreditEmailUpdated = res.value.usEmail;
                        gCACreditEmailUpdated = res.value.caEmail;
                        errorMessage("&nbsp;","CreditUS","");
                        //clicks the ca tab if no us results, not the best way of ref which tabset to click
                        //but the code in ws-lib doesn't seem to ref them anyother way
                        if (gUSCreditEmailUpdated.length == 0) { clickTab(2,1); } 
                        else                                   { clickTab(2,0); } 
                    }
                }
                else{
                    document.getElementById('mCreditUSTableDiv').innerHTML = '';
                    document.getElementById('mCreditCATableDiv').innerHTML = '';
                    errorMessage("Error Loading US Credit Results","CreditUS","Error");
                    errorMessage("Error Loading Canada Credit Results","CreditCA","Error");
                } 
            } 
            ///////////////////////////////////////////////////////////////////////////
            //creditClear
            //
            //clears the credit search values from the page and from the session
            function creditClear(){
                CCT.index.creditClear();
                
                document.mForm.mCreditTransID.value = "";
                document.mForm.mCreditCustNum.value = "";
                document.mForm.mCreditCustName.value = "";
                document.mForm.mCreditCSNum.value = "";
                document.mForm.mCreditODOC.value = "";
                document.mForm.mCreditOrderNum.value = "";
                document.mForm.mCreditDCT.value = "";
                document.mForm.mCreditDOC.value = "";
                document.mForm.mCreditKCO.value = "";
                document.mForm.mCreditCCSufix.value = "";
                document.mForm.mCreditSentFrom.value = "";
                document.mForm.mCreditSentTo.value = "";
                document.mForm.mCreditTypeSelect.value = "";   
            }
            ///////////////////////////////////////////////////////////////////////////
            //creditUSRow
            //
            //calls creditUSRow in index.aspx.cs
            //clears any error message, displays Crediting message and disables the buttons
            function creditUSRow(row){ 
                if (formatAmount(document.getElementById('amtCreditUS_'+row),'US')){
                    CCT.index.creditUSRow(row,gOldUSCreditAmt[row],document.getElementById('emailCreditUS_'+row).value
                                             ,document.getElementById('notesCreditUS_'+row).value,creditUSRowCallback);
                                             
                    document.getElementById("mCreditUSTableDiv").style.display = 'none';
                    errorMessage("Crediting ...","CreditUS","Message");
                    document.mForm.mCreditButton.disabled = true; 
                }
            }
            ///////////////////////////////////////////////////////////////////////////
            // creditUSRowCallback
            //
            // if there is no error displays returned table, updates array of amounts,and displays credit complete message
            // if there is no error, but the session has died refreshes page
            // if there is an error displays default error message (check ws_messages for error message)
            // enables buttons 
            function creditUSRowCallback(res){  
                document.getElementById("mCreditUSTableDiv").style.display = '';
                document.mForm.mCreditButton.disabled = false;
                
                if (!res.error && !res.value.error){   
                    if (res.value.message == "REFRESH"){
                        reloadSession();
                    }
                    else{    
                        document.getElementById('mCreditUSTableDiv').innerHTML = res.value.usHTML;
                        errorMessage(res.value.message,"CreditUS","Message");
                        gOldUSCreditAmt = res.value.usAmt;
                        gUSCreditEmailUpdated = res.value.usEmail;
                    }
                }
                else{
                    document.getElementById('mCreditUSTableDiv').innerHTML = "";
                    errorMessage("Error Crediting Transaction","CreditUS","Error");
                }    
            }
            ///////////////////////////////////////////////////////////////////////////
            //creditCARow
            //
            //calls creditCARow in index.aspx.cs
            //clears any error message, displays Crediting message and disables buttons
            function creditCARow(row){      
                if (formatAmount(document.getElementById('amtCreditCA_'+row),'CA')){                               
                    CCT.index.creditCARow(row,gOldCACreditAmt[row],document.getElementById('emailCreditCA_'+row).value
                                             ,document.getElementById('notesCreditCA_'+row).value,creditCARowCallback);
                    
                    document.getElementById("mCreditCATableDiv").style.display = 'none';
                    errorMessage("Crediting ...","CreditCA","Message");
                    document.mForm.mCreditButton.disabled = true;         
                }               
            }
            ///////////////////////////////////////////////////////////////////////////
            // creditCARowCallback
            //
            // if there is no error displays returned table, updates array of amounts, displays credit complete message,
            //    and opens file save box
            // if there is no error, but the session has died refreshes page
            // if there is an error displays default error message (check ws_messages for error message)
            // enables buttons 
            function creditCARowCallback(res){ 
                document.getElementById("mCreditCATableDiv").style.display = '';
                document.mForm.mCreditButton.disabled = false; 
                
                if (!res.error && !res.value.error){   
                    if (res.value.message == "REFRESH"){
                        reloadSession();
                    }
                    else{     
                        document.getElementById('mCreditCATableDiv').innerHTML = res.value.caHTML;
                        errorMessage(res.value.message,"CreditCA","Message");
                        gOldCACreditAmt = res.value.caAmt;
                        gCACreditEmailUpdated = res.value.caEmail;
                        //get new files
                        getFiles(false,"CACredit");
                        //open file
                        if (res.value.filePath != null && res.value.filePath != ""){
                            printPage("F",res.value.filePath);
                        }
                    }
                }
                else{
                    document.getElementById('mCreditCATableDiv').innerHTML = "";
                    errorMessage("Error Crediting Transaction","CreditCA","Error");
                }
           }
            ///////////////////////////////////////////////////////////////////////////
            //sortCreditUS
            //
            //calls sortCreditUS which saves any changed data to the dataview and sorts the us credit table by the col given
            //displays loading messages and disables buttons
            function sortCreditUS(col){       
                CCT.index.sortCreditUS(col,gUSCreditEmailUpdated,gUSCreditAmountUpdated,gUSCreditNotesUpdated,sortCreditUSCallBack);

                document.getElementById("mCreditUSTableDiv").style.display = 'none';
                errorMessage("Sorting ...","CreditUS","Message");
                document.mForm.mCreditButton.disabled = true; 
            }
            ///////////////////////////////////////////////////////////////////////////
            //sortCreditUSCallBack
            //
            // if there is no error displays returned table, updates array of amount
            // if there is no error, but the session has died refreshes page
            // if there is an error displays default error message (check ws_messages for error message)
            // enables buttons 
            function sortCreditUSCallBack(res){  
                document.getElementById("mCreditUSTableDiv").style.display = '';
                document.mForm.mCreditButton.disabled = false; 
                     
                if (!res.error && !res.value.error){   
                    if (res.value.message == "REFRESH"){
                        reloadSession();
                    }
                    else{    
                        document.getElementById('mCreditUSTableDiv').innerHTML = res.value.usHTML;
                        errorMessage("&nbsp;","CreditUS","");
                        gOldUSCreditAmt = res.value.usAmt;
                        gUSCreditEmailUpdated = res.value.usEmail;
                        
                        gUSCreditAmountUpdated = new Array();
                        gUSCreditNotesUpdated = new Array();

                    }
                }
                else{
                    document.getElementById('mCreditUSTableDiv').innerHTML = "";
                    errorMessage("Error Sorting Transactions","CreditUS","Error");
                }   
            }
            ///////////////////////////////////////////////////////////////////////////
            //sortCreditCA
            //
            //calls sortCreditCA which saves any changed data to the dataview and sorts the us credit table by the col given
            //displays loading messages and disables buttons
            function sortCreditCA(col){       
                CCT.index.sortCreditCA(col,gCACreditEmailUpdated,gCACreditAmountUpdated,gCACreditNotesUpdated,sortCreditCACallBack);

                document.getElementById("mCreditCATableDiv").style.display = 'none';
                errorMessage("Sorting ...","CreditCA","Message");
                document.mForm.mCreditButton.disabled = true; 
            }
            ///////////////////////////////////////////////////////////////////////////
            //sortCreditCACallBack
            //
            // if there is no error displays returned table, updates array of amount
            // if there is no error, but the session has died refreshes page
            // if there is an error displays default error message (check ws_messages for error message)
            // enables buttons and clears loading message from the page box
            function sortCreditCACallBack(res){  
                document.getElementById("mCreditCATableDiv").style.display = '';
                document.mForm.mCreditButton.disabled = false; 
                     
                if (!res.error && !res.value.error){   
                    if (res.value.message == "REFRESH"){
                        reloadSession();
                    }
                    else{    
                        document.getElementById('mCreditCATableDiv').innerHTML = res.value.caHTML;
                        errorMessage("&nbsp;","CreditCA","");
                        gOldCACreditAmt = res.value.caAmt;
                        gCACreditEmailUpdated = res.value.caEmail;
                        
                       gCACreditAmountUpdated = new Array();
                       gCACreditNotesUpdated = new Array();
                    }
                }
                else{
                    document.getElementById('mCreditCATableDiv').innerHTML = "";
                    errorMessage("Error Sorting Transactions","CreditCA","Error");
                }   
            }