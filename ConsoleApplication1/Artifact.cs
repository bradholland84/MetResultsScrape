namespace ConsoleApplication1
{
    /// <summary>
    /// Simple artifact structure that holds named data strings and provides a clean ToString() implementation
    /// </summary>
    internal struct Artifact
    {
        public string Title { get; set; }
        public string Image { get; set; }
        public string Date { get; set; }
        public string Medium { get; set; }
        public string AccessionNum { get; set; }
        public string Location { get; set; }

        public string ToString()
        {
            return Date + ", " + Medium + ", " + AccessionNum + ", " + Location;
        }
    }
}