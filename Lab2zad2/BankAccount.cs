using System;

//Zadanie 2.
//Napisz klasę BankAccount, która będzie symulowała konto bankowe.
//Zaimplementuj właściwości Saldo (publiczne, tylko do odczytu) i Wlasciciel.
//Dodaj metodę Wplata(decimal kwota), która pozwala na zwiększenie salda, oraz metodę
//Wyplata(decimal kwota), która sprawdzi, czy jest wystarczająca ilość środków, a następnie
//odejmie odpowiednią kwotę.
//Użyj operatorów dostępu, aby zabezpieczyć saldo przed bezpośrednią modyfikacją


namespace lab2zad2;

internal class BankAccount
{

    public decimal Saldo { get; private set; }
    public string Wlasciciel { get; set; }
    public BankAccount(string wlasciciel, decimal poczatkoweSaldo)
    {
        Wlasciciel = wlasciciel;
        Saldo = poczatkoweSaldo;
    }
    public void Wplata(decimal kwota)
    {
        if (kwota > 0)
        {
            Saldo += kwota;
            Console.WriteLine($"Wpłacono: {kwota}. Nowe saldo: {Saldo}");
        }
        else
        {
            throw new ArgumentException("Kwota wpłaty musi być większa od zera.");
        }
    }
    public void Wyplata(decimal kwota)
    {
        if (kwota > 0)
        {
            if (kwota <= Saldo)
            {
                Saldo -= kwota;
                Console.WriteLine($"Wypłacono: {kwota}. Nowe saldo: {Saldo}");
            }
            else
            {
                throw new InvalidOperationException("Niewystarczające środki na koncie.");
            }
        }
        else
        {
            throw new ArgumentException("Kwota wypłaty musi być większa od zera.");
        }
    }


}
