import type { Libro } from "./types";

const API_URL = "http://localhost:5026";

export async function getLibri(): Promise<Libro[]> {
  const response = await fetch(`${API_URL}/api/libri`);
  if (!response.ok) throw new Error(`Errore ${response.status}`);
  return response.json();
}

export async function login(username: string, password: string): Promise<string> {
  const response = await fetch(`${API_URL}/api/auth/login`, {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify({ username, password }),
  });
  if (response.status === 401) throw new Error("Credenziali non valide");
  if (!response.ok) throw new Error(`Errore ${response.status}`);
  const data = await response.json();
  return data.token;
}

export async function eliminaLibro(id: number, token: string): Promise<void> {
  const response = await fetch(`${API_URL}/api/libri/${id}`, {
    method: "DELETE",
    headers: { Authorization: `Bearer ${token}` },
  });
  if (response.status === 401) throw new Error("Sessione scaduta, rifai il login");
  if (!response.ok && response.status !== 404) {
    throw new Error(`Errore ${response.status}`);
  }
}

export async function creaLibro(
  titolo: string,
  autore: string,
  token: string
): Promise<Libro> {
  const response = await fetch(`${API_URL}/api/libri`, {
    method: "POST",
    headers: {
      "Content-Type": "application/json",
      Authorization: `Bearer ${token}`,
    },
    body: JSON.stringify({ titolo, autore }),
  });

  if (response.ok) return response.json();

  if (response.status === 401) {
    throw new Error("Sessione scaduta, rifai il login");
  }

  // 400 e 409 arrivano come ProblemDetails (JSON)
  const problema = await response.json().catch(() => null);

  if (response.status === 409) {
    throw new Error(problema?.detail ?? "Libro già esistente");
  }

  if (response.status === 400 && problema?.errors) {
    const messaggi = Object.values(problema.errors).flat();
    throw new Error(messaggi.join(" "));
  }

  throw new Error(`Errore ${response.status}`);
}