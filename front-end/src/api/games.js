import { apiFetch } from "./client";
export const GamesAPI = {
  getAll: () => apiFetch("/Games"),
  getById: (id) => apiFetch(`/Games/${id}`),
  create: (data) => apiFetch("/Games", { method:"POST", body: JSON.stringify(data) }),
  update: (id, data) => apiFetch(`/Games/${id}`, { method:"PUT", body: JSON.stringify(data) }),
  remove: (id) => apiFetch(`/Games/${id}`, { method:"DELETE" }),
};