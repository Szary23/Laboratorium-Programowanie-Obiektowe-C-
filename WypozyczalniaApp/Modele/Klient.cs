namespace WypozyczalniaApp.Modele
{
    // Klasa odzwierciedlająca tabelę 'Klienci' w systemie bazodanowym.
    // Dziedziczenie po klasie Osoba pozwala na wykorzystanie już zdefiniowanych pól (Imię, Nazwisko, Id)
    // oraz rozszerzenie ich o dane kontaktowe specyficzne dla klienta.
    public class Klient : Osoba
    {
        // Pola odpowiadające kolumnom 'telefon' i 'email' zdefiniowanym w strukturze bazy danych.
        // Służą do przechowywania informacji kontaktowych niezbędnych do obsługi wypożyczenia.
        public string Telefon { get; set; }
        public string Email { get; set; }

        // Implementacja polimorfizmu poprzez nadpisanie (override) metody wirtualnej.
        // Dostosowanie formatu zwracanych danych, aby jednoznacznie identyfikować obiekt jako Klienta w interfejsie.
        public override string PrzedstawSie()
        {
            // Ponowne wykorzystanie logiki z klasy bazowej (base.PrzedstawSie())
            // w celu uniknięcia redundancji kodu formatującego imię i nazwisko.
            return $"Klient: {base.PrzedstawSie()} | Email: {Email}";
        }
    }
}