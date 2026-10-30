import { useState } from "react";

interface Props {
  onCrea: (titolo: string, autore: string) => Promise<void>;
}

export default function NuovoLibroForm({ onCrea }: Props) {
  const [titolo, setTitolo] = useState("");
  const [autore, setAutore] = useState("");
  const [errore, setErrore] = useState<string | null>(null);

  async function invia(e: React.FormEvent) {
    e.preventDefault();
    setErrore(null);
    try {
      await onCrea(titolo, autore);
      setTitolo("");
      setAutore("");
    } catch (err) {
      setErrore(err instanceof Error ? err.message : "Errore sconosciuto");
    }
  }

  return (
    <form onSubmit={invia}>
      <input
        placeholder="Titolo"
        value={titolo}
        onChange={(e) => setTitolo(e.target.value)}
      />
      <input
        placeholder="Autore"
        value={autore}
        onChange={(e) => setAutore(e.target.value)}
      />
      <button type="submit">Aggiungi</button>
      {errore && <p style={{ color: "red" }}>{errore}</p>}
    </form>
  );
}