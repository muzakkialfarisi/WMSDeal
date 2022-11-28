namespace WMS.Models.ViewModels
{
    public class PutAwayViewModel
    {
        public int DOProductId { get; set; }
        public string IKU { get; set; }
        public Guid StorageCode { get; set; }
        public int Quantity { get; set; }

    }
}
