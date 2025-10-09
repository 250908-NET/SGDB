import { apiFetch } from "./client";

// GET /api/Genres   { genreId, name, games: [etc.] }
function getAll() {
  return apiFetch("/Genres");
}

export const GenresAPI = { getAll };