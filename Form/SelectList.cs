using System;
using System.Collections.Generic;
using System.Drawing;
using System.Xml;
using System.Linq;
using Labels;
using System.Text;
using System.Globalization;

namespace Form
{
    //genericka trida pro vyber polozky z listu pomoci 
    //kurzorovych sipek a enter
    class SelectFromList<T> : ConsoleForm<string> where T : IComparable
    {
        private List<T> primaryItems;
        private List<T> items = new List<T>(); //interni list polozek
        private bool isRecusive = false;
        private string FilterInfo { get; set; }
        private int visibleRows; //pocet viditelnych radku seznamu
        private readonly int displayHeight;
        private readonly int xStartPos; //odkus se zacina vykreslovat
        private readonly int yStartPos;
        private readonly int pageDU; //o kolik pozic posouva pgdown, pgup
        private int lastCursorPos = -1;
        private bool saveCursosPosition = true; //zda ukladat posledni pozici
        private bool saveFilter = true;
        // private readonly FieldReadOnly header;
        private readonly FieldReadOnly<string> help;
        private readonly UInputField<string, string> filterInput;
        //konstruktory
        public SelectFromList()
        {
            displayHeight = Configuration.MaxLines;
            visibleRows = displayHeight;
            xStartPos = 0;
            yStartPos = 5;
            pageDU = displayHeight;
            //string text = string.Format("Výběr \"ENTER\", návrat \"ESC\", pohyb \"\u2191, \u2193, pgUp, pgDown, Home, End\"\n     ukládání filtru: {0} přepnout: \"BackSpace\".", saveFilter == true ? "ON" : "OFF");
            string text = string.Format("Výběr \"ENTER\", návrat \"ESC\", pohyb \"\u2191, \u2193, pgUp, pgDown, Home, End\"\nPro editaci nastavení zadej: {0}", Configuration.Editace);
            help = new FieldReadOnly<string>(5, displayHeight + yStartPos + 2, text, Console.WindowWidth - 5, null);
            filterInput = new UInputField<string, string>("filter", xStartPos + 5, displayHeight + yStartPos + 4, "Filtr: ", 40, null, "", null, new AditionalParms()
            {
                cursorToStart = false,
                end = true,
                switchToPrevious = new List<ConsoleKeyInfo>(),
                switchToNext = new List<ConsoleKeyInfo>() { new ConsoleKeyInfo('\r', ConsoleKey.Enter, false, false, false) }
            });
            // header = new FieldReadOnly(0, 4, "".PadRight(Polozka.col1)
            //     + "Název".PadRight(Polozka.col2)
            //     + "Katalogové číslo".PadRight(Polozka.col3)
            //     + "     Počet ks.".PadLeft(Polozka.col4), Console.WindowWidth, null);
        }

        //metoda ktera vraci vybranou polozku z listu
        public List<T> Select(List<T> values)
        {
            //reset konzole
            ResetConsole();

            //osetreni nuloveho a prazdneho vstupu
            if (values == null || values.Count == 0)
            {
                if (isRecusive)
                {
                    ShowEmptySelectionMessage();
                    isRecusive = false;
                    FilterInfo = "";
                    return Select(primaryItems);
                }
                ShowEmptySelectionMessage();
                return default;
            }

            //setrideni
            values.Sort(); //polozky musi podporovat IComparable rozhrani

            //ulozeni primarniho listu - pred rekurzivnim volanim
            if (!isRecusive)
            {
                primaryItems = new List<T>(values);
            }

            //aplikovani ulozeneho filtru na nove volani fce - ne rekurzivni
            if (saveFilter && !string.IsNullOrEmpty(FilterInfo) && !isRecusive)
            {
                items = LoadFilter(values, FilterInfo);
                if (items.Count == 0)
                {
                    FilterInfo = string.Empty;
                    items = primaryItems;
                    ResetConsole();
                }
            }
            else items = values;

            //reset visibleRows
            visibleRows = Math.Min(displayHeight, items.Count);

            //nastaveni pozice kurzoru
            int currentPosition = 0;
            if (saveCursosPosition && lastCursorPos != -1)
            {
                currentPosition = lastCursorPos < items.Count ? lastCursorPos : lastCursorPos - 1;
                if (currentPosition >= items.Count) currentPosition = 0; //pojistka
                Display(visibleRows, currentPosition);
                lastCursorPos = -1; // Reset pozice, aby se nepoužila znovu
            }
            else
            {
                //zobrazeni zacatku seznamu
                Display(visibleRows, 0);
            }
            return NavigateSelection(currentPosition);
        }
        private List<T> NavigateSelection(int currentPosition)
        {
            ConsoleKeyInfo keyPress;
            //int poziceVListu = 0;
            T selectedItem = default(T);
            //cyklus s vyberem
            do
            {
                //nacteni stisknute klavesy do promenne + definice switche
                switch ((keyPress = Console.ReadKey(true)).Key)
                {
                    case ConsoleKey.DownArrow: //sipka dolu - vpred
                        MoveDown(ref currentPosition);
                        break;
                    case ConsoleKey.UpArrow: //sipka nahoru - zpet
                        MoveUp(ref currentPosition);
                        break;
                    case ConsoleKey.PageDown: //vpred o vice radku
                        MovePageDown(ref currentPosition);
                        break;
                    case ConsoleKey.PageUp: //zpet o vice radku
                        MovePageUp(ref currentPosition);
                        break;
                    case ConsoleKey.Home: //na 1 pozici
                        MoveHome(ref currentPosition);
                        break;
                    case ConsoleKey.End: //na pposledni pozici
                        MoveEnd(ref currentPosition);
                        break;
                    case ConsoleKey.Backspace:
                        saveFilterToggle();
                        break;
                    case ConsoleKey.Insert:
                        saveCursosPositionToggle();
                        break;
                    case ConsoleKey.Enter: //vyber zvyraznene polozky
                        selectedItem = SelectCurrentItem(ref currentPosition);
                        break;
                    case ConsoleKey.Escape: //ukonceni bez vyberu
                        return Quit();
                    default: //zapis a vyhodnoceni filtru
                        if (keyPress.KeyChar < 32) break; //pomineme ostatni ridici znaky
                        return HandleFilterInput(keyPress, currentPosition);
                }
            } while (keyPress.Key != ConsoleKey.Enter); //ceka se ne stisknuti ENTER
            return new List<T>() { selectedItem }; ;
        }
        private void MoveDown(ref int currentPosition)
        {
            if (currentPosition + 1 < items.Count)
            {
                currentPosition++;
                Display(visibleRows, currentPosition);
            }
        }
        private void MoveUp(ref int currentPosition)
        {
            if (currentPosition > 0)
            {
                currentPosition--;
                Display(visibleRows, currentPosition);
            }
        }

        private void MovePageDown(ref int currentPosition)
        {
            if (currentPosition + pageDU < items.Count)
            {
                currentPosition += pageDU;
                Display(visibleRows, currentPosition);
            }
            else
            {
                MoveEnd(ref currentPosition);
            }
        }

        private void MovePageUp(ref int currentPosition)
        {
            if (currentPosition - pageDU >= 0)
            {
                currentPosition -= pageDU;
                Display(visibleRows, currentPosition);
            }
            else
            {
                MoveHome(ref currentPosition);
            }
        }

        private void MoveHome(ref int currentPosition)
        {
            currentPosition = 0;
            Display(visibleRows, currentPosition);
        }

        private void MoveEnd(ref int currentPosition)
        {
            currentPosition = items.Count - 1;
            Display(visibleRows, currentPosition);
        }

        private T SelectCurrentItem(ref int currentPosition)
        {
            if (items.Count == 0) return default(T);
            T selectedItem = items[currentPosition];
            if (saveFilter) lastCursorPos = currentPosition;
            else lastCursorPos = primaryItems.FindIndex(p => p.Equals(selectedItem));
            isRecusive = false;
            return selectedItem;
        }
        private void saveFilterToggle()
        {
            saveFilter = !saveFilter;
            //help.Label = string.Format("Výběr \"ENTER\", návrat \"ESC\", pohyb \"\u2191, \u2193, pgUp, pgDown, Home, End\"\n     ukládání filtru: {0} přepnout: \"BackSpace\".", saveFilter == true ? "ON" : "OFF");
            //help.Display();
        }
        private void saveCursosPositionToggle()
        {
            saveCursosPosition = !saveCursosPosition;
        }
        private new List<T> Quit()
        {
            if (isRecusive || (saveFilter && !string.IsNullOrEmpty(FilterInfo)))
            {
                isRecusive = false;
                FilterInfo = string.Empty;
                return Select(primaryItems);
            }
            return default;
        }
        private List<T> HandleFilterInput(ConsoleKeyInfo keyPress, int currentPosition)
        {
            //zviditelneni kurzoru
            Console.CursorVisible = true;
            //vlozeni stisknute klavesy do textu pole filterInput
            filterInput.Text = keyPress.KeyChar.ToString();
            //prepnuti na vstupni pole filterInput
            //pole se opusti po zadani Enter|Esc
            filterInput.SwitchTo(true);
            //deklarace promene filtru
            string filter;
            //nastaveni filtru + osetreni esc
            if (filterInput.Quit == true) filter = "";
            else filter = filterInput.Text;
            filterInput.Text = "";
            if (filter == Configuration.Editace)
            {
                new Manager().Interface();
            }
            //odchytneme ciselne zadani
            int startOfTable = currentPosition - (currentPosition % visibleRows);
            if (TrySelectItems(filter, startOfTable, items, out List<T> selectedItems1))
            {
                if (saveFilter) lastCursorPos = currentPosition;
                else lastCursorPos = primaryItems.FindIndex(p => p.Equals(selectedItems1[0]));
                isRecusive = false;
                return selectedItems1;
            }

            // int inputNumber;
            // int startOfTable = currentPosition - (currentPosition % visibleRows);
            // if (int.TryParse(filter, out inputNumber))
            // {
            //     if (inputNumber > 0 && inputNumber <= visibleRows && startOfTable + inputNumber <= items.Count)
            //     {
            //         int positionOfItem = startOfTable + inputNumber - 1;
            //         return SelectCurrentItem(ref positionOfItem);
            //     }
            // }

            //aplikujeme filtr na polozky
            List<T> fileredItems = FilterItems(items, filter);
            //pro osetreni zadaneho navratu z rekurze
            isRecusive = true;
            //zobrazeni aplikovaneho filtru
            FilterInfo += "+" + filter;
            FilterInfo = string.Format("{0}", FilterInfo.Trim('+'));
            //vyber z vyfiltrovanych polozek
            List<T> selectedItems = Select(fileredItems);
            return selectedItems;
        }
        //pomocna metoda pro vypis radku seznamu
        private void Display(int visibleRows, int currentPosition)
        {
            int positionInTable = currentPosition % visibleRows;
            int startOfTable = currentPosition - positionInTable;
            //reset barev konzole
            ResetColor();
            //vypis radku podle visible rows
            for (int i = 0; i < visibleRows; i++)
            {
                Console.SetCursorPosition(xStartPos, yStartPos + 1 + i);
                //pokud nejsme mimo seznam vypiseme polozku seznamu
                if (i + startOfTable < items.Count)
                    //Console.Write((string.Empty.PadRight(Polozka.col1) + itemList[i + start]).PadRight(Console.WindowWidth));
                    Console.Write((string.Format("{0,3}.", i + 1).PadRight(Polozka.col1) + items[i + startOfTable]).PadRight(Console.WindowWidth));
                //jinak se vypisi prazdne radky
                else
                    Console.Write(string.Empty.PadRight(Console.WindowWidth));
            }
            //zvyrazneni aktivniho radku
            Console.SetCursorPosition(xStartPos, yStartPos + positionInTable + 1);
            //Console.BackgroundColor = ConsoleColor.DarkYellow; //hodit do konfigurace
            Console.BackgroundColor = Configuration.ActiveBackgroundColor;
            Console.ForegroundColor = Configuration.ActiveForegroundColor;
            //vypsani zvyrazneneho radku
            if (items.Count != 0)
                //Console.Write((string.Empty.PadRight(Polozka.col1) + itemList[poziceVListu]).PadRight(Console.WindowWidth));
                Console.Write((string.Format("{0,3}.", positionInTable + 1).PadRight(Polozka.col1) + items[currentPosition]).PadRight(Console.WindowWidth));
            ResetColor();
        }
        //https://stackoverflow.com/questions/249087/how-do-i-remove-diacritics-accents-from-a-string-in-net
        static string RemoveDiacritics(string text)
        {
            var normalizedString = text.Normalize(NormalizationForm.FormD);
            var stringBuilder = new StringBuilder(capacity: normalizedString.Length);

            for (int i = 0; i < normalizedString.Length; i++)
            {
                char c = normalizedString[i];
                var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(c);
                }
            }
            return stringBuilder
                .ToString()
                .Normalize(NormalizationForm.FormC);
        }
        public override void Display()
        {
            throw new NotImplementedException();
        }

        private List<T> FilterItems(List<T> items, string filter)
        {
            filter = RemoveDiacritics(filter.ToLower());
            List<T> selectedItems = new List<T>();

            if (typeof(T) == typeof(Polozka))
            {
                selectedItems = items.FindAll(p => RemoveDiacritics((p as Polozka).Jmeno.ToLower()).Contains(filter) || (p as Polozka).KatCislo.ToLower().Contains(filter));
            }
            else if (typeof(T) == typeof(string))
            {
                selectedItems = items.FindAll(p => RemoveDiacritics((p as string).ToLower()).Contains(filter));
            }
            else
            {
                selectedItems = items.FindAll(p => p.ToString().ToLower().Contains(filter));
            }

            return selectedItems;
        }
        private List<T> LoadFilter(List<T> items, string filter)
        {
            List<T> selectedItems = new List<T>(items);
            foreach (string f in filter.Split('+'))
            {
                selectedItems = FilterItems(selectedItems, f);
            }
            return selectedItems;
        }
        private void ShowEmptySelectionMessage()
        {
            Console.SetCursorPosition(xStartPos + 5, yStartPos + 5);
            /*if (typeof(T) == typeof(Polozka))
                Console.Write("Nejsou žádné objednané položky od tohoto dodavatele.");
            else Console.Write("Požadovaný výběr neposkytl žádné výsledky.");*/
            Console.Write("Požadovaný výběr neposkytl žádné výsledky.");
            Console.ReadKey();
        }
        private void ResetConsole()
        {
            ResetColor();
            Console.Clear();
            Console.CursorVisible = false;
            new Background<string>().Display();
            help.Display();
            //zobrazeni vstupniho pole pro filtr
            if (isRecusive)
            {
                filterInput.Label = string.Format("Filtr ({0}): ", FilterInfo);
            }
            else if (saveFilter && !string.IsNullOrEmpty(FilterInfo))
            {
                filterInput.Label = string.Format("Filtr ({0}): ", FilterInfo);
            }
            else
            {
                FilterInfo = string.Empty;
                filterInput.Label = "Filtr: ";
            }
            filterInput.Display();
            // if (typeof(T) == typeof(Polozka)) header.Display();
        }

        public override string Fill()
        {
            throw new NotImplementedException();
        }
        private bool TrySelectItems(
        string input,
        int startOfTable,
        List<T> availableItems,
        out List<T> selectedItems)
        {
            selectedItems = new List<T>();

            try
            {
                var ranges = input.Split(',');
                foreach (var range in ranges)
                {
                    if (range.Contains('-'))
                    {
                        var bounds = range.Split('-');
                        int startIndex = int.Parse(bounds[0]);
                        int endIndex = int.Parse(bounds[1]);

                        // Validate bounds are within 1 to visibleRows
                        if (startIndex < 1 || startIndex > visibleRows || endIndex < 1 || endIndex > visibleRows)
                        {
                            return false;
                        }

                        int adjustedStartIndex = startIndex - 1 + startOfTable;  // Convert to zero-based index
                        int adjustedEndIndex = endIndex - 1 + startOfTable;

                        // Validate adjusted indices within availableItems
                        if (adjustedStartIndex < 0 || adjustedEndIndex >= availableItems.Count || adjustedStartIndex > adjustedEndIndex)
                        {
                            return false;
                        }

                        for (int i = adjustedStartIndex; i <= adjustedEndIndex; i++)
                        {
                            selectedItems.Add(availableItems[i]);
                        }
                    }
                    else
                    {
                        int index = int.Parse(range);

                        // Validate index is within 1 to visibleRows
                        if (index < 1 || index > visibleRows)
                        {
                            return false;
                        }

                        int adjustedIndex = index - 1 + startOfTable; // Convert to zero-based index

                        // Validate adjusted index within availableItems
                        if (adjustedIndex >= availableItems.Count)
                        {
                            return false;
                        }
                        selectedItems.Add(availableItems[adjustedIndex]);
                    }
                }

                return true;
            }
            catch (FormatException)
            {
                // Handle parsing errors
                return false;
            }
            catch (Exception ex)
            {
                // Handle any other unexpected exceptions
                ErrorHandler.HandleError(this, ex);
                return false;
            }
        }
    }
}