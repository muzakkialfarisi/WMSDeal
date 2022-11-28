namespace WMS.Models.ViewModels
{
    public class RepackAndRelableViewModel
    {
        public InvRepacking? invRepacking { get; set; }

        public IEnumerable<InvRepacking>? invRepackings { get; set; }

        public IncDeliveryOrderProduct? incDeliveryOrderProduct { get; set; }

        public InvRelabeling? invRelabeling { get; set; }

        public IEnumerable<InvRelabeling>? invRelabelings { get; set; }
    }
}
