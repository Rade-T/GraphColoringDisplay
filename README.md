# GraphColoringDisplay
Projekat urađen za vreme prakse u kompaniji Schneider Electric DMS
Ovaj program sadrži tri različita algoritma za bojenje neusmerenih grafova

Pregled programa
Program je razvijen u programskom jeziku C# koristeći WPF razvojni okvir. Realizovan je kao grafička desktop aplikacija koja se izvršava na korisnikovom računaru. Kada se program pokrene prikazuje se glavni prozor aplikacije koji se sastoji od trake menija, statusne trake i glavnog dela prozora na kom se grafovi iscrtavaju. Traka menija nudi opcije fajla i opcije algoritama. U opcijama fajla možemo da generišemo novi graf, da učitamo postojeći graf ili da sačuvamo trenutni graf. Grafovi se čuvaju u .txt formatu. U opcijama algoritma možemo da biramo kojim bojama da obojimo graf. Program nudi tri različita algoritma za bojenje grafova. To su Greedy, Genetic i Backtracking algoritmi. Te tri opcije se nalaze pod opcijama algoritma.

Greedy algoritam je takozvani naivan algoritam. On ne garantuje minimalni broj boja ali nikad neće koristiti više boja nego što ima čvorova u grafu. On funkcioniše na sledeći način:
Oboji prvi čvor jednom bojom. Oboji ostale čvorove istom bojom. Pogledaj trenutno odabrani čvor i oboji ga prvom bojom iz niza boja koja još nije iskorišćena u susednim čvorovima. Ako su sve dosadašnje boje korištene u susednim čvorovima iskoristi novu boju.

Genetski algoritam je baziran na populaciji i može da koristi mutiranje kandidata za rešenje. Prvo se generiše populacija rešenja koja se nazivaju hromozomi. Svaki kandidat je graf sa nasumično odabranim bojama čvorova. Svaka generacija potomaka hromozoma se bira turnirskom selekcijom. U svakom turniru učestvuju dva hromozoma. Potomak može da mutira i hromozom sa najvećim brojem kršenja ograničenja fitnes funkcije se izbacuje. Ako se broj rešenja smanji na lokalni minimum pokušava se restart. Restart znači povećanje broja populacije za dva. Početni broj populacije je broj čvorova u grafu. Konstanta verovatnoća mutacije je 25%, a maksimalna veličina generacije je 100 * broj čvorova.

Kod backtracking algoritma se boje dodeljuju jedna po jedna različitim čvorovima. Početak je prvi čvor grafa. Pre nego što dodelimo boju proveravamo da li možemo da je dodelimo gledajući susedne boje čvorova koje smo već dodelili. Ako vidimo da možemo da je dodelimo označavamo da je to dodavanje bezbedno i dodamo ga u rešenje. Ako nismo našli boju onda se vraćamo označavamo boju da nije deo rešenja.
