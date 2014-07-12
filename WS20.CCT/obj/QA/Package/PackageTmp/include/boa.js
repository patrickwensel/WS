/* BOA Code */
    

    //holds asp.ajax call back functions
    var prm = Sys.WebForms.PageRequestManager.getInstance();  
    prm.add_initializeRequest(InitializeRequest);
    prm.add_endRequest(EndRequest);

    //Checks page before post back
    function InitializeRequest(sender, args) 
    {
        //args.get_postBackElement().id
        if (document.getElementById('mBOARowsPerPage').value < 1){
            args.set_cancel(true);
            alert("You must have at least one row per page.");
            document.getElementById('mBOARowsPerPage').focus();
            //enableButtons();
            return false;
        }
        document.getElementById('mBOASearchMsgDiv').innerHTML = "";
        document.getElementById('mBOAResultsDiv').innerHTML = "";
        document.getElementById('mBOASearchLoadingDiv').style.display = "";
        currBOATablePage = "mBOAPage1";
    }
    
    //Displays error message if post back errored
    function EndRequest(sender, args) 
    {
        if (args._error != null){
            document.getElementById('mBOASearchMsgDiv').innerHTML = 'Error: Please contact tech support.';
           // args._errorHandled = true;
        }
        else{
            //enableButtons();
        }
    }
    
    
    function boaSort(theCol){
        if (document.getElementById('boaSortBy').value == theCol){
            document.getElementById('boaSortBy').value = theCol + " desc ";
        }
        else{
            document.getElementById('boaSortBy').value = theCol;
        }
        __doPostBack('mBOASearchButton',"");
    }

	        
    var currBOATablePage = "mBOAPage1";  //default page for update table
    //updates a table to a new page by hidding the current, displaying the new, 
    //  and updating the class on the current page number
    function changeTablePage(thePage){
        document.getElementById(currBOATablePage).style.display = "none";
        document.getElementById(currBOATablePage+"Link").className = "";
        currBOATablePage = "mBOAPage"+thePage;

        document.getElementById("mBOAPage"+thePage).style.display = "";
        document.getElementById("mBOAPage"+thePage+"Link").className = "selectedPageLink";
        
        
    }
    
    var currentRow = "";
    function expandBOARow(theRow){
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
    function boaAlphaBlock() {
        if (event.keyCode < 46 || event.keyCode > 57 || event.keyCode == 47) {
	        event.returnValue = false;
        }
    }
    
    
    ///////////////////////////////////////////////////////////////////////////
    function boaSearchClear()
    {
        document.getElementById('mBOASearchMsgDiv').innerHTML = "<br>";
        document.getElementById('mBOASearchLoadingDiv').style.display = "none";
        document.getElementById('mBOAResultsDiv').innerHTML = "";

        document.getElementById('mBOATransID').value = "";
        document.getElementById('mBOACnfrmNum').value = "";
        document.getElementById('mBOACB').value = "";
        document.getElementById('mBOACustName').value = "";
        document.getElementById('mBOACS').value = "";
        document.getElementById('mBOAOdoc').value = "";
        document.getElementById('mBOAOrder').value = "";
        document.getElementById('mBOADCT').value = "";
        document.getElementById('mBOADOC').value  = "";
        document.getElementById('mBOAKCO').value  = "";
        document.getElementById('mBOASuffx').value = "";
        document.getElementById('mBOAChargeDateFrom').value  = "";
        document.getElementById('mBOAChargeDateTO').value  = "";
        document.getElementById('mBOACreateDateFrom').value  = "";
        document.getElementById('mBOACreateDateTO').value  = "";
        document.getElementById('mBOAReturnDateFrom').value  = "";
        document.getElementById('mBOAReturnDateTO').value  = "";
        document.getElementById('mBOAStatusID').value  = "";
        document.getElementById('mBOATypeID').value = "";
        document.getElementById('mBOACCType').value = "";
    }