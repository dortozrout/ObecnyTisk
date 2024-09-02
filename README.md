# ObecnyTisk
UNIVERZÁLNÍ PROGRAM NA TISK ŠTÍTKŮ
==================================
Program tiskne štítky podle šablon v daném adresáři. Pomocí konfiguračního souboru lze nastavit
různé parametry. Konfiguračních souborů může být několik, každý pro jednu úlohu. Výchozí
konfigurační soubor je : %appdata%/TiskStisku/conf.txt. Jiný konfigurační soubor lze zadat pomocí
parametru při spuštění (např. conf01.txt). Pokud konfigurační soubor neexituje, bude vytvořen ve 
výchozím adresáři (%appdata%/TiskStisku/), nebo jinde pokud je specifikovaná celá cesta.

Popis konfiguračního souboru
----------------------------
`IPtiskarny:`
Následuje adresa nebo název tiskárny Zebra nebo jiné která podporuje tiskový jazyk EPL2 nebo ZPL.
Lze tisknout na síťové, místní nebo sdílené tiskárny.

`TypTiskarny:`
Je číslo, které určuje typ tiskárny.  
0 - sdílená tiskárna (např: \\\172.16.54.121\Zebra)  
1 - lokální (např: TSC_TP2224)  
2 - síťová (např: PRN196283a-TLP2824, nebo 172.16.36.209)  
3 - výstup na obrazovku  

`Adresar:`
Určuje cestu k adresáři kde jsou umístěny šablony EPL příkazů.

`HledanyText:`
Nepovinný parametr. Pokud je zadán, zobrazí se pouze soubory které obsahují
hledanyText.  

`JedenSoubor:`
TRUE nebo FALSE. Pokud je TRUE vytiskne se pouze jeden soubor.

`OpakovanyTisk:`
TRUE nebo FALSE. Souvisí s parametrem JedenSoubor. Pokud je TRUE, výtisk jednoho souboru
se opakuje dokud uživatel neukončí aplikaci stisknutím CTRL+C nebo křížkem.
Výtisk je omezen na 20 cyklů. Tato možnost slouží například pro výtisk několika štítků 
s ručně zadávanými barkódy.

`Kodovani:`
Určuje kódování uložených šablon. Zpravidla UTF-8 nebo windows-1250.

`Prihlasit:`
TRUE nebo FALSE. Určuje zda vyžadovat identifikaci uživatele. Např. pro tisk štítků na alikvoty 
kontrol.

`Data:`
Parametr s cestou k souboru s doplňujícími daty ve tvaru otazka:odpoved.
Pro pochopení viz popis šablony příkazu.

Všechny parametry se píší bez uvozovek. Můžou obsahovat diakritiku a mezery.

Příklad konfiguračního souboru:
-------------------------------
```
# IP adresa nebo jmeno tiskarny
IPtiskarny: PRN196283a-TLP2824

# typ tiskarny 0 - sdilena, 1 - mistni, 2 - sitova, 3 - výstup na obrazovku
TypTiskarny: 2

# adresar souboru s epl prikazy
adresar: C:\Users\infolab\Documents\StitkyNaAlikvoty

# text ktery se hleda v nazvu souboru
hledanyText: glyhb

# jestli se ma tisknout jenom jeden soubor
jedenSoubor:false

# jeden soubor se tiskne opakovane
opakovanytisk:false

# kodovani ulozenych souboru (UTF-8 nebo windows-1250)
kodovani: UTF-8

# zda vyzadovat login
prihlasit: TRUE

# adresa souboru s daty
data: C:\Users\infolab\Documents\StitkyNaAlikvoty\data\nastaveni QC.txt
```
Popis šablony EPL příkazu.
--------------------------
Šablony jsou uložené v adresáři definovaném v konfiguračním souboru.
Šablona obsahuje jeden EPL2 příkaz, který bude odeslán na tiskárnu.
Šablona může obsahovat pole ve špičatých závorkách (např: \<pole\>).
Takto definované pole program chápe jako dotaz, na který se program snaží
zjistit odpověď.

Odpověď hledá  
a) v souboru definovaném parametrem "data:" v konfiguračním souboru  
b) dotazem na uživatele  
c) nebo se jedná o speciální pole.  

Speciální pole jsou:
\<time\> - nahradí se aktuálním časem  
\<time+30\> - nahradí se časem za 30 minut  
\<time+> - zobrazí dotaz na počet minut o který se má čas posunout  
\<date\> - nahradí se aktuálním datem.  
\<date+10\> - nahradí se datem za deset dní  
\<date+30|expirace_sarze\> - nahradí se datem za 30 dní nebo datem definovaným textem za značkou '|'. Může být datum nebo klíč v souboru primárních dat. Pokud nerozpozná datum nebo nenajde klíč zobrazí program dotaz na expiraci.  
\<uzivatel\> - pokud je vyžadována identifikace uživatele, nahradí se značkou uživatele.  
\<pocet|20\> - zeptá se uživatele na počet štítků s přednastaveným množstvím 20.  

Šablona může obsahovat libovolné pole např: \<libovolné pole\>.
Pokud najde program odpověď v souboru s daty (definovaný v konfig. parametrem "data:"),
například: "libovolné pole:karfiol", dosadí místo \<libovolné pole\> slovo "karfiol".
Pokud program nenajde odpověď v souboru s daty, zeptá se uživatele ("Zadej libovolné pole: ").
Pokud šablona končí na velké "P" položí program dotaz na počet štítků.

Příklady šablon:
----------------
Šablona na tisk štítků na alikvoty kontrol:

```
N
I8,B
A110,0,0,4,1,2,N,"GLYHB 1"
A110,57,0,2,1,1,N,"lot: <GLYHB1 šarže>"
A110,82,0,2,1,1,N,"exp: <date+30>"
A315,0,1,1,1,1,N,"<date>"
A345,10,1,3,1,1,N,"<uzivatel>"
P<pocet|24>
```

Šablona na tisk doplněno:
```
N
I8,B
A146,5,0,3,1,2,N,"Doplněno:"
A146,60,0,3,1,2,N,"<date>"
P1
```

Příklad souboru s daty:
-----------------------

adresa:

C:\Users\username\Documents\StitkyNaAlikvoty\data\nastaveni QC.txt

obsah:
```
# kontroly na HbA1c
GLYHB1 šarže: 85841
GLYHB2 šarže: 85842
```
"\#" značí komentář  
":" odděluje klíč a hodnotu   

Poznámka na závěr:
------------------
Program je k dispozici včetně zdrojových kódů v naději, že bude užitečný, ale bez jakýchkoli záruk.
