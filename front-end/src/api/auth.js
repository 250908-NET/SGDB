import { apiFetch } from "./client";

export const AuthAPI = {
  createAccount: (user) =>
    apiFetch("/Authentication/CreateAccount", { method: "POST", body: user }),
  loginAccount: ({ username }) =>
    apiFetch("/Authentication/LoginAccount", { method: "POST", body: { username } }),
  logoutAccount: () =>
    apiFetch("/Authentication/Logout", { method: "DELETE" }),
  testAuthorization: () =>
    apiFetch("/Authentication/TestAuthorization", { method: "GET" }),
};
