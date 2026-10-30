import { useEffect, useState } from "react";
import type { Libro } from "./types";
import { getLibri, eliminaLibro, creaLibro } from "./api";
import LoginForm from "./LoginForm";
import NuovoLibroForm from "./NuovoLibroForm";

function App() {
  const [libri, setLibri] = useState<Libro[]>([]);
  const [errore, setErrore] = useState<string | null>(null);
  const [caricamento, setCaricamento] = useState(true);
  const [token, setToken] = useState<string | null>(
    sessionStorage.getItem("token")
  );

  async function carica() {
    try {
      setLibri(await getLibri());
    } catch (e) {
      setErrore(e instanceof Error ? e.message : "Errore sconosciuto");
    } finally {
      setCaricamento(false);
    }
  }

  useEffect(() => {
    carica();
  }, []);

  function gestisciLogin(nuovoToken: string) {
    sessionStorage.setItem("token", nuovoToken);
    setToken(nuovoToken);
  }

  function logout() {
    sessionStorage.removeItem("token");
    setToken(null);
  }

  async function elimina(id: number) {
    if (!token) return;
    try {
      await eliminaLibro(id, token);
      await carica();
    } catch (e) {
      setErrore(e instanceof Error ? e.message : "Errore sconosciuto");
      logout();
    }
  }

  async function crea(titolo: string, autore: string) {
    if (!token) return;
    await creaLibro(titolo, autore, token);
    await carica();
  }

  if (caricamento) return <p>Caricamento...</p>;

  return (
    <div>
      <h1>Libri</h1>

      {token ? (
        <>
          <button onClick={logout}>Esci</button>
          <NuovoLibroForm onCrea={crea} />
        </>
      ) : (
        <LoginForm onLogin={gestisciLogin} />
      )}

      {errore && <p style={{ color: "red" }}>Errore: {errore}</p>}

      <ul>
        {libri.map((libro) => (
          <li key={libro.id}>
            {libro.titolo} - {libro.autore}{" "}
            {token && <button onClick={() => elimina(libro.id)}>Elimina</button>}
          </li>
        ))}
      </ul>
    </div>
  );
}

export default App;