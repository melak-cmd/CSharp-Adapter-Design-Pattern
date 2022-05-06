using Adapter.DataContracts;
using Framework.BusinessObjectLayer.BusinessObjects;
using System.Collections.Generic;

namespace Adapter.BusinessObjects
{
    public class CalendarBo : ValidatableObject, IRentalRecordContract
    {

        private List<IBaseRentalDataContract> _Holidays = new List<IBaseRentalDataContract>();

        public List<IBaseRentalDataContract> ProductList
        {
            get { return _Holidays; }
            set { _Holidays = value; }
        }


        public string CustomerName { get; set; }
    }

    public class RentalRecordBo : ValidatableObject, IBaseRentalDataContract
    {

        #region "IHolidayDataContract"
        private string _Comment;
        public string Comment
        {
            get { return _Comment; }
            set
            {
                this._Comment = value;
                this.Validation.ClearError(nameof(Comment));
                this.Validation.ValidateEmptyString(nameof(Comment), value);
            }
        }

        private System.DateTime _startDate;
        public System.DateTime StartDate
        {
            get { return _startDate; }
            set
            {
                this._startDate = value;
                this.Validation.ClearError(nameof(StartDate));
                this.Validation.ValidateDateRange(nameof(StartDate), value, new System.DateTime(2010, 1, 1), new System.DateTime(2012, 12, 31));
            }
        }

        private Dictionary<string, ICContract> _AdditionalInfo = new Dictionary<string, ICContract>();
        public System.Collections.Generic.Dictionary<string, ICContract> AdditionalInfo
        {
            get { return _AdditionalInfo; }
            set { _AdditionalInfo = value; }
        }
        #endregion
    }

    public class AdditionalInfoBo : ValidatableObject, ICContract
    {
        public string InfoDescription { get; set; }
    }
}
