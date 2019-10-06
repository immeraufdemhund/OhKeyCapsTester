namespace OhKeyCapsTester.Contracts.Messages
{
    public struct KeyEvent
    {
        public int Pressed;
        public int Row;
        public int Col;

        public KeyEvent(int row, int col, int pressed)
        {
            Row = row;
            Col = col;
            Pressed = pressed;
        }
    }
}
