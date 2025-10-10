const API_URL = import.meta.env.VITE_API_URL; 

export async function apiFetch(path, options = {}) {
  const { body, headers: h } = options;
  const headers = new Headers(h || {});
  let finalBody = body;

  if (finalBody !== undefined && finalBody !== null) {
    if (!headers.has("Content-Type")) {
      headers.set("Content-Type", "application/json");
    }
    if (headers.get("Content-Type")?.startsWith("application/json") && typeof finalBody !== "string") {
      finalBody = JSON.stringify(finalBody);
    }
  }

  const res = await fetch(`${API_URL}${path}`, {
    ...options,
    credentials: "include",
    headers: { Accept: "application/json", ...Object.fromEntries(headers.entries()) },
    body: finalBody,
  });

  if (res.status === 204) return null;
  if (res.status === 401) throw new Error("Unauthorized: not logged in or session expired.");
  if (res.status === 403) throw new Error("Forbidden: insufficient permissions.");

  const text = await res.text();
  let data = null;
  if (text) { try { data = JSON.parse(text); } catch {} }

  if (!res.ok) {
    throw new Error(data?.message || text?.slice(0, 300) || `API error ${res.status}: ${res.statusText}`);
  }
  return data;
}
