$(document).ready(function () {
    $("#loginForm").submit(function (e) {
      
        e.preventDefault();

        var data = {
            username: $("#username").val(), 
            password: $("#password").val()
        };
        alert(JSON.stringify(data));
        $.ajax({
            
            url: API_BASE + "/api/Auth/login",
            method: "POST",
            contentType: "application/json",
            data: JSON.stringify(data),
            success: function (res) {
              
                setTokens(res); // Save tokens globally
                window.location.href = "/Home"; // Redirect
            },
            error: function (xhr) {
                alert(JSON.stringify(xhr));
                $("#error").text("Invalid username or password");
            }
        });
    });
});
