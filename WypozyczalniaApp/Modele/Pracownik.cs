namespace WypozyczalniaApp.Modele
{
    // Klasa reprezentująca pracownika wypożyczalni.
    // Dziedziczenie po klasie abstrakcyjnej Osoba pozwala na współdzielenie pól (Imię, Nazwisko)
    // oraz stanowi realizację wymaganej hierarchii obiektowej.
    public class Pracownik : Osoba
    {
        // Odzwierciedlenie kolumny 'stanowisko' z tabeli Pracownicy w bazie danych.
        // Przechowywanie informacji o funkcji pełnionej przez osobę (np. Kierownik, Serwisant).
        public string Stanowisko { get; set; }

        // Nadpisanie (override) metody wirtualnej z klasy bazowej.
        // Zastosowanie polimorfizmu w celu dostosowania wyświetlanych danych specyficznie dla pracownika.
        public override string PrzedstawSie()
        {
            // Wykorzystanie słowa kluczowego base do wywołania oryginalnej metody z klasy Osoba,
            // co pozwala uniknąć duplikacji kodu formatującego imię i nazwisko.
            return $"Pracownik: {base.PrzedstawSie()} | Stanowisko: {Stanowisko}";
        }
    }
}