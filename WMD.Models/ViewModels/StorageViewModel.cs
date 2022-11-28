using Microsoft.AspNetCore.Mvc.Rendering;

namespace WMS.Models.ViewModels
{
    public class StorageViewModel
    {
        public InvStorageRow? invStorageRow { get; set; }
        public IEnumerable<InvStorageRow>? invStorageRows { get; set; }

        public InvStorageColumn? invStorageColumn { get; set; }

        public InvStorageSection? invStorageSection { get; set; }

        public InvStorageLevel? invStorageLevel { get; set; }
        public IEnumerable<InvStorageLevel>? invStorageLevels { get; set; }

        public InvStorageCode? invStorageCode { get; set; }
        public IEnumerable<InvStorageCode>? invStorageCodes { get; set; }
    }

    public class StorageSizeViewModel
    {
        public IEnumerable<InvStorageSize>? StorageSizes { get; set; }
        public IEnumerable<InvStorageBesaran>? Besarans { get; set; }
        public IEnumerable<InvStorageTebal>? Tebals { get; set; }
        public InvStorageSize? InvStorageSize { get; set; }
        public InvStorageBesaran? InvStorageBesaran { get; set; }
        public InvStorageTebal? InvStorageTebal { get; set; }
    }
}
