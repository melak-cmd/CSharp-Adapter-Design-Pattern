using System.Collections.Generic;

namespace Adapter.DataContracts
{
    public interface IRentalRecordContract
    {
        List<IBaseRentalDataContract> ProductList { get; set; }
        string CustomerName { get; set; }
    }

    public interface ICContract
    {
        string InfoDescription { get; set; }
    }

    public interface IBaseRentalDataContract
    {
        Dictionary<string, ICContract> AdditionalInfo { get; set; }
        string Comment { get; set; }
        System.DateTime StartDate { get; set; }
    }
}

