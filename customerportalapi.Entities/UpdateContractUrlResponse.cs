using System.Collections.Generic;

namespace customerportalapi.Entities
{
    public class UpdateContractsUrlResponse
    {
        public int? skip { get; set; }

        public int? limit { get; set; }
        public int NumContracts { get; set; }

        public List<ContractUrlResponse> ContractsUrl { get; set; }
        public int TotalContracts { get; set; }
        public string Error { get; set; }
    }

    public class ContractUrlResponse
    {
        public string StoreName { get; set; }
        public string Dni { get; set; }
        public string CustomerType { get; set; }
        public string ContractId { get; set; }
        public string SMContractCode { get; set; }
        public string ContractUrl { get; set; }
        public string Environment { get; set; }
    }
}
