/* Cyber Code */
    

    //holds asp.ajax call back functions
    var prm = Sys.WebForms.PageRequestManager.getInstance();  
    prm.add_initializeRequest(InitializeRequest);
    prm.add_endRequest(EndRequest);

    //Checks page before post back
    function InitializeRequest(sender, args) 
    {
        //args.get_postBackElement().id
        if (document.getElementById('mCyberRowsPerPage').value < 1){
            args.set_cancel(true);
            alert("You must have at least one row per page.");
            document.getElementById('mCyberRowsPerPage').focus();
            //enableButtons();
            return false;
        }
        document.getElementById('mCyberSearchMsgDiv').innerHTML = "";
        document.getElementById('mCyberResultsDiv').innerHTML = "";
        document.getElementById('mCyberSearchLoadingDiv').style.display = "";
        currCyberTablePage = "mCyberPage1";
    }
    
    //Displays error message if post back errored
    function EndRequest(sender, args) 
    {
        if (args._error != null){
            document.getElementById('mCyberSearchMsgDiv').innerHTML = 'Error: Please contact tech support.';
           // args._errorHandled = true;
        }
        else{
            //enableButtons();
        }
    }
    
    
    function CyberSort(theCol){
        if (document.getElementById('CyberSortBy').value == theCol){
            document.getElementById('CyberSortBy').value = theCol + " desc ";
        }
        else{
            document.getElementById('CyberSortBy').value = theCol;
        }
        __doPostBack('mCyberSearchButton',"");
    }

	        
    var currCyberTablePage = "mCyberPage1";  //default page for update table
    //updates a table to a new page by hidding the current, displaying the new, 
    //  and updating the class on the current page number
    function changeTablePage(thePage){
        document.getElementById(currCyberTablePage).style.display = "none";
        document.getElementById(currCyberTablePage+"Link").className = "";
        currCyberTablePage = "mCyberPage"+thePage;

        document.getElementById("mCyberPage"+thePage).style.display = "";
        document.getElementById("mCyberPage"+thePage+"Link").className = "selectedPageLink";
        
        
    }
    
    var currentRow = "";
    function expandCyberRow(theRow){
        if (currentRow != ""){
            document.getElementById('detailCell0'+currentRow).className = "leftCell";
            document.getElementById('detailCell1'+currentRow).className = "cell";
            document.getElementById('detailCell2'+currentRow).className = "cell";
            document.getElementById('detailCell3'+currentRow).className = "cell";
            document.getElementById('detailCell4'+currentRow).className = "cell";
            document.getElementById('detailCell5'+currentRow).className = "cell";
            document.getElementById('detailCell6'+currentRow).className = "cell";
            document.getElementById('detailCell7'+currentRow).className = "cell";
            document.getElementById('detailCell8'+currentRow).className = "cell right";
            document.getElementById('detailCell9'+currentRow).className = "cell";
            document.getElementById('detailCell10'+currentRow).className = "cell";
            document.getElementById('detailCell11'+currentRow).className = "cell";
            document.getElementById('detailRow'+currentRow).style.display = "none";    
        }
        if (currentRow != theRow){
            document.getElementById('detailCell0'+theRow).className = "cellBorder";
            document.getElementById('detailCell1'+theRow).className = "cellBottom";
            document.getElementById('detailCell2'+theRow).className = "cellBottom";
            document.getElementById('detailCell3'+theRow).className = "cellBottom";
            document.getElementById('detailCell4'+theRow).className = "cellBottom";
            document.getElementById('detailCell5'+theRow).className = "cellBottom";
            document.getElementById('detailCell6'+theRow).className = "cellBottom";
            document.getElementById('detailCell7'+theRow).className = "cellBottom";
            document.getElementById('detailCell8'+theRow).className = "cellBottom right";
            document.getElementById('detailCell9'+theRow).className = "cellBottom";
            document.getElementById('detailCell10'+theRow).className = "cellBottom";
            document.getElementById('detailCell11'+theRow).className = "cellBottom";
            document.getElementById('detailRow'+theRow).style.display = "";
            currentRow = theRow;
        }
        else{
            currentRow = "";
        }
    }
    
    
    ///////////////////////////////////////////////////////////////////////////
    //alphaBlock
    //
    // blocks none alpha chars
    function CyberAlphaBlock() {
        if (event.keyCode < 46 || event.keyCode > 57 || event.keyCode == 47) {
	        event.returnValue = false;
        }
    }
    
    
    ///////////////////////////////////////////////////////////////////////////
    function CyberSearchClear()
    {
        document.getElementById('mCyberSearchMsgDiv').innerHTML = "<br>";
        document.getElementById('mCyberSearchLoadingDiv').style.display = "none";
        document.getElementById('mCyberResultsDiv').innerHTML = "";

        document.getElementById('mCyberTransID').value = "";
        document.getElementById('mCyberCnfrmNum').value = "";
        document.getElementById('mCyberCB').value = "";
        document.getElementById('mCyberCustName').value = "";
        document.getElementById('mCyberCS').value = "";
        document.getElementById('mCyberOdoc').value = "";
        document.getElementById('mCyberOrder').value = "";
        document.getElementById('mCyberDCT').value = "";
        document.getElementById('mCyberDOC').value  = "";
        document.getElementById('mCyberKCO').value  = "";
        document.getElementById('mCyberSuffx').value = "";
        document.getElementById('mCyberChargeDateFrom').value  = "";
        document.getElementById('mCyberChargeDateTO').value  = "";
        document.getElementById('mCyberCreateDateFrom').value  = "";
        document.getElementById('mCyberCreateDateTO').value  = "";
        document.getElementById('mCyberReturnDateFrom').value  = "";
        document.getElementById('mCyberReturnDateTO').value  = "";
        document.getElementById('mCyberStatusID').value  = "";
        document.getElementById('mCyberTypeID').value = "";
        document.getElementById('mCyberCCType').value = "";
    }