using System;
using System.Linq;
using System.Collections.Generic;
using CCData;

namespace Form
{
    public abstract class FormItem
    {
        //identifikace prvku
        public string Id { get; protected set; }
        //pozice prvku vzhledem k levemu okraji 
        public int LeftPosition { get; protected set; }
        //pozice prvku vzhledem k vrchnimu okraji okna
        public int TopPosition { get; protected set; }
        //popis prvku (formularoveho pole)
        public string Label { get; set; }
        //delka pole
        public int Length { get; protected set; }
        //ulozeni puvodnich barev konzole do promennych
        protected ConsoleColor defaultBgColor = Konfigurace.defaultBgColor;
        protected ConsoleColor defaultFgColor = Konfigurace.defaultFgColor;
        //varovne barvy konzole
        protected const ConsoleColor warningBg = ConsoleColor.DarkRed;
        protected const ConsoleColor warningFg = ConsoleColor.White;
        //nadrazeny formular
        public ConsoleForm SuperiorForm { get; set; }
        public string Text { get; set; }
        public FormItem Next { get; set; }
        public FormItem Previous { get; set; }
        public FormItem() { }
        public FormItem(int leftPosition, int topPosition, string label, int length, ConsoleForm consoleForm)
        {
            LeftPosition = leftPosition;
            TopPosition = topPosition;
            Label = label;
            Length = length;
            SuperiorForm = consoleForm;
        }
        public FormItem(string id, int leftPosition, int topPosition, string label, int length, ConsoleForm consoleForm)
        {
            Id = id;
            LeftPosition = leftPosition;
            TopPosition = topPosition;
            Label = label;
            Length = length;
            SuperiorForm = consoleForm;
        }
        public abstract void Display();
        public abstract void SwitchTo(bool forward);
        public abstract void ToggleVisible(bool display);
    }
}