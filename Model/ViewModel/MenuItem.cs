namespace targeteo.pl.Model.ViewModel
{
    public class MenuItem
    {
        public MenuItem Parent { get; set; }
        public string label { get; set; }
        public string icon { get; set; }
        public string ThumbnailFileName { get; set; }
        public MenuItem[] items { get; set; }

        public string url { get; set; }
        public object routerLink { get; set; }
    }
}
