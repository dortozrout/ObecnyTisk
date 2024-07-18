using System;
using System.Collections.Generic;

namespace Form
{
    public class AditionalParms
    {
        public bool eraseText;
        public bool end;
        public bool hidenInput;
        public bool isActivable;
        public bool isHidden;
        public bool cursorToStart;
        public List<ConsoleKeyInfo> switchToNext;
        public List<ConsoleKeyInfo> switchToPrevious;
        public AditionalParms()
        {
            eraseText = false;
            end = false;
            hidenInput = false;
            isActivable = true;
            cursorToStart = true;
            isHidden = false;
            switchToNext = new List<ConsoleKeyInfo>()
            {
                new ConsoleKeyInfo('\t', ConsoleKey.Tab, false, false, false),
                new ConsoleKeyInfo('\r', ConsoleKey.Enter, false, false, false),
                new ConsoleKeyInfo('\0', ConsoleKey.DownArrow, false, false, false)
            };
            //seznam klaves ktere po ukonceni zapisu prepinaji dozadu na predchozi pole
            switchToPrevious = new List<ConsoleKeyInfo>()
            {
                new ConsoleKeyInfo('\0',ConsoleKey.Tab,true,false,false),
                new ConsoleKeyInfo('\0',ConsoleKey.UpArrow,false,false,false)
            };
        }
    }

}