import { useEffect, useState } from "react";
import type { Libro } from "./types";

const API_URL = "http://localhost:5026";

function App() {
  const [libri, setLibri] = useState<Libro[]>([]);
  const [errore, setErrore] = useState<string | null>(null);
  const [caricamento, setCaricamento] = useState(true);

  useEffect(() => {
    async function carica() {
      try {
        const response = await fetch(`${API_URL}/api/libri`);
        if (!response.ok) {
          throw new Error(`Errore ${response.status}`);
        }
        setLibri(await response.json());
      } catch (e) {
        setErrore(e instanceof Error ? e.message : "Errore sconosciuto");
      } finally {
        setCaricamento(false);
      }
    }
    carica();
  }, []);

  if (caricamento) return <p>Caricamento...</p>;
  if (errore) return <p>Errore: {errore}</p>;

  return (
    <div>
      <h1>Libri</h1>
      <ul>
        {libri.map((libro) => (
          <li key={libro.id}>
            {libro.titolo} - {libro.autore}
          </li>
        ))}
      </ul>
    </div>
  );
}

export default App;