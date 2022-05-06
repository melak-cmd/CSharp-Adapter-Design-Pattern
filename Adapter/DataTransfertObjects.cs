using Adapter.DataContracts;
using Framework.BusinessObjectLayer.BusinessObjects;
using System.Collections.Generic;

namespace Adapter.DataObjects
{
    public class RentalDataDto : IBaseRentalDataContract
    {
        private Dictionary<string, ICContract> _AdditionalInfo = new Dictionary<string, ICContract>();

        public Dictionary<string, ICContract> AdditionalInfo
        {
            get { return _AdditionalInfo; }
            set { _AdditionalInfo = value; }
        }

        public string Comment { get; set; }
        public System.DateTime StartDate { get; set; }
    }

    public class AdditionalInfoDto : ICContract
    {

        public string InfoDescription { get; set; }

    }

    public class RentalRecordDto : ValidatableObject, IRentalRecordContract
    {

        private List<IBaseRentalDataContract> _productList = new List<IBaseRentalDataContract>();

        public List<IBaseRentalDataContract> ProductList
        {
            get { return _productList; }
            set { _productList = value; }
        }

        public string CustomerName { get; set; }
    }
}

