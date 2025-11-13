//Przykład użycia:
//BankAccount konto = new BankAccount("Jan Kowalski", 1000);
//konto.Wplata(500);
//konto.Wyplata(200);
//Console.WriteLine($"Saldo: {konto.Saldo}");


using lab2zad2;

BankAccount konto = new BankAccount("Jan Kowalski", 1000);
konto.Wplata(500);
konto.Wyplata(200);
Console.WriteLine($"Saldo: {konto.Saldo}");
