    
function init(){
    setMonths('');
    calcValues();
    focusFun();
    textareaResizer.addToAll();
}
   
function setReq(){
    var reqVal = document.getElementById('selStatus').value ==  statusDraft ? "":"*";
    document.getElementById('labelBranchReq').innerHTML = reqVal;
    //document.getElementById('labelProductReq').innerHTML = reqVal;
    document.getElementById('labelProcessReq').innerHTML = reqVal;
    document.getElementById('labelSavingsReq').innerHTML = reqVal;
    document.getElementById('labelKeyWordsReq').innerHTML = reqVal;
    document.getElementById('labelDescReq').innerHTML = reqVal;
    document.getElementById('labelSavingsCalcReq').innerHTML = reqVal;
    //remove stars
    //should remove these from main page 
    document.getElementById('labelMSG').className = "";
    document.getElementById('labelMSG').innerHTML = "";
    document.getElementById('labelProjectTitleReq').className = "reqField"; 
    document.getElementById('labelProjectTitle').className = "label";
    document.getElementById('labelProjectTitleInput').className = "input";
    document.getElementById('labelProjectTitleEnglishReq').className = "reqField";
    document.getElementById('labelProjectTitleEnglish').className = "label";
    document.getElementById('labelProjectTitleEnglishInput').className = "input";
    document.getElementById('labelBranchReq').className = "reqField";
    document.getElementById('labelBranch').className = "label";
    document.getElementById('labelBranchInput').className = "input";
    //document.getElementById('labelProductReq').className = "reqField";
    //document.getElementById('labelProduct').className = "label";
    //document.getElementById('labelProductInput').className = "inputR";
    document.getElementById('labelProcessReq').className = "reqField";
    document.getElementById('labelProcess').className = "label";
    document.getElementById('labelProcessInput').className = "inputR";
    document.getElementById('labelSavingsReq').className = "reqField";
    document.getElementById('labelSavings').className = "label";
    document.getElementById('labelSavingsInput').className = "";
    document.getElementById('labelKeyWordsReq').className = "reqField";
    document.getElementById('labelKeyWords').className = "label";
    document.getElementById('labelDescReq').className = "reqField";
    document.getElementById('labelDesc').className = "label";
    document.getElementById('labelSavingsCalcReq').className = "reqField";
    document.getElementById('labelSavingsCalc').className = "label";
}


//sets datas for Monthly Savings
function setMonths(theDate){
    var d=new Date(document.getElementById('inputStartDate').value);
    if (isNaN(d)){
        d=new Date()
        document.getElementById('inputStartDate').value = (d.getMonth()+1) + "/" + d.getDate() + "/" + d.getFullYear();
    }
    //calc doesn't work if I don't do this
    //if day is 31 it will skip the next month when it adds on to it, then there if feb...
    d.setDate(1); 
    for (var i=1; i<=12; i++){
        document.getElementById('labelMS'+i).innerHTML = dates[d.getMonth()]+d.getFullYear().toString().substring(2,4);
        d.setMonth(d.getMonth()+1);
    }
}

//delete MSrow
function deleteMSRow(theObj){
    document.getElementById('tableMSRow').deleteRow(theObj.parentNode.parentNode.rowIndex);
}

//delete row
function deleteImpPlanRow(theObj){
    document.getElementById('tableImpPlanRow').deleteRow(theObj.parentNode.parentNode.rowIndex);
}

//calc values
function calcValues(){
    var ms1=ms2=ms3=ms4=ms5=ms6=ms7=ms8=ms9=ms10=ms11=ms12=tms1=tms2=tms3=tms4=tms5=tms6=ms7=tms8=tms9=tms10=tms11=tms12=0;
    for (var i=0;i<parseInt(document.getElementById('hiddenMSRowCT').value);i++){
        if (document.getElementById('inputMS1_'+i)){
            tms1  = parseFloat(document.getElementById('inputMS1_'+i).value.replace(",",""));
            if (isNaN(tms1)) { tms1 = 0; }
            ms1 += tms1;
            tms2  = parseFloat(document.getElementById('inputMS2_'+i).value.replace(",",""));
            if (isNaN(tms2)) { tms2 = 0 }
            ms2 += tms2;
            tms3  = parseFloat(document.getElementById('inputMS3_'+i).value.replace(",",""));
            if (isNaN(tms3)) { tms3 = 0 }
            ms3 += tms3;
            tms4  = parseFloat(document.getElementById('inputMS4_'+i).value.replace(",",""));
            if (isNaN(tms4)) { tms4 = 0 }
            ms4 += tms4;
            tms5  = parseFloat(document.getElementById('inputMS5_'+i).value.replace(",",""));
            if (isNaN(tms5)) { tms5 = 0 }
            ms5 += tms5;
            tms6  = parseFloat(document.getElementById('inputMS6_'+i).value.replace(",",""));
            if (isNaN(tms6)) { tms6 = 0 }
            ms6 += tms6;
            tms7  = parseFloat(document.getElementById('inputMS7_'+i).value.replace(",",""));
            if (isNaN(tms7)) { tms7 = 0 }
            ms7 += tms7;
            tms8  = parseFloat(document.getElementById('inputMS8_'+i).value.replace(",",""));
            if (isNaN(tms8)) { tms8 = 0 }
            ms8 += tms8;
            tms9  = parseFloat(document.getElementById('inputMS9_'+i).value.replace(",",""));
            if (isNaN(tms9)) { tms9 = 0 }
            ms9 += tms9;
            tms10 = parseFloat(document.getElementById('inputMS10_'+i).value.replace(",",""));
            if (isNaN(tms10)) { tms10 = 0 }
            ms10 += tms10;
            tms11 = parseFloat(document.getElementById('inputMS11_'+i).value.replace(",",""));
            if (isNaN(tms11)) { tms11 = 0 }
            ms11 += tms11;
            tms12 = parseFloat(document.getElementById('inputMS12_'+i).value.replace(",",""));
            if (isNaN(tms12)) { tms12 = 0 }
            ms12 += tms12;
            document.getElementById('inputMS1_'+i).value = addCommas(tms1.toFixed(0));
            document.getElementById('inputMS2_'+i).value = addCommas(tms2.toFixed(0));
            document.getElementById('inputMS3_'+i).value = addCommas(tms3.toFixed(0));
            document.getElementById('inputMS4_'+i).value = addCommas(tms4.toFixed(0));
            document.getElementById('inputMS5_'+i).value = addCommas(tms5.toFixed(0));
            document.getElementById('inputMS6_'+i).value = addCommas(tms6.toFixed(0));
            document.getElementById('inputMS7_'+i).value = addCommas(tms7.toFixed(0));
            document.getElementById('inputMS8_'+i).value = addCommas(tms8.toFixed(0));
            document.getElementById('inputMS9_'+i).value = addCommas(tms9.toFixed(0));
            document.getElementById('inputMS10_'+i).value = addCommas(tms10.toFixed(0));
            document.getElementById('inputMS11_'+i).value = addCommas(tms11.toFixed(0));
            document.getElementById('inputMS12_'+i).value = addCommas(tms12.toFixed(0));
        }
    }
    var capx = parseFloat(document.getElementById('inputCapx').value.replace(",",""));
    if (isNaN(capx)) { capx = 0 }
    var expense = parseFloat(document.getElementById('inputExpense').value.replace(",",""));
    if (isNaN(expense)) { expense = 0 }
    
    document.getElementById('inputCapx').value = addCommas(capx.toFixed(0));
    document.getElementById('inputExpense').value = addCommas(expense.toFixed(0));
    
    //savings / payback calc
    var savings = ms1+ms2+ms3+ms4+ms5+ms6+ms7+ms8+ms9+ms10+ms11+ms12;
    if (isNaN(savings)) { savings = 0; } 
    document.getElementById('inputSavings').value = addCommas(savings.toFixed(0));
    var payback = (expense / savings) * 12;
    if (isNaN(payback) || payback == Infinity) { payback = 0; }
    document.getElementById('inputPayback').value = addCommas(payback.toFixed(0));
    
    //current year savings
    var currYearSavings = 0;
    var d=new Date(document.getElementById('inputStartDate').value);
    var ct = 1;
    for (; ct<=12 - d.getMonth(); ct++){
        currYearSavings += eval('ms'+ct)
    }
    document.getElementById('inputCurrYearSaving').value = addCommas(currYearSavings.toFixed(0));
    
    //tool tips
    //savings whole year
    document.getElementById('inputSavings').title = tipSavings + " ("+document.getElementById('labelMS1').innerHTML+ " - " + document.getElementById('labelMS12').innerHTML+")";
    document.getElementById('labelSavings').title = document.getElementById('inputSavings').title;
    //savings current year
    document.getElementById('labelCurrYearSaving').title = document.getElementById('labelMS1').innerHTML+ " - " + document.getElementById('labelMS'+(ct-1)).innerHTML;
    document.getElementById('inputCurrYearSaving').title = document.getElementById('labelCurrYearSaving').title;
    //payback
    document.getElementById('labelPayback').title = tipPayback + " ("+document.getElementById('labelExpense').innerHTML+" / "+document.getElementById('labelSavings').innerHTML+" * 12)";
    document.getElementById('inputPayback').title = document.getElementById('labelPayback').title;
    
}

function addCommas(nStr)
{
	nStr += '';
	x = nStr.split('.');
	x1 = x[0];
	x2 = x.length > 1 ? '.' + x[1] : '';
	var rgx = /(\d+)(\d{3})/;
	while (rgx.test(x1)) {
		x1 = x1.replace(rgx, '$1' + ',' + '$2');
	}
	return x1 + x2;
}

///////////////////////////////////////////////////////////////////////////
//alphaBlock
//
// blocks none alpha chars , .
function alphaBlock() {
    if (event.keyCode < 44 || event.keyCode > 57 || event.keyCode == 45 || event.keyCode == 47 ) {
        event.returnValue = false;
    }
}

function submitEntryForm(theValue){
    //disable buttons
    document.getElementById("buttonSaveProject").disabled = true;
    document.getElementById("buttonCopyProject").disabled = true;
    document.getElementById("buttonDeleteProject").disabled = true;
    //enable selects so can get values...stupid selects
    document.getElementById("selBranch").disabled = false;
    document.getElementById("selProduct").disabled = false;
    document.getElementById("selProcess").disabled = false;
    document.getElementById("selTeamMembers").disabled = false;
    document.getElementById("selStatus").disabled = false;
    document.getElementById("selStatus").disabled = false;
    document.getElementById("selCurrency").disabled = false;
    for (var i=0;i<parseInt(document.getElementById("hiddenMSRowCT").value);i++){
        if (document.getElementById("selSavingsCategory"+i)){
            document.getElementById("selSavingsCategory"+i).disabled = false;
        }
    }
    for (var i=0;i<parseInt(document.getElementById("hiddenImpPlanRowCT").value);i++){
        if (document.getElementById("selActionStatus"+i)){
            document.getElementById("selActionStatus"+i).disabled = false;
        }
    }

    buildHiddenTeamMember();
    document.getElementById('hiddenSearchProjectTitle').value = document.getElementById('selSearchProjectTitle').value;
    document.getElementById('hiddenEntryAction').value = theValue;
    document.mEntryForm.submit();
}

function addTeamMember(){
    //if (document.getElementById('inputTeamMembers').value != ""){
    //var selValue = document.getElementById('selOneTeamMembers').options[document.getElementById('selOneTeamMembers').selectedIndex].value
    //var selText  = document.getElementById('selOneTeamMembers').options[document.getElementById('selOneTeamMembers').selectedIndex].text
    var selText = document.getElementById('selOneTeamMembers').value
    if (selText != ""){
        var ck = true;
        for (var i=0; i<document.getElementById('selTeamMembers').options.length && ck; i++){
            //if (document.getElementById('inputTeamMembers').selectedValue == document.getElementById('selTeamMembers').options[i].value){
            if (selText == document.getElementById('selTeamMembers').options[i].value){
              ck = false;
            }
        }
        if (ck){
            var newOption   = document.createElement('option');
            newOption.text  = selText;
            newOption.value = selText;
            try {
                document.getElementById('selTeamMembers').add(newOption, null); // standards compliant; doesn't work in IE
            }
            catch(ex) {
              document.getElementById('selTeamMembers').add(newOption); // IE only
            }
        }
    }
}

function removeTeamMember(){
    for (var i=document.getElementById('selTeamMembers').options.length-1; i>=0; i--){
        if (document.getElementById('selTeamMembers').options[i].selected){
            document.getElementById('selTeamMembers').remove(i);
        }
    }
}

function buildHiddenTeamMember(){
    document.getElementById('hiddenTeamMembers').value = "";
    for (var i=0; i<document.getElementById('selTeamMembers').options.length; i++){
        document.getElementById('hiddenTeamMembers').value += (document.getElementById('hiddenTeamMembers').value == "" ? "'" : ",'") + document.getElementById('selTeamMembers').options[i].value + "'";
    }
 }