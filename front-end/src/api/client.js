const API_URL = import.meta.env.VITE_API_URL;

/**
 * Wrapper around fetch() that automatically prefixes requests with the backend base URL.
 *
 * @param {string} path - The path to append to the base API URL.
 * @param {RequestInit} options - Fetch options.
 *
 * @returns {Promise<any>} The parsed JSON response, or null for 204 responses.
 * @throws {Error} If the response is not ok, throws an error with status and message.
 */
export async function apiFetch(path, options) {
  const url = `${API_URL}${path}`;

  const response = await fetch(url, {
    ...options,
    headers: {
      Accept: "application/json",
      ...options?.headers,
    },
  });

  let data = null;

  if (response.status != 204) {
    try {
      data = await response.json();
    } catch (err) {
      throw new Error(`Failed to parse JSON response: ${err.message}`);
    }

    if (!response.ok) {
      const message = (data && data.message) || response.statusText;
      throw new Error(`API error ${response.status}: ${message}`);
    }
  }

  return data;
}