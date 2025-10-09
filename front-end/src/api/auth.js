import { apiFetch } from "./client";


// CREATE ACCOUNT
function createAccount(userData) {
  return apiFetch("/authentication/CreateAccount", {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify(userData),
    credentials: "include", // allows cookies
  });
}

// LOGIN ACCOUNT
function loginAccount(userData) {
  return apiFetch("/authentication/LoginAccount", {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify(userData),
    credentials: "include",
  });
}

// LOGOUT ACCOUNT
function logoutAccount() {
  return apiFetch("/authentication/Logout", {
    method: "DELETE",
    credentials: "include",
  });
}

// TEST AUTHORIZATION
function testAuthorization() {
  return apiFetch("/authentication/TestAuthorization", {
    method: "GET",
    credentials: "include",
  });
}

export const AuthAPI = {
  createAccount,
  loginAccount,
  logoutAccount,
  testAuthorization,
};
