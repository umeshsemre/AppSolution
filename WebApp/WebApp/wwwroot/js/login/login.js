// Login Page JavaScript Functions
// This file contains all login page specific functionality

// Password toggle functionality
function togglePassword() {
    const passwordField = document.getElementById('password');
    const toggleBtn = document.querySelector('.password-toggle');

    if (passwordField && toggleBtn) {
        if (passwordField.type === 'password') {
            passwordField.type = 'text';
            toggleBtn.textContent = '🙈';
            toggleBtn.setAttribute('aria-label', 'Hide password');
        } else {
            passwordField.type = 'password';
            toggleBtn.textContent = '👁️';
            toggleBtn.setAttribute('aria-label', 'Show password');
        }
    }
}

// Email validation function
function validateEmail(email) {
    const re = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    return re.test(email);
}

// Show/hide error messages
function showLoginError(fieldId, show = true) {
    const errorElement = document.getElementById(fieldId + 'Error');
    if (errorElement) {
        if (show) {
            errorElement.classList.add('show');
        } else {
            errorElement.classList.remove('show');
        }
    }
}

// Show general error messages
function showGeneralError(message) {
    // Remove existing general error if any
    const existingError = document.querySelector('.login-general-error');
    if (existingError) {
        existingError.remove();
    }

    // Create and show new error message
    const errorDiv = document.createElement('div');
    errorDiv.className = 'login-error-message login-general-error show';
    errorDiv.textContent = message;

    const form = document.querySelector('.login-form');
    const title = form.querySelector('.form-title');
    title.parentNode.insertBefore(errorDiv, title.nextSibling);

    // Auto-hide after 10 seconds
    setTimeout(() => {
        if (errorDiv && errorDiv.parentNode) {
            errorDiv.classList.remove('show');
            setTimeout(() => {
                if (errorDiv && errorDiv.parentNode) {
                    errorDiv.remove();
                }
            }, 300);
        }
    }, 10000);
}

// Get return URL from query string or form
function getReturnUrl() {
    const urlParams = new URLSearchParams(window.location.search);
    const returnUrl = urlParams.get('returnUrl');

    // Also check hidden input field
    const returnUrlInput = document.querySelector('input[name="ReturnUrl"]');
    const inputReturnUrl = returnUrlInput ? returnUrlInput.value : null;

    return returnUrl || inputReturnUrl || null;
}

// Check if user is already logged in
function checkAuthStatus() {
    const token = localStorage.getItem('authToken') || sessionStorage.getItem('authToken');

    if (token) {
        // Optionally validate token with server or redirect if already logged in
        const returnUrl = getReturnUrl();
        if (confirm('You are already logged in. Do you want to go to the dashboard?')) {
            window.location.href = returnUrl || '/Dashboard';
        } else {
            // Clear existing tokens if user wants to login again
            localStorage.removeItem('authToken');
            localStorage.removeItem('refreshToken');
            sessionStorage.removeItem('authToken');
            sessionStorage.removeItem('refreshToken');
        }
    }
}

// Utility function to make authenticated API calls
function makeAuthenticatedRequest(url, options = {}) {
    const token = localStorage.getItem('authToken') || sessionStorage.getItem('authToken');

    const defaultHeaders = {
        'Content-Type': 'application/json',
        'Accept': 'application/json'
    };

    if (token) {
        defaultHeaders['Authorization'] = `Bearer ${token}`;
    }

    const config = {
        ...options,
        headers: {
            ...defaultHeaders,
            ...options.headers
        }
    };

    return fetch(url, config)
        .then(response => {
            // If token expired, try to refresh
            if (response.status === 401) {
                return refreshAuthToken()
                    .then(() => {
                        // Retry the original request with new token
                        const newToken = localStorage.getItem('authToken') || sessionStorage.getItem('authToken');
                        config.headers['Authorization'] = `Bearer ${newToken}`;
                        return fetch(url, config);
                    })
                    .catch(() => {
                        // Refresh failed, redirect to login
                        window.location.href = '/Account/Login';
                        throw new Error('Session expired');
                    });
            }
            return response;
        });
}

// API Configuration
const API_CONFIG = {
    baseUrl: 'https://localhost:44380/api/Auth',
    endpoints: {
        login: '/login',
        refresh: '/refresh'
    }
};

// AJAX Login function
function performLogin(username, password, rememberMe = false) {
    const loginData = {
        username: username,
        password: password
    };

    return fetch(API_CONFIG.baseUrl + API_CONFIG.endpoints.login, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
            'Accept': 'application/json'
        },
        body: JSON.stringify(loginData)
    })
        .then(response => {
            if (!response.ok) {
                throw new Error(`HTTP error! status: ${response.status}`);
            }
            return response.json();
        })
        .then(data => {
            // Handle successful login
            if (data.token) {
                // Store token (you can modify this based on your needs)
                if (rememberMe) {
                    localStorage.setItem('authToken', data.token);
                    if (data.refreshToken) {
                        localStorage.setItem('refreshToken', data.refreshToken);
                    }
                } else {
                    sessionStorage.setItem('authToken', data.token);
                    if (data.refreshToken) {
                        sessionStorage.setItem('refreshToken', data.refreshToken);
                    }
                }
                return data;
            } else {
                throw new Error('No token received');
            }
        });
}

// Refresh Token function
function refreshAuthToken() {
    const refreshToken = localStorage.getItem('refreshToken') || sessionStorage.getItem('refreshToken');

    if (!refreshToken) {
        throw new Error('No refresh token available');
    }

    const refreshData = {
        refreshToken: refreshToken
    };

    return fetch(API_CONFIG.baseUrl + API_CONFIG.endpoints.refresh, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
            'Accept': 'application/json'
        },
        body: JSON.stringify(refreshData)
    })
        .then(response => {
            if (!response.ok) {
                throw new Error(`HTTP error! status: ${response.status}`);
            }
            return response.json();
        })
        .then(data => {
            if (data.token) {
                // Update stored tokens
                const storage = localStorage.getItem('authToken') ? localStorage : sessionStorage;
                storage.setItem('authToken', data.token);
                if (data.refreshToken) {
                    storage.setItem('refreshToken', data.refreshToken);
                }
                return data;
            } else {
                throw new Error('No token received from refresh');
            }
        });
}

// Initialize login form functionality
function initializeLogin() {
    const loginForm = document.getElementById('loginForm');

    if (!loginForm) return;

    // Form submission handler
    loginForm.addEventListener('submit', function (e) {
        e.preventDefault(); // Always prevent default form submission

        const email = document.getElementById('email');
        const password = document.getElementById('password');
        const rememberMe = document.getElementById('remember');
        const loginBtn = document.getElementById('loginBtn');

        if (!email || !password || !loginBtn) return;

        // Reset previous errors
        showLoginError('email', false);
        showLoginError('password', false);

        let isValid = true;

        // Validate email
        if (!email.value ) {
            showLoginError('email');
            isValid = false;
        }

        // Validate password
        if (!password.value || password.value.length < 6) {
            showLoginError('password');
            isValid = false;
        }

        if (!isValid) {
            return false;
        }

        // Show loading state
        loginBtn.classList.add('login-btn-loading');
        loginBtn.textContent = '';
        loginBtn.disabled = true;

        // Perform AJAX login
        performLogin(email.value, password.value, rememberMe ? rememberMe.checked : false)
            .then(response => {
                // Handle successful login
                showLoginSuccess('Login successful! Redirecting...');

                // Redirect after success (customize as needed)
                setTimeout(() => {
                    const returnUrl = getReturnUrl();
                    window.location.href = returnUrl || '/Dashboard';
                }, 1500);
            })
            .catch(error => {
                console.error('Login error:', error);

                // Handle different error types
                let errorMessage = 'Login failed. Please try again.';

                if (error.message.includes('401')) {
                    errorMessage = 'Invalid email or password.';
                } else if (error.message.includes('429')) {
                    errorMessage = 'Too many login attempts. Please try again later.';
                } else if (error.message.includes('500')) {
                    errorMessage = 'Server error. Please try again later.';
                } else if (error.message.includes('NetworkError') || error.message.includes('Failed to fetch')) {
                    errorMessage = 'Network error. Please check your connection.';
                }

                // Show error message
                showGeneralError(errorMessage);
            })
            .finally(() => {
                // Reset button state
                loginBtn.classList.remove('login-btn-loading');
                loginBtn.textContent = 'Sign In';
                loginBtn.disabled = false;
            });
    });

    // Input focus effects
    const formInputs = document.querySelectorAll('.login-form-input');
    formInputs.forEach(input => {
        input.addEventListener('focus', function () {
            const parentGroup = this.closest('.login-input-group');
            if (parentGroup) {
                parentGroup.classList.add('focused');
            }
        });

        input.addEventListener('blur', function () {
            const parentGroup = this.closest('.login-input-group');
            if (parentGroup) {
                parentGroup.classList.remove('focused');
            }
        });
    });

    // Auto-focus first input
    const emailInput = document.getElementById('email');
    if (emailInput) {
        emailInput.focus();
    }

    // Password toggle button event
    const passwordToggle = document.querySelector('.password-toggle');
    if (passwordToggle) {
        passwordToggle.addEventListener('click', togglePassword);
    }

    // Real-time validation
    const emailInput = document.getElementById('email');
    const passwordInput = document.getElementById('password');

    if (emailInput) {
        emailInput.addEventListener('input', function () {
            if (this.value && ) {
                showLoginError('email', false);
            }
        });
    }

    if (passwordInput) {
        passwordInput.addEventListener('input', function () {
            if (this.value.length >= 6) {
                showLoginError('password', false);
            }
        });
    }
}

// Show success messages
function showLoginSuccess(message) {
    const successElement = document.getElementById('loginSuccessMessage');
    if (successElement) {
        successElement.textContent = message;
        successElement.classList.add('show');

        setTimeout(() => {
            successElement.classList.remove('show');
        }, 5000);
    }
}

// Initialize when DOM is loaded
document.addEventListener('DOMContentLoaded', function () {
    initializeLogin();
    checkAuthStatus(); // Check if already logged in
});

// Handle page visibility changes (for better UX)
document.addEventListener('visibilitychange', function () {
    if (!document.hidden) {
        const emailInput = document.getElementById('email');
        if (emailInput && !emailInput.value) {
            emailInput.focus();
        }
    }
});

// Handle page visibility changes (for better UX)
document.addEventListener('visibilitychange', function () {
    if (!document.hidden) {
        const emailInput = document.getElementById('email');
        if (emailInput && !emailInput.value) {
            emailInput.focus();
        }
    }
});

// Keyboard shortcuts
document.addEventListener('keydown', function (e) {
    // Alt + L to focus email field
    if (e.altKey && e.key === 'l') {
        e.preventDefault();
        const emailInput = document.getElementById('email');
        if (emailInput) {
            emailInput.focus();
        }
    }

    // Escape to clear form
    if (e.key === 'Escape') {
        const loginForm = document.getElementById('loginForm');
        if (loginForm && confirm('Clear the form?')) {
            loginForm.reset();
            showLoginError('email', false);
            showLoginError('password', false);
            const emailInput = document.getElementById('email');
            if (emailInput) {
                emailInput.focus();
            }
        }
    }
});

// Form auto-save to localStorage (optional - remove if not needed)
function autoSaveFormData() {
    const emailInput = document.getElementById('email');
    const rememberCheckbox = document.getElementById('remember');

    if (emailInput && rememberCheckbox) {
        emailInput.addEventListener('input', function () {
            if (rememberCheckbox.checked) {
                localStorage.setItem('savedLoginEmail', this.value);
            } else {
                localStorage.removeItem('savedLoginEmail');
            }
        });

        rememberCheckbox.addEventListener('change', function () {
            if (this.checked && emailInput.value) {
                localStorage.setItem('savedLoginEmail', emailInput.value);
            } else {
                localStorage.removeItem('savedLoginEmail');
            }
        });

        // Load saved email on page load (only if not already logged in)
        const token = localStorage.getItem('authToken') || sessionStorage.getItem('authToken');
        if (!token) {
            const savedEmail = localStorage.getItem('savedLoginEmail');
            if (savedEmail) {
                emailInput.value = savedEmail;
                rememberCheckbox.checked = true;
            }
        }
    }
}

// Logout function (utility)
function logout() {
    // Clear all stored tokens
    localStorage.removeItem('authToken');
    localStorage.removeItem('refreshToken');
    sessionStorage.removeItem('authToken');
    sessionStorage.removeItem('refreshToken');

    // Redirect to login page
    window.location.href = '/Account/Login';
}

// Initialize auto-save (call this if you want the feature)
// Uncomment the line below to enable email auto-save
// autoSaveFormData();