const API_BASE = "https://localhost:44380";

// Save tokens in localStorage
function setTokens(data) {
    localStorage.setItem("accessToken", data.accessToken);
    localStorage.setItem("refreshToken", data.refreshToken);
    localStorage.setItem("accessTokenExpiresAt", data.accessTokenExpiresAt);
}

function getAccessToken() {
    return localStorage.getItem("accessToken");
}

function clearTokens() {
    localStorage.removeItem("accessToken");
    localStorage.removeItem("refreshToken");
    localStorage.removeItem("accessTokenExpiresAt");
}

// Wrapper for AJAX with JWT
function apiRequest(endpoint, method, data, successCallback, errorCallback) {
    $.ajax({
        url: API_BASE + endpoint,
        method: method,
        data: data ? JSON.stringify(data) : null,
        contentType: "application/json",
        headers: {
            "Authorization": "Bearer " + getAccessToken()
        },
        success: function (res) {
            if (successCallback) successCallback(res);
        },
        error: function (xhr) {
            if (xhr.status === 401) {
                // Unauthorized → back to login
                //clearTokens();
                //window.location.href = "/Account/Login";
            } else {
                if (errorCallback) errorCallback(xhr);
            }
        }
    });
}

