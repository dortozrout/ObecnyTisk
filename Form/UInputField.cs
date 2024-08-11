using System;
using System.Linq;
using System.Collections.Generic;
using TiskStitku;
//using FormChatGPT;

namespace Form
{
    //genericka trida pro vstupni pole formulare
    class UInputField<TForm,TField> : FormItem<TForm>
    {
        //hodnota, kterou pole obsahuje
        public virtual TField Value { get; protected set; }
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
        public UInputField(string id, int leftPosition, int topPosition, string label, int length, ConsoleForm<TForm> consoleForm, string defaulText, EventHandler<MyEventArgs> inputEv, AditionalParms aditioNalParms) : this(id, leftPosition, topPosition, label, length, consoleForm, defaulText, inputEv, null, aditioNalParms) { }
        //konstruktor vcetne metod pro udalost switchToEvent
        public UInputField(string id, int leftPosition, int topPosition, string label, int length, ConsoleForm<TForm> consoleForm, string defaulText, EventHandler<MyEventArgs> inputEv, EventHandler<MyEventArgs> switchToEv, AditionalParms aditioNalParms) : base(id, leftPosition, topPosition, label, length, consoleForm)
        {
            //vychozi text
            DefaulText = defaulText;
            Text = defaulText;
            //prirazeni metod k udalosti
            inputEvent += inputEv;
            switchToEvent += switchToEv;
            //vychozi hodnoty
            IsActivable = true;
            Forward = true;
            Quit = false;
            //formular
            SuperiorForm = consoleForm;
            //data ziskana z aditionalParms
            SwitchToNext = aditioNalParms.switchToNext;
            SwitchToPrevious = aditioNalParms.switchToPrevious;
            End = aditioNalParms.end;
            EraseText = aditioNalParms.eraseText;
            HideInputText = aditioNalParms.hidenInput;
            IsActivable = aditioNalParms.isActivable;
            IsHidden = aditioNalParms.isHidden;
            CursorToStart = aditioNalParms.cursorToStart;
            //preformatovani data
            if (typeof(TField) == typeof(DateTime))
            {
                DateTime datum;
                if (DateTime.TryParse(Text, out datum))
                    Text = datum.ToString(Configuration.DateFormat);
            }
        }
        //metoda pro zobrazeni pole
        public override void Display()
        {
            if (IsHidden) return;
            //reset barev
            ResetColor();
            //nastaveni vyraznych barev pouze pro jedno zobrazeni
            if (WarningColors)
            {
                Console.BackgroundColor = warningBg;
                Console.ForegroundColor = warningFg;
                WarningColors = false;
            }
            //nastaveni pozice kurzoru
            Console.SetCursorPosition(LeftPosition, TopPosition);
            //vypis nazvu pole a textu, nebo pouze nazvu pole 
            Console.Write(Label + (HideInputText ? "".PadRight(Text.Length, '*') : Text.PadRight(Length)));
        }
        //aktivace pole pro zapis
        public virtual void Activate()
        {
            //reset Quit
            Quit=false;
            //nastaveni barev podle skladu
            SetConsoleColors();
            //nastaveni pozice kurzoru
            Console.SetCursorPosition(LeftPosition, TopPosition);
            //deklarace argumentu udalosti prirazeni default textu do OldTex
            MyEventArgs eventArgs = new MyEventArgs() { OldText = DefaulText };
            //vymazani textu pokud je to treba - pole pro skenovani kodu
            if (EraseText) Text = string.Empty;
            //vstup textu s vychozi hodnotou
            Value = ReadLine(Text);
            //formatovani data
            if (Value != null) FormatTextValue();
            //ukonceni metody po stisknuti escape
            if (Quit) return;
            //prirazeni zadaneho textu do eventArgs.NewText
            eventArgs.NewText = Text;
            //spusteni udalosti
            inputEvent?.Invoke(this, eventArgs);
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
            inputEvent?.Invoke(this, eventArgs);
        }
        //metoda prvku pro prepnuti na nej
        //parametr nastavuje zda jit dopredu nebo zpet
        public override void SwitchTo(bool forward)
        {
            Forward = forward;
            MyEventArgs myEventArgs = new MyEventArgs() { Label = this.Label, Id = this.Id };
            switchToEvent?.Invoke(this, myEventArgs);
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
            IsActivable = display;
            IsHidden = !display;
            if (display) Display();
            else Hide();
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
        protected TField ReadLine(string text)
        {
            //secteni vsech klaves ktere ukoncuji zadavani do jednoho listu
            List<ConsoleKeyInfo> keyInfos = SwitchToNext.Concat(SwitchToPrevious).ToList();
            //tohle by asi bylo lepsi nahradit tuple
            Tuple<string, ConsoleKeyInfo> stringDirection;
            //parsovani podle typu
            if (typeof(TField) == typeof(DateTime))
            {
                DateTime dateTime;
                do
                {
                    stringDirection = ReadInputWithDefault(text, keyInfos.ToArray());
                    if (stringDirection.Item2.Key == ConsoleKey.Escape) return default;
                } while (!DateTime.TryParse(stringDirection.Item1, out dateTime));
                Forward = SwitchToNext.Contains(stringDirection.Item2);
                return (TField)Convert.ChangeType(dateTime, typeof(TField));
            }
            else if (typeof(TField) == typeof(int))
            {
                //if (!int.TryParse(DefaulText, out int maxQuantity)) maxQuantity = Configuration.maxQuantity;
                int maxQuantity=Configuration.maxQuantity;
                int number;
                do
                {
                    stringDirection = ReadInputWithDefault(text, keyInfos.ToArray());
                    if (stringDirection.Item2.Key == ConsoleKey.Escape) return default;
                } while (!int.TryParse(stringDirection.Item1, out number) || number < 0 || number > maxQuantity);
                Forward = SwitchToNext.Contains(stringDirection.Item2);
                return (TField)Convert.ChangeType(number, typeof(TField));
            }
            else if (typeof(TField) == typeof(bool))
            {
                do
                {
                    stringDirection = ReadInputWithDefault(text, keyInfos.ToArray());
                    if (stringDirection.Item2.Key == ConsoleKey.Escape) return default;
                } while (!stringDirection.Item1.ToLower().StartsWith("a") && !stringDirection.Item1.ToLower().StartsWith("n"));
                Forward = SwitchToNext.Contains(stringDirection.Item2);
                return (TField)Convert.ChangeType(stringDirection.Item1.ToLower().StartsWith("n") ? false : true, typeof(TField));
            }
            else
            {
                stringDirection = ReadInputWithDefault(text, keyInfos.ToArray());
                if (stringDirection.Item2.Key == ConsoleKey.Escape) return default;
                Forward = SwitchToNext.Contains(stringDirection.Item2);
                return (TField)Convert.ChangeType(stringDirection.Item1, typeof(TField));
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
            ConsoleKeyInfo keyInfo;
            //cyklus kde se vyhodnoti akce
            do
            {
                //nacteni stisknute klavesy
                keyInfo = Console.ReadKey(true);
                HandleKeyInput(keyInfo, CursorLeftStart, ref buffer, ref insert);
                // osetreni klavesy ESC
                if (Quit) return new Tuple<string, ConsoleKeyInfo>(string.Empty, keyInfo);
            } while (!consoleKeyInfos.Contains(keyInfo));
            // while (!consoleKeyInfos.Contains(keyInfo))
            // //while (keyInfo.Key != ConsoleKey.Enter)
            // {
            //     HandleKeyInput(keyInfo, CursorLeftStart, ref buffer, ref insert);
            //     // osetreni klavesy ESC
            //     if (Quit) return new Tuple<string, ConsoleKeyInfo>(string.Empty, keyInfo);
            //     //nacteni dalsi klavesy
            //     keyInfo = Console.ReadKey(true);
            // }
            return new Tuple<string, ConsoleKeyInfo>(new string(buffer.ToArray()), keyInfo);
        }
        private void HandleKeyInput(ConsoleKeyInfo keyInfo, int cursorLeftStart, ref List<char> buffer, ref bool insert)
        {
            switch (keyInfo.Key)
            {
                case ConsoleKey.LeftArrow:
                    MoveCursorLeft(cursorLeftStart);
                    break;
                case ConsoleKey.RightArrow:
                    MoveCursorRight(cursorLeftStart, buffer);
                    break;
                case ConsoleKey.Home:
                    Console.SetCursorPosition(cursorLeftStart, Console.CursorTop);
                    break;
                case ConsoleKey.End:
                    Console.SetCursorPosition(cursorLeftStart + buffer.Count, Console.CursorTop);
                    break;
                case ConsoleKey.Insert:
                    insert = !insert;
                    break;
                case ConsoleKey.Backspace:
                    HandleBackspace(cursorLeftStart, ref buffer);
                    break;
                case ConsoleKey.Delete:
                    HandleDelete(cursorLeftStart, ref buffer);
                    break;
                case ConsoleKey.Escape:
                    Quit = true;
                    break;
                case ConsoleKey.Enter:
                    return;
                default:
                    HandleCharacterInput(keyInfo, cursorLeftStart, ref buffer, ref insert);
                    break;
            }
        }
        private void HandleCharacterInput(ConsoleKeyInfo keyInfo, int cursorLeftStart, ref List<char> buffer, ref bool insert)
        {
            var character = keyInfo.KeyChar;
            if (character < 29 || (character > 29 && character < 32) || buffer.Count >= Length - 1) return;

            int index = Console.CursorLeft - cursorLeftStart;

            if (insert)
            {
                buffer.Insert(index, keyInfo.KeyChar);
                RewriteLine(cursorLeftStart, buffer);
                Console.SetCursorPosition(cursorLeftStart + index + 1, Console.CursorTop);
            }
            else if (index < buffer.Count)
            {
                buffer[index] = keyInfo.KeyChar;
                RewriteLine(cursorLeftStart, buffer);
                Console.SetCursorPosition(cursorLeftStart + index + 1, Console.CursorTop);
            }
            else
            {
                buffer.Add(keyInfo.KeyChar);
                RewriteLine(cursorLeftStart, buffer);
                Console.SetCursorPosition(cursorLeftStart + index + 1, Console.CursorTop);
            }
        }

        private void HandleDelete(int cursorLeftStart, ref List<char> buffer)
        {
            int index = Console.CursorLeft - cursorLeftStart;
            if (index >= buffer.Count) return;

            buffer.RemoveAt(index);
            RewriteLine(cursorLeftStart, buffer);
            Console.SetCursorPosition(cursorLeftStart + index, Console.CursorTop);
        }

        private void HandleBackspace(int cursorLeftStart, ref List<char> buffer)
        {
            int index = Console.CursorLeft - cursorLeftStart;
            if (index <= 0) return;

            buffer.RemoveAt(index - 1);
            Console.CursorLeft--;
            RewriteLine(cursorLeftStart, buffer);
            Console.SetCursorPosition(cursorLeftStart + index - 1, Console.CursorTop);
        }

        private void MoveCursorRight(int cursorLeftStart, List<char> buffer)
        {
            if (Console.CursorLeft < cursorLeftStart + buffer.Count)
                Console.CursorLeft++;
        }

        private void MoveCursorLeft(int cursorLeftStart)
        {
            if (Console.CursorLeft > cursorLeftStart)
                Console.CursorLeft--;
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
        private void SetConsoleColors()
        {
            //nastaveni barev podle skladu
            Console.BackgroundColor = Configuration.ActiveBackgroundColor;
            Console.ForegroundColor = Configuration.ActiveForegroundColor;
        }
        private void FormatTextValue()
        {
            //formatovani data
            if (Value.GetType() == typeof(DateTime))
            {
                if (DateTime.TryParse(Value.ToString(), out DateTime date))
                    //Text = ((DateTime)(object)Value).ToString(Configuration.DateFormat);
                    Text = date.ToString(Configuration.DateFormat);
            }
            else if (Value.GetType() == typeof(bool))
            {
                if (bool.TryParse(Value.ToString(), out bool yes))
                    if (yes) Text = "ANO";
                    else
                    {
                        Text = "NE";
                        //kvuli ukonceni pri neshodnem kat cisle
                        if (End)
                        {
                            Quit = true;
                        }
                    }
            }
            else Text = Value.ToString();
        }
    }
}