import { apiFetch } from "./client";

function getAll() {
  return apiFetch("/Genres");
}

export const GenresAPI = { getAll };