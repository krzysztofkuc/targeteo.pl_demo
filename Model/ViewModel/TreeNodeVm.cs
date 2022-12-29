namespace targeteo.pl.Model.ViewModel
{
    public class TreeNodeVm
    {
        public string label { get; set; }
        public string data { get; set; }
        //public string expandedIcon { get; set; }
        //public string collapsedIcon { get; set; }

        //public bool expanded { get; set; }

        //public string icon { get; set; }
        public TreeNodeVm[] children { get; set; }

        public object routerLink { get; set; }
    }
}
