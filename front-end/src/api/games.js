import { apiFetch } from "./client";

function getAll() {
  return apiFetch("/games");
}

export const GamesAPI = {
  getAll,
};
