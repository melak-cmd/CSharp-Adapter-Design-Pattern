using Microsoft.VisualBasic.CompilerServices;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Framework.BusinessObjectLayer.BusinessObjects
{
    public abstract class ValidatableObject<T> : ValidatableObject where T : BOBase
    {

        protected override Validation.Validation Validation
        {
            get
            {
                return base.Validation;
            }
        }

        public ValidatableObject()
        {

        }
    }

    public class ValidatableObject : NotifyingObject, IDataErrorInfo, IEditableObject
    {
        /// <summary>
        /// Enable client code to react to change of state (entitystate/validity) of BO.
        /// </summary>
        /// <remarks>Triggered if EntityState or Validity change.</remarks>
        public event StateChangedEventHandler StateChangedEvent;

        public delegate void StateChangedEventHandler(EntityStateEnum state, bool isValid);

        protected Validation.Validation _Validation;

        public int Count
        {
            get
            {
                return this.ValidationList.Count;
            }
        }

        public string Error
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public bool IsValid
        {
            get
            {
                return this.Validation.Count == 0;
            }
        }

        public Dictionary<string, string> ValidationList { get; set; }

        protected virtual Validation.Validation Validation
        {
            get
            {
                if (this._Validation == null)
                {
                    this._Validation = new Validation.Validation();
                    this._Validation.ValidationChangedEvent += new Validation.Validation.ValidationChangedEventHandler(this.ValidationChanged);
                }
                return this._Validation;
            }
        }

        public void BeginEdit()
        {
            throw new NotImplementedException();
        }

        public void CancelEdit()
        {
            throw new NotImplementedException();
        }

        public void EndEdit()
        {
            throw new NotImplementedException();
        }

        protected override void OnPropertyChanged(string propertyName)
        {
            base.OnPropertyChanged(propertyName);
            if (Operators.CompareString(propertyName, "EntityState", false) == 0)
            {
                return;
            }
        }

        protected virtual void ValidationChanged()
        {
            this.OnPropertyChanged("IsValid");
            ValidatableObject.StateChangedEventHandler stateChangedEvent = this.StateChangedEvent;
            if (stateChangedEvent == null)
            {
                return;
            }
        }

        public string this[string columnName]
        {
            get
            {
                throw new NotImplementedException();
            }
        }
    }
}
