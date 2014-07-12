var currMenuPage = "update_"; //default app page
//Update the page displayed in the app
//if user has access to the app displays the buttons for that page also
function changeMenuPage(thePage) {
    document.getElementById(currMenuPage + "page").style.display = "none";
    document.getElementById(thePage + "page").style.display = "";
    currMenuPage = thePage;
    document.getElementById('div1').innerHTML = '';
}

var currTablePage = "tbl_1";  //default page for update table
//updates a table to a new page by hidding the current, displaying the new, 
//  and updating the class on the current page number
function changeTablePage(thePage) {

    document.getElementById(currTablePage).style.display = "none";
    document.getElementById(currTablePage + "Link").className = "";
    currTablePage = thePage;
    document.getElementById(thePage).style.display = "";
    document.getElementById(thePage + "Link").className = "selectedPageLink";


}

//holds asp.ajax call back functions
var prm = Sys.WebForms.PageRequestManager.getInstance();
prm.add_initializeRequest(InitializeRequest);
prm.add_endRequest(EndRequest);
var postBackID = "";

//Checks page before post back
function InitializeRequest(sender, args) {
    disableButtons();
    postBackID = args.get_postBackElement().id;
    var mPage = 0;

    if (postBackID == "upd_but") {

        var tmp = IsNumeric(document.getElementById('pct_txt').value);

        if (document.getElementById('pct_txt').value == "" ||
            document.getElementById('pct_txt').value == null ||
            document.getElementById('pct_txt').value > 1 ||
            tmp == 'false') {
            alert("You have not entered a valid percentage!  Please try again");
            args.set_cancel(true);
            document.getElementById('div1').innerHTML = 'Request Canceled';
        }
        else {
            document.getElementById('div1').innerHTML = 'Updating...';
        }


    }
    else if (postBackID == "question_button") {


        for (var a = 1; a <= document.getElementById('hid_quest_cnt').value; a++) {

            if (IsNumeric(document.getElementById('weitxt' + a).value) == 'false' ||
                IsNumeric(document.getElementById('qsttxt' + a).value) == 'false' ||
                IsNumeric(document.getElementById('sectxt' + a).value) == 'false') {
                alert('You have entered an invalid number.  Please try again');
                args.set_cancel(true);
                document.getElementById('div1').innerHTML = 'Request Canceled';
                a = 99999999;
                e = 'true';
            }
            else {
                document.getElementById('div1').innerHTML = 'Updating...';
                e = 'false';

            }
        }

        if (e == 'false') {
            document.getElementById('question_div').style.display = "none";

            currTablePage = "tbl_1";
        }


    }
    else if (postBackID == "branch_main_button") {
        document.getElementById('branch_main').style.display = "none";
        document.getElementById('div1').innerHTML = 'Updating...';
    }
    else if (postBackID == "add_ques_sec_button") {
        if (document.getElementById('add_list').value == '' &&
            document.getElementById('add_ques_txt').value != '') {
            alert("You have entered text for a new question, but have not selected a section to store it under.  Please select a section and resubmit.");
            args.set_cancel(true);
            document.getElementById('div1').innerHTML = 'Request Canceled';
        }

        if (document.getElementById('add_list').value != '' &&
            document.getElementById('add_ques_txt').value.replace(/^\s+|\s+$/g, "") == '') {
            alert("You have selected a section, but have entered spaces into the question text field.  Please enter a valid question in the question text field and resubmit");
            args.set_cancel(true);
            document.getElementById('div1').innerHTML = 'Request Canceled';
        }
    }


}

//Displays error message if post back errored
function EndRequest(sender, args) {
    if (args._error != null) {
        document.getElementById('div1').innerHTML = 'Error: Please contact tech support.';
        args._errorHandled = true;
    }
    else {
        enableButtons();
    }
}

function disableButtons() {

}

function enableButtons() {

}

function chk_current_pct() {
    var tot_pct = 0;

    document.getElementById('question_button').disabled = false;

    for (var a = 1; a <= document.getElementById('hid_quest_cnt').value; a++) {
        if (document.getElementById('ststxt' + a).value == 'Active') {

            tot_pct += Number(document.getElementById('weitxt' + a).value);

            if (document.getElementById('weitxt' + a).value == 0) {
                document.getElementById('question_button').disabled = true;
                changeTablePage('tbl_' + document.getElementById('sectxt' + a).value);
                alert('Weight equals zero!  Please enter a weight greater than zero!');
                return;

            }

        }
    }

    document.getElementById('chk_pct').value = tot_pct;
}

function IsNumeric(sText) {
    var ValidChars = "0123456789.";
    var IsNumber = 'true';
    var Char;


    for (i = 0; i < sText.length && IsNumber == 'true'; i++) {
        Char = sText.charAt(i);
        if (ValidChars.indexOf(Char) == -1) {
            IsNumber = 'false';
        }
    }

    return IsNumber;

}

function IsDate() {
    var ValidChars = "0123456789";
    var ValidSep = "/";
    var Char;
    var sText;
    var sFiletxt;
    var e = 'true';
    var dt = new Date();
    var fd = new Date(document.getElementById('insp_dt').value);

    sText = document.getElementById('insp_dt').value;

    if (sText.length != 10) {
        alert('You have entered an invalid date format.  Please try again!');
        e = 'false';

        document.getElementById('excel_but').style.display = "none";
    }
    else {

        for (i = 0; i < sText.length && e == 'true'; i++) {
            Char = sText.charAt(i);
            if (i == 0 || i == 1 || i == 3 || i == 4 || i > 5) {
                if (ValidChars.indexOf(Char) == -1) {
                    alert('You have entered an invalid date format.  Please try again!');
                    e = 'false';
                    document.getElementById('excel_but').style.display = "none";
                }
            }
            else {
                if (ValidSep.indexOf(Char) == -1) {
                    alert('You have entered an invalid date format.  Please try again!');
                    e = 'false';
                    document.getElementById('excel_but').style.display = "none";
                }
            }
        }

    }

    if (fd.getYear() > dt.getYear()) {
        alert('You have entered an invalid year.  Please try again!');
        document.getElementById('excel_but').style.display = "none";
        e = 'false';
    }

    if (e == 'true') {
        document.getElementById('excel_but').style.display = "block";

    }

}

function IsFile() {

}
