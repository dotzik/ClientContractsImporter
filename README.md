# ClientContractsImporter

Nástroj pro převod dat z Excel (.xlsx) do strukturované objektové podoby v JSON nebo XML.

## 🔧 Funkcionalita

* Načtení XLSX souboru se strukturou:

  * **Sloupec A:** Název klienta
  * **Sloupec B:** IČ klienta
  * **Sloupec C:** Název zakázky
  * **Sloupce D až XX:** Počet kusů v jednotlivých měsících (hlavička = období)
* Podpora více worksheetů
* Automatické mapování klient → zakázka → seznam výrobků
* Výstup ve formátu `JSON` nebo `XML`

## 📦 Architektura

| Vrstva      | Popis                                                                      |
| ----------- | -------------------------------------------------------------------------- |
| `Contracts` | Obsahuje doménové modely, a interfaces                                     |
| `Core`      | Implementace konvertor, exportéry a 'orchestrátor' (`JsonImportHandler`)   |
| `Cli`       | Konzolová aplikace s možností předat cestu a formát jako argument          |
| `Web`       | ASP.NET Core MVC aplikace s formulářem pro upload a volbu formátu          |
| `Tests`     | xUnit testy validující zpracování vstupu a funkčnost exportu               |

## 📁 Ukázka struktury XLSX

| Název klienta | IČO    | Zakázka | 01/2023 | 02/2023 | ... |
| ------------- | ------ | ------- | ------- | ------- | --- |
| ACME a.s.     | 123456 | ZAK001  | 10      | 12      | ... |

## 🧠 Logika zpracování (z `XlsxClientDataConverter`)

1. **Pro každý worksheet:**

   * Načtou se období ze záhlaví (řádek 1)
   * Pro každý řádek (od druhého řádku):

     * Získá se jméno, IČ a název zakázky
     * Z každého měsíčního sloupce se přečte počet kusů
     * Hodnoty se namapují na `Client` → `Contract` → `ProductionRecord`
   * Pokud klient existuje, přidá se zakázka; jinak se vytvoří nový klient

2. **Validace a logování:**

   * Pokud chybí IČ nebo název zakázky → log warning
   * Pokud není možné převést buňku na číslo → log warning

## 🚀 Spuštění

### CLI

```bash
dotnet run --project src/ClientContractsImporter.Cli test.xlsx output.json json
```

### Web

```bash
dotnet run --project src/ClientContractsImporter.Web
```

> Následně otevřete prohlížeč na `https://localhost:5065`

## 🧪 Testy

```bash
dotnet test src/ClientContractsImporter.Tests
```

## 🛠 Použité technologie

* .NET 8
* ClosedXML (čtení XLSX)
* Newtonsoft.Json (JSON export)
* XmlSerializer (XML export)
* ASP.NET Core MVC
* xUnit, Moq (testy)

## 📌 Poznámky

* Používáme `ExportFormat` jako `enum` + extension metody pro MIME type a příponu
* Všechny modely mají veřejné konstruktory a `set` pro podporu serializace do XML
* Využíváme DI (`IClientDataConverter`, `IClientExporter`, `IImportHandler`)

---

© 2025 – Ukázkové řešení pro SoftGate s.r.o.

