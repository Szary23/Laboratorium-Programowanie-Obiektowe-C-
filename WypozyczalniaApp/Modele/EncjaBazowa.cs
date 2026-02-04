namespace WypozyczalniaApp.Modele
{
    // Abstrakcyjna klasa bazowa dla wszystkich modeli domenowych w aplikacji.
    // Jej zadaniem jest zapewnienie wspólnej struktury dla obiektów posiadających tożsamość w bazie danych.
    // Zastosowanie dziedziczenia pozwala uniknąć redundancji kodu w klasach pochodnych (realizacja zasady DRY).
    public abstract class EncjaBazowa
    {
        // Odzwierciedlenie kolumny klucza głównego (Primary Key), która występuje w każdej tabeli systemu (np. id_klienta, id_sprzetu).
        // Dzięki dziedziczeniu, każda klasa pochodna automatycznie uzyskuje dostęp do tego pola, co ujednolica obsługę identyfikatorów.
        public int Id { get; set; }
    }
}