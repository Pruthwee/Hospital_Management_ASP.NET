//----------------------Function1-----------------------------//
function validateEmail(Email)
{
    if (Email == "") {
        alert("Email missing. Enter Email.");
        return false;
    }

    else if (Email.indexOf("@") == -1 || Email.indexOf(".com") == -1) {
        alert("Your email address seems incorrect. Please enter a new one.");
        return false;
    }

    else {
        var location = Email.indexOf("@");

        if (Email[0] == '@' || Email[location + 1] == '.') {
            alert("Your email address seems incorrect. Please enter a new one.");
            return false;
        }

        var emailPat = /^(([^<>()\[\]\\.,;:\s@"]+(\.[^<>()\[\]\\.,;:\s@"]+)*)|(".+"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/;
        var EmailmatchArray = Email.match(emailPat);

        if (EmailmatchArray == null) {
            alert("Your email address seems incorrect. Please enter a new one.");
            return false;
        }
    }

    return true;
}






//----------------------Function2-----------------------------//
function validateS()
{
    var Name      = document.getElementById('sName').value;
    var Bdate     = document.getElementById('sBirthDate').value;
    var Email     = document.getElementById('sEmail').value;
    var phone     = document.getElementById('Phone').value;
    var pass      = document.getElementById('sPassword').value;
    var cpass     = document.getElementById('scPassword').value;

    /*now the validation code*/

    if (Name == "")
    {
        alert("Name missing. Enter Name.");
        return false;
    }


    var arrDbirth = Bdate.split("-");

    if (Bdate == "")
    {
        alert("Birth Date missing. Enter Birth Date.");
        return false;
    }

    else if ((Bdate == arrDbirth[0]) || (arrDbirth[0].length != 2) || arrDbirth[1].length != 2 || arrDbirth[2].length != 4 || !arrDbirth[0].match(/^[0-9]*$/) || !arrDbirth[1].match(/^[0-9]*$/) || !arrDbirth[2].match(/^[0-9]*$/) || Number(arrDbirth[0]) > 31 || Number(arrDbirth[1]) > 12)
    {
        alert("Birth Date Format Incorrect or out of Range.");
        return false;
    }


    if (!validateEmail(Email))
        return false;


    if (pass == "" || cpass == "")
    {
        alert("Password field is empty.");
        return false;
    }

    else if (pass != cpass)
    {
        alert("Passwords do not match.");
        return false;
    }


    if (phone.length != 11)
    {
        alert("Phone number should be of 11 digits.");
        return false;
    }


    if (document.getElementById('Gender').value == null)
    {
        alert("Gender not selected.");
        return false;
    }

    return true;
}





//----------------------Function3-----------------------------//
function validateL()
{
    var Email        = document.getElementById('loginEmail').value;
    var Password     = document.getElementById('loginPassword').value;

    /*now the validation code*/

    if (!validateEmail(Email))
        return false;


    if (Password == "")
    {
        alert("Password missing. Enter Password.");
        return false;
    }

    return true;
}
