
function parseDate(d)
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


function checkDate(c)
{
		c.value = parseDate(c.value);
}

