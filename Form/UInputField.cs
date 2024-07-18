using System;
using System.Linq;
using System.Collections.Generic;
using CCData;

namespace Form
{
    //genericka trida pro vstupni pole formulare
    class UInputField<T> : FormItem
    {
        //hodnota, kterou pole obsahuje
        public virtual T Value { get; protected set; }
        //jestli se presunout dopredu, nebo dozadu
        public bool Forward { get; set; }
        //jestli je pole zapisovatelne 
        public bool IsActivable { get; set; }
        //skryte pole
        public bool IsHidden { get; set; }
        //slouzi pro ukonceni po zadani esc kdekoli
        public bool Quit { get; protected set; }
        //jestli po zadani enter ukoncit vyplnovani formulare
        public bool End { get; protected set; }
        //jestli po prepnuti na pole vymazat ulozenou hodnotu
        public bool EraseText { get; private set; }
        //udalost ktera se spusti po zadani
        public event EventHandler<MyEventArgs> inputEvent;
        //udalost, ktera se spusti po prepnuti na pole
        public event EventHandler<MyEventArgs> switchToEvent;
        //vychozi text
        public string DefaulText { get; protected set; }
        //list klaves+modifikatoru, ktere zpusobi prechod na nasledujici pole
        public List<ConsoleKeyInfo> SwitchToNext { get; protected set; }
        //list klaves+modifikatoru, ktere zpusobi prechod na predchozi pole
        public List<ConsoleKeyInfo> SwitchToPrevious { get; protected set; }
        //zda skryt zadavany text - heslo
        public bool HideInputText { get; protected set; }
        //zda pouzit vyrazne barvy
        public bool WarningColors { get; set; }
        //zda nastavit kurzor na start nebo na konec textu
        public bool CursorToStart { get; set; }
        //konstruktor bez metod pro udalost switchToEvent
        public UInputField(string id, int leftPosition, int topPosition, string label, int length, ConsoleForm consoleForm, string defaulText, EventHandler<MyEventArgs> inputEv, AditionalParms aditioNalParms) : base(id, leftPosition, topPosition, label, length, consoleForm)
        {
            //identifikace pole
            Id = id;
            //pozice, nazev a delka pole
            LeftPosition = leftPosition;
            TopPosition = topPosition;
            Label = label;
            Length = length;
            //vychozi hodnoty
            IsActivable = true;
            Forward = true;
            Quit = false;
            //formular
            SuperiorForm = consoleForm;
            //vychozi text
            DefaulText = defaulText;
            Text = defaulText;
            //preformatovani data
            if (typeof(T) == typeof(DateTime))
            {
                DateTime datum;
                if (DateTime.TryParse(Text, out datum))
                    Text = datum.ToString(Konfigurace.Format);
            }
            //prirazeni metod k udalosti
            inputEvent += inputEv;
            //data ziskana z aditionalParms
            SwitchToNext = aditioNalParms.switchToNext;
            SwitchToPrevious = aditioNalParms.switchToPrevious;
            End = aditioNalParms.end;
            EraseText = aditioNalParms.eraseText;
            HideInputText = aditioNalParms.hidenInput;
            IsActivable = aditioNalParms.isActivable;
            IsHidden = aditioNalParms.isHidden;
            CursorToStart = aditioNalParms.cursorToStart;
        }
        //konstruktor vcetne metod pro udalost switchToEvent
        public UInputField(string id, int leftPosition, int topPosition, string label, int length, ConsoleForm consoleForm, string defaulText, EventHandler<MyEventArgs> inputEv, EventHandler<MyEventArgs> switchToEv, AditionalParms aditioNalParms) : base(id, leftPosition, topPosition, label, length, consoleForm)
        {
            //indentifikace
            Id = id;
            //pozice, nazev a delka pole
            LeftPosition = leftPosition;
            TopPosition = topPosition;
            Label = label;
            Length = length;
            //vychozi hodnoty
            IsActivable = true;
            Forward = true;
            Quit = false;
            //formular
            SuperiorForm = consoleForm;
            //vychozi text
            DefaulText = defaulText;
            Text = defaulText;
            //preformatovani data
            if (typeof(T) == typeof(DateTime))
            {
                DateTime datum;
                if (DateTime.TryParse(Text, out datum))
                    Text = datum.ToString(Konfigurace.Format);
            }
            //prirazeni metod k udalosti
            inputEvent += inputEv;
            switchToEvent += switchToEv;
            //data ziskana z aditionalParms
            SwitchToNext = aditioNalParms.switchToNext;
            SwitchToPrevious = aditioNalParms.switchToPrevious;
            End = aditioNalParms.end;
            EraseText = aditioNalParms.eraseText;
            HideInputText = aditioNalParms.hidenInput;
            IsActivable = aditioNalParms.isActivable;
            IsHidden = aditioNalParms.isHidden;
            CursorToStart = aditioNalParms.cursorToStart;
        }
        //metoda pro zobrazeni pole
        public override void Display()
        {
            if (IsHidden) return;
            //reset barev
            Console.ResetColor();
            //nastaveni vyraznych barev pouze pro jedno zobrazeni
            if (WarningColors)
            {
                Console.BackgroundColor = warningBg;
                Console.ForegroundColor = warningFg;
                WarningColors = false;
            }
            //nastaveni pozice kurzoru
            Console.SetCursorPosition(LeftPosition, TopPosition);
            //vypis nazvu pole a textu
            if (!HideInputText) Console.Write(Label + Text.PadRight(Length));
            //nebo pouze nazvu pole 
            else Console.Write(Label + "".PadRight(Text.Length, '*').PadRight(Length));
        }
        //aktivace pole pro zapis
        public virtual void Activate()
        {
            //nastaveni barev podle skladu
            Console.BackgroundColor = Konfigurace.BgColor;
            Console.ForegroundColor = Konfigurace.FgColor;
            //nastaveni pozice kurzoru
            Console.SetCursorPosition(LeftPosition, TopPosition);
            //deklarace argumentu udalosti prirazeni default textu do OldTex
            MyEventArgs eventArgs = new MyEventArgs() { OldText = DefaulText };
            //vymazani textu pokud je to treba - pole pro skenovani kodu
            if (EraseText) Text = string.Empty;
            //vstup textu s vychozi hodnotou
            Value = ReadLine(Text);
            //ukonceni metody po stisknuti escape
            if (Quit) return;
            //formatovani data
            if (Value.GetType() == typeof(DateTime))
                //Text = string.Format("{0:yyyy-MM-dd}", Value);
                Text = ((DateTime)(object)Value).ToString(Konfigurace.Format);
            else if (Value.GetType() == typeof(bool))
            {
                bool yes = (bool)Convert.ChangeType(Value, typeof(bool));
                if (yes) Text = "ANO";
                else
                {
                    Text = "NE";
                    //kvuli ukonceni pri neshodnem kat cisle
                    if (End)
                    {
                        Quit = true;
                        return;
                    }
                }
            }
            else Text = Value.ToString();
            //prirazeni zadaneho textu do eventArgs.NewText
            eventArgs.NewText = Text;
            //spusteni udalosti
            if (inputEvent != null) inputEvent(this, eventArgs);
        }
        //metoda pro pruchod polem bez aktivace
        public virtual void NonActivate()
        {
            //nastaveni argumentu udalosti
            MyEventArgs eventArgs = new MyEventArgs()
            {
                OldText = DefaulText,
                //text se mohl zmenit prirazenim zvenku
                NewText = Text,
            };
            //spusteni udalosti
            if (inputEvent != null) inputEvent(this, eventArgs);
        }
        //metoda prvku pro prepnuti na nej
        //parametr nastavuje zda jit dopredu nebo zpet
        public override void SwitchTo(bool forward)
        {
            Forward = forward;
            MyEventArgs myEventArgs = new MyEventArgs() { Label = this.Label, Id = this.Id };
            if (switchToEvent != null) switchToEvent(this, myEventArgs);
            //pokud je prvek zapisovatelny|aktivovatelny
            if (IsActivable)
            {
                Activate();
                //ukonceni metody po stisknuti escape
                if (Quit)
                {
                    if (SuperiorForm != null) SuperiorForm.Quit = true;
                    return;
                }
                //metoda inputTextWithDefault kterou vola Activate()
                //nastavi promenou Forwad podle stisknute klavesy
            }
            //kdyz prvek neni aktivovatelny
            else NonActivate();
            //zobrazeni prvku v stadnardnich barvach
            Display();
            //prechod na dalsi|predchozi|aktualni pole, nebo konec
            if (Forward) //pohyb vpred v posloupnosti prvku
            {
                //osetreni prazdneho dalsiho prvku
                if (Next != null)
                {
                    //prepnuti na dalsi prvek
                    Next.SwitchTo(Forward);
                }
                //prazdny prvek + End ukonci pohyb mezi vstupnimi poli
                else if (End) return;
                //prazdny prvek zpusobi prepnuti na aktualni pole s opacnym argumentem forward
                else SwitchTo(!Forward);
            }
            else //pohyb vzad
            {
                //analogicky k predchozimu akorat bez End
                if (Previous != null)
                {
                    Previous.SwitchTo(Forward);
                }
                else SwitchTo(!Forward);
            }
        }
        //metoda pro udalost, ktera prepina zapisovatelnos pole
        public override void ToggleVisible(bool display)
        {
            if (display)
            {
                IsActivable = true;
                IsHidden = false;
                Display();
            }
            else
            {
                IsActivable = false;
                IsHidden = true;
                Hide();
            }
        }
        public virtual void Hide()
        {
            Console.BackgroundColor = defaultBgColor;
            Console.ForegroundColor = defaultFgColor;
            Console.SetCursorPosition(LeftPosition, TopPosition);
            Console.Write("".PadRight(Label.Length + Length));
        }
        //metoda pro vstup s vychozi hodnotou
        //https://stackoverflow.com/questions/1655318/how-to-set-default-input-value-in-net-console-app
        //Matt Breckon, Efrain
        protected T ReadLine(string text)
        {
            //secteni vsech klaves ktere ukoncuji zadavani do jednoho listu
            List<ConsoleKeyInfo> keyInfos = SwitchToNext.Concat(SwitchToPrevious).ToList<ConsoleKeyInfo>();
            //tohle by asi bylo lepsi nahradit tuple
            Tuple<string, ConsoleKeyInfo> stringDirection;
            //parsovani podle typu
            if (typeof(T) == typeof(DateTime)) //DateTime
            {
                DateTime dateTime;
                do
                {
                    stringDirection = ReadInputWithDefault(text, keyInfos.ToArray());
                    if (stringDirection.Item2.Key == ConsoleKey.Escape) return default(T);
                } while (!DateTime.TryParse(stringDirection.Item1, out dateTime));
                //nastaveni zda se bude pokracovat na dalsi nebo na predchozi pole
                if (SwitchToNext.Contains(stringDirection.Item2)) Forward = true;
                else Forward = false;
                return (T)Convert.ChangeType(dateTime, typeof(T));
            }
            else if (typeof(T) == typeof(int)) //int
            {
                int maxQuantity = int.Parse(DefaulText);
                int number;
                do
                {
                    stringDirection = ReadInputWithDefault(text, keyInfos.ToArray());
                    if (stringDirection.Item2.Key == ConsoleKey.Escape) return default(T);
                } while (!int.TryParse(stringDirection.Item1, out number) || number < 0 || number > maxQuantity);
                if (SwitchToNext.Contains(stringDirection.Item2)) Forward = true;
                else Forward = false;
                return (T)Convert.ChangeType(number, typeof(T));
            }
            else if (typeof(T) == typeof(bool)) //ano,ne
            {
                do
                {
                    stringDirection = ReadInputWithDefault(text, keyInfos.ToArray());
                    if (stringDirection.Item2.Key == ConsoleKey.Escape) return default(T);
                } while (!stringDirection.Item1.ToLower().StartsWith("a") && !stringDirection.Item1.ToLower().StartsWith("n"));
                if (SwitchToNext.Contains(stringDirection.Item2)) Forward = true;
                else Forward = false;
                if (stringDirection.Item1.ToLower().StartsWith("n")) return (T)Convert.ChangeType(false, typeof(T));
                return (T)Convert.ChangeType(true, typeof(T));
            }
            else
            {
                stringDirection = ReadInputWithDefault(text, keyInfos.ToArray());
                if (stringDirection.Item2.Key == ConsoleKey.Escape) return default(T);
                if (SwitchToNext.Contains(stringDirection.Item2)) Forward = true;
                else Forward = false;
                return (T)Convert.ChangeType(stringDirection.Item1, typeof(T));
            }
        }
        protected Tuple<string, ConsoleKeyInfo> ReadInputWithDefault(string defaultValue, ConsoleKeyInfo[] consoleKeyInfos)
        {
            //startovni pozice zapisu po nazvu
            int CursorLeftStart = LeftPosition + Label.Length;
            //nacteni default textu do listu omezeneho na sirku pole
            List<char> buffer = defaultValue.ToCharArray().Take(Length - 1).ToList();
            //vypis nazvu pole
            Console.CursorLeft = LeftPosition;
            Console.Write(Label);
            //vypis defaultni hodnoty
            RewriteLine(CursorLeftStart, buffer);
            //kurzor na zacatek|konec textu
            if (CursorToStart) Console.SetCursorPosition(CursorLeftStart, Console.CursorTop);
            else Console.SetCursorPosition(CursorLeftStart + defaultValue.Length, Console.CursorTop);
            //nastaveni vychoziho prepisovani textu
            bool insert = false;
            //nacteni stisknute klavesy
            ConsoleKeyInfo keyInfo = Console.ReadKey(true);
            //cyklus kde se vyhodnoti akce
            while (!consoleKeyInfos.Contains(keyInfo))
            //while (keyInfo.Key != ConsoleKey.Enter)
            {
                switch (keyInfo.Key)
                {
                    //sipka doleva
                    case ConsoleKey.LeftArrow:
                        Console.SetCursorPosition(Math.Max(Console.CursorLeft - 1, CursorLeftStart), Console.CursorTop);
                        break;
                    //sipka doprava
                    case ConsoleKey.RightArrow:
                        Console.SetCursorPosition(Math.Min(Console.CursorLeft + 1, CursorLeftStart + buffer.Count), Console.CursorTop);
                        break;
                    //na zacatek textu
                    case ConsoleKey.Home:
                        Console.SetCursorPosition(CursorLeftStart, Console.CursorTop);
                        break;
                    //na konec textu
                    case ConsoleKey.End:
                        Console.SetCursorPosition(CursorLeftStart + buffer.Count, Console.CursorTop);
                        break;
                    //prepnuti insert
                    case ConsoleKey.Insert:
                        insert = !insert;
                        break;
                    //mazani backspace
                    case ConsoleKey.Backspace:
                        if (Console.CursorLeft <= CursorLeftStart)
                        {
                            break;
                        }
                        var cursorColumnAfterBackspace = Math.Max(Console.CursorLeft - 1, CursorLeftStart);
                        buffer.RemoveAt(Console.CursorLeft - CursorLeftStart - 1);
                        RewriteLine(CursorLeftStart, buffer);
                        Console.SetCursorPosition(cursorColumnAfterBackspace, Console.CursorTop);
                        break;
                    //mazani delete
                    case ConsoleKey.Delete:
                        if (Console.CursorLeft >= buffer.Count + CursorLeftStart)
                        {
                            break;
                        }
                        var cursorColumnAfterDelete = Console.CursorLeft;
                        buffer.RemoveAt(Console.CursorLeft - CursorLeftStart);
                        RewriteLine(CursorLeftStart, buffer);
                        Console.SetCursorPosition(cursorColumnAfterDelete, Console.CursorTop);
                        break;
                    //preruseni zadavani ESC
                    case ConsoleKey.Escape:
                        Quit = true;
                        return new Tuple<string, ConsoleKeyInfo>(string.Empty, keyInfo);
                    //zapis textu
                    default:
                        var character = keyInfo.KeyChar;
                        if (character < 29 || (character > 29 && character < 32)) // not a printable chars
                            break;
                        var cursorAfterNewChar = Console.CursorLeft + 1;
                        if (cursorAfterNewChar > Length + CursorLeftStart || buffer.Count >= Length - 1)
                        {
                            break; // currently only one line of input is supported
                        }
                        if (Console.CursorLeft - CursorLeftStart < buffer.Count && !insert)
                            buffer.RemoveAt(Console.CursorLeft - CursorLeftStart);
                        buffer.Insert(Console.CursorLeft - CursorLeftStart, character);
                        RewriteLine(CursorLeftStart, buffer);
                        Console.SetCursorPosition(cursorAfterNewChar, Console.CursorTop);
                        break;
                }
                //nacteni dalsi klavesy
                keyInfo = Console.ReadKey(true);
            }
            return new Tuple<string, ConsoleKeyInfo>(new string(buffer.ToArray()), keyInfo);
        }
        //metoda pro prepsani radku pole akt. stavu
        protected void RewriteLine(int startPosition, List<char> buffer)
        {
            //nastaveni startovni pozice
            Console.SetCursorPosition(startPosition, Console.CursorTop);
            //prekryti predchoziho textu
            Console.Write(new string(' ', Length - 1));
            //nastaveni kurzoru zpet
            Console.SetCursorPosition(startPosition, Console.CursorTop);
            //zapis aktualniho textu
            if (!HideInputText) Console.Write(buffer.ToArray());
            else Console.Write("".PadRight(buffer.Count, '*'));
            //nastaveni kurzoru
            Console.SetCursorPosition(startPosition, Console.CursorTop);
        }
    }
}