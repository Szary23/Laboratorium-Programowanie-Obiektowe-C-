namespace WypozyczalniaApp.Modele
{
    // Klasa odwzorowująca tabelę "Sprzet" znajdującą się w bazie danych.
    // Dziedziczenie po klasie EncjaBazowa w celu uzyskania dostępu do pola Id (uniknięcie duplikacji kodu).
    public class Sprzet : EncjaBazowa
    {
        // Nazwa konkretnego modelu sprzętu.
        public string Nazwa { get; set; }

        // Zastosowanie typu decimal dla operacji finansowych, co zapewnia
        // większą precyzję obliczeń niż typ double (uniknięcie błędów zaokrągleń).
        public decimal CenaZaDobe { get; set; }

        // Odpowiednik kolumny 'stan_techniczny' z bazy SQL (np. wartości "Nowy", "Sprawny").
        public string Stan { get; set; }

        // Klucz obcy (Foreign Key) służący do relacji z tabelą Kategorie.
        public int IdKategorii { get; set; }
    }
}