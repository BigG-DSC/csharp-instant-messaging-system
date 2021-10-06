using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Servizi
{
  class ListaUtenti : Servizio
  {
    /* Servizio per recuperare la lista di amici */
    override public void fornisciServizio(Ricettore client, List<string> lista)
    {
      /* Istanza di convertitore */
      Convertitore converti = new Convertitore();
      /* Variabile predisposta per contenere la lista degli amici */
      List<string> listaUtentiOnline = new List<string>();
      /* Recupero Client Connessi */
      /* Variabile per la copia di Dictionary per effettuare elaborazioni locali al metodo */
      Dictionary<Ricettore, string> clientConnessi;
      lock(Connessione.Istanza.ClientConnessi)
        clientConnessi = Connessione.Istanza.ClientConnessi;

      /* Costruzione della lista di utenti Online (viene escluso il richiedente) */
      foreach (KeyValuePair<Ricettore, string> utente in clientConnessi)
        listaUtentiOnline.Add(utente.Value);

      /* Invia la lista di amici */
      Pacchetto messaggio = new Pacchetto("ListaUtentiOnline", converti.listaAStringa(listaUtentiOnline));
      client.InviaPacchetto(messaggio);
    }
  }
}
