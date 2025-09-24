# EPD Console Applicatie

## Functies

### Helperfunctie
De functie `ReadInput` zorgt ervoor dat code herhaling wordt vermeden. Tegelijkertijd geeft deze functie de gebruiker de mogelijkheid om op elk moment uit een geselecteerde optie te stappen en terug te keren naar het menu. Dit maakt de interactie gebruiksvriendelijk en consistent door de hele applicatie.

### Validatie
Bij de invoer van gegevens wordt altijd gecontroleerd of de input het juiste formaat heeft. Dit geldt bijvoorbeeld voor het rijksregisternummer, geboortedata en e-mailadressen. Door deze validatie wordt ervoor gezorgd dat alleen correcte gegevens in de database terechtkomen, wat essentieel is voor medische gegevens.

### Afsprakenbeheer
Voor afspraken zijn vaste tijdsloten geprogrammeerd waaruit de gebruiker kan kiezen. Hierdoor wordt dubbele boeking bij dezelfde arts voorkomen. Dit systeem maakt het plannen overzichtelijk en betrouwbaar.

### Duidelijk overzicht
De gegevens van afspraken, patiënten en artsen worden in tabellen weergegeven in de console. Dit geeft een overzichtelijk beeld van de inhoud en zorgt ervoor dat de gebruiker snel en gemakkelijk informatie kan vinden.

## Persoonlijke aanpassingen
De applicatie is opgesplitst in meerdere onderdelen om beter overzicht in de code te creëren, zonder dat je naar één lange functie of bestand hoeft te zoeken.  

De applicatie is verdeeld in drie hoofdmodules:

- **Patient** – bevat patiëntentaken en bijbehorende logica  
- **Physician** – bevat artsentaken en bijbehorende logica  
- **Appointment** – bevat de afsprakenlogica  

Binnen deze modules worden services gebruikt om specifieke functionaliteit te bundelen, zoals toevoegen, verwijderen of tonen van gegevens. Dit maakt de code overzichtelijker, onderhoudbaarder en makkelijker uit te breiden.
